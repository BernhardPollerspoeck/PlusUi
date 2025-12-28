namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays text values.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridTextColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the text value from an item.
    /// </summary>
    public Func<T, string>? ValueGetter { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTextColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the value binding for this column.
    /// </summary>
    /// <param name="valueGetter">A function that extracts the display value from an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTextColumn<T> SetBinding(Func<T, string> valueGetter)
    {
        ValueGetter = valueGetter;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTextColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var label = new Label();
        if (ValueGetter != null)
        {
            var getter = ValueGetter;
            label.SetText(getter(item));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    label.SetText(getter(item));
                };
            }
        }
        return label;
    }
}
