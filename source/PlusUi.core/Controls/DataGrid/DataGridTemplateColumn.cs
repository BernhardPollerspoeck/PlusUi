namespace PlusUi.core;

/// <summary>
/// A DataGrid column that uses a custom template for cell content.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridTemplateColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the cell template function.
    /// </summary>
    public Func<T, int, UiElement>? CellTemplate { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTemplateColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the cell template.
    /// </summary>
    /// <param name="template">A function that creates the cell content for each item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTemplateColumn<T> SetCellTemplate(Func<T, int, UiElement> template)
    {
        CellTemplate = template;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTemplateColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        return CellTemplate?.Invoke(item, rowIndex) ?? new Solid();
    }
}
