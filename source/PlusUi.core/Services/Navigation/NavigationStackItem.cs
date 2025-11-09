namespace PlusUi.core;

/// <summary>
/// Represents an item in the navigation stack, containing a page and its associated navigation parameter.
/// </summary>
internal sealed class NavigationStackItem
{
    /// <summary>
    /// Gets the page element in the navigation stack.
    /// </summary>
    public UiPageElement Page { get; }

    /// <summary>
    /// Gets the parameter that was passed when navigating to this page.
    /// </summary>
    public object? Parameter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationStackItem"/> class.
    /// </summary>
    /// <param name="page">The page element.</param>
    /// <param name="parameter">The navigation parameter.</param>
    public NavigationStackItem(UiPageElement page, object? parameter = null)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        Parameter = parameter;
    }
}
