using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.Web;

/// <summary>
/// Main entry point for PlusUi Web applications using Blazor WebAssembly.
/// Usage:
/// <code>
/// var builder = WebAssemblyHostBuilder.CreateDefault(args);
/// var app = new PlusUiWebApp(builder);
/// await app.CreateApp(() => new App());
/// </code>
/// </summary>
public class PlusUiWebApp(WebAssemblyHostBuilder builder)
{
    public async Task CreateApp(Func<IAppConfiguration> appFactory)
    {
        // Create the app configuration
        var appConfig = appFactory();

        // Register PlusUi core services (shared with all platforms)
        builder.Services.AddPlusUiCore(appConfig, []);

        // Register web-specific services
        // Platform service
        builder.Services.AddSingleton<WebPlatformService>();
        builder.Services.AddSingleton<IPlatformService>(sp => sp.GetRequiredService<WebPlatformService>());

        // Keyboard handler
        builder.Services.AddSingleton<WebKeyboardHandler>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<WebKeyboardHandler>());

        // Haptic service
        builder.Services.AddSingleton<WebHapticService>();
        builder.Services.AddSingleton<IHapticService>(sp => sp.GetRequiredService<WebHapticService>());

        // Configure the app (pages, styles, etc.)
        builder.ConfigurePlusUiApp(appConfig);

        // Add root component
        builder.RootComponents.Add<PlusUiRootComponent>("#app");

        var host = builder.Build();
        PlusUiHostExtensions.InitializePlusUi(host.Services);
        await host.RunAsync();
    }
}
