using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.DebugServer.Services;

/// <summary>
/// WebSocket server for receiving debug data from debugged applications.
/// Listens on a specified port and manages multiple connected clients.
/// </summary>
internal class DebugBridgeServer : IDisposable
{
    private readonly int _port;
    private readonly ILogger<DebugBridgeServer> _logger;
    private HttpListener? _httpListener;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly Dictionary<string, ClientConnection> _clients = [];
    private readonly SemaphoreSlim _clientsLock = new(1, 1);
    private bool _isRunning;
    private bool _disposed;

    public event EventHandler<ClientMessageReceivedEventArgs>? ClientMessageReceived;
    public event EventHandler<string>? ClientConnected;
    public event EventHandler<string>? ClientDisconnected;

    public DebugBridgeServer(ILogger<DebugBridgeServer> logger, int port = 5555)
    {
        _port = port;
        _logger = logger;
    }

    /// <summary>
    /// Starts the WebSocket server.
    /// </summary>
    public async Task StartAsync()
    {
        _logger.LogDebug("StartAsync called");
        if (_isRunning || _disposed)
        {
            _logger.LogDebug("Already running or disposed, returning");
            return;
        }

        _logger.LogDebug("Creating HttpListener on port {Port}", _port);
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://localhost:{_port}/");
        _httpListener.Start();
        _logger.LogInformation("Debug Bridge Server listening on port {Port}", _port);

        _cancellationTokenSource = new CancellationTokenSource();
        _isRunning = true;

        _logger.LogDebug("Starting AcceptClientsAsync task");
        _ = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));

        await Task.CompletedTask;
        _logger.LogDebug("StartAsync completed");
    }

    /// <summary>
    /// Accepts incoming client connections.
    /// </summary>
    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("AcceptClientsAsync started, waiting for connections");
        while (!cancellationToken.IsCancellationRequested && _httpListener != null)
        {
            try
            {
                _logger.LogDebug("Calling GetContextAsync");
                var context = await _httpListener.GetContextAsync();
                _logger.LogDebug("GetContextAsync returned, IsWebSocketRequest: {IsWebSocketRequest}", context.Request.IsWebSocketRequest);

                if (context.Request.IsWebSocketRequest)
                {
                    _logger.LogDebug("Starting HandleClientAsync");
                    _ = HandleClientAsync(context, cancellationToken);
                }
                else
                {
                    _logger.LogDebug("Not a WebSocket request, returning 400");
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AcceptClientsAsync");
            }
        }
        _logger.LogDebug("AcceptClientsAsync loop ended, Cancelled={Cancelled}", cancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// Handles a connected client.
    /// </summary>
    private async Task HandleClientAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        WebSocketContext? webSocketContext = null;
        string clientId = Guid.NewGuid().ToString();

        try
        {
            _logger.LogDebug("Accepting WebSocket for client {ClientId}", clientId);
            webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;
            _logger.LogDebug("WebSocket accepted, State: {State}", webSocket.State);

            var client = new ClientConnection(clientId, webSocket);

            await _clientsLock.WaitAsync(cancellationToken);
            try
            {
                _clients[clientId] = client;
            }
            finally
            {
                _clientsLock.Release();
            }

            _logger.LogInformation("Client connected: {ClientId}", clientId);
            _logger.LogDebug("Raising ClientConnected event");
            ClientConnected?.Invoke(this, clientId);
            _logger.LogDebug("Starting receive loop for client {ClientId}", clientId);

            await ReceiveMessagesAsync(clientId, webSocket, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling client {ClientId}", clientId);
        }
        finally
        {
            await _clientsLock.WaitAsync(CancellationToken.None);
            try
            {
                _clients.Remove(clientId);
            }
            finally
            {
                _clientsLock.Release();
            }

            _logger.LogInformation("Client disconnected: {ClientId}", clientId);
            ClientDisconnected?.Invoke(this, clientId);
        }
    }

    /// <summary>
    /// Receives messages from a connected client.
    /// </summary>
    private async Task ReceiveMessagesAsync(string clientId, WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        try
        {
            _logger.LogDebug("Starting receive loop for {ClientId}, WebSocket state: {State}", clientId, webSocket.State);
            while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
            {
                _logger.LogDebug("Waiting to receive from {ClientId}", clientId);
                var messageBytes = new List<byte>();
                WebSocketReceiveResult result;

                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    _logger.LogDebug("Received chunk from {ClientId}: Type={MessageType}, Count={Count}, EndOfMessage={EndOfMessage}",
                        clientId, result.MessageType, result.Count, result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogDebug("Client {ClientId} sent Close message", clientId);
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closing",
                            CancellationToken.None);
                        return;
                    }

                    messageBytes.AddRange(buffer.Take(result.Count));

                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(messageBytes.ToArray());
                    _logger.LogDebug("Received complete message from {ClientId}: {MessagePreview}",
                        clientId, json.Substring(0, Math.Min(100, json.Length)));
                    var message = JsonSerializer.Deserialize<DebugMessage>(json);

                    if (message != null)
                    {
                        _logger.LogDebug("Raising ClientMessageReceived event for message type: {MessageType}", message.Type);
                        ClientMessageReceived?.Invoke(this, new ClientMessageReceivedEventArgs(clientId, message));
                    }
                }
            }
            _logger.LogDebug("Receive loop ended for {ClientId}, Cancelled={Cancelled}, State={State}",
                clientId, cancellationToken.IsCancellationRequested, webSocket.State);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving from client {ClientId}", clientId);
        }
    }

    /// <summary>
    /// Sends a message to a specific client.
    /// </summary>
    public async Task SendToClientAsync(string clientId, DebugMessage message)
    {
        _logger.LogDebug("SendToClientAsync called for {ClientId}, message type: {MessageType}", clientId, message.Type);
        await _clientsLock.WaitAsync();
        ClientConnection? client;

        try
        {
            if (!_clients.TryGetValue(clientId, out client))
            {
                _logger.LogWarning("Client {ClientId} not found in clients dictionary", clientId);
                return;
            }
            _logger.LogDebug("Client {ClientId} found, WebSocket state: {State}", clientId, client.WebSocket.State);
        }
        finally
        {
            _clientsLock.Release();
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            _logger.LogDebug("Sending {ByteCount} bytes to {ClientId}", bytes.Length, clientId);
            await client.WebSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                CancellationToken.None);
            _logger.LogDebug("Message sent successfully to {ClientId}", clientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending to client {ClientId}", clientId);
        }
    }

    /// <summary>
    /// Gets all connected client IDs.
    /// </summary>
    public async Task<List<string>> GetConnectedClientsAsync()
    {
        await _clientsLock.WaitAsync();
        try
        {
            return _clients.Keys.ToList();
        }
        finally
        {
            _clientsLock.Release();
        }
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public async Task StopAsync()
    {
        if (!_isRunning)
            return;

        _cancellationTokenSource?.Cancel();
        _httpListener?.Stop();
        _isRunning = false;

        await _clientsLock.WaitAsync(CancellationToken.None);
        try
        {
            foreach (var client in _clients.Values)
            {
                if (client.WebSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        await client.WebSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Server shutting down",
                            CancellationToken.None);
                    }
                    catch
                    {
                    }
                }
                client.WebSocket.Dispose();
            }
            _clients.Clear();
        }
        finally
        {
            _clientsLock.Release();
        }

        _logger.LogInformation("Debug Bridge Server stopped");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _httpListener?.Stop();
        _httpListener?.Close();
        _clientsLock.Dispose();

        GC.SuppressFinalize(this);
    }

    private record ClientConnection(string Id, WebSocket WebSocket);
}

internal class ClientMessageReceivedEventArgs(string clientId, DebugMessage message) : EventArgs
{
    public string ClientId { get; } = clientId;
    public DebugMessage Message { get; } = message;
}
