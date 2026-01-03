using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// Extension methods for WebAssemblyHostBuilder to support PlusUi.
/// </summary>
public static class WebAssemblyHostBuilderExtensions
{
    public static WebAssemblyHostBuilder ConfigurePlusUiApp(
        this WebAssemblyHostBuilder builder,
        IAppConfiguration appConfiguration)
    {
        builder.Services.Configure<PlusUiConfiguration>(appConfiguration.ConfigureWindow);
        appConfiguration.ConfigureApp(new WebAppBuilder(builder));
        return builder;
    }
}
