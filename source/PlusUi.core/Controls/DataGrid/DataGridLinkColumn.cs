using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a clickable link/hyperlink.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridLinkColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the link text from an item.
    /// </summary>
    public Func<T, string>? TextGetter { get; private set; }

    /// <summary>
    /// Gets the command to execute when the link is clicked.
    /// </summary>
    public ICommand? Command { get; private set; }

    /// <summary>
    /// Gets the function that extracts the command parameter from an item.
    /// </summary>
    public Func<T, object?>? CommandParameterGetter { get; private set; }

    /// <summary>
    /// Gets the link color.
    /// </summary>
    public Color LinkColor { get; private set; } = new Color(0, 122, 255); // iOS blue

    /// <summary>
    /// Gets the hover color.
    /// </summary>
    public Color HoverColor { get; private set; } = new Color(0, 90, 200);


    /// <summary>
    /// Sets the column header text.
    /// </summary>
    public DataGridLinkColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the binding for the link text.
    /// </summary>
    public DataGridLinkColumn<T> SetBinding(Func<T, string> textGetter)
    {
        TextGetter = textGetter;
        return this;
    }

    /// <summary>
    /// Sets the command for the link.
    /// </summary>
    public DataGridLinkColumn<T> SetCommand(ICommand command)
    {
        Command = command;
        return this;
    }

    /// <summary>
    /// Sets the command with parameter getter.
    /// </summary>
    public DataGridLinkColumn<T> SetCommand(ICommand command, Func<T, object?> parameterGetter)
    {
        Command = command;
        CommandParameterGetter = parameterGetter;
        return this;
    }

    /// <summary>
    /// Sets the link color.
    /// </summary>
    public DataGridLinkColumn<T> SetLinkColor(Color color)
    {
        LinkColor = color;
        return this;
    }

    /// <summary>
    /// Sets the hover color.
    /// </summary>
    public DataGridLinkColumn<T> SetHoverColor(Color color)
    {
        HoverColor = color;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    public DataGridLinkColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var text = TextGetter?.Invoke(item) ?? string.Empty;

        var label = new Label()
            .SetText(text)
            .SetTextColor(LinkColor);

        // Wrap in TapGestureDetector for click handling
        var detector = new TapGestureDetector(label);

        if (Command != null)
        {
            var parameter = CommandParameterGetter?.Invoke(item) ?? item;
            if (parameter != null)
            {
                detector.SetCommand(Command).SetCommandParameter(parameter);
            }
            else
            {
                detector.SetCommand(Command);
            }
        }

        // Update text on property changes
        if (TextGetter != null && item is System.ComponentModel.INotifyPropertyChanged notifyItem)
        {
            var getter = TextGetter;
            notifyItem.PropertyChanged += (_, _) =>
            {
                label.SetText(getter(item));
            };
        }

        return detector;
    }
}
