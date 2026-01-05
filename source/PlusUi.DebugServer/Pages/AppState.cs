using System.Collections.ObjectModel;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.DebugServer.Pages;

public class AppState
{
    public string ClientId { get; }
    public bool IsConnected { get; set; }
    public bool TreeReceived { get; set; }
    public ObservableCollection<TreeNodeDto> RootItems { get; } = new();
    public TreeNodeDto? SelectedNode { get; set; }
    public ObservableCollection<PropertyDto> SelectedProperties { get; } = new();
    public string StatusText { get; set; } = "Initializing...";
    public Timer? RetryTimer { get; set; }

    public AppState(string clientId)
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
