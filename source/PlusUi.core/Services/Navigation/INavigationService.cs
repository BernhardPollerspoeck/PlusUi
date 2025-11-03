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
}
