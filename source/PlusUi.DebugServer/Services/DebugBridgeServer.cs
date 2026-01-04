using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.DebugServer.Services;

/// <summary>
/// WebSocket server for receiving debug data from debugged applications.
/// Listens on a specified port and manages multiple connected clients.
/// </summary>
internal class DebugBridgeServer : IDisposable
{
    private readonly int _port;
    private HttpListener? _httpListener;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly Dictionary<string, ClientConnection> _clients = new();
    private readonly SemaphoreSlim _clientsLock = new(1, 1);
    private bool _isRunning;
    private bool _disposed;

    public event EventHandler<ClientMessageReceivedEventArgs>? ClientMessageReceived;
    public event EventHandler<string>? ClientConnected;
    public event EventHandler<string>? ClientDisconnected;

    public DebugBridgeServer(int port = 5555)
    {
        _port = port;
    }

    /// <summary>
    /// Starts the WebSocket server.
    /// </summary>
    public async Task StartAsync()
    {
        if (_isRunning || _disposed)
            return;

        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://localhost:{_port}/");
        _httpListener.Start();

        _cancellationTokenSource = new CancellationTokenSource();
        _isRunning = true;

        Console.WriteLine($"Debug Bridge Server listening on port {_port}");

        // Accept client connections
        _ = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));

        await Task.CompletedTask;
    }

    /// <summary>
    /// Accepts incoming client connections.
    /// </summary>
    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _httpListener != null)
        {
            try
            {
                var context = await _httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    _ = HandleClientAsync(context, cancellationToken);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
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
            webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;

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

            Console.WriteLine($"Client connected: {clientId}");
            ClientConnected?.Invoke(this, clientId);

            // Receive messages from client
            await ReceiveMessagesAsync(clientId, webSocket, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client {clientId}: {ex.Message}");
        }
        finally
        {
            await _clientsLock.WaitAsync(cancellationToken);
            try
            {
                _clients.Remove(clientId);
            }
            finally
            {
                _clientsLock.Release();
            }

            Console.WriteLine($"Client disconnected: {clientId}");
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
            while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None);
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var message = JsonSerializer.Deserialize<DebugMessage>(json);

                    if (message != null)
                    {
                        ClientMessageReceived?.Invoke(this, new ClientMessageReceivedEventArgs(clientId, message));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving from client {clientId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends a message to a specific client.
    /// </summary>
    public async Task SendToClientAsync(string clientId, DebugMessage message)
    {
        await _clientsLock.WaitAsync();
        ClientConnection? client;

        try
        {
            if (!_clients.TryGetValue(clientId, out client))
                return;
        }
        finally
        {
            _clientsLock.Release();
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await client.WebSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending to client {clientId}: {ex.Message}");
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

        await _clientsLock.WaitAsync();
        try
        {
            foreach (var client in _clients.Values)
            {
                client.WebSocket.Dispose();
            }
            _clients.Clear();
        }
        finally
        {
            _clientsLock.Release();
        }

        Console.WriteLine("Debug Bridge Server stopped");
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
