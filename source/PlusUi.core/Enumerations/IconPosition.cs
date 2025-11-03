namespace PlusUi.core;

/// <summary>
/// Specifies the position of an icon relative to text in a control. This is a flags enumeration.
/// </summary>
[Flags]
public enum IconPosition
{
    /// <summary>
    /// No icon is displayed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Icon is positioned before (to the left of) the text.
    /// </summary>
    Leading = 1,

    /// <summary>
    /// Icon is positioned after (to the right of) the text.
    /// </summary>
    Trailing = 2
}
