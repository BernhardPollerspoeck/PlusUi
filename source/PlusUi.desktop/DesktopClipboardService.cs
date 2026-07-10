using PlusUi.core.Services;

namespace PlusUi.desktop;

public class DesktopClipboardService(DesktopKeyboardHandler keyboardHandler) : IClipboardService
{
    public string? GetText() => keyboardHandler.Keyboard?.ClipboardText;

    public void SetText(string text)
    {
        var keyboard = keyboardHandler.Keyboard;
        if (keyboard is not null)
        {
            keyboard.ClipboardText = text;
        }
    }
}
