namespace PlusUi.core;

/// <summary>
/// Event arguments for page change events.
/// </summary>
public class PageChangedEventArgs(UiPageElement newPage, UiPageElement? previousPage) : EventArgs
{
    /// <summary>
    /// The page being navigated to (new current page).
    /// </summary>
    public UiPageElement NewPage { get; } = newPage;

    /// <summary>
    /// The page being navigated from (previous current page), or null if this is the initial page.
    /// </summary>
    public UiPageElement? PreviousPage { get; } = previousPage;
}

/// <summary>
/// Container that manages the navigation stack for page-based navigation.
/// </summary>
public class NavigationContainer
{
    private readonly Stack<NavigationStackItem> _navigationStack = new();
    private readonly bool _stackEnabled;
    private readonly bool _preservePageState;
    private readonly int _maxStackDepth;

    /// <summary>
    /// Raised when the current page changes (navigation, go back, pop to root).
    /// </summary>
    public event EventHandler<PageChangedEventArgs>? PageChanged;

    /// <summary>
    /// Gets the current page being displayed.
    /// </summary>
    public UiPageElement CurrentPage => _navigationStack.Count > 0
        ? _navigationStack.Peek().Page
        : throw new InvalidOperationException("Navigation stack is empty.");

    /// <summary>
    /// Gets the current navigation parameter.
    /// </summary>
    public object? CurrentParameter => _navigationStack.Count > 0 ? _navigationStack.Peek().Parameter : null;

    /// <summary>
    /// Gets a value indicating whether backward navigation is possible.
    /// </summary>
    public bool CanGoBack => _navigationStack.Count > 1;

    /// <summary>
    /// Gets the current depth of the navigation stack.
    /// </summary>
    public int StackDepth => _navigationStack.Count;

    /// <summary>
    /// Gets a value indicating whether the navigation stack is enabled.
    /// </summary>
    public bool IsStackEnabled => _stackEnabled;

    /// <summary>
    /// Gets a value indicating whether page state is preserved in the stack.
    /// </summary>
    public bool PreservePageState => _preservePageState;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationContainer"/> class with the root page.
    /// </summary>
    /// <param name="rootPage">The initial page to display.</param>
    /// <param name="configuration">The PlusUi configuration containing navigation settings.</param>
    public NavigationContainer(PlusUiConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _stackEnabled = configuration.EnableNavigationStack;
        _preservePageState = configuration.PreservePageState;
        _maxStackDepth = configuration.MaxStackDepth;
    }

    /// <summary>
    /// Pushes a new page onto the navigation stack.
    /// </summary>
    /// <param name="page">The page to push.</param>
    /// <param name="parameter">Optional navigation parameter.</param>
    /// <exception cref="InvalidOperationException">Thrown when max stack depth is reached.</exception>
    public void Push(UiPageElement page, object? parameter = null)
    {
        ArgumentNullException.ThrowIfNull(page);

        UiPageElement? previousPage = _navigationStack.Count > 0 ? _navigationStack.Peek().Page : null;

        if (_stackEnabled)
        {
            // Check max stack depth
            if (_navigationStack.Count >= _maxStackDepth)
            {
                throw new InvalidOperationException(
                    $"Maximum navigation stack depth of {_maxStackDepth} reached. " +
                    $"Consider using PopToRoot() or increasing MaxStackDepth in PlusUiConfiguration.");
            }

            // If not preserving state, dispose old pages except the current one
            if (!_preservePageState && _navigationStack.Count > 1)
            {
                // Keep only the current page and dispose the rest
                var currentItem = _navigationStack.Pop();
                while (_navigationStack.Count > 0)
                {
                    var item = _navigationStack.Pop();
                    item.Page.Dispose();
                }
                _navigationStack.Push(currentItem);
            }

            _navigationStack.Push(new NavigationStackItem(page, parameter));
        }
        else
        {
            // Stack disabled - replace current page (old behavior)
            if (_navigationStack.Count > 0)
            {
                var oldItem = _navigationStack.Pop();
                oldItem.Page.Dispose();
            }
            _navigationStack.Push(new NavigationStackItem(page, parameter));
        }

        PageChanged?.Invoke(this, new PageChangedEventArgs(page, previousPage));
    }

    /// <summary>
    /// Pops the current page from the navigation stack and returns to the previous page.
    /// </summary>
    /// <returns>The previous page's navigation item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when trying to pop the last page.</exception>
    public NavigationStackItem Pop()
    {
        if (!CanGoBack)
        {
            throw new InvalidOperationException("Cannot go back. Navigation stack has only one page.");
        }

        var currentItem = _navigationStack.Pop();

        // Dispose the current page if not preserving state
        if (!_preservePageState)
        {
            currentItem.Page.Dispose();
        }

        var newItem = _navigationStack.Peek();
        PageChanged?.Invoke(this, new PageChangedEventArgs(newItem.Page, currentItem.Page));

        return newItem;
    }

    /// <summary>
    /// Pops all pages except the root page.
    /// </summary>
    /// <returns>The root page's navigation item.</returns>
    public NavigationStackItem PopToRoot()
    {
        if (_navigationStack.Count <= 1)
        {
            return _navigationStack.Peek();
        }

        var previousPage = _navigationStack.Peek().Page;

        // Dispose all pages except the root if not preserving state
        if (!_preservePageState)
        {
            var itemsToDispose = new List<NavigationStackItem>();
            while (_navigationStack.Count > 1)
            {
                itemsToDispose.Add(_navigationStack.Pop());
            }

            foreach (var item in itemsToDispose)
            {
                item.Page.Dispose();
            }
        }
        else
        {
            while (_navigationStack.Count > 1)
            {
                _navigationStack.Pop();
            }
        }

        var newItem = _navigationStack.Peek();
        PageChanged?.Invoke(this, new PageChangedEventArgs(newItem.Page, previousPage));

        return newItem;
    }

    /// <summary>
    /// Gets the previous page in the stack without popping.
    /// </summary>
    /// <returns>The previous page, or null if there is none.</returns>
    public UiPageElement? PeekPrevious()
    {
        if (!CanGoBack)
        {
            return null;
        }

        var items = _navigationStack.ToArray();
        return items.Length > 1 ? items[1].Page : null;
    }
}