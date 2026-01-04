using PlusUi.DebugServer.Services;

Console.WriteLine("PlusUi Debug Bridge Server");
Console.WriteLine("==========================");

var server = new DebugBridgeServer(port: 5555);

// Subscribe to events
server.ClientConnected += (s, clientId) =>
{
    Console.WriteLine($"[Connected] Client: {clientId}");
};

server.ClientDisconnected += (s, clientId) =>
{
    Console.WriteLine($"[Disconnected] Client: {clientId}");
};

server.ClientMessageReceived += (s, e) =>
{
    Console.WriteLine($"[Message] Client: {e.ClientId}, Type: {e.Message.Type}");
};

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
