using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.desktop;
using PlusUi.DebugServer;
using PlusUi.DebugServer.Pages;
using PlusUi.DebugServer.Services;

var plusUiApp = new PlusUiApp(args);

plusUiApp.CreateApp(builder =>
{
    // Register DebugBridgeServer as singleton
    builder.Services.AddSingleton(sp =>
    {
        var server = new DebugBridgeServer(port: 5555);

        // Start server automatically
        _ = server.StartAsync();

        return server;
    });

    // Create app configuration
    return new DebugServerApp();
});
