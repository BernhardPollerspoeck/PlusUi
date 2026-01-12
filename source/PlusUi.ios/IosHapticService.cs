using PlusUi.core;
using UIKit;

namespace PlusUi.ios;

public class IosHapticService : IHapticService
{
    public bool IsSupported => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);

    public void Emit(HapticFeedback feedback)
    {
        if (!IsSupported)
        {
            return;
        }

        switch (feedback)
        {
            case HapticFeedback.Light:
                EmitImpact(UIImpactFeedbackStyle.Light);
                break;
            case HapticFeedback.Medium:
                EmitImpact(UIImpactFeedbackStyle.Medium);
                break;
            case HapticFeedback.Heavy:
                EmitImpact(UIImpactFeedbackStyle.Heavy);
                break;
            case HapticFeedback.Success:
                EmitNotification(UINotificationFeedbackType.Success);
                break;
            case HapticFeedback.Warning:
                EmitNotification(UINotificationFeedbackType.Warning);
                break;
            case HapticFeedback.Error:
                EmitNotification(UINotificationFeedbackType.Error);
                break;
            case HapticFeedback.Selection:
                EmitSelection();
                break;
        }
    }

    private static void EmitImpact(UIImpactFeedbackStyle style)
    {
        // iOS 17.5+ has GetFeedbackGenerator, but .NET bindings not yet available
        // TODO: Update when .NET iOS bindings support iOS 17.5 APIs
#pragma warning disable CA1422 // .NET bindings for iOS 17.5 GetFeedbackGenerator not yet available
        var generator = new UIImpactFeedbackGenerator(style);
        generator.Prepare();
        generator.ImpactOccurred();
#pragma warning restore CA1422
    }

    private static void EmitNotification(UINotificationFeedbackType type)
    {
        var generator = new UINotificationFeedbackGenerator();
        generator.Prepare();
        generator.NotificationOccurred(type);
    }

    private static void EmitSelection()
    {
        var generator = new UISelectionFeedbackGenerator();
        generator.Prepare();
        generator.SelectionChanged();
    }
}
