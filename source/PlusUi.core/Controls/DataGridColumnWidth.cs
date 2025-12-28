namespace PlusUi.core;

/// <summary>
/// Specifies the type of column width for a DataGrid column.
/// </summary>
public enum DataGridColumnWidthType
{
    /// <summary>
    /// Column has a fixed absolute width in pixels.
    /// </summary>
    Absolute,

    /// <summary>
    /// Column width is proportional to other star-sized columns.
    /// </summary>
    Star,

    /// <summary>
    /// Column width is automatically sized to fit content.
    /// </summary>
    Auto
}

/// <summary>
/// Represents the width of a DataGrid column.
/// </summary>
public readonly struct DataGridColumnWidth
{
    /// <summary>
    /// Gets the type of width.
    /// </summary>
    public DataGridColumnWidthType Type { get; }

    /// <summary>
    /// Gets the width value. For Absolute, this is the pixel width.
    /// For Star, this is the proportion value. For Auto, this is ignored.
    /// </summary>
    public float Value { get; }

    private DataGridColumnWidth(DataGridColumnWidthType type, float value = 0)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Creates an absolute width with the specified pixel value.
    /// </summary>
    /// <param name="pixels">The width in pixels.</param>
    /// <returns>A new DataGridColumnWidth with absolute sizing.</returns>
    public static DataGridColumnWidth Absolute(float pixels) =>
        new(DataGridColumnWidthType.Absolute, pixels);

    /// <summary>
    /// Creates a star width with the specified proportion.
    /// </summary>
    /// <param name="value">The proportion value. Defaults to 1.</param>
    /// <returns>A new DataGridColumnWidth with star sizing.</returns>
    public static DataGridColumnWidth Star(float value = 1) =>
        new(DataGridColumnWidthType.Star, value);

    /// <summary>
    /// Gets an auto-sized width that fits content.
    /// </summary>
    public static DataGridColumnWidth Auto =>
        new(DataGridColumnWidthType.Auto);
}
