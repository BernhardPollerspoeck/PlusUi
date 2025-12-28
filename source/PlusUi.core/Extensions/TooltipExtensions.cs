namespace PlusUi.core;

/// <summary>
/// Extension methods for attaching tooltips to UI elements.
/// </summary>
public static class TooltipExtensions
{
    /// <summary>
    /// Ensures the element has a TooltipAttachment, creating one if necessary.
    /// </summary>
    private static TooltipAttachment EnsureTooltip<T>(T element) where T : UiElement
    {
        return element.Tooltip ??= new TooltipAttachment();
    }
    /// <summary>
    /// Attaches a tooltip with text content to the element.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="content">The text content to display in the tooltip.</param>
    /// <returns>The element for method chaining.</returns>
    /// <example>
    /// <code>
    /// new Button()
    ///     .SetText("Save")
    ///     .SetTooltip("Saves the current document");
    /// </code>
    /// </example>
    public static T SetTooltip<T>(this T element, string content) where T : UiElement
    {
        EnsureTooltip(element).SetContent(content);
        return element;
    }

    /// <summary>
    /// Attaches a tooltip with custom UI element content to the element.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="content">The UI element to display in the tooltip.</param>
    /// <returns>The element for method chaining.</returns>
    /// <example>
    /// <code>
    /// new Button()
    ///     .SetText("Info")
    ///     .SetTooltip(new VStack()
    ///         .AddChild(new Label().SetText("Title").SetFontWeight(FontWeight.Bold))
    ///         .AddChild(new Label().SetText("Description")));
    /// </code>
    /// </example>
    public static T SetTooltip<T>(this T element, UiElement content) where T : UiElement
    {
        EnsureTooltip(element).SetContent(content);
        return element;
    }

    /// <summary>
    /// Configures the tooltip using a fluent builder.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="configure">Action to configure the tooltip.</param>
    /// <returns>The element for method chaining.</returns>
    /// <example>
    /// <code>
    /// new Button()
    ///     .SetText("Export")
    ///     .SetTooltip(t => t
    ///         .SetContent("Export data to CSV")
    ///         .SetPlacement(TooltipPlacement.Bottom)
    ///         .SetShowDelay(1000));
    /// </code>
    /// </example>
    public static T SetTooltip<T>(this T element, Action<TooltipAttachment> configure) where T : UiElement
    {
        configure(EnsureTooltip(element));
        return element;
    }

    /// <summary>
    /// Sets the tooltip placement.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="placement">The preferred tooltip placement.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetTooltipPlacement<T>(this T element, TooltipPlacement placement) where T : UiElement
    {
        EnsureTooltip(element).SetPlacement(placement);
        return element;
    }

    /// <summary>
    /// Sets the delay before showing the tooltip.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="milliseconds">The delay in milliseconds.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetTooltipShowDelay<T>(this T element, int milliseconds) where T : UiElement
    {
        EnsureTooltip(element).SetShowDelay(milliseconds);
        return element;
    }

    /// <summary>
    /// Sets the delay before hiding the tooltip.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="milliseconds">The delay in milliseconds.</param>
    /// <returns>The element for method chaining.</returns>
    public static T SetTooltipHideDelay<T>(this T element, int milliseconds) where T : UiElement
    {
        EnsureTooltip(element).SetHideDelay(milliseconds);
        return element;
    }

    /// <summary>
    /// Binds the tooltip content to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindTooltipContent<T>(this T element, string propertyName, Func<object?> propertyGetter) where T : UiElement
    {
        EnsureTooltip(element).BindContent(propertyName, propertyGetter);
        return element;
    }

    /// <summary>
    /// Binds the tooltip placement to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindTooltipPlacement<T>(this T element, string propertyName, Func<TooltipPlacement> propertyGetter) where T : UiElement
    {
        EnsureTooltip(element).BindPlacement(propertyName, propertyGetter);
        return element;
    }

    /// <summary>
    /// Binds the tooltip show delay to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindTooltipShowDelay<T>(this T element, string propertyName, Func<int> propertyGetter) where T : UiElement
    {
        EnsureTooltip(element).BindShowDelay(propertyName, propertyGetter);
        return element;
    }

    /// <summary>
    /// Binds the tooltip hide delay to a property.
    /// </summary>
    /// <typeparam name="T">The type of UI element.</typeparam>
    /// <param name="element">The target element.</param>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">Function to get the property value.</param>
    /// <returns>The element for method chaining.</returns>
    public static T BindTooltipHideDelay<T>(this T element, string propertyName, Func<int> propertyGetter) where T : UiElement
    {
        EnsureTooltip(element).BindHideDelay(propertyName, propertyGetter);
        return element;
    }
}
