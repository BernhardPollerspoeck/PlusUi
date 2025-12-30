namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a slider for numeric value selection.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridSliderColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the value from an item.
    /// </summary>
    public Func<T, float>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the action that sets the value on an item.
    /// </summary>
    public Action<T, float>? ValueSetter { get; private set; }

    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    public float MinValue { get; private set; } = 0f;

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    public float MaxValue { get; private set; } = 100f;

    /// <summary>
    /// Gets the minimum track color (filled portion).
    /// </summary>
    public Color? MinimumTrackColor { get; private set; }

    /// <summary>
    /// Gets the maximum track color (unfilled portion).
    /// </summary>
    public Color? MaximumTrackColor { get; private set; }

    /// <summary>
    /// Gets the thumb color.
    /// </summary>
    public Color? ThumbColor { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridSliderColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the two-way binding for this slider column.
    /// </summary>
    public DataGridSliderColumn<T> SetBinding(Func<T, float> valueGetter, Action<T, float> valueSetter)
    {
        ValueGetter = valueGetter;
        ValueSetter = valueSetter;
        return this;
    }

    /// <summary>
    /// Sets the minimum value.
    /// </summary>
    public DataGridSliderColumn<T> SetMinValue(float minValue)
    {
        MinValue = minValue;
        return this;
    }

    /// <summary>
    /// Sets the maximum value.
    /// </summary>
    public DataGridSliderColumn<T> SetMaxValue(float maxValue)
    {
        MaxValue = maxValue;
        return this;
    }

    /// <summary>
    /// Sets the value range.
    /// </summary>
    public DataGridSliderColumn<T> SetRange(float minValue, float maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        return this;
    }

    /// <summary>
    /// Sets the minimum track color (filled portion).
    /// </summary>
    public DataGridSliderColumn<T> SetMinimumTrackColor(Color color)
    {
        MinimumTrackColor = color;
        return this;
    }

    /// <summary>
    /// Sets the maximum track color (unfilled portion).
    /// </summary>
    public DataGridSliderColumn<T> SetMaximumTrackColor(Color color)
    {
        MaximumTrackColor = color;
        return this;
    }

    /// <summary>
    /// Sets the thumb color.
    /// </summary>
    public DataGridSliderColumn<T> SetThumbColor(Color color)
    {
        ThumbColor = color;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridSliderColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var slider = new Slider()
            .SetMinimum(MinValue)
            .SetMaximum(MaxValue)
            .SetDesiredHeight(24);

        if (MinimumTrackColor.HasValue)
        {
            slider.SetMinimumTrackColor(MinimumTrackColor.Value);
        }

        if (MaximumTrackColor.HasValue)
        {
            slider.SetMaximumTrackColor(MaximumTrackColor.Value);
        }

        if (ThumbColor.HasValue)
        {
            slider.SetThumbColor(ThumbColor.Value);
        }

        if (ValueGetter != null)
        {
            var value = ValueGetter(item);
            slider.SetValue(value);
        }

        if (ValueSetter != null && ValueGetter != null)
        {
            var setter = ValueSetter;
            var getter = ValueGetter;

            slider.BindValue(
                $"DataGridSlider_{item?.GetHashCode()}_{Header}",
                () => getter(item),
                value => setter(item, value));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    slider.SetValue(getter(item));
                };
            }
        }

        return slider;
    }
}
