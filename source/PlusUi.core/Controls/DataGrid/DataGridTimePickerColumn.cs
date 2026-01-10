namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a time picker for time selection.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridTimePickerColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the time value from an item.
    /// </summary>
    public Func<T, TimeOnly?>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the action that sets the time value on an item.
    /// </summary>
    public Action<T, TimeOnly?>? ValueSetter { get; private set; }

    /// <summary>
    /// Gets the minimum allowed time.
    /// </summary>
    public TimeOnly? MinTime { get; private set; }

    /// <summary>
    /// Gets the maximum allowed time.
    /// </summary>
    public TimeOnly? MaxTime { get; private set; }

    /// <summary>
    /// Gets the display format for times.
    /// </summary>
    public string DisplayFormat { get; private set; } = "HH:mm";

    /// <summary>
    /// Gets the minute increment.
    /// </summary>
    public int MinuteIncrement { get; private set; } = 1;

    /// <summary>
    /// Gets whether to use 24-hour format.
    /// </summary>
    public bool Is24HourFormat { get; private set; } = true;

    /// <summary>
    /// Gets the placeholder text.
    /// </summary>
    public string? Placeholder { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridTimePickerColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the two-way binding for this time picker column.
    /// </summary>
    public DataGridTimePickerColumn<T> SetBinding(Func<T, TimeOnly?> valueGetter, Action<T, TimeOnly?> valueSetter)
    {
        ValueGetter = valueGetter;
        ValueSetter = valueSetter;
        return this;
    }

    /// <summary>
    /// Sets the minimum allowed time.
    /// </summary>
    public DataGridTimePickerColumn<T> SetMinTime(TimeOnly? minTime)
    {
        MinTime = minTime;
        return this;
    }

    /// <summary>
    /// Sets the maximum allowed time.
    /// </summary>
    public DataGridTimePickerColumn<T> SetMaxTime(TimeOnly? maxTime)
    {
        MaxTime = maxTime;
        return this;
    }

    /// <summary>
    /// Sets the display format for times.
    /// </summary>
    public DataGridTimePickerColumn<T> SetDisplayFormat(string format)
    {
        DisplayFormat = format;
        return this;
    }

    /// <summary>
    /// Sets the minute increment.
    /// </summary>
    public DataGridTimePickerColumn<T> SetMinuteIncrement(int increment)
    {
        MinuteIncrement = increment;
        return this;
    }

    /// <summary>
    /// Sets whether to use 24-hour format.
    /// </summary>
    public DataGridTimePickerColumn<T> Set24HourFormat(bool is24Hour)
    {
        Is24HourFormat = is24Hour;
        return this;
    }

    /// <summary>
    /// Sets the placeholder text.
    /// </summary>
    public DataGridTimePickerColumn<T> SetPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridTimePickerColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var timePicker = new TimePicker()
            .SetDisplayFormat(DisplayFormat)
            .SetMinuteIncrement(MinuteIncrement)
            .Set24HourFormat(Is24HourFormat);

        if (MinTime.HasValue)
        {
            timePicker.SetMinTime(MinTime);
        }

        if (MaxTime.HasValue)
        {
            timePicker.SetMaxTime(MaxTime);
        }

        if (Placeholder != null)
        {
            timePicker.SetPlaceholder(Placeholder);
        }

        if (ValueGetter != null)
        {
            var selectedValue = ValueGetter(item);
            timePicker.SetSelectedTime(selectedValue);
        }

        if (ValueSetter != null && ValueGetter != null)
        {
            var setter = ValueSetter;
            var getter = ValueGetter;

            timePicker.SetOnSelectedTimeChanged(value => setter(item, value));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    timePicker.SetSelectedTime(getter(item));
                };
            }
        }

        return timePicker;
    }
}
