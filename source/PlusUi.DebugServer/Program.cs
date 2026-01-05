using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.desktop;
using PlusUi.DebugServer;
using PlusUi.DebugServer.Pages;
using PlusUi.DebugServer.Services;

var plusUiApp = new PlusUiApp(args);

plusUiApp.CreateApp(builder =>
{
    // Add Debug logging to console
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);

    builder.Services.AddSingleton<PinnedPropertiesService>();

    builder.Services.AddSingleton(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<DebugBridgeServer>>();
        var server = new DebugBridgeServer(logger, port: 5555);

        _ = server.StartAsync();

        return server;
    });

    return new DebugServerApp();
});
