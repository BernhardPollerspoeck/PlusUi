using Android.Views.Accessibility;

namespace PlusUi.droid.Accessibility;

/// <summary>
/// Accessibility delegate for the host view.
/// </summary>
public sealed class PlusUiAccessibilityDelegate(PlusUiAccessibilityNodeProvider nodeProvider) : Android.Views.View.AccessibilityDelegate
{
    public override AccessibilityNodeProvider? GetAccessibilityNodeProvider(Android.Views.View? host)
    {
        return nodeProvider;
    }
}
