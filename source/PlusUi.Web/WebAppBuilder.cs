using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// Wrapper for WebAssemblyHostBuilder implementing IPlusUiAppBuilder.
/// </summary>
public class WebAppBuilder(WebAssemblyHostBuilder builder) : IPlusUiAppBuilder
{
    public IServiceCollection Services => builder.Services;
    public WebAssemblyHostBuilder Inner => builder;
}
