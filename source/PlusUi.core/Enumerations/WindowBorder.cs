namespace PlusUi.core;

/// <summary>
/// Specifies the border style of a window.
/// </summary>
public enum WindowBorder
{
    /// <summary>
    /// The window can be resized by the user.
    /// </summary>
    Resizable = 0,

    /// <summary>
    /// The window has a fixed size and cannot be resized.
    /// </summary>
    Fixed = 1,

    /// <summary>
    /// The window has no visible border.
    /// </summary>
    Hidden = 2
}
