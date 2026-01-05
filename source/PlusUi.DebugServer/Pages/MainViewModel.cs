using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Services;

namespace PlusUi.DebugServer.Pages;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly DebugBridgeServer _server;
    private readonly IPopupService _popupService;
    private readonly ILogger<MainViewModel> _logger;
    private readonly Dictionary<string, AppState> _apps = new();

    [ObservableProperty]
    private string? _selectedAppId;

    [ObservableProperty]
    private string _statusText = "Waiting for clients...";

    public ObservableCollection<string> ConnectedApps { get; } = new();

    public ObservableCollection<TreeNodeDto> RootItems => CurrentApp?.RootItems ?? new();

    public TreeNodeDto? SelectedNode
    {
        get => CurrentApp?.SelectedNode;
        set
        {
            if (CurrentApp != null)
            {
                CurrentApp.SelectedNode = value;
                OnPropertyChanged();
                UpdateSelectedProperties();
            }
        }
    }

    public ObservableCollection<PropertyDto> SelectedProperties => CurrentApp?.SelectedProperties ?? new();

    private AppState? CurrentApp => SelectedAppId != null && _apps.TryGetValue(SelectedAppId, out var app) ? app : null;

    public MainViewModel(DebugBridgeServer server, IPopupService popupService, ILogger<MainViewModel> logger)
    {
        _server = server;
        _popupService = popupService;
        _logger = logger;

        _logger.LogDebug("ViewModel constructor called");

        _server.ClientConnected -= OnClientConnected;
        _server.ClientDisconnected -= OnClientDisconnected;
        _server.ClientMessageReceived -= OnClientMessageReceived;

        _server.ClientConnected += OnClientConnected;
        _server.ClientDisconnected += OnClientDisconnected;
        _server.ClientMessageReceived += OnClientMessageReceived;

        _logger.LogDebug("Event handlers registered");
    }

    partial void OnSelectedAppIdChanged(string? value)
    {
        OnPropertyChanged(nameof(RootItems));
        OnPropertyChanged(nameof(SelectedNode));
        OnPropertyChanged(nameof(SelectedProperties));
        UpdateStatusText();
    }

    private void UpdateSelectedProperties()
    {
        if (CurrentApp == null)
            return;

        CurrentApp.SelectedProperties.Clear();
        if (CurrentApp.SelectedNode?.Properties != null)
        {
            foreach (var prop in CurrentApp.SelectedNode.Properties)
            {
                CurrentApp.SelectedProperties.Add(prop);
            }
        }
        OnPropertyChanged(nameof(SelectedProperties));
    }

    private void UpdateStatusText()
    {
        if (CurrentApp != null)
        {
            StatusText = CurrentApp.StatusText;
        }
        else if (_apps.Count > 0)
        {
            StatusText = $"{_apps.Count} app(s) connected - select one";
        }
        else
        {
            StatusText = "Waiting for clients...";
        }
    }

    private void OnClientConnected(object? sender, string clientId)
    {
        _logger.LogDebug("Client connected: {ClientId}", clientId);

        var app = new AppState(clientId)
        {
            IsConnected = true,
            StatusText = $"Connected: {clientId} - requesting tree..."
        };

        _apps[clientId] = app;
        ConnectedApps.Add(clientId);

        if (SelectedAppId == null)
        {
            SelectedAppId = clientId;
        }

        app.RetryTimer = new Timer(async _ =>
        {
            if (!app.TreeReceived && app.IsConnected)
            {
                await _server.SendToClientAsync(clientId, new DebugMessage { Type = "get_tree" });
            }
        }, null, TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(2));

        UpdateStatusText();
    }

    private void OnClientDisconnected(object? sender, string clientId)
    {
        _logger.LogDebug("Client disconnected: {ClientId}", clientId);

        if (_apps.TryGetValue(clientId, out var app))
        {
            app.IsConnected = false;
            app.StatusText = "Disconnected";
            app.Dispose();
        }

        ConnectedApps.Remove(clientId);

        if (SelectedAppId == clientId)
        {
            SelectedAppId = ConnectedApps.FirstOrDefault();
        }

        UpdateStatusText();
    }

    private void OnClientMessageReceived(object? sender, ClientMessageReceivedEventArgs e)
    {
        if (!_apps.TryGetValue(e.ClientId, out var app))
            return;

        if (e.Message.Type == "ui_tree" && e.Message.Data != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(e.Message.Data);
                var tree = JsonSerializer.Deserialize<TreeNodeDto>(json);

                if (tree != null)
                {
                    app.RootItems.Clear();
                    app.RootItems.Add(tree);
                    app.SelectedNode = null;
                    app.StatusText = $"Tree received - {CountNodes(tree)} elements";
                    app.TreeReceived = true;
                    app.RetryTimer?.Dispose();
                    app.RetryTimer = null;

                    if (SelectedAppId == e.ClientId)
                    {
                        OnPropertyChanged(nameof(RootItems));
                        OnPropertyChanged(nameof(SelectedNode));
                        OnPropertyChanged(nameof(SelectedProperties));
                    }

                    UpdateStatusText();
                }
            }
            catch (Exception ex)
            {
                app.StatusText = $"Error parsing tree: {ex.Message}";
                _logger.LogError(ex, "Error parsing tree from {ClientId}", e.ClientId);
                UpdateStatusText();
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
        if (CurrentApp == null || SelectedAppId == null)
            return;

        CurrentApp.StatusText = "Refreshing tree...";
        UpdateStatusText();
        await _server.SendToClientAsync(SelectedAppId, new DebugMessage { Type = "get_tree" });
    }

    public async void UpdatePropertyValue(PropertyDto property, string newValue)
    {
        if (SelectedAppId == null || string.IsNullOrEmpty(property.ElementId))
            return;

        await _server.SendToClientAsync(SelectedAppId, new DebugMessage
        {
            Type = "set_property",
            Data = new
            {
                elementId = property.ElementId,
                propertyPath = property.Path,
                value = newValue
            }
        });

        if (CurrentApp != null)
        {
            CurrentApp.StatusText = $"Updated {property.Path} = {newValue}";
            UpdateStatusText();
        }
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
        if (SelectedAppId == null || result == null)
            return;

        foreach (var field in result.Fields)
        {
            await _server.SendToClientAsync(SelectedAppId, new DebugMessage
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

        if (CurrentApp != null)
        {
            CurrentApp.StatusText = $"Updated {property.Name}";
            UpdateStatusText();
        }
    }

    public void Dispose()
    {
        _logger.LogDebug("Dispose called - cleaning up apps and unsubscribing from events");

        foreach (var app in _apps.Values)
        {
            app.Dispose();
        }
        _apps.Clear();

        _server.ClientConnected -= OnClientConnected;
        _server.ClientDisconnected -= OnClientDisconnected;
        _server.ClientMessageReceived -= OnClientMessageReceived;
    }
}
