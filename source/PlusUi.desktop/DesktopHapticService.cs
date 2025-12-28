using PlusUi.core;

namespace PlusUi.desktop;

public class DesktopHapticService : IHapticService
{
    public bool IsSupported => false;

    public void Emit(HapticFeedback feedback)
    {
        // No haptic feedback on desktop
    }
}
