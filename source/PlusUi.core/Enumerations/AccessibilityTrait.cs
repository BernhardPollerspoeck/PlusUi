namespace PlusUi.core;

/// <summary>
/// Defines accessibility traits that describe the state and characteristics of UI elements.
/// These are flags that can be combined.
/// </summary>
[Flags]
public enum AccessibilityTrait
{
    /// <summary>
    /// No traits.
    /// </summary>
    None = 0,

    /// <summary>
    /// The element is selected.
    /// </summary>
    Selected = 1 << 0,

    /// <summary>
    /// The element is disabled and cannot be interacted with.
    /// </summary>
    Disabled = 1 << 1,

    /// <summary>
    /// The element is expanded (for collapsible elements).
    /// </summary>
    Expanded = 1 << 2,

    /// <summary>
    /// The element is checked (for checkboxes, toggles, etc.).
    /// </summary>
    Checked = 1 << 3,

    /// <summary>
    /// The element is read-only.
    /// </summary>
    ReadOnly = 1 << 4,

    /// <summary>
    /// The element is required (for form fields).
    /// </summary>
    Required = 1 << 5,

    /// <summary>
    /// The element has an error state.
    /// </summary>
    HasError = 1 << 6,

    /// <summary>
    /// The element is busy or loading.
    /// </summary>
    Busy = 1 << 7,

    /// <summary>
    /// The element is a modal element that blocks interaction with other elements.
    /// </summary>
    Modal = 1 << 8,

    /// <summary>
    /// The element is hidden from accessibility.
    /// </summary>
    Hidden = 1 << 9,

    /// <summary>
    /// The element triggers a popup or dropdown.
    /// </summary>
    HasPopup = 1 << 10,

    /// <summary>
    /// The element is currently pressed.
    /// </summary>
    Pressed = 1 << 11,

    /// <summary>
    /// The element updates frequently (live region).
    /// </summary>
    LiveRegion = 1 << 12,

    /// <summary>
    /// The element is focusable and can receive keyboard focus.
    /// </summary>
    Focusable = 1 << 13,

    /// <summary>
    /// The element currently has focus.
    /// </summary>
    Focused = 1 << 14,

    /// <summary>
    /// The element is indeterminate (for checkboxes with mixed state).
    /// </summary>
    Indeterminate = 1 << 15,

    /// <summary>
    /// The element is multiselectable (for lists that allow multiple selection).
    /// </summary>
    MultiSelectable = 1 << 16,

    /// <summary>
    /// The element is part of a group.
    /// </summary>
    InGroup = 1 << 17
}
