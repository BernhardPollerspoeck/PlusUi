namespace PlusUi.core;

/// <summary>
/// Interface for UI elements that can receive keyboard focus.
/// </summary>
public interface IFocusable
{
    /// <summary>
    /// Gets a value indicating whether this element can receive focus.
    /// </summary>
    bool IsFocusable { get; }

    /// <summary>
    /// Gets the tab index for focus order. Null means automatic order (declaration order).
    /// Negative values exclude the element from tab navigation.
    /// </summary>
    int? TabIndex { get; }

    /// <summary>
    /// Gets or sets whether the element should be included in tab navigation.
    /// When false, the element is skipped during Tab/Shift+Tab navigation.
    /// </summary>
    bool TabStop { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this element currently has focus.
    /// </summary>
    bool IsFocused { get; set; }

    /// <summary>
    /// Called when the element receives focus.
    /// </summary>
    void OnFocus();

    /// <summary>
    /// Called when the element loses focus.
    /// </summary>
    void OnBlur();
}
