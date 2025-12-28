namespace PlusUi.core;

/// <summary>
/// Extension methods for attaching context menus to UI elements.
/// </summary>
public static class ContextMenuExtensions
{
    /// <summary>
    /// Ensures the element has a ContextMenu, creating one if necessary.
    /// </summary>
    private static ContextMenu EnsureContextMenu<T>(T element) where T : UiElement
    {
        return element.ContextMenu ??= new ContextMenu();
    }

    /// <summary>
    /// Attaches a context menu to the element.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="menu">The context menu to attach.</param>
    /// <returns>The element for method chaining.</returns>
    /// <example>
    /// <code>
    /// new Button()
    ///     .SetText("Click me")
    ///     .SetContextMenu(new ContextMenu()
    ///         .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X"))
    ///         .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C"))
    ///         .AddSeparator()
    ///         .AddItem(new MenuItem().SetText("Delete")));
    /// </code>
    /// </example>
    public static T SetContextMenu<T>(this T element, ContextMenu menu) where T : UiElement
    {
        element.ContextMenu = menu;
        return element;
    }

    /// <summary>
    /// Configures the context menu using a fluent builder.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="configure">Action to configure the context menu.</param>
    /// <returns>The element for method chaining.</returns>
    /// <example>
    /// <code>
    /// new Button()
    ///     .SetText("Click me")
    ///     .SetContextMenu(m => m
    ///         .AddItem(new MenuItem().SetText("Option 1"))
    ///         .AddItem(new MenuItem().SetText("Option 2")));
    /// </code>
    /// </example>
    public static T SetContextMenu<T>(this T element, Action<ContextMenu> configure) where T : UiElement
    {
        configure(EnsureContextMenu(element));
        return element;
    }

    /// <summary>
    /// Sets the background of the element's context menu.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="background">The background.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetContextMenuBackground<T>(this T element, IBackground background) where T : UiElement
    {
        EnsureContextMenu(element).SetBackground(background);
        return element;
    }

    /// <summary>
    /// Sets the background color of the element's context menu.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="color">The background color.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetContextMenuBackground<T>(this T element, Color color) where T : UiElement
    {
        EnsureContextMenu(element).SetBackground(color);
        return element;
    }

    /// <summary>
    /// Sets the hover background color of the element's context menu.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="color">The hover background color.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetContextMenuHoverBackgroundColor<T>(this T element, Color color) where T : UiElement
    {
        EnsureContextMenu(element).SetHoverBackgroundColor(color);
        return element;
    }

    /// <summary>
    /// Sets the text color of the element's context menu.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="color">The text color.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetContextMenuTextColor<T>(this T element, Color color) where T : UiElement
    {
        EnsureContextMenu(element).SetTextColor(color);
        return element;
    }

    /// <summary>
    /// Binds the context menu background to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindContextMenuBackground<T>(this T element, string propertyName, Func<IBackground> propertyGetter) where T : UiElement
    {
        EnsureContextMenu(element).BindBackground(propertyName, propertyGetter);
        return element;
    }

    /// <summary>
    /// Binds the context menu background color to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindContextMenuBackground<T>(this T element, string propertyName, Func<Color> propertyGetter) where T : UiElement
    {
        EnsureContextMenu(element).BindBackground(propertyName, propertyGetter);
        return element;
    }

    /// <summary>
    /// Binds the context menu hover background color to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindContextMenuHoverBackgroundColor<T>(this T element, string propertyName, Func<Color> propertyGetter) where T : UiElement
    {
        EnsureContextMenu(element).BindHoverBackgroundColor(propertyName, propertyGetter);
        return element;
    }

    /// <summary>
    /// Binds the context menu text color to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindContextMenuTextColor<T>(this T element, string propertyName, Func<Color> propertyGetter) where T : UiElement
    {
        EnsureContextMenu(element).BindTextColor(propertyName, propertyGetter);
        return element;
    }
}
