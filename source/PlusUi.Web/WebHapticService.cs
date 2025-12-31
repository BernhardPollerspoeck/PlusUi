using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// No-op haptic service for web platform (browsers don't support haptics).
/// </summary>
public class WebHapticService : IHapticService
{
    public bool IsSupported => false;

    public void Emit(HapticFeedback feedback)
    {
        // No-op: Web browsers don't support haptic feedback
    }
}
