namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a combo box for selecting from a list of options.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
/// <typeparam name="TOption">The type of options in the combo box.</typeparam>
public class DataGridComboBoxColumn<T, TOption> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the selected value from an item.
    /// </summary>
    public Func<T, TOption?>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the action that sets the selected value on an item.
    /// </summary>
    public Action<T, TOption?>? ValueSetter { get; private set; }

    /// <summary>
    /// Gets the available options for the combo box.
    /// </summary>
    public IEnumerable<TOption>? ItemsSource { get; private set; }

    /// <summary>
    /// Gets the function that extracts items source per row.
    /// </summary>
    public Func<T, IEnumerable<TOption>?>? ItemsSourceGetter { get; private set; }

    /// <summary>
    /// Gets the display function for options.
    /// </summary>
    public Func<TOption, string>? DisplayFunc { get; private set; }

    /// <summary>
    /// Gets the placeholder text.
    /// </summary>
    public string? Placeholder { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the two-way binding for this combo box column.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetBinding(Func<T, TOption?> valueGetter, Action<T, TOption?> valueSetter)
    {
        ValueGetter = valueGetter;
        ValueSetter = valueSetter;
        return this;
    }

    /// <summary>
    /// Sets the available options for all rows.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetItemsSource(IEnumerable<TOption> items)
    {
        ItemsSource = items;
        return this;
    }

    /// <summary>
    /// Sets a function to get items source per row.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetItemsSourceGetter(Func<T, IEnumerable<TOption>?> getter)
    {
        ItemsSourceGetter = getter;
        return this;
    }

    /// <summary>
    /// Sets the display function for options.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetDisplayFunc(Func<TOption, string> displayFunc)
    {
        DisplayFunc = displayFunc;
        return this;
    }

    /// <summary>
    /// Sets the placeholder text.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridComboBoxColumn<T, TOption> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var items = ItemsSourceGetter?.Invoke(item) ?? ItemsSource;
        var comboBox = new ComboBox<TOption>()
            .SetItemsSource(items);

        if (DisplayFunc != null)
        {
            comboBox.SetDisplayFunc(DisplayFunc);
        }

        if (Placeholder != null)
        {
            comboBox.SetPlaceholder(Placeholder);
        }

        if (ValueGetter != null)
        {
            var selectedValue = ValueGetter(item);
            comboBox.SetSelectedItem(selectedValue);
        }

        if (ValueSetter != null && ValueGetter != null)
        {
            var setter = ValueSetter;
            var getter = ValueGetter;

            // Use callback for two-way binding (since property name is dynamic and runtime-dependent)
            comboBox.SetOnSelectionChanged(value => setter(item, value));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    comboBox.SetSelectedItem(getter(item));
                };
            }
        }

        return comboBox;
    }
}
