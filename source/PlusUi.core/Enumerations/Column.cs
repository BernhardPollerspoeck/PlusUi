namespace PlusUi.core;

/// <summary>
/// Specifies the sizing mode for grid columns.
/// </summary>
public enum Column
{
    /// <summary>
    /// Fixed width in pixels. Use with a specific value (e.g., 100 pixels).
    /// </summary>
    Absolute,

    /// <summary>
    /// Proportional width using star (*) notation. Available space is divided proportionally (e.g., 1*, 2*).
    /// </summary>
    Star,

    /// <summary>
    /// Automatically sizes to fit the content within the column.
    /// </summary>
    Auto,
}
