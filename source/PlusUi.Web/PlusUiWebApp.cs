using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// Main entry point for PlusUi Web applications using Blazor WebAssembly.
/// Usage:
/// <code>
/// var builder = WebAssemblyHostBuilder.CreateDefault(args);
/// var app = new PlusUiWebApp(builder);
/// app.CreateApp(b => new App());
/// </code>
/// </summary>
public class PlusUiWebApp(WebAssemblyHostBuilder builder)
{
    public async Task CreateApp(Func<HostApplicationBuilder, IAppConfiguration> appBuilder)
    {
        // Let the user configure their app
        //var appConfig = appBuilder(hostBuilder);
        
        //// Register PlusUi core services
        //builder.UsePlusUiInternal(appConfig, Array.Empty<string>());
        
        //// Register web-specific services
        //builder.Services.AddSingleton<WebKeyboardHandler>();
        //builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<WebKeyboardHandler>());

        //// Configure the app (pages, styles, etc.)
        //builder.ConfigurePlusUiApp(appConfig);
        
        // Add root component
        builder.RootComponents.Add<PlusUiRootComponent>("#app");
        
        var host = builder.Build();
        await host.RunAsync();
    }
}
