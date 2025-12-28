namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a checkbox for boolean values.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridCheckboxColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the boolean value from an item.
    /// </summary>
    public Func<T, bool>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the action that sets the boolean value on an item.
    /// </summary>
    public Action<T, bool>? ValueSetter { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridCheckboxColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the two-way binding for this checkbox column.
    /// </summary>
    /// <param name="valueGetter">A function that gets the boolean value from an item.</param>
    /// <param name="valueSetter">An action that sets the boolean value on an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridCheckboxColumn<T> SetBinding(Func<T, bool> valueGetter, Action<T, bool> valueSetter)
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
    public DataGridCheckboxColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var isChecked = ValueGetter?.Invoke(item) ?? false;
        var checkbox = new Checkbox().SetIsChecked(isChecked);

        if (ValueSetter != null && ValueGetter != null)
        {
            var setter = ValueSetter;
            var getter = ValueGetter;

            checkbox.BindIsChecked(
                $"DataGridCheckbox_{item?.GetHashCode()}_{Header}",
                () => getter(item),
                value => setter(item, value));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    checkbox.SetIsChecked(getter(item));
                };
            }
        }

        return checkbox;
    }
}
