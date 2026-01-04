using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services.DebugBridge;
using PlusUi.core.Services.Rendering;

namespace PlusUi.core;

/// <summary>
/// Extension methods for enabling debug bridge functionality.
/// </summary>
public static class DebugBridgeExtensions
{
    /// <summary>
    /// Enables debug bridge client that connects to a debug server.
    /// RECOMMENDED: Wrap this call in #if DEBUG to prevent it in release builds.
    /// Example:
    /// <code>
    /// #if DEBUG
    /// builder.EnableDebugBridge("192.168.1.100", 5555);
    /// #endif
    /// </code>
    /// Minimal overhead when disabled (host = null).
    /// </summary>
    /// <param name="builder">The PlusUi app builder.</param>
    /// <param name="host">Server host (IP address or hostname). If null or empty, debug bridge is disabled.</param>
    /// <param name="port">Server port (default: 5555).</param>
    /// <returns>The builder for method chaining.</returns>
    public static IPlusUiAppBuilder EnableDebugBridge(
        this IPlusUiAppBuilder builder,
        string? host = null,
        int port = 5555)
    {
        // Disabled if host not provided
        if (string.IsNullOrWhiteSpace(host))
            return builder;

        var serverUrl = $"ws://{host}:{port}";

        // Register DebugBridgeClient as singleton
        builder.Services.AddSingleton(sp =>
        {
            var navigationContainer = sp.GetRequiredService<NavigationContainer>();
            var invalidationTracker = sp.GetRequiredService<InvalidationTracker>();
            var logger = sp.GetService<ILogger<DebugBridgeClient>>();

            var client = new DebugBridgeClient(serverUrl, navigationContainer, invalidationTracker, logger);

            // Connect asynchronously
            _ = client.ConnectAsync();

            return client;
        });

        return builder;
    }
}
