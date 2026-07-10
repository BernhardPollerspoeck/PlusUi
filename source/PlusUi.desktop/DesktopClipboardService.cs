using PlusUi.core.Services;
using Silk.NET.GLFW;

namespace PlusUi.desktop;

public class DesktopClipboardService(DesktopKeyboardHandler keyboardHandler) : IClipboardService
{
    public string? GetText()
    {
        try
        {
            return keyboardHandler.Keyboard?.ClipboardText;
        }
        catch (GlfwException)
        {
            return null;
        }
    }

    public void SetText(string text)
    {
        try
        {
            var keyboard = keyboardHandler.Keyboard;
            if (keyboard is not null)
            {
                keyboard.ClipboardText = text;
            }
        }
        catch (GlfwException)
        {
        }
    }
}
