using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.Headless.Enumerations;
using PlusUi.Headless.Services;
using Silk.NET.Maths;

namespace PlusUi.Headless;

/// <summary>
/// Factory class for creating isolated PlusUi Headless instances.
/// Each instance has its own internal ServiceProvider and is completely isolated.
/// </summary>
public static class PlusUiHeadless
{
    /// <summary>
    /// Creates a new isolated headless instance.
    /// </summary>
    /// <param name="appConfiguration">App configuration (standard IAppConfiguration)</param>
    /// <param name="format">Output format for frames (default: PNG)</param>
    /// <returns>Headless instance with its own isolated ServiceProvider</returns>
    public static IPlusUiHeadlessService Create(
        IAppConfiguration appConfiguration,
        ImageFormat format = ImageFormat.Png)
    {
        // Create internal host
        var builder = Host.CreateApplicationBuilder();

        // Register core PlusUi services
        builder.UsePlusUiInternal(appConfiguration, args: []);

        // Extract frame size from PlusUiConfiguration
        var plusUiConfig = new PlusUiConfiguration();
        appConfiguration.ConfigureWindow(plusUiConfig);
        var frameSize = new Size(plusUiConfig.Size.Width, plusUiConfig.Size.Height);

        // Headless Platform Service
        var platformService = new HeadlessPlatformService(frameSize);
        builder.Services.AddSingleton(platformService);
        builder.Services.AddSingleton<IPlatformService>(platformService);

        // Headless Keyboard Handler
        var keyboardHandler = new HeadlessKeyboardHandler();
        builder.Services.AddSingleton(keyboardHandler);
        builder.Services.AddSingleton<IKeyboardHandler>(keyboardHandler);

        // Headless Service Implementation (internal)
        builder.Services.AddSingleton<PlusUiHeadlessService>();

        // Let user-app configure
        builder.ConfigurePlusUiApp(appConfiguration);

        // Build and start host
        var host = builder.Build();
        host.Start(); // Start synchronously

        // Return wrapper that implements IPlusUiHeadlessService
        // and internally manages the host
        var headlessService = host.Services.GetRequiredService<PlusUiHeadlessService>();
        return new PlusUiHeadlessWrapper(host, headlessService, frameSize, format);
    }
}
