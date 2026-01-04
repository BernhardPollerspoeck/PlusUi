using System.Text.Json;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Services;

Console.WriteLine("PlusUi Debug Bridge Server");
Console.WriteLine("==========================");

var server = new DebugBridgeServer(port: 5555);

// Subscribe to events
server.ClientConnected += async (s, clientId) =>
{
    Console.WriteLine($"[Connected] Client: {clientId}");

    // Auto-request tree on connection
    await Task.Delay(500); // Give client time to initialize
    await server.SendToClientAsync(clientId, new DebugMessage { Type = "get_tree" });
    Console.WriteLine($"[Command] Sent get_tree to {clientId}");
};

server.ClientDisconnected += (s, clientId) =>
{
    Console.WriteLine($"[Disconnected] Client: {clientId}");
};

server.ClientMessageReceived += (s, e) =>
{
    Console.WriteLine($"[Message] Client: {e.ClientId}, Type: {e.Message.Type}");

    // Handle tree data
    if (e.Message.Type == "ui_tree" && e.Message.Data != null)
    {
        try
        {
            // Deserialize TreeNodeDto from JsonElement
            var json = JsonSerializer.Serialize(e.Message.Data);
            var tree = JsonSerializer.Deserialize<TreeNodeDto>(json);

            if (tree != null)
            {
                Console.WriteLine("\n=== UI TREE ===");
                PrintTree(tree, 0);
                Console.WriteLine("===============\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to parse tree: {ex.Message}");
        }
    }
};

static void PrintTree(TreeNodeDto node, int depth)
{
    var indent = new string(' ', depth * 2);
    Console.WriteLine($"{indent}{node.Type} (ID: {node.Id}, Props: {node.Properties.Count})");

    foreach (var child in node.Children)
    {
        PrintTree(child, depth + 1);
    }
}

// Start server
await server.StartAsync();

Console.WriteLine("Server started. Press CTRL+C to stop...");
Console.WriteLine();

// Wait for cancellation
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (TaskCanceledException)
{
    // Expected
}

Console.WriteLine("Stopping server...");
await server.StopAsync();
server.Dispose();
Console.WriteLine("Server stopped.");
