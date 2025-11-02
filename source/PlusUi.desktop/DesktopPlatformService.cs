using PlusUi.core;
using PlusUi.core.Services;
using Silk.NET.Windowing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PlusUi.desktop;

/// <summary>
/// Desktop platform service implementation
/// </summary>
public class DesktopPlatformService : IPlatformService
{
    private readonly RenderService _renderService;
    private IWindow? _window;

    public DesktopPlatformService(RenderService renderService)
    {
        _renderService = renderService;
    }

    /// <summary>
    /// Sets the window reference (called by WindowManager after window is created)
    /// </summary>
    internal void SetWindow(IWindow window)
    {
        _window = window;
    }

    public PlatformType Platform => PlatformType.Desktop;

    public Size WindowSize
    {
        get
        {
            if (_window == null)
            {
                return new Size(0, 0);
            }
            return new Size(_window.Size.X, _window.Size.Y);
        }
    }

    public float DisplayDensity => _renderService.DisplayDensity;

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
