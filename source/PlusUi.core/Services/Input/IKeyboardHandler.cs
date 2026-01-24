namespace PlusUi.core;

public interface IKeyboardHandler
{
    event EventHandler<PlusKey> KeyInput;
    event EventHandler<char> CharInput;
    event EventHandler<bool>? ShiftStateChanged;
    event EventHandler<bool>? CtrlStateChanged;
    void Show();
    void Hide();
    void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword);
}
