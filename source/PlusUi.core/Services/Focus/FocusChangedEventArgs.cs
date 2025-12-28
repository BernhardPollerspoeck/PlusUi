namespace PlusUi.core.Services.Focus;

/// <summary>
/// Event arguments for focus change events.
/// </summary>
public class FocusChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the element that previously had focus, or null if no element had focus.
    /// </summary>
    public IFocusable? OldElement { get; }

    /// <summary>
    /// Gets the element that now has focus, or null if focus was cleared.
    /// </summary>
    public IFocusable? NewElement { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FocusChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldElement">The element that previously had focus.</param>
    /// <param name="newElement">The element that now has focus.</param>
    public FocusChangedEventArgs(IFocusable? oldElement, IFocusable? newElement)
    {
        OldElement = oldElement;
        NewElement = newElement;
    }
}
