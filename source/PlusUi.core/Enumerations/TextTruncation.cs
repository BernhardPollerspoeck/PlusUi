namespace PlusUi.core;

/// <summary>
/// Specifies where text should be truncated with an ellipsis (...) when it exceeds the available space.
/// </summary>
public enum TextTruncation
{
    /// <summary>
    /// No truncation. Text will be clipped at the boundary without an ellipsis.
    /// </summary>
    None,

    /// <summary>
    /// Truncates at the start of the text with a leading ellipsis (e.g., "...end of text").
    /// </summary>
    Start,

    /// <summary>
    /// Truncates in the middle of the text with an ellipsis (e.g., "start...end").
    /// </summary>
    Middle,

    /// <summary>
    /// Truncates at the end of the text with a trailing ellipsis (e.g., "start of text...").
    /// </summary>
    End
}
