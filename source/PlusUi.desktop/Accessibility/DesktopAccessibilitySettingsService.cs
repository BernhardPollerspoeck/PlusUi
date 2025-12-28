using PlusUi.core.Services.Accessibility;
using PlusUi.desktop.Interop;
using System.Runtime.InteropServices;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// Desktop implementation of accessibility settings service.
/// Detects high contrast mode, font scaling, and reduced motion preferences.
/// </summary>
public class DesktopAccessibilitySettingsService : AccessibilitySettingsService
{
    public DesktopAccessibilitySettingsService()
    {
        RefreshSettings();
    }

    /// <inheritdoc />
    public override void RefreshSettings()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            RefreshWindowsSettings();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            RefreshMacOSSettings();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            RefreshLinuxSettings();
        }
    }

    private void RefreshWindowsSettings()
    {
        try
        {
            // Check Windows high contrast mode
            var highContrast = new WindowsAccessibilityInterop.HIGHCONTRAST();
            highContrast.cbSize = (uint)Marshal.SizeOf(highContrast);
            if (WindowsAccessibilityInterop.SystemParametersInfo(
                WindowsAccessibilityInterop.SPI_GETHIGHCONTRAST,
                highContrast.cbSize,
                ref highContrast,
                0))
            {
                SetHighContrastEnabled((highContrast.dwFlags & WindowsAccessibilityInterop.HCF_HIGHCONTRASTON) != 0);
            }

            // Check Windows text scale factor from registry
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Accessibility");
                if (key != null)
                {
                    var textScaleFactor = key.GetValue("TextScaleFactor");
                    if (textScaleFactor is int scaleFactor)
                    {
                        SetFontScaleFactor(scaleFactor / 100f);
                    }
                }
            }
            catch
            {
                // Registry access failed - use default
            }

            // Check reduce animations setting
            bool animationsEnabled = true;
            if (WindowsAccessibilityInterop.SystemParametersInfo(
                WindowsAccessibilityInterop.SPI_GETCLIENTAREAANIMATION,
                0,
                ref animationsEnabled,
                0))
            {
                SetReducedMotionEnabled(!animationsEnabled);
            }
        }
        catch
        {
            // Platform API calls failed - use defaults
        }
    }

    private void RefreshMacOSSettings()
    {
        // macOS accessibility settings would be read via NSUserDefaults or accessibility APIs
        // For now, use defaults - can be enhanced with proper macOS bindings
        SetHighContrastEnabled(false);
        SetFontScaleFactor(1.0f);
        SetReducedMotionEnabled(false);
    }

    private void RefreshLinuxSettings()
    {
        // Linux accessibility settings from GTK/GNOME settings
        // Check environment variable or gsettings
        try
        {
            var highContrast = Environment.GetEnvironmentVariable("GTK_THEME")?.Contains("HighContrast") ?? false;
            SetHighContrastEnabled(highContrast);

            // Check GNOME text scaling factor
            var textScaling = Environment.GetEnvironmentVariable("GDK_DPI_SCALE");
            if (float.TryParse(textScaling, out var scale))
            {
                SetFontScaleFactor(scale);
            }

            // Check reduced motion
            var reduceMotion = Environment.GetEnvironmentVariable("GTK_ENABLE_ANIMATIONS") == "0";
            SetReducedMotionEnabled(reduceMotion);
        }
        catch
        {
            // Use defaults
        }
    }
}
