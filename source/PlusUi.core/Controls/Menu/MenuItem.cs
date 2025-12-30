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

    #region Text
    internal string Text { get; set; } = string.Empty;

    public MenuItem SetText(string text)
    {
        Text = text;
        return this;
    }

    public MenuItem BindText(string propertyName, Func<string> propertyGetter)
    {
        RegisterBinding(propertyName, () => Text = propertyGetter());
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

    public MenuItem BindIcon(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Icon = propertyGetter());
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

    public MenuItem BindShortcut(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Shortcut = propertyGetter());
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

    public MenuItem BindCommand(string propertyName, Func<ICommand?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Command = propertyGetter());
        return this;
    }

    internal object? CommandParameter { get; set; }

    public MenuItem SetCommandParameter(object parameter)
    {
        CommandParameter = parameter;
        return this;
    }

    public MenuItem BindCommandParameter(string propertyName, Func<object?> propertyGetter)
    {
        RegisterBinding(propertyName, () => CommandParameter = propertyGetter());
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

    public MenuItem BindIsEnabled(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsEnabled = propertyGetter());
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

    public MenuItem BindIsChecked(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsChecked = propertyGetter());
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
    private void RegisterBinding(string propertyName, Action updateAction)
    {
        if (!_bindings.TryGetValue(propertyName, out var updateActions))
        {
            updateActions = [];
            _bindings.Add(propertyName, updateActions);
        }
        updateActions.Add(updateAction);
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

/// <summary>
/// Represents a visual separator between menu items.
/// </summary>
public class MenuSeparator { }
