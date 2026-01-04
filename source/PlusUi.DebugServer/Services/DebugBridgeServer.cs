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
public class DebugBridgeServer : IDisposable
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
        Console.WriteLine($"[SERVER] StartAsync called...");
        if (_isRunning || _disposed)
        {
            Console.WriteLine($"[SERVER] Already running or disposed. Returning.");
            return;
        }

        Console.WriteLine($"[SERVER] Creating HttpListener on port {_port}...");
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://localhost:{_port}/");
        _httpListener.Start();
        Console.WriteLine($"[SERVER] HttpListener started successfully");

        _cancellationTokenSource = new CancellationTokenSource();
        _isRunning = true;

        Console.WriteLine($"[SERVER] Debug Bridge Server listening on port {_port}");

        // Accept client connections
        Console.WriteLine($"[SERVER] Starting AcceptClientsAsync task...");
        _ = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));

        await Task.CompletedTask;
        Console.WriteLine($"[SERVER] StartAsync completed");
    }

    /// <summary>
    /// Accepts incoming client connections.
    /// </summary>
    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"[SERVER] AcceptClientsAsync started. Waiting for connections...");
        while (!cancellationToken.IsCancellationRequested && _httpListener != null)
        {
            try
            {
                Console.WriteLine($"[SERVER] Calling GetContextAsync...");
                var context = await _httpListener.GetContextAsync();
                Console.WriteLine($"[SERVER] GetContextAsync returned. IsWebSocketRequest: {context.Request.IsWebSocketRequest}");

                if (context.Request.IsWebSocketRequest)
                {
                    Console.WriteLine($"[SERVER] Starting HandleClientAsync...");
                    _ = HandleClientAsync(context, cancellationToken);
                }
                else
                {
                    Console.WriteLine($"[SERVER] Not a WebSocket request. Returning 400.");
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER] ERROR in AcceptClientsAsync:");
                Console.WriteLine($"[SERVER]   Type: {ex.GetType().Name}");
                Console.WriteLine($"[SERVER]   Message: {ex.Message}");
                Console.WriteLine($"[SERVER]   StackTrace: {ex.StackTrace}");
            }
        }
        Console.WriteLine($"[SERVER] AcceptClientsAsync loop ended. Cancelled={cancellationToken.IsCancellationRequested}");
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
            Console.WriteLine($"[SERVER] Accepting WebSocket for client {clientId}...");
            webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;
            Console.WriteLine($"[SERVER] WebSocket accepted. State: {webSocket.State}");

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

            Console.WriteLine($"[SERVER] Client connected: {clientId}");
            Console.WriteLine($"[SERVER] Raising ClientConnected event...");
            ClientConnected?.Invoke(this, clientId);
            Console.WriteLine($"[SERVER] ClientConnected event raised. Starting receive loop...");

            // Receive messages from client
            await ReceiveMessagesAsync(clientId, webSocket, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVER] ERROR handling client {clientId}:");
            Console.WriteLine($"[SERVER]   Type: {ex.GetType().Name}");
            Console.WriteLine($"[SERVER]   Message: {ex.Message}");
            Console.WriteLine($"[SERVER]   StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[SERVER]   Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
        }
        finally
        {
            // Don't use cancellationToken in cleanup - it may be cancelled
            await _clientsLock.WaitAsync(CancellationToken.None);
            try
            {
                _clients.Remove(clientId);
            }
            finally
            {
                _clientsLock.Release();
            }

            Console.WriteLine($"[SERVER] Client disconnected: {clientId}");
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
            Console.WriteLine($"[SERVER] Starting receive loop for {clientId}. WebSocket state: {webSocket.State}");
            while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
            {
                Console.WriteLine($"[SERVER] Waiting to receive from {clientId}...");
                // Receive message in chunks until EndOfMessage is true
                var messageBytes = new List<byte>();
                WebSocketReceiveResult result;

                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    Console.WriteLine($"[SERVER] Received chunk from {clientId}: Type={result.MessageType}, Count={result.Count}, EndOfMessage={result.EndOfMessage}");

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"[SERVER] Client {clientId} sent Close message");
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closing",
                            CancellationToken.None);
                        return;
                    }

                    // Append received bytes to message
                    messageBytes.AddRange(buffer.Take(result.Count));

                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(messageBytes.ToArray());
                    Console.WriteLine($"[SERVER] Received complete message from {clientId}: {json.Substring(0, Math.Min(100, json.Length))}...");
                    var message = JsonSerializer.Deserialize<DebugMessage>(json);

                    if (message != null)
                    {
                        Console.WriteLine($"[SERVER] Raising ClientMessageReceived event for message type: {message.Type}");
                        ClientMessageReceived?.Invoke(this, new ClientMessageReceivedEventArgs(clientId, message));
                    }
                }
            }
            Console.WriteLine($"[SERVER] Receive loop ended for {clientId}. Cancelled={cancellationToken.IsCancellationRequested}, State={webSocket.State}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVER] ERROR receiving from client {clientId}:");
            Console.WriteLine($"[SERVER]   Type: {ex.GetType().Name}");
            Console.WriteLine($"[SERVER]   Message: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends a message to a specific client.
    /// </summary>
    public async Task SendToClientAsync(string clientId, DebugMessage message)
    {
        Console.WriteLine($"[SERVER] SendToClientAsync called for {clientId}, message type: {message.Type}");
        await _clientsLock.WaitAsync();
        ClientConnection? client;

        try
        {
            if (!_clients.TryGetValue(clientId, out client))
            {
                Console.WriteLine($"[SERVER] Client {clientId} not found in clients dictionary");
                return;
            }
            Console.WriteLine($"[SERVER] Client {clientId} found. WebSocket state: {client.WebSocket.State}");
        }
        finally
        {
            _clientsLock.Release();
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            Console.WriteLine($"[SERVER] Sending {bytes.Length} bytes to {clientId}...");
            await client.WebSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                CancellationToken.None);
            Console.WriteLine($"[SERVER] Message sent successfully to {clientId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVER] ERROR sending to client {clientId}:");
            Console.WriteLine($"[SERVER]   Type: {ex.GetType().Name}");
            Console.WriteLine($"[SERVER]   Message: {ex.Message}");
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

        // Use CancellationToken.None in cleanup code
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
                        // Ignore errors during shutdown
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

public class ClientMessageReceivedEventArgs(string clientId, DebugMessage message) : EventArgs
{
    public string ClientId { get; } = clientId;
    public DebugMessage Message { get; } = message;
}
