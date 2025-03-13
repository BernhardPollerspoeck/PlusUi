using PlusUi.core;

namespace PlusUi.droid;

internal class AndroidKeyboardHandler : IKeyboardHandler
{
    public event EventHandler<Silk.NET.Input.Key>? KeyInput;
    public event EventHandler<char>? CharInput;

    public void Hide()
    {
        throw new NotImplementedException();
    }

    public void Show()
    {
        throw new NotImplementedException();
    }
}
