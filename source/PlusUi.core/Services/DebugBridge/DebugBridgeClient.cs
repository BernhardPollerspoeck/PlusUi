using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.core.Services.Rendering;

namespace PlusUi.core.Services.DebugBridge;

/// <summary>
/// WebSocket client for connecting to the debug server.
/// Runs in the debugged application and sends debug data to the server.
/// </summary>
internal class DebugBridgeClient : IDisposable
{
    private readonly string _serverUrl;
    private readonly NavigationContainer _navigationContainer;
    private readonly InvalidationTracker _invalidationTracker;
    private readonly ILogger<DebugBridgeClient>? _logger;

    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _receiveTask;
    private bool _isConnected;
    private bool _disposed;

    public DebugBridgeClient(
        string serverUrl,
        NavigationContainer navigationContainer,
        InvalidationTracker invalidationTracker,
        ILogger<DebugBridgeClient>? logger = null)
    {
        _serverUrl = serverUrl;
        _navigationContainer = navigationContainer;
        _invalidationTracker = invalidationTracker;
        _logger = logger;
    }

    /// <summary>
    /// Connects to the debug server.
    /// </summary>
    public async Task ConnectAsync()
    {
        if (_disposed || _isConnected)
            return;

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _webSocket = new ClientWebSocket();

            _logger?.LogInformation("Connecting to debug server at {ServerUrl}", _serverUrl);

            await _webSocket.ConnectAsync(new Uri(_serverUrl), _cancellationTokenSource.Token);

            _isConnected = true;
            _logger?.LogInformation("Connected to debug server");

            // Start receiving messages
            _receiveTask = ReceiveMessagesAsync(_cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to connect to debug server");
            _isConnected = false;

            // Auto-reconnect after delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                await ConnectAsync();
            });
        }
    }

    /// <summary>
    /// Sends a debug message to the server.
    /// </summary>
    public async Task SendAsync(DebugMessage message)
    {
        if (!_isConnected || _webSocket == null || _webSocket.State != WebSocketState.Open)
            return;

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                _cancellationTokenSource?.Token ?? CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to send message to debug server");
            _isConnected = false;

            // Trigger reconnect
            _ = ConnectAsync();
        }
    }

    /// <summary>
    /// Receives messages from the server.
    /// </summary>
    private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        try
        {
            while (!cancellationToken.IsCancellationRequested && _webSocket != null)
            {
                var result = await _webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger?.LogInformation("Server closed connection");
                    _isConnected = false;

                    // Auto-reconnect
                    _ = ConnectAsync();
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var message = JsonSerializer.Deserialize<DebugMessage>(json);

                    if (message != null)
                    {
                        await HandleCommandAsync(message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error receiving messages from debug server");
            _isConnected = false;

            // Auto-reconnect
            _ = ConnectAsync();
        }
    }

    /// <summary>
    /// Handles commands received from the server.
    /// </summary>
    private async Task HandleCommandAsync(DebugMessage message)
    {
        try
        {
            switch (message.Type)
            {
                case "get_tree":
                    // Will be implemented in DebugTreeInspector
                    _logger?.LogDebug("Received get_tree command");
                    break;

                case "get_element_details":
                    // Will be implemented in DebugTreeInspector
                    _logger?.LogDebug("Received get_element_details command");
                    break;

                case "set_property":
                    // Will be implemented in DebugPropertyReflector
                    _logger?.LogDebug("Received set_property command");
                    break;

                default:
                    _logger?.LogWarning("Unknown command type: {Type}", message.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling command {Type}", message.Type);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Disconnects from the debug server.
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_webSocket == null || !_isConnected)
            return;

        try
        {
            _cancellationTokenSource?.Cancel();

            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Client closing",
                    CancellationToken.None);
            }

            _isConnected = false;
            _logger?.LogInformation("Disconnected from debug server");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error during disconnect");
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _webSocket?.Dispose();

        GC.SuppressFinalize(this);
    }
}
