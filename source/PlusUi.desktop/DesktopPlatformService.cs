using PlusUi.core;
using PlusUi.core.Services;
using Silk.NET.Windowing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace PlusUi.desktop;

/// <summary>
/// Desktop platform service implementation
/// </summary>
public class DesktopPlatformService : IPlatformService
{
    private readonly RenderService _renderService;
    private readonly ILogger<DesktopPlatformService>? _logger;
    private IWindow? _window;

    public DesktopPlatformService(RenderService renderService, ILogger<DesktopPlatformService>? logger = null)
    {
        _renderService = renderService;
        _logger = logger;
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
            _logger?.LogWarning("OpenUrl called with null or empty URL");
            return false;
        }

        // Validate URL scheme to prevent command injection
        if (!IsValidUrlScheme(url))
        {
            _logger?.LogWarning("OpenUrl called with invalid URL scheme: {Url}", url);
            return false;
        }

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                _logger?.LogDebug("Opened URL on Windows: {Url}", url);
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
                _logger?.LogDebug("Opened URL on Linux: {Url}", url);
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
                _logger?.LogDebug("Opened URL on macOS: {Url}", url);
                return true;
            }
            else
            {
                // Fallback for other platforms
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                _logger?.LogDebug("Opened URL on unknown platform: {Url}", url);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to open URL: {Url}", url);
            return false;
        }
    }

    /// <summary>
    /// Validates that the URL has a safe scheme to prevent command injection
    /// </summary>
    private static bool IsValidUrlScheme(string url)
    {
        // Allow common URL schemes
        var allowedSchemes = new[]
        {
            "http://", "https://", "mailto:", "file://", "ftp://", "ftps://",
            "tel:", "sms:", "news:", "irc:", "steam://", "discord://"
        };

        foreach (var scheme in allowedSchemes)
        {
            if (url.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        // For URIs without explicit scheme, try to parse as URI
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            // Additional validation for the parsed URI
            var validSchemes = new[] { "http", "https", "mailto", "file", "ftp", "ftps", "tel", "sms", "news", "irc", "steam", "discord" };
            return validSchemes.Contains(uri.Scheme.ToLowerInvariant());
        }

        return false;
    }
}
