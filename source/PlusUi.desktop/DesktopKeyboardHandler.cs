using PlusUi.core;
using Silk.NET.Input;

namespace PlusUi.desktop;

public class DesktopKeyboardHandler : IKeyboardHandler
{
    private IKeyboard? _keyboard;

    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;
    public void SetKeyboard(IKeyboard keyboard)
    {
        if (_keyboard != null)
        {
            _keyboard.KeyDown -= HandleKeyboardInput;
            _keyboard.KeyChar -= HandleKeyCharInput;
        }

        _keyboard = keyboard;
        _keyboard.KeyDown += HandleKeyboardInput;
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


    private void HandleKeyboardInput(IKeyboard keyboard, Key key, int keyCode)
    {
        var plusKey = key switch
        {
            Key.Backspace => PlusKey.Backspace,
            Key.Enter => PlusKey.Enter,
            Key.Tab => PlusKey.Tab,
            Key.Space => PlusKey.Space,
            _ => PlusKey.Unknown
        };
        KeyInput?.Invoke(this, plusKey);
    }
    private void HandleKeyCharInput(IKeyboard keyboard, char chr)
    {
        CharInput?.Invoke(this, chr);
    }
}