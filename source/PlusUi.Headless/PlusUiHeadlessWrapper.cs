using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.Headless.Enumerations;
using PlusUi.Headless.Services;

namespace PlusUi.Headless;

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
