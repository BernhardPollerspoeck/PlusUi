using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// Web implementation of IKeyboardHandler.
/// In the browser context, keyboard handling is done through JavaScript events,
/// so this is primarily a pass-through that raises the appropriate events.
/// </summary>
public class WebKeyboardHandler : IKeyboardHandler
{
    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;

    /// <summary>
    /// Show virtual keyboard (primarily for mobile browsers)
    /// </summary>
    public void Show()
    {
        // In a web context, this would trigger focus on an invisible input element
        // to show the mobile keyboard. Implementation would use JSInterop.
    }

    /// <summary>
    /// Hide virtual keyboard (primarily for mobile browsers)
    /// </summary>
    public void Hide()
    {
        // In a web context, this would blur the input element to hide the keyboard
        // Implementation would use JSInterop.
    }



    /// <summary>
    /// Called from the Blazor component when a key is pressed
    /// </summary>
    internal void OnKeyDown(string key, string code)
    {
        // Map browser key codes to PlusKey enum
        var plusKey = MapBrowserKeyToPlusKey(key, code);
        if (plusKey != PlusKey.Unknown)
        {
            KeyInput?.Invoke(this, plusKey);
        }

        // For character input
        if (key.Length == 1)
        {
            CharInput?.Invoke(this, key[0]);
        }
    }

    /// <summary>
    /// Called from the Blazor component when a key is released
    /// </summary>
    internal void OnKeyUp(string key, string code)
    {
        // Currently not used, but could be implemented if needed
    }

    private PlusKey MapBrowserKeyToPlusKey(string key, string code)
    {
        // Map common keys - this is a basic implementation
        // You'll need to expand this based on your PlusKey enum
        return key switch
        {
            "Enter" => PlusKey.Enter,
            "Backspace" => PlusKey.Backspace,
            //"Delete" => PlusKey.Delete,
            "Tab" => PlusKey.Tab,
            //"Escape" => PlusKey.Escape,
            //"ArrowLeft" => PlusKey.Left,
            //"ArrowRight" => PlusKey.Right,
            //"ArrowUp" => PlusKey.Up,
            //"ArrowDown" => PlusKey.Down,
            //"Home" => PlusKey.Home,
            //"End" => PlusKey.End,
            //"PageUp" => PlusKey.PageUp,
            //"PageDown" => PlusKey.PageDown,
            _ => PlusKey.Unknown
        };
    }

    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword)
    {
        // Desktop keyboards don't need special configuration, just show it
        Show();
    }
}
