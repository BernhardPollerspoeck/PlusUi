namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// Service for accessing system accessibility settings like high contrast mode and font scaling.
/// </summary>
public interface IAccessibilitySettingsService
{
    /// <summary>
    /// Gets whether high contrast mode is enabled on the system.
    /// </summary>
    bool IsHighContrastEnabled { get; }

    /// <summary>
    /// Gets the system font scale factor (1.0 = normal, 1.5 = 150%, 2.0 = 200%, etc.).
    /// </summary>
    float FontScaleFactor { get; }

    /// <summary>
    /// Gets whether reduced motion is preferred by the user.
    /// </summary>
    bool IsReducedMotionEnabled { get; }

    /// <summary>
    /// Gets the minimum recommended touch target size in device-independent pixels.
    /// Returns 44 by default (Apple/Google recommendation).
    /// </summary>
    float MinimumTouchTargetSize { get; }

    /// <summary>
    /// Occurs when any accessibility setting changes.
    /// </summary>
    event EventHandler<AccessibilitySettingsChangedEventArgs>? SettingsChanged;

    /// <summary>
    /// Refreshes settings from the system.
    /// </summary>
    void RefreshSettings();
}
