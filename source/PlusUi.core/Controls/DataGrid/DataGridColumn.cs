namespace PlusUi.core;

/// <summary>
/// Abstract base class for DataGrid columns.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public abstract class DataGridColumn<T>
{
    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    public string Header { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    public DataGridColumnWidth Width { get; protected set; } = DataGridColumnWidth.Star(1);

    /// <summary>
    /// Gets the actual calculated width of the column after layout.
    /// </summary>
    public float ActualWidth { get; internal set; }

    /// <summary>
    /// Creates the cell content for the specified item.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <param name="rowIndex">The row index.</param>
    /// <returns>The UI element for the cell.</returns>
    public abstract UiElement CreateCell(T item, int rowIndex);
}
