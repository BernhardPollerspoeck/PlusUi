using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace PlusUi.core;

public class PlusUiNavigationService(IServiceProvider serviceProvider, ILogger<PlusUiNavigationService>? logger = null) : INavigationService
{
    private NavigationContainer? _navigationContainer;
    private readonly ILogger<PlusUiNavigationService>? _logger = logger;

    /// <inheritdoc/>
    public bool CanGoBack => _navigationContainer?.CanGoBack ?? false;

    /// <inheritdoc/>
    public int StackDepth => _navigationContainer?.StackDepth ?? 0;

    /// <inheritdoc/>
    public void NavigateTo<TPage>() where TPage : UiPageElement
    {
        NavigateTo<TPage>(null);
    }

    /// <inheritdoc/>
    public void NavigateTo<TPage>(object? parameter) where TPage : UiPageElement
    {
        NavigateToInternal(typeof(TPage), parameter, false);
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        if (_navigationContainer is null)
        {
            var exception = new InvalidOperationException("NavigationContainer is not initialized. Ensure Initialize() is called before navigation.");
            _logger?.LogError(exception, "GoBack failed: NavigationContainer not initialized");
            throw exception;
        }

        if (!_navigationContainer.IsStackEnabled)
        {
            var exception = new InvalidOperationException(
                "Navigation stack is not enabled. Set EnableNavigationStack = true in PlusUiConfiguration to use GoBack().");
            _logger?.LogError(exception, "GoBack failed: Navigation stack disabled");
            throw exception;
        }

        if (!_navigationContainer.CanGoBack)
        {
            var exception = new InvalidOperationException("Cannot go back. Already at the root page.");
            _logger?.LogError(exception, "GoBack failed: At root page");
            throw exception;
        }

        // Call lifecycle methods
        var currentPage = _navigationContainer.CurrentPage;
        currentPage.Disappearing();
        currentPage.OnNavigatedFrom();

        // Unsubscribe from property changes
        if (currentPage.ViewModel is not null)
        {
            currentPage.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _logger?.LogDebug("Navigating back from page: {PageType}", currentPage.GetType().Name);

        // Pop the current page
        var previousItem = _navigationContainer.Pop();

        // Setup the previous page
        var previousPage = previousItem.Page;

        // If preserving state, the page is already built, just need to update bindings
        if (_navigationContainer.PreservePageState)
        {
            previousPage.UpdateBindings();
        }
        else
        {
            // Page was disposed, need to rebuild it (this shouldn't happen with Pop)
            // But we handle it for safety
            previousPage.BuildPage();
        }

        // Subscribe to property changes
        if (previousPage.ViewModel is not null)
        {
            previousPage.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        // Call lifecycle methods
        previousPage.OnNavigatedTo(previousItem.Parameter);
        previousPage.Appearing();

        _logger?.LogDebug("Navigated back to page: {PageType}", previousPage.GetType().Name);
    }

    /// <inheritdoc/>
    public void PopToRoot()
    {
        if (_navigationContainer is null)
        {
            var exception = new InvalidOperationException("NavigationContainer is not initialized. Ensure Initialize() is called before navigation.");
            _logger?.LogError(exception, "PopToRoot failed: NavigationContainer not initialized");
            throw exception;
        }

        if (!_navigationContainer.IsStackEnabled)
        {
            var exception = new InvalidOperationException(
                "Navigation stack is not enabled. Set EnableNavigationStack = true in PlusUiConfiguration to use PopToRoot().");
            _logger?.LogError(exception, "PopToRoot failed: Navigation stack disabled");
            throw exception;
        }

        if (_navigationContainer.StackDepth <= 1)
        {
            _logger?.LogDebug("PopToRoot called but already at root page");
            return; // Already at root
        }

        // Call lifecycle methods for current page
        var currentPage = _navigationContainer.CurrentPage;
        currentPage.Disappearing();
        currentPage.OnNavigatedFrom();

        // Unsubscribe from property changes
        if (currentPage.ViewModel is not null)
        {
            currentPage.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _logger?.LogDebug("Popping to root from page: {PageType}, Stack depth: {Depth}",
            currentPage.GetType().Name, _navigationContainer.StackDepth);

        // Pop to root
        var rootItem = _navigationContainer.PopToRoot();

        // Setup the root page
        var rootPage = rootItem.Page;

        // If preserving state, the page is already built
        if (_navigationContainer.PreservePageState)
        {
            rootPage.UpdateBindings();
        }
        else
        {
            // Root page should always be preserved, but handle it for safety
            rootPage.BuildPage();
        }

        // Subscribe to property changes
        if (rootPage.ViewModel is not null)
        {
            rootPage.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        // Call lifecycle methods
        rootPage.OnNavigatedTo(rootItem.Parameter);
        rootPage.Appearing();

        _logger?.LogDebug("Popped to root page: {PageType}", rootPage.GetType().Name);
    }

    private void NavigateToInternal(Type pageType, object? parameter, bool isInitCall)
    {
        if (_navigationContainer is null)
        {
            var exception = new InvalidOperationException("NavigationContainer is not initialized. Ensure Initialize() is called before navigation.");
            _logger?.LogError(exception, "Navigation failed: NavigationContainer not initialized");
            throw exception;
        }

        // Check if navigating to the same page type (only if not init call)
        if (!isInitCall && _navigationContainer.CurrentPage.GetType() == pageType)
        {
            _logger?.LogDebug("Already on page type {PageType}, ignoring navigation", pageType.Name);
            return;
        }

        // Call lifecycle methods on current page if not init call
        if (!isInitCall)
        {
            var currentPage = _navigationContainer.CurrentPage;
            currentPage.Disappearing();
            currentPage.OnNavigatedFrom();

            if (currentPage.ViewModel is not null)
            {
                currentPage.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        try
        {
            // Resolve the new page
            var page = serviceProvider.GetRequiredService(pageType) as UiPageElement;
            if (page is null)
            {
                var exception = new InvalidOperationException(
                    $"Page of type {pageType.Name} could not be resolved or is not a UiPageElement. " +
                    $"Ensure the page is registered in the service collection.");
                _logger?.LogError(exception, "Navigation failed: Page not found for type {PageType}", pageType.Name);
                throw exception;
            }

            // Push the new page onto the stack
            _navigationContainer.Push(page, parameter);

            // Subscribe to property changes
            if (page.ViewModel is not null)
            {
                page.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            // Build and show the page
            page.BuildPage();

            // Call navigation lifecycle method
            page.OnNavigatedTo(parameter);

            _logger?.LogDebug("Navigated to page: {PageType}, Parameter: {HasParameter}, Stack depth: {Depth}",
                pageType.Name, parameter is not null, _navigationContainer.StackDepth);
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw our own exceptions
        }
        catch (Exception e)
        {
            var exception = new InvalidOperationException(
                $"Failed to navigate to page {pageType.Name}. " +
                $"Ensure the page is registered in Program.cs via services.AddTransient<{pageType.Name}>()", e);
            _logger?.LogError(e, "Navigation failed for page type {PageType}", pageType.Name);
            throw exception;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not null && _navigationContainer is not null)
        {
            _navigationContainer.CurrentPage.UpdateBindings(e.PropertyName);
        }
    }

    public void Initialize()
    {
        _navigationContainer ??= serviceProvider.GetRequiredService<NavigationContainer>();
        NavigateToInternal(_navigationContainer.CurrentPage.GetType(), null, true);
    }
}
