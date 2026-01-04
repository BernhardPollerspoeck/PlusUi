using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Services;

namespace PlusUi.DebugServer.Pages;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly DebugBridgeServer _server;

    [ObservableProperty]
    private string _statusText = "Waiting for client...";

    [ObservableProperty]
    private ObservableCollection<TreeNodeDto> _rootItems = new();

    [ObservableProperty]
    private TreeNodeDto? _selectedNode;

    public ObservableCollection<PropertyDto> SelectedProperties { get; } = new();

    public MainViewModel(DebugBridgeServer server)
    {
        Console.WriteLine("[VIEWMODEL] Constructor called");
        _server = server;

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

    private async void OnClientConnected(object? sender, string clientId)
    {
        StatusText = $"Client connected: {clientId}";

        // Auto-request tree
        await Task.Delay(500);
        await _server.SendToClientAsync(clientId, new DebugMessage { Type = "get_tree" });
    }

    private void OnClientDisconnected(object? sender, string clientId)
    {
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
                // Deserialize TreeNodeDto from JsonElement
                var json = JsonSerializer.Serialize(e.Message.Data);
                var tree = JsonSerializer.Deserialize<TreeNodeDto>(json);

                if (tree != null)
                {
                    RootItems.Clear();
                    RootItems.Add(tree);
                    StatusText = $"Tree received from {e.ClientId} - {CountNodes(tree)} elements";
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

    public void Dispose()
    {
        Console.WriteLine("[VIEWMODEL] Dispose called - unsubscribing from events");
        // Unsubscribe from events to prevent memory leaks
        _server.ClientConnected -= OnClientConnected;
        _server.ClientDisconnected -= OnClientDisconnected;
        _server.ClientMessageReceived -= OnClientMessageReceived;
    }
}
