using Android.Content;
using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.droid;

/// <summary>
/// Android platform service implementation
/// </summary>
public class AndroidPlatformService(RenderService renderService, Context context) : IPlatformService
{
    private Size _windowSize = new Size(0, 0);

    /// <summary>
    /// Updates the window size (called by SilkRenderer when surface changes)
    /// </summary>
    internal void SetWindowSize(int width, int height)
    {
        _windowSize = new Size(width, height);
    }

    public PlatformType Platform => PlatformType.Android;

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
            var uri = Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);

            // Add FLAG_ACTIVITY_NEW_TASK if we're not in an Activity context
            intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
