namespace PlusUi.core;

/// <summary>
/// Specifies how a UI element is positioned vertically within its parent container.
/// </summary>
public enum VerticalAlignment
{

    /// <summary>
    /// Represents an undefined or uninitialized value.
    /// </summary>
    /// <remarks>Use this member to indicate that a value has not been set or is not applicable in the current
    /// context. The specific meaning of 'undefined' may vary depending on how this member is used within the
    /// application.</remarks>
    Undefined,

    /// <summary>
    /// Aligns the element to the top edge of the parent container.
    /// </summary>
    Top,

    /// <summary>
    /// Centers the element vertically within the parent container.
    /// </summary>
    Center,

    /// <summary>
    /// Aligns the element to the bottom edge of the parent container.
    /// </summary>
    Bottom,

    /// <summary>
    /// Stretches the element to fill the entire vertical space of the parent container.
    /// </summary>
    Stretch
}