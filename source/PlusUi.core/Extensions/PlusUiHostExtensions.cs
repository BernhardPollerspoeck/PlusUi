using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core.Services.DebugBridge;

namespace PlusUi.core;

public static class PlusUiHostExtensions
{
    /// <summary>
    /// Initializes PlusUi services. Call this after Build() and before Run().
    /// </summary>
    public static IHost InitializePlusUi(this IHost host)
    {
        InitializePlusUi(host.Services);
        return host;
    }

    /// <summary>
    /// Initializes PlusUi services from a service provider.
    /// Use this for platforms that don't use IHost (e.g., Blazor WebAssembly).
    /// </summary>
    public static void InitializePlusUi(IServiceProvider services)
    {
        // Ensure ServiceProviderService is instantiated (sets static ServiceProvider)
        services.GetRequiredService<ServiceProviderService>();
        // Configure application styles
        var style = services.GetRequiredService<Style>();
        services.GetService<IApplicationStyle>()?.ConfigureStyle(style);

        // Initialize Debug Bridge if enabled
        services.InitializeDebugBridgeIfEnabled();
    }

    /// <summary>
    /// Initializes debug bridge if it was enabled via EnableDebugBridge().
    /// </summary>
    private static void InitializeDebugBridgeIfEnabled(this IServiceProvider services)
    {
        // Force resolution to trigger client creation and connection (if registered)
        // Returns null if EnableDebugBridge() was not called - that's OK
        _ = services.GetService<DebugBridgeClient>();
    }
}