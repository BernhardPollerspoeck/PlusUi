namespace PlusUi.core;

public interface IKeyboardHandler
{
    event EventHandler<PlusKey> KeyInput;
    event EventHandler<char> CharInput;
    void Show();
    void Hide();
}
