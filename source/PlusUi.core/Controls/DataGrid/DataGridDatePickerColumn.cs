namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a date picker for date selection.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridDatePickerColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the date value from an item.
    /// </summary>
    public Func<T, DateOnly?>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the action that sets the date value on an item.
    /// </summary>
    public Action<T, DateOnly?>? ValueSetter { get; private set; }

    /// <summary>
    /// Gets the minimum allowed date.
    /// </summary>
    public DateOnly? MinDate { get; private set; }

    /// <summary>
    /// Gets the maximum allowed date.
    /// </summary>
    public DateOnly? MaxDate { get; private set; }

    /// <summary>
    /// Gets the display format for dates.
    /// </summary>
    public string DisplayFormat { get; private set; } = "dd.MM.yyyy";

    /// <summary>
    /// Gets the placeholder text.
    /// </summary>
    public string? Placeholder { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridDatePickerColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the two-way binding for this date picker column.
    /// </summary>
    public DataGridDatePickerColumn<T> SetBinding(Func<T, DateOnly?> valueGetter, Action<T, DateOnly?> valueSetter)
    {
        ValueGetter = valueGetter;
        ValueSetter = valueSetter;
        return this;
    }

    /// <summary>
    /// Sets the minimum allowed date.
    /// </summary>
    public DataGridDatePickerColumn<T> SetMinDate(DateOnly? minDate)
    {
        MinDate = minDate;
        return this;
    }

    /// <summary>
    /// Sets the maximum allowed date.
    /// </summary>
    public DataGridDatePickerColumn<T> SetMaxDate(DateOnly? maxDate)
    {
        MaxDate = maxDate;
        return this;
    }

    /// <summary>
    /// Sets the display format for dates.
    /// </summary>
    public DataGridDatePickerColumn<T> SetDisplayFormat(string format)
    {
        DisplayFormat = format;
        return this;
    }

    /// <summary>
    /// Sets the placeholder text.
    /// </summary>
    public DataGridDatePickerColumn<T> SetPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridDatePickerColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var datePicker = new DatePicker()
            .SetDisplayFormat(DisplayFormat);

        if (MinDate.HasValue)
        {
            datePicker.SetMinDate(MinDate);
        }

        if (MaxDate.HasValue)
        {
            datePicker.SetMaxDate(MaxDate);
        }

        if (Placeholder != null)
        {
            datePicker.SetPlaceholder(Placeholder);
        }

        if (ValueGetter != null)
        {
            var selectedValue = ValueGetter(item);
            datePicker.SetSelectedDate(selectedValue);
        }

        if (ValueSetter != null && ValueGetter != null)
        {
            var setter = ValueSetter;
            var getter = ValueGetter;

            datePicker.SetOnSelectedDateChanged(value => setter(item, value));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    datePicker.SetSelectedDate(getter(item));
                };
            }
        }

        return datePicker;
    }
}
