using PlusUi.core.Binding;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Represents a single item in a Menu or ContextMenu.
/// Supports text, icons, keyboard shortcuts, commands, and nested submenus.
/// </summary>
/// <example>
/// <code>
/// new MenuItem()
///     .SetText("Save")
///     .SetIcon("save.png")
///     .SetShortcut("Ctrl+S")
///     .SetCommand(vm.SaveCommand);
///
/// // With submenu
/// new MenuItem()
///     .SetText("Recent Files")
///     .AddItem(new MenuItem().SetText("Document1.txt"))
///     .AddItem(new MenuItem().SetText("Document2.txt"));
/// </code>
/// </example>
public class MenuItem
{
    private readonly Dictionary<string, List<Action>> _bindings = [];
    private readonly ExpressionPathService _expressionPathService = new();

    #region Text
    internal string Text { get; set; } = string.Empty;

    public MenuItem SetText(string text)
    {
        Text = text;
        return this;
    }

    public MenuItem BindText(Expression<Func<string>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => Text = getter());
        return this;
    }
    #endregion

    #region Icon
    internal string? Icon { get; set; }

    public MenuItem SetIcon(string icon)
    {
        Icon = icon;
        return this;
    }

    public MenuItem BindIcon(Expression<Func<string?>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => Icon = getter());
        return this;
    }
    #endregion

    #region Shortcut
    internal string? Shortcut { get; set; }

    public MenuItem SetShortcut(string shortcut)
    {
        Shortcut = shortcut;
        return this;
    }

    public MenuItem BindShortcut(Expression<Func<string?>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => Shortcut = getter());
        return this;
    }
    #endregion

    #region Command
    internal ICommand? Command { get; set; }

    public MenuItem SetCommand(ICommand command)
    {
        Command = command;
        return this;
    }

    public MenuItem BindCommand(Expression<Func<ICommand?>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => Command = getter());
        return this;
    }

    internal object? CommandParameter { get; set; }

    public MenuItem SetCommandParameter(object parameter)
    {
        CommandParameter = parameter;
        return this;
    }

    public MenuItem BindCommandParameter(Expression<Func<object?>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => CommandParameter = getter());
        return this;
    }
    #endregion

    #region IsEnabled
    internal bool IsEnabled { get; set; } = true;

    public MenuItem SetIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
        return this;
    }

    public MenuItem BindIsEnabled(Expression<Func<bool>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => IsEnabled = getter());
        return this;
    }
    #endregion

    #region IsChecked
    internal bool IsChecked { get; set; }

    public MenuItem SetIsChecked(bool isChecked)
    {
        IsChecked = isChecked;
        return this;
    }

    public MenuItem BindIsChecked(Expression<Func<bool>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => IsChecked = getter());
        return this;
    }
    #endregion

    #region Items (Submenu)
    internal List<object> Items { get; } = new();

    /// <summary>
    /// Gets whether this menu item has sub-items (is a submenu).
    /// </summary>
    public bool HasSubItems => Items.Count > 0;

    /// <summary>
    /// Adds a menu item to the submenu.
    /// </summary>
    public MenuItem AddItem(MenuItem item)
    {
        Items.Add(item);
        return this;
    }

    /// <summary>
    /// Adds a separator to the submenu.
    /// </summary>
    public MenuItem AddItem(MenuSeparator separator)
    {
        Items.Add(separator);
        return this;
    }

    /// <summary>
    /// Adds a separator to the submenu.
    /// </summary>
    public MenuItem AddSeparator()
    {
        Items.Add(new MenuSeparator());
        return this;
    }
    #endregion

    #region Bindings
    private void RegisterBinding(string[] propertyNames, Action updateAction)
    {
        foreach (var propertyName in propertyNames)
        {
            if (!_bindings.TryGetValue(propertyName, out var updateActions))
            {
                updateActions = [];
                _bindings.Add(propertyName, updateActions);
            }
            updateActions.Add(updateAction);
        }
        updateAction();
    }

    /// <summary>
    /// Updates all bindings for this menu item.
    /// </summary>
    internal void UpdateBindings()
    {
        foreach (var propertyGroup in _bindings)
        {
            foreach (var update in propertyGroup.Value)
            {
                update();
            }
        }
    }

    /// <summary>
    /// Updates bindings for a specific property.
    /// </summary>
    internal void UpdateBindings(string propertyName)
    {
        if (_bindings.TryGetValue(propertyName, out var updateActions))
        {
            foreach (var update in updateActions)
            {
                update();
            }
        }
    }
    #endregion

    /// <summary>
    /// Executes the command if available and enabled.
    /// </summary>
    internal void Execute()
    {
        if (!IsEnabled) return;

        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
}
