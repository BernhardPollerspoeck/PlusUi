using Silk.NET.Input;

namespace PlusUi.core;

public interface IKeyboardHandler
{
    event EventHandler<Key> KeyInput;
    event EventHandler<char> CharInput;
    void Show();
    void Hide();
}
