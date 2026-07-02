namespace PlusUi.core;

public interface IKeyboardHandler
{
    event EventHandler<PlusKey> KeyInput;
    event EventHandler<char> CharInput;
    event EventHandler<bool>? ShiftStateChanged;
    event EventHandler<bool>? CtrlStateChanged;

    /// <summary>
    /// Raised when any key goes down, using the full unfiltered <see cref="PlusKey"/> set
    /// (letters, digits, function keys, modifiers, …). This feeds the framework-wide
    /// <see cref="IGlobalInputService"/> and is independent of the UI-focused <see cref="KeyInput"/>.
    /// </summary>
    event EventHandler<PlusKey>? RawKeyDown;

    /// <summary>
    /// Raised when any key is released, using the full unfiltered <see cref="PlusKey"/> set.
    /// </summary>
    event EventHandler<PlusKey>? RawKeyUp;

    void Show();
    void Hide();
    void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword);
}
