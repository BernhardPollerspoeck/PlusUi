using Silk.NET.Input;

namespace PlusUi.core;

public class UpdateService(NavigationContainer navigationContainer)
{
    private bool _isMousePressed;
    private IKeyboard? _keyboard;
    private ITextInputControl? _textInputControl;

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

    public void Update(IMouse mouse)
    {
        if (mouse.IsButtonPressed(MouseButton.Left))
        {
            _isMousePressed = true;
            return;
        }

        var isReleased = _isMousePressed && !mouse.IsButtonPressed(MouseButton.Left);

        if (isReleased)
        {
            _isMousePressed = false;

            var location = mouse.Position;

            var hitControl = navigationContainer.Page.HitTest(new(location.X, location.Y));
            if (hitControl is IInputControl inputControl)
            {
                inputControl.InvokeCommand();
            }





            if (hitControl is ITextInputControl textInputControl
                && textInputControl != _textInputControl)
            {
                _textInputControl?.SetSelectionStatus(false);
                _textInputControl = textInputControl;
                _textInputControl.SetSelectionStatus(true);
                _keyboard?.BeginInput();
            }
            else if (_textInputControl != null && _textInputControl != hitControl)
            {
                _keyboard?.EndInput();
                _textInputControl.SetSelectionStatus(false);
                _textInputControl = null;
            }
        }
    }


    private void HandleKeyboardInput(IKeyboard keyboard, Key key, int keyCode)
    {
        _textInputControl?.HandleInput(key);
    }
    private void HandleKeyCharInput(IKeyboard keyboard, char chr)
    {
        _textInputControl?.HandleInput(chr);
    }
}
