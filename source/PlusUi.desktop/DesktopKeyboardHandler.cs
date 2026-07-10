using PlusUi.core;
using Silk.NET.Input;

namespace PlusUi.desktop;

public class DesktopKeyboardHandler : IKeyboardHandler
{
    private IKeyboard? _keyboard;
    private bool _isShiftPressed;
    private bool _isCtrlPressed;

    internal IKeyboard? Keyboard => _keyboard;

    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;
    public event EventHandler<bool>? ShiftStateChanged;
    public event EventHandler<bool>? CtrlStateChanged;
    public event EventHandler<PlusKey>? RawKeyDown;
    public event EventHandler<PlusKey>? RawKeyUp;

    public void SetKeyboard(IKeyboard keyboard)
    {
        if (_keyboard != null)
        {
            _keyboard.KeyDown -= HandleKeyboardInput;
            _keyboard.KeyUp -= HandleKeyboardUp;
            _keyboard.KeyChar -= HandleKeyCharInput;
        }

        _keyboard = keyboard;
        _keyboard.KeyDown += HandleKeyboardInput;
        _keyboard.KeyUp += HandleKeyboardUp;
        _keyboard.KeyChar += HandleKeyCharInput;
    }
    public void Hide()
    {
        _keyboard?.BeginInput();
    }

    public void Show()
    {
        _keyboard?.EndInput();
    }

    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword)
    {
        // Desktop keyboards don't need special configuration, just show it
        Show();
    }


    private void HandleKeyboardInput(IKeyboard keyboard, Key key, int keyCode)
    {
        // Raw, unfiltered key-down for the global input bus (full key set incl. modifiers).
        var rawKey = MapToPlusKey(key);
        if (rawKey != PlusKey.Unknown)
        {
            RawKeyDown?.Invoke(this, rawKey);
        }

        // Track modifier key states
        if (key == Key.ShiftLeft || key == Key.ShiftRight)
        {
            _isShiftPressed = true;
            ShiftStateChanged?.Invoke(this, true);
            return;
        }
        if (key == Key.ControlLeft || key == Key.ControlRight)
        {
            _isCtrlPressed = true;
            CtrlStateChanged?.Invoke(this, true);
            return;
        }

        var plusKey = key switch
        {
            Key.Backspace => PlusKey.Backspace,
            Key.Enter => PlusKey.Enter,
            Key.Tab => _isShiftPressed ? PlusKey.ShiftTab : PlusKey.Tab,
            Key.Space => PlusKey.Space,
            Key.Escape => PlusKey.Escape,
            Key.Up => PlusKey.ArrowUp,
            Key.Down => PlusKey.ArrowDown,
            Key.Left => PlusKey.ArrowLeft,
            Key.Right => PlusKey.ArrowRight,
            Key.Delete => PlusKey.Delete,
            Key.Home => PlusKey.Home,
            Key.End => PlusKey.End,
            Key.A => _isCtrlPressed ? PlusKey.A : PlusKey.Unknown,
            Key.C => _isCtrlPressed ? PlusKey.C : PlusKey.Unknown,
            Key.V => _isCtrlPressed ? PlusKey.V : PlusKey.Unknown,
            Key.X => _isCtrlPressed ? PlusKey.X : PlusKey.Unknown,
            _ => PlusKey.Unknown
        };

        if (plusKey != PlusKey.Unknown)
        {
            KeyInput?.Invoke(this, plusKey);
        }
    }

    private void HandleKeyboardUp(IKeyboard keyboard, Key key, int keyCode)
    {
        // Raw, unfiltered key-up for the global input bus.
        var rawKey = MapToPlusKey(key);
        if (rawKey != PlusKey.Unknown)
        {
            RawKeyUp?.Invoke(this, rawKey);
        }

        // Track modifier key release
        if (key == Key.ShiftLeft || key == Key.ShiftRight)
        {
            _isShiftPressed = false;
            ShiftStateChanged?.Invoke(this, false);
        }
        if (key == Key.ControlLeft || key == Key.ControlRight)
        {
            _isCtrlPressed = false;
            CtrlStateChanged?.Invoke(this, false);
        }
    }

    private void HandleKeyCharInput(IKeyboard keyboard, char chr)
    {
        // Don't send char input for Ctrl combinations (they're handled as key commands)
        if (_isCtrlPressed) return;

        CharInput?.Invoke(this, chr);
    }

    /// <summary>
    /// Maps a Silk.NET key to the full <see cref="PlusKey"/> set for the raw global input bus.
    /// </summary>
    private static PlusKey MapToPlusKey(Key key) => key switch
    {
        >= Key.A and <= Key.Z => PlusKey.A + (key - Key.A),
        >= Key.Number0 and <= Key.Number9 => PlusKey.D0 + (key - Key.Number0),
        >= Key.Keypad0 and <= Key.Keypad9 => PlusKey.D0 + (key - Key.Keypad0),
        >= Key.F1 and <= Key.F12 => PlusKey.F1 + (key - Key.F1),
        Key.Space => PlusKey.Space,
        Key.Enter or Key.KeypadEnter => PlusKey.Enter,
        Key.Tab => PlusKey.Tab,
        Key.Escape => PlusKey.Escape,
        Key.Backspace => PlusKey.Backspace,
        Key.Delete => PlusKey.Delete,
        Key.Home => PlusKey.Home,
        Key.End => PlusKey.End,
        Key.Up => PlusKey.ArrowUp,
        Key.Down => PlusKey.ArrowDown,
        Key.Left => PlusKey.ArrowLeft,
        Key.Right => PlusKey.ArrowRight,
        Key.ShiftLeft => PlusKey.LeftShift,
        Key.ShiftRight => PlusKey.RightShift,
        Key.ControlLeft => PlusKey.LeftCtrl,
        Key.ControlRight => PlusKey.RightCtrl,
        Key.AltLeft => PlusKey.LeftAlt,
        Key.AltRight => PlusKey.RightAlt,
        _ => PlusKey.Unknown
    };
}