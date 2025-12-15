namespace PlusUi.core;

/// <summary>
/// Interface for controls that handle keyboard input specially when focused.
/// For example, Slider uses arrow keys to change value instead of moving focus.
/// </summary>
public interface IKeyboardInputHandler
{
    /// <summary>
    /// Handles keyboard input when this control is focused.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    /// <returns>True if the key was handled, false to let the input service handle it.</returns>
    bool HandleKeyboardInput(PlusKey key);
}
