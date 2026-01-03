using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlusUi.core;

/// <summary>
/// Default wrapper for HostApplicationBuilder implementing IPlusUiAppBuilder.
/// Used by Desktop, Headless, h264, iOS, and Android platforms.
/// </summary>
public class HostAppBuilder(HostApplicationBuilder builder) : IPlusUiAppBuilder
{
    public IServiceCollection Services => builder.Services;
    public HostApplicationBuilder Inner => builder;
}
