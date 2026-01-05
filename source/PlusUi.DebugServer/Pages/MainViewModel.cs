using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    private readonly Dictionary<string, AppViewModel> _apps = new();

    [ObservableProperty]
    private string? _selectedAppId;

    [ObservableProperty]
    private AppViewModel? _selectedApp;

    [ObservableProperty]
    private string _statusText = "Waiting for clients...";

    [ObservableProperty]
    private TreeNodeDto? _selectedNode;

    [ObservableProperty]
    private int _selectedAppTabIndex = -1;

    public ObservableCollection<TabItem> AppTabs { get; } = new();
    public ObservableCollection<TreeNodeDto> RootItems { get; } = new();
    public ObservableCollection<PropertyDto> SelectedProperties { get; } = new();

    public bool HasConnectedApps => AppTabs.Count > 0;

    public MainViewModel(DebugBridgeServer server, IPopupService popupService, ILogger<MainViewModel> logger)
    {
        _server = server;
        _popupService = popupService;
        _logger = logger;

        _logger.LogDebug("ViewModel constructor called");

        AppTabs.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasConnectedApps));

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
        SelectedApp = value != null && _apps.TryGetValue(value, out var app) ? app : null;
        UpdateStatusText();
    }

    partial void OnSelectedAppTabIndexChanged(int value)
    {
        if (value >= 0 && value < AppTabs.Count)
        {
            var tab = AppTabs[value];
            SelectedAppId = tab.Header;
        }
        else
        {
            SelectedAppId = null;
        }
    }

    partial void OnSelectedAppChanged(AppViewModel? oldValue, AppViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnAppPropertyChanged;
            oldValue.RootItems.CollectionChanged -= OnAppRootItemsChanged;
            oldValue.SelectedProperties.CollectionChanged -= OnAppSelectedPropertiesChanged;
        }

        if (newValue != null)
        {
            newValue.PropertyChanged += OnAppPropertyChanged;
            newValue.RootItems.CollectionChanged += OnAppRootItemsChanged;
            newValue.SelectedProperties.CollectionChanged += OnAppSelectedPropertiesChanged;
        }

        SyncCollectionsFromSelectedApp();
    }

    partial void OnSelectedNodeChanged(TreeNodeDto? value)
    {
        if (SelectedApp != null)
        {
            SelectedApp.SelectedNode = value;
        }
    }

    private void OnAppPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppViewModel.StatusText))
        {
            UpdateStatusText();
        }
        else if (e.PropertyName == nameof(AppViewModel.SelectedNode))
        {
            SelectedNode = SelectedApp?.SelectedNode;
        }
    }

    private void OnAppRootItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (SelectedApp == null) return;

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            RootItems.Clear();
        }
        else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (TreeNodeDto item in e.NewItems)
            {
                RootItems.Add(item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (TreeNodeDto item in e.OldItems)
            {
                RootItems.Remove(item);
            }
        }
    }

    private void OnAppSelectedPropertiesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (SelectedApp == null) return;

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            SelectedProperties.Clear();
        }
        else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (PropertyDto item in e.NewItems)
            {
                SelectedProperties.Add(item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (PropertyDto item in e.OldItems)
            {
                SelectedProperties.Remove(item);
            }
        }
    }

    private void SyncCollectionsFromSelectedApp()
    {
        RootItems.Clear();
        SelectedProperties.Clear();

        if (SelectedApp != null)
        {
            foreach (var item in SelectedApp.RootItems)
            {
                RootItems.Add(item);
            }

            SelectedNode = SelectedApp.SelectedNode;

            foreach (var prop in SelectedApp.SelectedProperties)
            {
                SelectedProperties.Add(prop);
            }
        }
        else
        {
            SelectedNode = null;
        }

        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (SelectedApp != null)
        {
            StatusText = SelectedApp.StatusText;
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

        var app = new AppViewModel(clientId)
        {
            IsConnected = true,
            StatusText = $"Connected: {clientId} - requesting tree..."
        };

        _apps[clientId] = app;

        var tabItem = new TabItem()
            .SetHeader($"● {clientId}")
            .SetContent(new Label().SetText($"App: {clientId}"));

        AppTabs.Add(tabItem);

        if (SelectedAppTabIndex < 0)
        {
            SelectedAppTabIndex = 0;
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

        var tabToRemove = AppTabs.FirstOrDefault(t => t.Header == $"● {clientId}");
        if (tabToRemove != null)
        {
            var index = AppTabs.IndexOf(tabToRemove);
            AppTabs.Remove(tabToRemove);

            if (SelectedAppTabIndex == index)
            {
                SelectedAppTabIndex = AppTabs.Count > 0 ? 0 : -1;
            }
            else if (SelectedAppTabIndex > index)
            {
                SelectedAppTabIndex--;
            }
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
                }
            }
            catch (Exception ex)
            {
                app.StatusText = $"Error parsing tree: {ex.Message}";
                _logger.LogError(ex, "Error parsing tree from {ClientId}", e.ClientId);
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
        if (SelectedApp == null || SelectedAppId == null)
            return;

        SelectedApp.StatusText = "Refreshing tree...";
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

        if (SelectedApp != null)
        {
            SelectedApp.StatusText = $"Updated {property.Path} = {newValue}";
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

        if (SelectedApp != null)
        {
            SelectedApp.StatusText = $"Updated {property.Name}";
        }
    }

    public void CloseApp(string appId)
    {
        _logger.LogDebug("Closing app: {AppId}", appId);

        if (_apps.TryGetValue(appId, out var app))
        {
            app.Dispose();
            _apps.Remove(appId);
        }

        var tabToRemove = AppTabs.FirstOrDefault(t => t.Header == $"● {appId}");
        if (tabToRemove != null)
        {
            var index = AppTabs.IndexOf(tabToRemove);
            AppTabs.Remove(tabToRemove);

            if (SelectedAppTabIndex == index)
            {
                SelectedAppTabIndex = AppTabs.Count > 0 ? 0 : -1;
            }
            else if (SelectedAppTabIndex > index)
            {
                SelectedAppTabIndex--;
            }
        }

        UpdateStatusText();
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
