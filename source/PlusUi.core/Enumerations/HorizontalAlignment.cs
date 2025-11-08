namespace PlusUi.core;

/// <summary>
/// Specifies how a UI element is positioned horizontally within its parent container.
/// </summary>
public enum HorizontalAlignment
{
    /// <summary>
    /// Represents an undefined or uninitialized value.
    /// </summary>
    /// <remarks>Use this member to indicate that a value has not been set or is not applicable in the current
    /// context. The specific meaning of 'undefined' may vary depending on how this member is used within the
    /// application.</remarks>
    Undefined,

    /// <summary>
    /// Aligns the element to the left edge of the parent container.
    /// </summary>
    Left,

    /// <summary>
    /// Centers the element horizontally within the parent container.
    /// </summary>
    Center,

    /// <summary>
    /// Aligns the element to the right edge of the parent container.
    /// </summary>
    Right,

    /// <summary>
    /// Stretches the element to fill the entire horizontal space of the parent container.
    /// </summary>
    Stretch
}
