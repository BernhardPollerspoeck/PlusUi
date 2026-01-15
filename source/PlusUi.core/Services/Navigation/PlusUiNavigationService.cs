using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core.Animations;
using System.ComponentModel;

namespace PlusUi.core;

public class PlusUiNavigationService(
    IServiceProvider serviceProvider,
    ILogger<PlusUiNavigationService>? logger = null) : INavigationService
{
    private NavigationContainer? _navigationContainer;
    private ITransitionService? _transitionService;
    private IOverlayService? _overlayService;
    private PlusUiConfiguration? _configuration;
    private readonly ILogger<PlusUiNavigationService>? _logger = logger;

    /// <inheritdoc/>
    public bool CanGoBack => _navigationContainer?.CanGoBack ?? false;

    /// <inheritdoc/>
    public int StackDepth => _navigationContainer?.StackDepth ?? 0;

    /// <inheritdoc/>
    public void NavigateTo<TPage>(object? parameter = null) where TPage : UiPageElement
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

        // Dismiss all overlays before navigating
        _overlayService?.DismissAll();

        // Store reference to outgoing page for transition
        var outgoingPage = _navigationContainer.CurrentPage;

        // Call lifecycle methods
        outgoingPage.Disappearing();
        outgoingPage.OnNavigatedFrom();

        // Unsubscribe from property changes
        if (outgoingPage.ViewModel is not null)
        {
            outgoingPage.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _logger?.LogInformation("[Event] GoBack: from {PageType}", outgoingPage.GetType().Name);

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

        // Start reversed transition
        if (_transitionService != null)
        {
            var transition = GetTransitionForPage(outgoingPage);
            if (transition != null && transition is not NoneTransition)
            {
                var reversedTransition = transition.GetReversed();
                _transitionService.StartTransition(outgoingPage, previousPage, reversedTransition);
            }
        }

        // Subscribe to property changes
        if (previousPage.ViewModel is not null)
        {
            previousPage.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        // Call lifecycle methods
        previousPage.OnNavigatedTo(previousItem.Parameter);
        previousPage.Appearing();

        _logger?.LogInformation("[Event] GoBack: to {PageType}", previousPage.GetType().Name);
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

        // Dismiss all overlays before navigating
        _overlayService?.DismissAll();

        // Call lifecycle methods for current page
        var currentPage = _navigationContainer.CurrentPage;
        currentPage.Disappearing();
        currentPage.OnNavigatedFrom();

        // Unsubscribe from property changes
        if (currentPage.ViewModel is not null)
        {
            currentPage.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _logger?.LogInformation("[Event] PopToRoot: from {PageType}, depth {Depth}",
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

        _logger?.LogInformation("[Event] PopToRoot: to {PageType}", rootPage.GetType().Name);
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

        // Resolve the new page
        if (serviceProvider.GetRequiredService(pageType) is not UiPageElement page)
        {
            var exception = new InvalidOperationException(
                $"Page of type {pageType.Name} could not be resolved or is not a UiPageElement. " +
                $"Ensure the page is registered in the service collection.");
            _logger?.LogError(exception, "Navigation failed: Page not found for type {PageType}", pageType.Name);
            throw exception;
        }

        // Use the overload that takes a page instance
        NavigateToInternal(page, parameter, isInitCall);
    }

    private void NavigateToInternal(UiPageElement page, object? parameter, bool isInitCall)
    {
        if (_navigationContainer is null)
        {
            var exception = new InvalidOperationException("NavigationContainer is not initialized. Ensure Initialize() is called before navigation.");
            _logger?.LogError(exception, "Navigation failed: NavigationContainer not initialized");
            throw exception;
        }

        var pageType = page.GetType();

        // Check if navigating to the same page type (only if not init call)
        if (!isInitCall && _navigationContainer.CurrentPage.GetType() == pageType)
        {
            _logger?.LogDebug("Already on page type {PageType}, ignoring navigation", pageType.Name);
            return;
        }

        // Dismiss all overlays before navigating
        _overlayService?.DismissAll();

        // Store reference to outgoing page for transition
        UiPageElement? outgoingPage = null;

        // Call lifecycle methods on current page if not init call
        if (!isInitCall)
        {
            var currentPage = _navigationContainer.CurrentPage;
            outgoingPage = currentPage;
            currentPage.Disappearing();
            currentPage.OnNavigatedFrom();

            if (currentPage.ViewModel is not null)
            {
                currentPage.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        try
        {

            // Push the new page onto the stack
            _navigationContainer.Push(page, parameter);

            // Subscribe to property changes
            if (page.ViewModel is not null)
            {
                page.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            page.BuildPage();

            _navigationContainer.RaisePageChangedForPush(page, outgoingPage);

            if (!isInitCall && outgoingPage != null && _transitionService != null)
            {
                var transition = GetTransitionForPage(page);
                if (transition != null && transition is not NoneTransition)
                {
                    _transitionService.StartTransition(outgoingPage, page, transition);
                }
            }

            // Call navigation lifecycle method
            page.OnNavigatedTo(parameter);

            _logger?.LogInformation("[Event] Navigate: to {PageType}, depth {Depth}",
                pageType.Name, _navigationContainer.StackDepth);
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

    private IPageTransition? GetTransitionForPage(UiPageElement page)
    {
        // Page-specific transition takes priority
        if (page.Transition != null)
        {
            return page.Transition;
        }

        // Fall back to global default transition
        return _configuration?.DefaultTransition;
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
        _transitionService ??= serviceProvider.GetService<ITransitionService>();
        _overlayService ??= serviceProvider.GetService<IOverlayService>();
        _configuration ??= serviceProvider.GetService<PlusUiConfiguration>();

        var appConfiguration = serviceProvider.GetRequiredService<IAppConfiguration>();
        var mainPage = appConfiguration.GetRootPage(serviceProvider);
        // Use the page instance directly instead of re-resolving by type
        NavigateToInternal(mainPage, null, true);
    }
}
