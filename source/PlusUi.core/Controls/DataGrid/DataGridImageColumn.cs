namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays an image.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridImageColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the image source from an item.
    /// </summary>
    public Func<T, string?>? ImageSourceGetter { get; private set; }

    /// <summary>
    /// Gets the aspect ratio mode.
    /// </summary>
    public Aspect Aspect { get; private set; } = Aspect.AspectFit;

    /// <summary>
    /// Gets the image size.
    /// </summary>
    public Size? ImageSize { get; private set; }

    /// <summary>
    /// Gets the tint color.
    /// </summary>
    public Color? TintColor { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridImageColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the binding for the image source.
    /// </summary>
    public DataGridImageColumn<T> SetBinding(Func<T, string?> imageSourceGetter)
    {
        ImageSourceGetter = imageSourceGetter;
        return this;
    }

    /// <summary>
    /// Sets the aspect ratio mode.
    /// </summary>
    public DataGridImageColumn<T> SetAspect(Aspect aspect)
    {
        Aspect = aspect;
        return this;
    }

    /// <summary>
    /// Sets the image size.
    /// </summary>
    public DataGridImageColumn<T> SetImageSize(Size size)
    {
        ImageSize = size;
        return this;
    }

    /// <summary>
    /// Sets the tint color.
    /// </summary>
    public DataGridImageColumn<T> SetTintColor(Color color)
    {
        TintColor = color;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridImageColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var image = new Image()
            .SetAspect(Aspect);

        if (ImageSize.HasValue)
        {
            image.SetDesiredSize(ImageSize.Value);
        }

        if (TintColor.HasValue)
        {
            image.SetTintColor(TintColor.Value);
        }

        if (ImageSourceGetter != null)
        {
            var source = ImageSourceGetter(item);
            if (!string.IsNullOrEmpty(source))
            {
                image.SetImageSource(source);
            }

            var getter = ImageSourceGetter;
            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    var newSource = getter(item);
                    if (!string.IsNullOrEmpty(newSource))
                    {
                        image.SetImageSource(newSource);
                    }
                };
            }
        }

        return image;
    }
}
