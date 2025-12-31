using Microsoft.Extensions.DependencyInjection;

namespace PlusUi.core;

/// <summary>
/// Abstraction over platform-specific host builders (HostApplicationBuilder, WebAssemblyHostBuilder, etc.).
/// Provides a common interface for configuring PlusUi applications across all platforms.
/// </summary>
public interface IPlusUiAppBuilder
{
    /// <summary>
    /// Gets the service collection for registering dependencies.
    /// </summary>
    IServiceCollection Services { get; }
}
