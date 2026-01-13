using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.core.Services.DebugBridge;

/// <summary>
/// WebSocket client for connecting to the debug server.
/// Runs in the debugged application and sends debug data to the server.
/// </summary>
internal class DebugBridgeClient : IDisposable
{
    private readonly string _serverUrl;
    private readonly NavigationContainer _navigationContainer;
    private readonly ILogger<DebugBridgeClient> _logger;
    private readonly IImageExportService _imageExportService;
    private readonly DebugTreeInspector _treeInspector = new();

    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _receiveTask;
    private bool _isConnected;
    private bool _disposed;
    private int _connectionAttempts = 0;
    private const int MaxConnectionAttempts = 10;

    public DebugBridgeClient(
        string serverUrl,
        NavigationContainer navigationContainer,
        ILogger<DebugBridgeClient> logger,
        IImageExportService imageExportService)
    {
        _serverUrl = serverUrl;
        _navigationContainer = navigationContainer;
        _logger = logger;
        _imageExportService = imageExportService;
    }

    /// <summary>
    /// Connects to the debug server.
    /// </summary>
    public async Task ConnectAsync()
    {
        if (_disposed || _isConnected)
            return;

        // Stop trying after max attempts
        if (_connectionAttempts >= MaxConnectionAttempts)
        {
            _logger?.LogInformation("Debug server connection abandoned after {Attempts} attempts", MaxConnectionAttempts);
            return;
        }

        _connectionAttempts++;

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _webSocket = new ClientWebSocket();

            _logger.LogInformation("Connecting to debug server at {ServerUrl} (attempt {Attempt}/{Max})",
                _serverUrl, _connectionAttempts, MaxConnectionAttempts);

            await _webSocket.ConnectAsync(new Uri(_serverUrl), _cancellationTokenSource.Token);

            _isConnected = true;
            _connectionAttempts = 0; // Reset on successful connection
            _logger?.LogInformation("Connected to debug server");

            // Start receiving messages
            _receiveTask = ReceiveMessagesAsync(_cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            _isConnected = false;

            // Log connection failures gracefully without stack traces
            if (ex is WebSocketException or HttpRequestException or SocketException)
            {
                _logger.LogInformation("Debug server not reachable at {ServerUrl} (attempt {Attempt}/{Max})",
                    _serverUrl, _connectionAttempts, MaxConnectionAttempts);
            }
            else
            {
                // Unexpected exceptions should be logged with full details
                _logger.LogWarning(ex, "Failed to connect to debug server");
            }

            // Auto-reconnect after delay if we haven't exceeded max attempts
            if (_connectionAttempts < MaxConnectionAttempts)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    await ConnectAsync();
                });
            }
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
            _logger.LogWarning(ex, "Failed to send message to debug server");
            _isConnected = false;

            // Trigger reconnect
            _ = ConnectAsync();
        }
    }

    /// <summary>
    /// Sends a log message to the server.
    /// </summary>
    public async Task SendLogAsync(LogMessageDto logMessage)
    {
        var message = new DebugMessage
        {
            Type = "log",
            Data = logMessage
        };

        await SendAsync(message);
    }

    /// <summary>
    /// Sends a batch of log messages to the server.
    /// </summary>
    public async Task SendLogBatchAsync(List<LogMessageDto> logs)
    {
        if (logs.Count == 0)
            return;

        var message = new DebugMessage
        {
            Type = "log_batch",
            Data = logs
        };

        await SendAsync(message);
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
                // Receive message in chunks until EndOfMessage is true
                var messageBytes = new List<byte>();
                WebSocketReceiveResult result;

                do
                {
                    result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger?.LogInformation("Server closed connection");
                        _isConnected = false;

                        // Auto-reconnect
                        _ = ConnectAsync();
                        return;
                    }

                    // Append received bytes to message
                    messageBytes.AddRange(buffer.Take(result.Count));

                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(messageBytes.ToArray());
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
                    await HandleGetTreeCommandAsync();
                    break;

                case "get_element_details":
                    if (message.Data is string elementId)
                    {
                        await HandleGetElementDetailsCommandAsync(elementId);
                    }
                    break;

                case "set_property":
                    await HandleSetPropertyCommandAsync(message.Data);
                    break;

                case "capture_screenshot":
                    await HandleCaptureScreenshotCommandAsync(message.Data);
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
    /// Handles get_tree command - serializes and sends the UI tree.
    /// </summary>
    private async Task HandleGetTreeCommandAsync()
    {
        try
        {
            // Check if navigation stack has any pages before accessing CurrentPage
            UiPageElement? currentPage;
            try
            {
                currentPage = _navigationContainer.CurrentPage;
            }
            catch (InvalidOperationException)
            {
                _logger?.LogWarning("Navigation stack is empty - cannot inspect tree");
                return;
            }

            if (currentPage == null)
            {
                _logger?.LogWarning("No current page to inspect");
                return;
            }

            var treeData = _treeInspector.SerializeTree(currentPage);

            await SendAsync(new DebugMessage
            {
                Type = "ui_tree",
                Data = treeData
            });

            _logger?.LogDebug("Sent UI tree with {Count} properties", treeData.Properties.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling get_tree command");
        }
    }

    /// <summary>
    /// Handles get_element_details command - sends details for a specific element.
    /// </summary>
    private async Task HandleGetElementDetailsCommandAsync(string elementId)
    {
        try
        {
            var currentPage = _navigationContainer.CurrentPage;
            if (currentPage == null)
            {
                _logger?.LogWarning("No current page to inspect");
                return;
            }

            var details = _treeInspector.GetElementDetails(currentPage, elementId);
            if (details == null)
            {
                _logger?.LogWarning("Element {ElementId} not found", elementId);
                return;
            }

            await SendAsync(new DebugMessage
            {
                Type = "element_details",
                Data = details
            });

            _logger?.LogDebug("Sent element details for {ElementId}", elementId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling get_element_details command");
        }
    }

    /// <summary>
    /// Handles set_property command - updates a property value on an element.
    /// </summary>
    private async Task HandleSetPropertyCommandAsync(object? data)
    {
        try
        {
            if (data == null)
            {
                _logger?.LogWarning("set_property command received with null data");
                return;
            }

            var json = JsonSerializer.Serialize(data);
            var propertyUpdate = JsonSerializer.Deserialize<PropertyUpdateDto>(json);

            if (propertyUpdate == null || string.IsNullOrEmpty(propertyUpdate.ElementId))
            {
                _logger?.LogWarning("Invalid property update data");
                return;
            }

            var currentPage = _navigationContainer.CurrentPage;
            if (currentPage == null)
            {
                _logger?.LogWarning("No current page to update");
                return;
            }

            // Update the property
            var success = _treeInspector.UpdateProperty(
                currentPage,
                propertyUpdate.ElementId,
                propertyUpdate.PropertyPath,
                propertyUpdate.Value);

            if (success)
            {
                _logger?.LogDebug("Updated property {Path} on element {ElementId}",
                    propertyUpdate.PropertyPath, propertyUpdate.ElementId);

                // Send updated tree back
                await HandleGetTreeCommandAsync();
            }
            else
            {
                _logger?.LogWarning("Failed to update property {Path} on element {ElementId}",
                    propertyUpdate.PropertyPath, propertyUpdate.ElementId);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling set_property command");
        }
    }

    private class PropertyUpdateDto
    {
        [JsonPropertyName("elementId")]
        public string ElementId { get; set; } = "";

        [JsonPropertyName("propertyPath")]
        public string PropertyPath { get; set; } = "";

        [JsonPropertyName("value")]
        public string Value { get; set; } = "";
    }

    private class CaptureScreenshotDto
    {
        [JsonPropertyName("elementId")]
        public string? ElementId { get; set; }
    }

    /// <summary>
    /// Handles capture_screenshot command - captures and sends a screenshot.
    /// </summary>
    private async Task HandleCaptureScreenshotCommandAsync(object? data)
    {
        try
        {
            string? elementId = null;

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                var captureDto = JsonSerializer.Deserialize<CaptureScreenshotDto>(json);
                elementId = captureDto?.ElementId;
            }

            var currentPage = _navigationContainer.CurrentPage;
            if (currentPage == null)
            {
                _logger?.LogWarning("No current page to capture");
                return;
            }

            UiElement targetElement;
            if (string.IsNullOrEmpty(elementId))
            {
                targetElement = currentPage;
            }
            else
            {
                var element = _treeInspector.FindElementById(currentPage, elementId);
                if (element == null)
                {
                    _logger?.LogWarning("Element {ElementId} not found for screenshot", elementId);
                    return;
                }
                targetElement = element;
            }

            var imageBytes = _imageExportService.ExportToBytes(targetElement);
            var base64 = Convert.ToBase64String(imageBytes);

            var screenshot = new ScreenshotDto
            {
                ElementId = elementId,
                ImageBase64 = base64,
                Width = (int)targetElement.ElementSize.Width,
                Height = (int)targetElement.ElementSize.Height,
                Timestamp = DateTimeOffset.Now
            };

            await SendAsync(new DebugMessage
            {
                Type = "screenshot",
                Data = screenshot
            });

            _logger?.LogDebug("Sent screenshot for {ElementId}", elementId ?? "full page");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error capturing screenshot");
        }
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
