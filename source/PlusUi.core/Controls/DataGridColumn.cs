using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Abstract base class for DataGrid columns.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public abstract class DataGridColumn<T>
{
    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    public string Header { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    public DataGridColumnWidth Width { get; protected set; } = DataGridColumnWidth.Star(1);

    /// <summary>
    /// Gets the actual calculated width of the column after layout.
    /// </summary>
    public float ActualWidth { get; internal set; }

    /// <summary>
    /// Creates the cell content for the specified item.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <param name="rowIndex">The row index.</param>
    /// <returns>The UI element for the cell.</returns>
    public abstract UiElement CreateCell(T item, int rowIndex);
}

/// <summary>
/// A DataGrid column that displays text values.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridTextColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the function that extracts the text value from an item.
    /// </summary>
    public Func<T, string>? ValueGetter { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTextColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the value binding for this column.
    /// </summary>
    /// <param name="valueGetter">A function that extracts the display value from an item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTextColumn<T> SetBinding(Func<T, string> valueGetter)
    {
        ValueGetter = valueGetter;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTextColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        var label = new Label();
        if (ValueGetter != null)
        {
            var getter = ValueGetter;
            label.SetText(getter(item));

            if (item is System.ComponentModel.INotifyPropertyChanged notifyItem)
            {
                notifyItem.PropertyChanged += (_, _) =>
                {
                    label.SetText(getter(item));
                };
            }
        }
        return label;
    }
}

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

/// <summary>
/// A DataGrid column that uses a custom template for cell content.
/// </summary>
/// <typeparam name="T">The type of items in the DataGrid.</typeparam>
public class DataGridTemplateColumn<T> : DataGridColumn<T>
{
    /// <summary>
    /// Gets the cell template function.
    /// </summary>
    public Func<T, int, UiElement>? CellTemplate { get; private set; }

    /// <summary>
    /// Sets the column header text.
    /// </summary>
    /// <param name="header">The header text.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTemplateColumn<T> SetHeader(string header)
    {
        Header = header;
        return this;
    }

    /// <summary>
    /// Sets the cell template.
    /// </summary>
    /// <param name="template">A function that creates the cell content for each item.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTemplateColumn<T> SetCellTemplate(Func<T, int, UiElement> template)
    {
        CellTemplate = template;
        return this;
    }

    /// <summary>
    /// Sets the column width.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <returns>This column for method chaining.</returns>
    public DataGridTemplateColumn<T> SetWidth(DataGridColumnWidth width)
    {
        Width = width;
        return this;
    }

    /// <inheritdoc />
    public override UiElement CreateCell(T item, int rowIndex)
    {
        return CellTemplate?.Invoke(item, rowIndex) ?? new Solid();
    }
}
