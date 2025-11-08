using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace PlusUi.core;

public class PlusUiNavigationService(IServiceProvider serviceProvider, ILogger<PlusUiNavigationService>? logger = null) : INavigationService
{
    private NavigationContainer? _navigationContainer;
    private readonly ILogger<PlusUiNavigationService>? _logger = logger;

    public void NavigateTo<TPage>() where TPage : UiPageElement
    {
        NavigateTo(typeof(TPage), false);
    }

    private void NavigateTo(Type pageType, bool isInitCall)
    {
        if (_navigationContainer is null)
        {
            var exception = new InvalidOperationException("NavigationContainer is not initialized. Ensure Initialize() is called before navigation.");
            _logger?.LogError(exception, "Navigation failed: NavigationContainer not initialized");
            throw exception;
        }

        if (isInitCall || _navigationContainer.Page.GetType() != pageType)
        {
            _navigationContainer.Page?.Disappearing();
            if (_navigationContainer.Page is not null)
            {
                _navigationContainer.Page.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
            try
            {
                var page = serviceProvider.GetRequiredService(pageType) as UiPageElement;
                if (page is null)
                {
                    var exception = new InvalidOperationException($"Page of type {pageType.Name} could not be resolved or is not a UiPageElement. Ensure the page is registered in the service collection.");
                    _logger?.LogError(exception, "Navigation failed: Page not found for type {PageType}", pageType.Name);
                    throw exception;
                }

                _navigationContainer.Page = page;
                _navigationContainer.Page.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
                _navigationContainer.Page.BuildPage();
                _logger?.LogDebug("Navigated to page: {PageType}", pageType.Name);
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw our own exceptions
            }
            catch (Exception e)
            {
                var exception = new InvalidOperationException($"Failed to navigate to page {pageType.Name}. Ensure the page is registered in Program.cs via services.AddTransient<{pageType.Name}>()", e);
                _logger?.LogError(e, "Navigation failed for page type {PageType}", pageType.Name);
                throw exception;
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not null)
        {
            _navigationContainer?.Page.UpdateBindings(e.PropertyName);
        }
    }

    public void Initialize()
    {
        _navigationContainer ??= serviceProvider.GetRequiredService<NavigationContainer>();
        NavigateTo(_navigationContainer.Page.GetType(), true);
    }
}
