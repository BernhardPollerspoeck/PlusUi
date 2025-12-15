using Android.Views.Accessibility;

namespace PlusUi.droid.Accessibility;

/// <summary>
/// Accessibility delegate for the host view.
/// </summary>
public sealed class PlusUiAccessibilityDelegate : Android.Views.View.AccessibilityDelegate
{
    private readonly PlusUiAccessibilityNodeProvider _nodeProvider;

    public PlusUiAccessibilityDelegate(PlusUiAccessibilityNodeProvider nodeProvider)
    {
        _nodeProvider = nodeProvider;
    }

    public override AccessibilityNodeProvider? GetAccessibilityNodeProvider(Android.Views.View? host)
    {
        return _nodeProvider;
    }
}
