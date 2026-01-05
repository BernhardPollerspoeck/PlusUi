namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays editable text values using Entry controls.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridEditorColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the text value from an item.
    /// </summary>
    public Func<T, string>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the action that sets the text value on an item.
    /// </summary>
    public Action<T, string>? ValueSetter { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridEditorColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the value binding for this column.
    /// </summary>
    /// <param name="valueGetter">A function that extracts the value from an item.</param>
    /// <param name="valueSetter">An action that sets the value on an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridEditorColumn<T> SetBinding(Func<T, string> valueGetter, Action<T, string> valueSetter)
    {
        ValueGetter = valueGetter;
        ValueSetter = valueSetter;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridEditorColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var entry = new Entry()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch);

        if (ValueGetter != null && ValueSetter != null)
        {
            var getter = ValueGetter;
            var setter = ValueSetter;

            // Two-way binding using BindText
            entry.BindText(
                $"DataGridEditor_{item?.GetHashCode()}_{Header}_{rowIndex}",
                () => getter(item),
                value => setter(item, value));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    entry.SetText(getter(item));
                };
            }
        }

        return entry;
    }
}
