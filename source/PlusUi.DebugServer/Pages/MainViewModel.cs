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
using PlusUi.DebugServer.Components;
using PlusUi.DebugServer.Services;

namespace PlusUi.DebugServer.Pages;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly DebugBridgeServer _server;
    private readonly IPopupService _popupService;
    private readonly ILogger<MainViewModel> _logger;
    private readonly PinnedPropertiesService _pinnedPropertiesService;
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
    public ObservableCollection<LogMessageDto> FilteredLogs { get; } = new();

    [ObservableProperty]
    private int _rootItemsCount;

    [ObservableProperty]
    private LogLevel _logLevelFilter = LogLevel.Trace;

    public bool HasConnectedApps => AppTabs.Count > 0;
    public PinnedPropertiesService PinnedPropertiesService => _pinnedPropertiesService;

    public MainViewModel(DebugBridgeServer server, IPopupService popupService, ILogger<MainViewModel> logger, PinnedPropertiesService pinnedPropertiesService)
    {
        _server = server;
        _popupService = popupService;
        _logger = logger;
        _pinnedPropertiesService = pinnedPropertiesService;

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
        _logger.LogDebug("SelectedAppTabIndex changed to {Index}", value);

        if (value >= 0 && value < AppTabs.Count)
        {
            var tab = AppTabs[value];
            var clientId = tab.Tag as string;
            _logger.LogDebug("Tab selected: Header={Header}, Tag={Tag}", tab.Header, clientId);
            SelectedAppId = clientId;
        }
        else
        {
            _logger.LogDebug("No tab selected (index out of range)");
            SelectedAppId = null;
        }
    }

    partial void OnSelectedAppChanged(AppViewModel? oldValue, AppViewModel? newValue)
    {
        _logger.LogDebug("SelectedApp changed: {OldApp} -> {NewApp}",
            oldValue?.ClientId ?? "null", newValue?.ClientId ?? "null");

        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnAppPropertyChanged;
            oldValue.RootItems.CollectionChanged -= OnAppRootItemsChanged;
            oldValue.SelectedProperties.CollectionChanged -= OnAppSelectedPropertiesChanged;
            oldValue.FilteredLogs.CollectionChanged -= OnAppFilteredLogsChanged;
        }

        if (newValue != null)
        {
            _logger.LogDebug("Subscribing to app {ClientId} events, RootItems.Count={Count}",
                newValue.ClientId, newValue.RootItems.Count);
            newValue.PropertyChanged += OnAppPropertyChanged;
            newValue.RootItems.CollectionChanged += OnAppRootItemsChanged;
            newValue.SelectedProperties.CollectionChanged += OnAppSelectedPropertiesChanged;
            newValue.FilteredLogs.CollectionChanged += OnAppFilteredLogsChanged;
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
        _logger.LogDebug("OnAppRootItemsChanged: Action={Action}, NewItems={NewCount}, OldItems={OldCount}",
            e.Action, e.NewItems?.Count ?? 0, e.OldItems?.Count ?? 0);

        if (SelectedApp == null)
        {
            _logger.LogWarning("OnAppRootItemsChanged called but SelectedApp is null");
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            _logger.LogDebug("Clearing MainViewModel.RootItems");
            RootItems.Clear();
        }
        else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            _logger.LogDebug("Adding {Count} items to MainViewModel.RootItems", e.NewItems.Count);
            foreach (TreeNodeDto item in e.NewItems)
            {
                RootItems.Add(item);
                _logger.LogDebug("Added tree node: Type={Type}, Id={Id}", item.Type, item.Id);
            }
            _logger.LogDebug("MainViewModel.RootItems.Count is now {Count}", RootItems.Count);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (TreeNodeDto item in e.OldItems)
            {
                RootItems.Remove(item);
            }
        }

        RootItemsCount = RootItems.Count;
        _logger.LogDebug("Updated RootItemsCount to {Count}", RootItemsCount);
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

    private void OnAppFilteredLogsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (SelectedApp == null) return;

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            FilteredLogs.Clear();
        }
        else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is LogMessageDto log)
                    FilteredLogs.Add(log);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is LogMessageDto log)
                    FilteredLogs.Remove(log);
            }
        }
    }

    partial void OnLogLevelFilterChanged(LogLevel value)
    {
        if (SelectedApp != null)
            SelectedApp.LogLevelFilter = value;
    }

    private void SyncCollectionsFromSelectedApp()
    {
        _logger.LogDebug("SyncCollectionsFromSelectedApp: SelectedApp={AppId}, RootItems.Count={Count}",
            SelectedApp?.ClientId ?? "null", SelectedApp?.RootItems.Count ?? 0);

        RootItems.Clear();
        SelectedProperties.Clear();
        FilteredLogs.Clear();

        if (SelectedApp != null)
        {
            _logger.LogDebug("Syncing {Count} RootItems from app {ClientId}", SelectedApp.RootItems.Count, SelectedApp.ClientId);
            foreach (var item in SelectedApp.RootItems)
            {
                RootItems.Add(item);
                _logger.LogDebug("Synced tree node: Type={Type}, Id={Id}", item.Type, item.Id);
            }
            _logger.LogDebug("MainViewModel.RootItems.Count after sync: {Count}", RootItems.Count);

            SelectedNode = SelectedApp.SelectedNode;

            foreach (var prop in SelectedApp.SelectedProperties)
            {
                SelectedProperties.Add(prop);
            }

            LogLevelFilter = SelectedApp.LogLevelFilter;
            foreach (var log in SelectedApp.FilteredLogs)
            {
                if (log != null)
                    FilteredLogs.Add(log);
            }
        }
        else
        {
            _logger.LogDebug("SelectedApp is null, clearing collections");
            SelectedNode = null;
        }

        RootItemsCount = RootItems.Count;
        _logger.LogDebug("Updated RootItemsCount to {Count} after sync", RootItemsCount);

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

    private async void OnClientConnected(object? sender, string clientId)
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
            .SetTag(clientId)  // Store clientId in Tag
            .SetContent(new AppContentView(this));

        AppTabs.Add(tabItem);

        if (SelectedAppTabIndex < 0)
        {
            SelectedAppTabIndex = 0;
        }

        // Send immediate tree request
        _logger.LogDebug("Sending initial get_tree request to {ClientId}", clientId);
        await _server.SendToClientAsync(clientId, new DebugMessage { Type = "get_tree" });

        app.RetryTimer = new Timer(async _ =>
        {
            if (!app.TreeReceived && app.IsConnected)
            {
                _logger.LogDebug("Retry: Sending get_tree request to {ClientId}", clientId);
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

        var tabToRemove = AppTabs.FirstOrDefault(t => t.Tag as string == clientId);
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
        {
            _logger.LogWarning("Received message from unknown client: {ClientId}", e.ClientId);
            return;
        }

        switch (e.Message.Type)
        {
            case "ui_tree" when e.Message.Data != null:
                try
                {
                    var json = JsonSerializer.Serialize(e.Message.Data);
                    var tree = JsonSerializer.Deserialize<TreeNodeDto>(json);

                    if (tree != null)
                    {
                        var nodeCount = CountNodes(tree);

                        app.RootItems.Clear();
                        app.RootItems.Add(tree);

                        app.SelectedNode = null;
                        app.StatusText = $"Tree received - {nodeCount} elements";
                        app.TreeReceived = true;
                        app.RetryTimer?.Dispose();
                        app.RetryTimer = null;
                    }
                    else
                    {
                        _logger.LogWarning("Tree deserialization returned null");
                    }
                }
                catch (Exception ex)
                {
                    app.StatusText = $"Error parsing tree: {ex.Message}";
                    _logger.LogError(ex, "Error parsing tree from {ClientId}", e.ClientId);
                }
                break;

            case "log" when e.Message.Data != null:
                try
                {
                    var json = JsonSerializer.Serialize(e.Message.Data);
                    var logMessage = JsonSerializer.Deserialize<LogMessageDto>(json);
                    if (logMessage != null)
                    {
                        app.AddLog(logMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing log message from {ClientId}", e.ClientId);
                }
                break;

            case "log_batch" when e.Message.Data != null:
                try
                {
                    var json = JsonSerializer.Serialize(e.Message.Data);
                    var logMessages = JsonSerializer.Deserialize<List<LogMessageDto>>(json);
                    if (logMessages != null)
                    {
                        foreach (var logMessage in logMessages)
                        {
                            app.AddLog(logMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing log batch from {ClientId}", e.ClientId);
                }
                break;
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

    public void RefreshProperties()
    {
        OnPropertyChanged(nameof(SelectedProperties));
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
