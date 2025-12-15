using PlusUi.core;
using Silk.NET.Input;

namespace PlusUi.desktop;

public class DesktopKeyboardHandler : IKeyboardHandler
{
    private IKeyboard? _keyboard;
    private bool _isShiftPressed;

    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;
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
        // Track shift key state
        if (key == Key.ShiftLeft || key == Key.ShiftRight)
        {
            _isShiftPressed = true;
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
            _ => PlusKey.Unknown
        };
        KeyInput?.Invoke(this, plusKey);
    }

    private void HandleKeyboardUp(IKeyboard keyboard, Key key, int keyCode)
    {
        // Track shift key release
        if (key == Key.ShiftLeft || key == Key.ShiftRight)
        {
            _isShiftPressed = false;
        }
    }

    private void HandleKeyCharInput(IKeyboard keyboard, char chr)
    {
        CharInput?.Invoke(this, chr);
    }
}