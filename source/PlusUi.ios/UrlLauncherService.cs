using Foundation;
using PlusUi.core.Services;
using UIKit;

namespace PlusUi.ios;

/// <summary>
/// iOS implementation of URL launcher service
/// </summary>
public class UrlLauncherService : IUrlLauncherService
{
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
