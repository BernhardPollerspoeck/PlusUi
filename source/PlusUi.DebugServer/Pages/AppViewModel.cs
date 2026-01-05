using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.DebugServer.Pages;

public partial class AppViewModel : ObservableObject, IDisposable
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

    public ObservableCollection<TreeNodeDto> RootItems { get; } = new();
    public ObservableCollection<PropertyDto> SelectedProperties { get; } = new();

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
