using PlusUi.core;

namespace PlusUi.Headless.Services;

/// <summary>
/// Mock keyboard handler implementation for headless environment.
/// Keyboard is controlled programmatically (no native keyboard).
/// </summary>
public class HeadlessKeyboardHandler : IKeyboardHandler
{
    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;
    public event EventHandler<bool>? ShiftStateChanged;
    public event EventHandler<bool>? CtrlStateChanged;

    // Show/Hide are no-ops in headless environment
    public void Show() { }
    public void Hide() { }
    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword) { }

    /// <summary>
    /// Raises a key input event programmatically.
    /// </summary>
    internal void RaiseKeyInput(PlusKey key)
    {
        KeyInput?.Invoke(this, key);
    }

    /// <summary>
    /// Raises a character input event programmatically.
    /// </summary>
    internal void RaiseCharInput(char c)
    {
        CharInput?.Invoke(this, c);
    }
}
