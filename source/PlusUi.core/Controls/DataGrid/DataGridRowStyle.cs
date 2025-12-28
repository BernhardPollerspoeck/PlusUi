namespace PlusUi.core;

/// <summary>
/// Represents the style of a DataGrid row, including background and foreground colors.
/// </summary>
/// <param name="Background">The background of the row, or null to use the default.</param>
/// <param name="Foreground">The foreground (text) color of the row, or null to use the default.</param>
public readonly record struct DataGridRowStyle(
    IBackground? Background = null,
    Color? Foreground = null
);
