namespace PlusUi.core;

/// <summary>
/// Provides navigation services for page-based navigation within the application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to a page of the specified type. The page must be registered in the service collection.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to.</typeparam>
    /// <remarks>
    /// The page type must be registered via dependency injection in your application startup (e.g., services.AddTransient&lt;MyPage&gt;()).
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the page is not registered in the service collection.</exception>
    void NavigateTo<TPage>() where TPage : UiPageElement;

    /// <summary>
    /// Navigates to a page of the specified type with an optional parameter.
    /// The page must be registered in the service collection.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to.</typeparam>
    /// <param name="parameter">Optional parameter to pass to the page. Can be accessed via OnNavigatedTo.</param>
    /// <remarks>
    /// The page type must be registered via dependency injection in your application startup.
    /// The parameter will be passed to the page's OnNavigatedTo method.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the page is not registered in the service collection.</exception>
    void NavigateTo<TPage>(object? parameter) where TPage : UiPageElement;

    /// <summary>
    /// Navigates back to the previous page in the navigation stack.
    /// Only works when EnableNavigationStack is true in PlusUiConfiguration.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when navigation stack is disabled or when already at the root page.
    /// </exception>
    void GoBack();

    /// <summary>
    /// Navigates back to the root page, clearing the entire navigation stack.
    /// Only works when EnableNavigationStack is true in PlusUiConfiguration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when navigation stack is disabled.</exception>
    void PopToRoot();

    /// <summary>
    /// Gets a value indicating whether backward navigation is possible.
    /// Returns true only when EnableNavigationStack is true and there are pages to go back to.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets the current depth of the navigation stack.
    /// Returns 1 when navigation stack is disabled (always one page).
    /// </summary>
    int StackDepth { get; }
}
