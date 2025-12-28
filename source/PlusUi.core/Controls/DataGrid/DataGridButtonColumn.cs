using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// A DataGrid column that displays a button.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridButtonColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the command to execute when the button is clicked.
    /// </summary>
    public ICommand? Command { get; private set; }

    /// <summary>
    /// Gets the function that extracts the command parameter from an item.
    /// </summary>
    public Func<T, object?>? CommandParameterGetter { get; private set; }

    /// <summary>
    /// Gets the function that extracts the command from an item.
    /// </summary>
    public Func<T, ICommand?>? ItemCommandGetter { get; private set; }

    /// <summary>
    /// Gets the static button text.
    /// </summary>
    public string ButtonText { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the function that generates button text for each item.
    /// </summary>
    public Func<T, string>? ButtonTextGetter { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the command for the button.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetCommand(ICommand command)
    {
        Command = command;
        return this;
    }

    /// <summary>
    /// Sets the command for the button with a parameter getter.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="parameterGetter">A function that extracts the command parameter from an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetCommand(ICommand command, Func<T, object?> parameterGetter)
    {
        Command = command;
        CommandParameterGetter = parameterGetter;
        return this;
    }

    /// <summary>
    /// Sets a function that gets the command from each item.
    /// </summary>
    /// <param name="commandGetter">A function that extracts the command from an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetItemCommand(Func<T, ICommand?> commandGetter)
    {
        ItemCommandGetter = commandGetter;
        return this;
    }

    /// <summary>
    /// Binds the item command to a property on each item.
    /// </summary>
    /// <param name="commandGetter">A function that extracts the command from an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> BindItemCommand(Func<T, ICommand?> commandGetter)
    {
        ItemCommandGetter = commandGetter;
        return this;
    }

    /// <summary>
    /// Sets a static button text.
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetButtonText(string text)
    {
        ButtonText = text;
        return this;
    }

    /// <summary>
    /// Sets a dynamic button text getter.
    /// </summary>
    /// <param name="textGetter">A function that generates button text for each item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetButtonTextGetter(Func<T, string> textGetter)
    {
        ButtonTextGetter = textGetter;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridButtonColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var text = ButtonTextGetter?.Invoke(item) ?? ButtonText;
        var button = new Button().SetText(text);

        if (ItemCommandGetter != null)
        {
            var itemCommand = ItemCommandGetter(item);
            if (itemCommand != null)
            {
                button.SetCommand(itemCommand);
            }
        }
        else if (Command != null)
        {
            var parameter = CommandParameterGetter?.Invoke(item) ?? item;
            button.SetCommand(Command).SetCommandParameter(parameter);
        }

        return button;
    }
}
