using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.ios;

/// <summary>
/// iOS platform service implementation
/// </summary>
public class IosPlatformService(RenderService renderService) : IPlatformService
{
    private Size _windowSize = new(0, 0);

    /// <summary>
    /// Updates the window size (called by OpenGlViewController when view layout changes)
    /// </summary>
    internal void SetWindowSize(float width, float height)
    {
        _windowSize = new Size(width, height);
    }

    public PlatformType Platform => PlatformType.iOS;

    public Size WindowSize => _windowSize;

    public float DisplayDensity => renderService.DisplayDensity;

    public bool OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        try
        {
            var nsUrl = new NSUrl(url);
            if (nsUrl == null)
            {
                return false;
            }

            // For iOS 10+ we use UIApplication.SharedApplication.OpenUrl with completion handler
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UIApplication.SharedApplication.OpenUrl(nsUrl, new UIApplicationOpenUrlOptions(), null);
                return true;
            }
            else
            {
                // Fallback for older iOS versions
#pragma warning disable CA1422 // Validate platform compatibility
                return UIApplication.SharedApplication.OpenUrl(nsUrl);
#pragma warning restore CA1422
            }
        }
        catch
        {
            return false;
        }
    }
}
