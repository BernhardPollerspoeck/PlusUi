using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PlusUi.core.Services;

/// <summary>
/// Default implementation of URL launcher service for desktop platforms
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
                return true;
            }
            else
            {
                // Fallback for other platforms
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
