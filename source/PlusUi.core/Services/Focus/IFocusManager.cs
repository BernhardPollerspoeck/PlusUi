namespace PlusUi.core.Services.Focus;

/// <summary>
/// Service for managing keyboard focus across UI elements.
/// </summary>
public interface IFocusManager
{
    /// <summary>
    /// Gets the currently focused element, or null if no element has focus.
    /// </summary>
    IFocusable? FocusedElement { get; }

    /// <summary>
    /// Occurs when focus changes from one element to another.
    /// </summary>
    event EventHandler<FocusChangedEventArgs>? FocusChanged;

    /// <summary>
    /// Sets focus to the specified element.
    /// </summary>
    /// <param name="element">The element to focus, or null to clear focus.</param>
    void SetFocus(IFocusable? element);

    /// <summary>
    /// Clears focus from the currently focused element.
    /// </summary>
    void ClearFocus();

    /// <summary>
    /// Moves focus in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to move focus.</param>
    /// <returns>True if focus was moved successfully, false otherwise.</returns>
    bool MoveFocus(FocusNavigationDirection direction);

    /// <summary>
    /// Moves focus to the next element.
    /// </summary>
    /// <returns>True if focus was moved, false if no next element.</returns>
    bool FocusNext();

    /// <summary>
    /// Moves focus to the previous element.
    /// </summary>
    /// <returns>True if focus was moved, false if no previous element.</returns>
    bool FocusPrevious();

    /// <summary>
    /// Navigates focus to a specific accessibility landmark.
    /// Skip-link functionality for screen reader users.
    /// </summary>
    /// <param name="landmark">The landmark to navigate to.</param>
    /// <returns>True if a landmark was found and focused, false otherwise.</returns>
    bool NavigateToLandmark(AccessibilityLandmark landmark);

    /// <summary>
    /// Navigates focus to the next landmark in the UI.
    /// </summary>
    /// <returns>True if a landmark was found and focused, false otherwise.</returns>
    bool NavigateToNextLandmark();
}
