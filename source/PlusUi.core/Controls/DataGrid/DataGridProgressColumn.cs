namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a progress bar.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridProgressColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the progress value (0-1) from an item.
    /// </summary>
    public Func<T, float>? ValueGetter { get; private set; }

    /// <summary>
    /// Gets the progress bar color.
    /// </summary>
    public Color? ProgressColor { get; private set; }

    /// <summary>
    /// Gets the progress bar track color.
    /// </summary>
    public Color? TrackColor { get; private set; }

    /// <summary>
    /// Gets the progress bar height.
    /// </summary>
    public float? BarHeight { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridProgressColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the binding for the progress value (0-1).
    /// </summary>
    public DataGridProgressColumn<T> SetBinding(Func<T, float> valueGetter)
    {
        ValueGetter = valueGetter;
        return this;
    }

    /// <summary>
    /// Sets the progress bar color.
    /// </summary>
    public DataGridProgressColumn<T> SetProgressColor(Color color)
    {
        ProgressColor = color;
        return this;
    }

    /// <summary>
    /// Sets the track color.
    /// </summary>
    public DataGridProgressColumn<T> SetTrackColor(Color color)
    {
        TrackColor = color;
        return this;
    }

    /// <summary>
    /// Sets the progress bar height.
    /// </summary>
    public DataGridProgressColumn<T> SetBarHeight(float height)
    {
        BarHeight = height;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridProgressColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var progressBar = new ProgressBar()
            .SetDesiredHeight(BarHeight ?? 10f);

        if (ProgressColor.HasValue)
        {
            progressBar.SetProgressColor(ProgressColor.Value);
        }

        if (ValueGetter != null)
        {
            var progress = ValueGetter(item);
            progressBar.SetProgress(progress);

            var getter = ValueGetter;
            progressBar.BindProgress(
                $"DataGridProgress_{item?.GetHashCode()}_{Header}",
                () => getter(item));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    progressBar.SetProgress(getter(item));
                };
            }
        }

        return progressBar;
    }
}
