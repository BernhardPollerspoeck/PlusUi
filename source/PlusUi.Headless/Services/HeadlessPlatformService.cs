using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.Headless.Services;

/// <summary>
/// Headless platform service implementation.
/// Provides platform-specific information for headless rendering environment.
/// </summary>
public class HeadlessPlatformService : IPlatformService
{
    private Size _windowSize;

    public HeadlessPlatformService(Size initialSize)
    {
        _windowSize = initialSize;
    }

    public PlatformType Platform => PlatformType.Headless;

    public Size WindowSize
    {
        get => _windowSize;
        set => _windowSize = value;
    }

    public float DisplayDensity => 1.0f;

    public bool OpenUrl(string url)
    {
        // Headless environment has no browser
        return false;
    }
}
