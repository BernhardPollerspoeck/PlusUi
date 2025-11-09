namespace PlusUi.core;

/// <summary>
/// Represents an item in the navigation stack, containing a page and its associated navigation parameter.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NavigationStackItem"/> class.
/// </remarks>
/// <param name="page">The page element.</param>
/// <param name="parameter">The navigation parameter.</param>
public sealed class NavigationStackItem(UiPageElement page, object? parameter = null)
{
    /// <summary>
    /// Gets the page element in the navigation stack.
    /// </summary>
    public UiPageElement Page { get; } = page;

    /// <summary>
    /// Gets the parameter that was passed when navigating to this page.
    /// </summary>
    public object? Parameter { get; } = parameter;
}
