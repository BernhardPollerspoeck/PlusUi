using Android.Content;
using Android.Provider;
using Android.Views.Accessibility;
using PlusUi.core.Services.Accessibility;

namespace PlusUi.droid.Accessibility;

/// <summary>
/// Android implementation of accessibility settings service.
/// </summary>
public class AndroidAccessibilitySettingsService : AccessibilitySettingsService
{
    private readonly Context _context;

    public AndroidAccessibilitySettingsService(Context context)
    {
        _context = context;
        RefreshSettings();
    }

    /// <inheritdoc />
    public override void RefreshSettings()
    {
        try
        {
            var resolver = _context.ContentResolver;
            if (resolver == null)
            {
                return;
            }

            // Check high contrast text (Android 5.0+)
            var highContrastText = Settings.Secure.GetInt(resolver, "high_text_contrast_enabled", 0);
            SetHighContrastEnabled(highContrastText == 1);

            // Get font scale
            var fontScale = Settings.System.GetFloat(resolver, Settings.System.FontScale, 1.0f);
            SetFontScaleFactor(fontScale);

            // Check reduce animations
            var animatorDurationScale = Settings.Global.GetFloat(resolver, Settings.Global.AnimatorDurationScale, 1.0f);
            var transitionAnimationScale = Settings.Global.GetFloat(resolver, Settings.Global.TransitionAnimationScale, 1.0f);
            SetReducedMotionEnabled(animatorDurationScale == 0 || transitionAnimationScale == 0);
        }
        catch
        {
            // Settings access failed - use defaults
        }
    }
}
