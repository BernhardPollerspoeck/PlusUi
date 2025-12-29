namespace PlusUi.core.Services.Focus;

/// <summary>
/// Event arguments for focus change events.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FocusChangedEventArgs"/> class.
/// </remarks>
/// <param name="oldElement">The element that previously had focus.</param>
/// <param name="newElement">The element that now has focus.</param>
public class FocusChangedEventArgs(IFocusable? oldElement, IFocusable? newElement) : EventArgs
{
    /// <summary>
    /// Gets the element that previously had focus, or null if no element had focus.
    /// </summary>
    public IFocusable? OldElement { get; } = oldElement;

    /// <summary>
    /// Gets the element that now has focus, or null if focus was cleared.
    /// </summary>
    public IFocusable? NewElement { get; } = newElement;
}
