using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Services;

namespace PlusUi.DebugServer.Pages;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly DebugBridgeServer _server;
    private readonly IPopupService _popupService;
    private readonly object _timerLock = new();
    private Timer? _retryTimer;
    private string? _currentClientId;
    private bool _treeReceived;

    [ObservableProperty]
    private string _statusText = "Waiting for client...";

    [ObservableProperty]
    private ObservableCollection<TreeNodeDto> _rootItems = new();

    [ObservableProperty]
    private TreeNodeDto? _selectedNode;

    public ObservableCollection<PropertyDto> SelectedProperties { get; } = new();

    public MainViewModel(DebugBridgeServer server, IPopupService popupService)
    {
        Console.WriteLine("[VIEWMODEL] Constructor called");
        _server = server;
        _popupService = popupService;

        // Unsubscribe first to prevent duplicate subscriptions
        _server.ClientConnected -= OnClientConnected;
        _server.ClientDisconnected -= OnClientDisconnected;
        _server.ClientMessageReceived -= OnClientMessageReceived;

        // Subscribe to server events
        _server.ClientConnected += OnClientConnected;
        _server.ClientDisconnected += OnClientDisconnected;
        _server.ClientMessageReceived += OnClientMessageReceived;
        Console.WriteLine("[VIEWMODEL] Event handlers registered");
    }

    partial void OnSelectedNodeChanged(TreeNodeDto? value)
    {
        SelectedProperties.Clear();
        if (value?.Properties != null)
        {
            foreach (var prop in value.Properties)
            {
                SelectedProperties.Add(prop);
            }
        }
    }

    private void OnClientConnected(object? sender, string clientId)
    {
        lock (_timerLock)
        {
            _currentClientId = clientId;
            _treeReceived = false;
            StatusText = $"Client connected: {clientId} - requesting tree...";

            _retryTimer?.Dispose();
            _retryTimer = new Timer(async _ =>
            {
                bool shouldRequest = false;
                string? currentClient = null;

                lock (_timerLock)
                {
                    if (!_treeReceived && _currentClientId != null)
                    {
                        shouldRequest = true;
                        currentClient = _currentClientId;
                    }
                }

                if (shouldRequest && currentClient != null)
                {
                    await _server.SendToClientAsync(currentClient, new DebugMessage { Type = "get_tree" });
                }
            }, null, TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(2));
        }
    }

    private void OnClientDisconnected(object? sender, string clientId)
    {
        lock (_timerLock)
        {
            _retryTimer?.Dispose();
            _retryTimer = null;
            _currentClientId = null;
            _treeReceived = false;
        }

        StatusText = "Client disconnected. Waiting for client...";
        RootItems.Clear();
        SelectedNode = null;
    }

    private void OnClientMessageReceived(object? sender, ClientMessageReceivedEventArgs e)
    {
        if (e.Message.Type == "ui_tree" && e.Message.Data != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(e.Message.Data);
                var tree = JsonSerializer.Deserialize<TreeNodeDto>(json);

                if (tree != null)
                {
                    RootItems.Clear();
                    RootItems.Add(tree);
                    SelectedNode = null; // Clear selection when new tree arrives
                    StatusText = $"Tree received from {e.ClientId} - {CountNodes(tree)} elements";

                    lock (_timerLock)
                    {
                        _treeReceived = true;
                        _retryTimer?.Dispose();
                        _retryTimer = null;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error parsing tree: {ex.Message}";
            }
        }
    }

    private int CountNodes(TreeNodeDto node)
    {
        int count = 1;
        foreach (var child in node.Children)
        {
            count += CountNodes(child);
        }
        return count;
    }

    [RelayCommand]
    private async Task RefreshTreeAsync()
    {
        if (_currentClientId != null)
        {
            StatusText = "Refreshing tree...";
            await _server.SendToClientAsync(_currentClientId, new DebugMessage { Type = "get_tree" });
        }
    }

    public async void UpdatePropertyValue(PropertyDto property, string newValue)
    {
        if (_currentClientId == null || string.IsNullOrEmpty(property.ElementId))
            return;

        await _server.SendToClientAsync(_currentClientId, new DebugMessage
        {
            Type = "set_property",
            Data = new
            {
                elementId = property.ElementId,
                propertyPath = property.Path,
                value = newValue
            }
        });

        StatusText = $"Updated {property.Path} = {newValue}";
    }

    [RelayCommand]
    private void EditProperty(PropertyDto property)
    {
        _popupService.ShowPopup<PropertyEditorPopup, PropertyDto, PropertyEditorResult>(
            property,
            onClosed: async (result) => await OnPropertyEditedAsync(property, result),
            configure: config =>
            {
                config.CloseOnBackgroundClick = true;
                config.CloseOnEscape = true;
                config.BackgroundColor = new Color(0, 0, 0, 180);
            });
    }

    private async Task OnPropertyEditedAsync(PropertyDto property, PropertyEditorResult? result)
    {
        if (_currentClientId == null || result == null)
            return;

        // Send update for each changed field
        foreach (var field in result.Fields)
        {
            await _server.SendToClientAsync(_currentClientId, new DebugMessage
            {
                Type = "set_property",
                Data = new
                {
                    elementId = field.ElementId,
                    propertyPath = field.Path,
                    value = field.Value
                }
            });
        }

        StatusText = $"Updated {property.Name}";
        // Client automatically refreshes tree after property change, no need to request again
    }

    public void Dispose()
    {
        Console.WriteLine("[VIEWMODEL] Dispose called - unsubscribing from events");

        lock (_timerLock)
        {
            _retryTimer?.Dispose();
            _retryTimer = null;
        }

        _server.ClientConnected -= OnClientConnected;
        _server.ClientDisconnected -= OnClientDisconnected;
        _server.ClientMessageReceived -= OnClientMessageReceived;
    }
}
