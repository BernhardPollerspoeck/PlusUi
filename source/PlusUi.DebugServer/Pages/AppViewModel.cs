using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.DebugServer.Pages;

internal partial class AppViewModel : ObservableObject, IDisposable
{
    public string ClientId { get; }

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private bool _treeReceived;

    [ObservableProperty]
    private TreeNodeDto? _selectedNode;

    [ObservableProperty]
    private string _statusText = "Initializing...";

    [ObservableProperty]
    private LogLevel _logLevelFilter = LogLevel.Trace;

    public ObservableCollection<TreeNodeDto> RootItems { get; } = [];
    public ObservableCollection<PropertyDto> SelectedProperties { get; } = [];
    public ObservableCollection<LogMessageDto> Logs { get; } = [];
    public ObservableCollection<LogMessageDto> FilteredLogs { get; } = [];

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

    partial void OnLogLevelFilterChanged(LogLevel value)
    {
        UpdateFilteredLogs();
    }

    public void AddLog(LogMessageDto log)
    {
        if (log == null)
            return;

        Logs.Add(log);

        // Apply filter and add to filtered list if it passes
        if (log.Level >= LogLevelFilter)
        {
            FilteredLogs.Add(log);
        }

        // Limit log count to prevent memory issues (keep last 1000)
        while (Logs.Count > 1000)
        {
            var removed = Logs[0];
            Logs.RemoveAt(0);
            if (removed != null)
            {
                FilteredLogs.Remove(removed);
            }
        }
    }

    public void ClearLogs()
    {
        Logs.Clear();
        FilteredLogs.Clear();
    }

    private void UpdateFilteredLogs()
    {
        FilteredLogs.Clear();
        foreach (var log in Logs)
        {
            if (log != null && log.Level >= LogLevelFilter)
            {
                FilteredLogs.Add(log);
            }
        }
    }

    public Timer? RetryTimer { get; set; }

    public AppViewModel(string clientId)
    {
        ClientId = clientId;
        IsConnected = true;
        StatusText = $"Connected: {clientId}";
    }

    public void Dispose()
    {
        RetryTimer?.Dispose();
        RetryTimer = null;
    }
}
