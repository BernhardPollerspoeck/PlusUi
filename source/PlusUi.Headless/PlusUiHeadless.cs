using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
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
        builder.UsePlusUiInternal(appConfiguration, args: null);

        // Extract frame size from PlusUiConfiguration
        var plusUiConfig = new PlusUiConfiguration();
        appConfiguration.ConfigureWindow(plusUiConfig);
        var frameSize = new Size(plusUiConfig.Size.X, plusUiConfig.Size.Y);

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

/// <summary>
/// Wrapper class that implements IPlusUiHeadlessService and manages the internal host.
/// </summary>
internal class PlusUiHeadlessWrapper : IPlusUiHeadlessService, IDisposable
{
    private readonly IHost _host;
    private readonly PlusUiHeadlessService _headlessService;

    internal PlusUiHeadlessWrapper(
        IHost host,
        PlusUiHeadlessService headlessService,
        Size frameSize,
        ImageFormat format)
    {
        _host = host;
        _headlessService = headlessService;
        // Initialize with frame size and format
        _headlessService.Initialize(frameSize, format);
    }

    public Task<byte[]> GetCurrentFrameAsync() => _headlessService.GetCurrentFrameAsync();
    public void MouseMove(float x, float y) => _headlessService.MouseMove(x, y);
    public void MouseDown() => _headlessService.MouseDown();
    public void MouseUp() => _headlessService.MouseUp();
    public void MouseWheel(float deltaX, float deltaY) => _headlessService.MouseWheel(deltaX, deltaY);
    public void KeyPress(PlusKey key) => _headlessService.KeyPress(key);
    public void CharInput(char c) => _headlessService.CharInput(c);

    public void Dispose()
    {
        _host?.StopAsync().GetAwaiter().GetResult();
        _host?.Dispose();
    }
}
