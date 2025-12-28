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
        var generator = new UIImpactFeedbackGenerator(style);
        generator.Prepare();
        generator.ImpactOccurred();
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
