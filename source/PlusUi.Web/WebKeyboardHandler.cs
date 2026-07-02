using Microsoft.JSInterop;
using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// Web implementation of IKeyboardHandler.
/// Handles keyboard input through JavaScript interop for both physical keyboards
/// and virtual keyboards on mobile devices.
/// </summary>
public class WebKeyboardHandler(IJSRuntime jsRuntime) : IKeyboardHandler
{
    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;
    public event EventHandler<bool>? ShiftStateChanged;
    public event EventHandler<bool>? CtrlStateChanged;
    public event EventHandler<PlusKey>? RawKeyDown;
    public event EventHandler<PlusKey>? RawKeyUp;

    private KeyboardType _currentKeyboardType = KeyboardType.Default;
    private ReturnKeyType _currentReturnKeyType = ReturnKeyType.Default;
    private bool _isPassword;

    /// <summary>
    /// Show virtual keyboard with default settings (primarily for mobile browsers)
    /// </summary>
    public void Show()
    {
        Show(KeyboardType.Default, ReturnKeyType.Default, false);
    }

    /// <summary>
    /// Show virtual keyboard with specific configuration
    /// </summary>
    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword)
    {
        _currentKeyboardType = keyboardType;
        _currentReturnKeyType = returnKeyType;
        _isPassword = isPassword;

        var keyboardTypeString = keyboardType switch
        {
            KeyboardType.Numeric => "numeric",
            KeyboardType.Telephone => "tel",
            KeyboardType.Email => "email",
            KeyboardType.Url => "url",
            _ => "text"
        };

        var returnKeyTypeString = returnKeyType switch
        {
            ReturnKeyType.Done => "done",
            ReturnKeyType.Go => "go",
            ReturnKeyType.Next => "next",
            ReturnKeyType.Search => "search",
            ReturnKeyType.Send => "send",
            _ => "enter"
        };

        _ = jsRuntime.InvokeVoidAsync(
            "PlusUiInterop.keyboard.show",
            keyboardTypeString,
            returnKeyTypeString,
            isPassword);
    }

    /// <summary>
    /// Hide virtual keyboard
    /// </summary>
    public void Hide()
    {
        _ = jsRuntime.InvokeVoidAsync("PlusUiInterop.keyboard.hide");
    }

    /// <summary>
    /// Called from the Blazor component when a key is pressed
    /// </summary>
    internal void OnKeyDown(string key, string code)
    {
        // Raw, unfiltered key-down for the global input bus (full key set incl. modifiers).
        var rawKey = MapCodeToRawPlusKey(code);
        if (rawKey != PlusKey.Unknown)
        {
            RawKeyDown?.Invoke(this, rawKey);
        }

        // Track modifier key states
        if (key == "Shift")
        {
            ShiftStateChanged?.Invoke(this, true);
            return;
        }
        if (key == "Control")
        {
            CtrlStateChanged?.Invoke(this, true);
            return;
        }

        var plusKey = MapBrowserKeyToPlusKey(key, code);
        if (plusKey != PlusKey.Unknown)
        {
            KeyInput?.Invoke(this, plusKey);
        }

        // For single character input (letters, numbers, symbols)
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
        // Raw, unfiltered key-up for the global input bus.
        var rawKey = MapCodeToRawPlusKey(code);
        if (rawKey != PlusKey.Unknown)
        {
            RawKeyUp?.Invoke(this, rawKey);
        }

        if (key == "Shift")
            ShiftStateChanged?.Invoke(this, false);
        else if (key == "Control")
            CtrlStateChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Maps a browser <c>KeyboardEvent.code</c> (physical, layout-independent) to the full
    /// <see cref="PlusKey"/> set for the raw global input bus.
    /// </summary>
    private static PlusKey MapCodeToRawPlusKey(string code)
    {
        // Letters: "KeyA".."KeyZ"
        if (code.Length == 4 && code.StartsWith("Key", StringComparison.Ordinal))
        {
            var c = code[3];
            if (c is >= 'A' and <= 'Z')
                return PlusKey.A + (c - 'A');
        }

        // Digits: "Digit0".."Digit9"
        if (code.Length == 6 && code.StartsWith("Digit", StringComparison.Ordinal))
        {
            var d = code[5];
            if (d is >= '0' and <= '9')
                return PlusKey.D0 + (d - '0');
        }

        // Function keys: "F1".."F12"
        if (code.Length is 2 or 3 && code[0] == 'F' && int.TryParse(code.AsSpan(1), out var fn) && fn is >= 1 and <= 12)
        {
            return PlusKey.F1 + (fn - 1);
        }

        return code switch
        {
            "Space" => PlusKey.Space,
            "Enter" or "NumpadEnter" => PlusKey.Enter,
            "Tab" => PlusKey.Tab,
            "Escape" => PlusKey.Escape,
            "Backspace" => PlusKey.Backspace,
            "Delete" => PlusKey.Delete,
            "Home" => PlusKey.Home,
            "End" => PlusKey.End,
            "ArrowUp" => PlusKey.ArrowUp,
            "ArrowDown" => PlusKey.ArrowDown,
            "ArrowLeft" => PlusKey.ArrowLeft,
            "ArrowRight" => PlusKey.ArrowRight,
            "ShiftLeft" => PlusKey.LeftShift,
            "ShiftRight" => PlusKey.RightShift,
            "ControlLeft" => PlusKey.LeftCtrl,
            "ControlRight" => PlusKey.RightCtrl,
            "AltLeft" => PlusKey.LeftAlt,
            "AltRight" => PlusKey.RightAlt,
            _ => PlusKey.Unknown
        };
    }

    /// <summary>
    /// Raises the CharInput event for text input from mobile keyboards
    /// </summary>
    internal void RaiseCharInput(char c)
    {
        CharInput?.Invoke(this, c);
    }

    /// <summary>
    /// Maps browser key names to PlusKey enum values
    /// </summary>
    private static PlusKey MapBrowserKeyToPlusKey(string key, string code)
    {
        // First check the key value (what the key represents)
        return key switch
        {
            // Special handling for ShiftTab (synthesized key)
            "ShiftTab" => PlusKey.ShiftTab,

            // Navigation keys
            "Enter" => PlusKey.Enter,
            "Backspace" => PlusKey.Backspace,
            "Tab" => PlusKey.Tab,
            " " => PlusKey.Space,
            "Escape" => PlusKey.Escape,

            // Arrow keys
            "ArrowUp" => PlusKey.ArrowUp,
            "ArrowDown" => PlusKey.ArrowDown,
            "ArrowLeft" => PlusKey.ArrowLeft,
            "ArrowRight" => PlusKey.ArrowRight,

            // If key doesn't match, try the code
            _ => MapCodeToPlusKey(code)
        };
    }

    /// <summary>
    /// Maps browser key codes to PlusKey enum values
    /// </summary>
    private static PlusKey MapCodeToPlusKey(string code)
    {
        return code switch
        {
            // Physical key codes (for keyboard layouts that might report differently)
            "Enter" or "NumpadEnter" => PlusKey.Enter,
            "Backspace" => PlusKey.Backspace,
            "Tab" => PlusKey.Tab,
            "Space" => PlusKey.Space,
            "Escape" => PlusKey.Escape,
            "ArrowUp" => PlusKey.ArrowUp,
            "ArrowDown" => PlusKey.ArrowDown,
            "ArrowLeft" => PlusKey.ArrowLeft,
            "ArrowRight" => PlusKey.ArrowRight,
            _ => PlusKey.Unknown
        };
    }
}
