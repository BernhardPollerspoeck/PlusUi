namespace PlusUi.core;

/// <summary>
/// Specifies how text should wrap when it exceeds the available width.
/// </summary>
public enum TextWrapping
{
    /// <summary>
    /// Text does not wrap. Text that exceeds the width will be clipped or truncated.
    /// </summary>
    NoWrap,

    /// <summary>
    /// Text wraps at character boundaries when it reaches the container width.
    /// </summary>
    Wrap,

    /// <summary>
    /// Text wraps at word boundaries, preferring to keep whole words together.
    /// </summary>
    WordWrap
}
