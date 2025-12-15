namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// Default implementation of accessibility settings service.
/// Platform-specific implementations can override to access real system settings.
/// </summary>
public class AccessibilitySettingsService : IAccessibilitySettingsService
{
    private bool _isHighContrastEnabled;
    private float _fontScaleFactor = 1.0f;
    private bool _isReducedMotionEnabled;

    /// <inheritdoc />
    public bool IsHighContrastEnabled => _isHighContrastEnabled;

    /// <inheritdoc />
    public float FontScaleFactor => _fontScaleFactor;

    /// <inheritdoc />
    public bool IsReducedMotionEnabled => _isReducedMotionEnabled;

    /// <inheritdoc />
    public float MinimumTouchTargetSize => 44f;

    /// <inheritdoc />
    public event EventHandler<AccessibilitySettingsChangedEventArgs>? SettingsChanged;

    /// <inheritdoc />
    public virtual void RefreshSettings()
    {
        // Base implementation does nothing - override in platform-specific implementations
    }

    /// <summary>
    /// Sets high contrast mode state. Call from platform-specific code.
    /// </summary>
    protected void SetHighContrastEnabled(bool enabled)
    {
        if (_isHighContrastEnabled != enabled)
        {
            _isHighContrastEnabled = enabled;
            OnSettingsChanged(nameof(IsHighContrastEnabled));
        }
    }

    /// <summary>
    /// Sets font scale factor. Call from platform-specific code.
    /// </summary>
    protected void SetFontScaleFactor(float factor)
    {
        if (Math.Abs(_fontScaleFactor - factor) > 0.001f)
        {
            _fontScaleFactor = factor;
            OnSettingsChanged(nameof(FontScaleFactor));
        }
    }

    /// <summary>
    /// Sets reduced motion preference. Call from platform-specific code.
    /// </summary>
    protected void SetReducedMotionEnabled(bool enabled)
    {
        if (_isReducedMotionEnabled != enabled)
        {
            _isReducedMotionEnabled = enabled;
            OnSettingsChanged(nameof(IsReducedMotionEnabled));
        }
    }

    /// <summary>
    /// Raises the SettingsChanged event.
    /// </summary>
    protected virtual void OnSettingsChanged(string settingName)
    {
        SettingsChanged?.Invoke(this, new AccessibilitySettingsChangedEventArgs(settingName));
    }
}
