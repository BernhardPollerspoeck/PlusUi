using Silk.NET.Input;

namespace PlusUi.core.Services;

internal class UpdateService(CurrentPage rootPage)
{
    private bool _isPressed;
    public void Update(IMouse mouse)
    {
        if (mouse.IsButtonPressed(MouseButton.Left))
        {
            _isPressed = true;
            return;
        }

        var isReleased = _isPressed && !mouse.IsButtonPressed(MouseButton.Left);

        if (isReleased)
        {
            _isPressed = false;

            var location = mouse.Position;

            var hitControl = rootPage.Page.HitTest(new(location.X, location.Y));
            if (hitControl is IInputControl inputControl)
            {
                inputControl.InvokeCommand();
            }
        }
    }

}