using Foundation;
using PlusUi.core.Services.Accessibility;
using UIKit;

namespace PlusUi.ios.Accessibility;

/// <summary>
/// iOS implementation of accessibility settings service.
/// </summary>
public class IosAccessibilitySettingsService : AccessibilitySettingsService, IDisposable
{
    private NSObject? _contentSizeObserver;
    private bool _disposed;

    public IosAccessibilitySettingsService()
    {
        RefreshSettings();
        SubscribeToNotifications();
    }

    private void SubscribeToNotifications()
    {
        // Subscribe to content size category change notifications
        _contentSizeObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.ContentSizeCategoryChangedNotification,
            _ => RefreshSettings());
    }

    /// <inheritdoc />
    public override void RefreshSettings()
    {
        try
        {
            // Check for bold text as a proxy for high contrast preference
            var isBoldTextEnabled = UIAccessibility.IsBoldTextEnabled;
            SetHighContrastEnabled(isBoldTextEnabled);

            // Get Dynamic Type font scale from content size category
            var contentSizeString = UIApplication.SharedApplication.PreferredContentSizeCategory?.ToString();
            var fontScale = GetFontScaleForContentSize(contentSizeString);
            SetFontScaleFactor(fontScale);

            // Check reduced motion
            SetReducedMotionEnabled(UIAccessibility.IsReduceMotionEnabled);
        }
        catch
        {
            // Settings access failed - use defaults
        }
    }

    private float GetFontScaleForContentSize(string? category)
    {
        if (string.IsNullOrEmpty(category))
        {
            return 1.0f;
        }

        // Map iOS content size categories to scale factors
        return category switch
        {
            var c when c.Contains("ExtraSmall") => 0.82f,
            var c when c.Contains("Small") => 0.88f,
            var c when c.Contains("Medium") && !c.Contains("Accessibility") => 0.94f,
            var c when c.Contains("Large") && !c.Contains("Extra") && !c.Contains("Accessibility") => 1.0f,
            var c when c.Contains("ExtraLarge") && !c.Contains("Extra2") && !c.Contains("Extra3") => 1.12f,
            var c when c.Contains("ExtraExtraLarge") => 1.24f,
            var c when c.Contains("ExtraExtraExtraLarge") => 1.35f,
            var c when c.Contains("AccessibilityMedium") => 1.60f,
            var c when c.Contains("AccessibilityLarge") => 1.90f,
            var c when c.Contains("AccessibilityExtraLarge") && !c.Contains("Extra2") && !c.Contains("Extra3") => 2.35f,
            var c when c.Contains("AccessibilityExtraExtraLarge") => 2.75f,
            var c when c.Contains("AccessibilityExtraExtraExtraLarge") => 3.10f,
            _ => 1.0f
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_contentSizeObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_contentSizeObserver);
                _contentSizeObserver = null;
            }
            _disposed = true;
        }
    }
}
