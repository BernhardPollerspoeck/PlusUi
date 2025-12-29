namespace PlusUi.core.Services.Focus;

/// <summary>
/// Manages keyboard focus across UI elements.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FocusManager"/> class.
/// </remarks>
public class FocusManager(
    NavigationContainer navigationContainer,
    PlusUiPopupService popupService,
    OverlayService overlayService) : IFocusManager
{
    private IFocusable? _focusedElement;

    /// <inheritdoc />
    public IFocusable? FocusedElement => _focusedElement;

    /// <inheritdoc />
    public event EventHandler<FocusChangedEventArgs>? FocusChanged;

    /// <inheritdoc />
    public void SetFocus(IFocusable? element)
    {
        if (_focusedElement == element)
        {
            return;
        }

        // Don't focus non-focusable elements
        if (element != null && !element.IsFocusable)
        {
            return;
        }

        var oldElement = _focusedElement;

        // Blur the old element
        if (oldElement != null)
        {
            oldElement.IsFocused = false;
            oldElement.OnBlur();
        }

        // Focus the new element
        _focusedElement = element;
        if (_focusedElement != null)
        {
            _focusedElement.IsFocused = true;
            _focusedElement.OnFocus();

            // Scroll the focused element into view
            if (_focusedElement is UiElement focusedUiElement)
            {
                ScrollIntoView(focusedUiElement);
            }
        }

        // Raise the event
        FocusChanged?.Invoke(this, new FocusChangedEventArgs(oldElement, _focusedElement));
    }

    /// <inheritdoc />
    public void ClearFocus()
    {
        SetFocus(null);
    }

    /// <inheritdoc />
    public bool FocusNext()
    {
        return MoveFocus(FocusNavigationDirection.Next);
    }

    /// <inheritdoc />
    public bool FocusPrevious()
    {
        return MoveFocus(FocusNavigationDirection.Previous);
    }

    /// <inheritdoc />
    public bool NavigateToLandmark(AccessibilityLandmark landmark)
    {
        if (landmark == AccessibilityLandmark.None)
        {
            return false;
        }

        var landmarkContainers = GetLandmarkContainers();
        var targetContainer = landmarkContainers.FirstOrDefault(c => c.AccessibilityLandmark == landmark);

        if (targetContainer == null)
        {
            return false;
        }

        return FocusFirstInContainer(targetContainer);
    }

    /// <inheritdoc />
    public bool NavigateToNextLandmark()
    {
        var landmarkContainers = GetLandmarkContainers();
        if (landmarkContainers.Count == 0)
        {
            return false;
        }

        // Find the current landmark container
        UiLayoutElement? currentLandmark = null;
        if (_focusedElement is UiElement focusedUiElement)
        {
            currentLandmark = FindLandmarkContainer(focusedUiElement);
        }

        if (currentLandmark == null)
        {
            // No current landmark, focus the first one
            return FocusFirstInContainer(landmarkContainers[0]);
        }

        // Find the next landmark
        var currentIndex = landmarkContainers.IndexOf(currentLandmark);
        var nextIndex = (currentIndex + 1) % landmarkContainers.Count;

        return FocusFirstInContainer(landmarkContainers[nextIndex]);
    }

    /// <summary>
    /// Focuses the first focusable element within a container.
    /// </summary>
    private bool FocusFirstInContainer(UiLayoutElement container)
    {
        var elements = new List<IFocusable>();
        CollectFocusableElements(container, elements);

        if (elements.Count == 0)
        {
            return false;
        }

        var sorted = SortByTabIndex(elements);
        SetFocus(sorted[0]);
        return true;
    }

    /// <summary>
    /// Gets all containers with accessibility landmarks in the current UI context.
    /// </summary>
    private List<UiLayoutElement> GetLandmarkContainers()
    {
        var containers = new List<UiLayoutElement>();

        // Check popup first
        var currentPopup = popupService.CurrentPopup;
        if (currentPopup != null)
        {
            CollectLandmarkContainers(currentPopup, containers);
            if (containers.Count > 0)
            {
                return containers;
            }
        }

        // Then check page
        CollectLandmarkContainers(navigationContainer.CurrentPage, containers);

        return containers;
    }

    /// <summary>
    /// Recursively collects all containers with accessibility landmarks.
    /// </summary>
    private void CollectLandmarkContainers(UiElement root, List<UiLayoutElement> containers)
    {
        if (!root.IsVisible)
        {
            return;
        }

        if (root is UiLayoutElement layoutElement)
        {
            if (layoutElement.AccessibilityLandmark != AccessibilityLandmark.None)
            {
                containers.Add(layoutElement);
            }

            foreach (var child in layoutElement.Children)
            {
                CollectLandmarkContainers(child, containers);
            }
        }
    }

    /// <summary>
    /// Finds the nearest parent container with an accessibility landmark.
    /// </summary>
    private UiLayoutElement? FindLandmarkContainer(UiElement element)
    {
        var current = element as UiElement;
        while (current != null)
        {
            if (current is UiLayoutElement layoutElement &&
                layoutElement.AccessibilityLandmark != AccessibilityLandmark.None)
            {
                return layoutElement;
            }
            current = current.Parent;
        }
        return null;
    }

    /// <inheritdoc />
    public bool MoveFocus(FocusNavigationDirection direction)
    {
        var focusableElements = GetFocusableElements();
        if (focusableElements.Count == 0)
        {
            return false;
        }

        // If nothing is focused, focus the first element
        if (_focusedElement == null)
        {
            SetFocus(focusableElements[0]);
            return true;
        }

        var currentIndex = ((List<IFocusable>)focusableElements).IndexOf(_focusedElement);
        if (currentIndex == -1)
        {
            // Current focused element not in list, focus first
            SetFocus(focusableElements[0]);
            return true;
        }

        int nextIndex;
        switch (direction)
        {
            case FocusNavigationDirection.Next:
            case FocusNavigationDirection.Down:
            case FocusNavigationDirection.Right:
                nextIndex = (currentIndex + 1) % focusableElements.Count;
                break;

            case FocusNavigationDirection.Previous:
            case FocusNavigationDirection.Up:
            case FocusNavigationDirection.Left:
                nextIndex = currentIndex - 1;
                if (nextIndex < 0)
                {
                    nextIndex = focusableElements.Count - 1;
                }
                break;

            default:
                return false;
        }

        SetFocus(focusableElements[nextIndex]);
        return true;
    }

    /// <summary>
    /// Gets all focusable elements in tab order from the current UI context.
    /// Respects FocusScope settings on containers.
    /// </summary>
    /// <returns>A list of focusable elements sorted by tab order.</returns>
    public IReadOnlyList<IFocusable> GetFocusableElements()
    {
        var elements = new List<IFocusable>();

        // Check overlays first (top-most)
        foreach (var overlay in overlayService.Overlays)
        {
            CollectFocusableElements(overlay, elements);
        }

        // If overlays have focusable elements, only use those
        if (elements.Count > 0)
        {
            return SortByTabIndex(elements);
        }

        // Then check popup
        var currentPopup = popupService.CurrentPopup;
        if (currentPopup != null)
        {
            CollectFocusableElements(currentPopup, elements);
            if (elements.Count > 0)
            {
                return SortByTabIndex(elements);
            }
        }

        // Check if currently focused element is within a FocusScope
        if (_focusedElement is UiElement focusedUiElement)
        {
            var scopeContainer = FindFocusScopeContainer(focusedUiElement);
            if (scopeContainer != null)
            {
                // Only collect elements within the scope
                CollectFocusableElements(scopeContainer, elements);
                return SortByTabIndex(elements);
            }
        }

        // Finally check page
        CollectFocusableElements(navigationContainer.CurrentPage, elements);

        return SortByTabIndex(elements);
    }

    /// <summary>
    /// Finds the nearest parent container with a FocusScope set to Trap or TrapWithEscape.
    /// </summary>
    private UiLayoutElement? FindFocusScopeContainer(UiElement element)
    {
        var current = element.Parent;
        while (current != null)
        {
            if (current is UiLayoutElement layoutElement &&
                layoutElement.FocusScope != FocusScopeMode.None)
            {
                return layoutElement;
            }
            current = current.Parent;
        }
        return null;
    }

    /// <summary>
    /// Recursively collects all focusable elements from an element tree.
    /// </summary>
    private void CollectFocusableElements(UiElement root, List<IFocusable> elements)
    {
        if (!root.IsVisible)
        {
            return;
        }

        // Check if this element is focusable and should participate in tab navigation
        if (root is IFocusable focusable
            && focusable.IsFocusable
            && focusable.TabStop
            && (focusable.TabIndex == null || focusable.TabIndex >= 0))
        {
            elements.Add(focusable);
        }

        // Handle UiPageElement specially - it stores content in ContentTree, not Children
        if (root is UiPageElement pageElement)
        {
            CollectFocusableElements(pageElement.ContentTree, elements);
            return;
        }

        // Recursively check children
        if (root is UiLayoutElement layoutElement)
        {
            foreach (var child in layoutElement.Children)
            {
                CollectFocusableElements(child, elements);
            }
        }
    }

    /// <summary>
    /// Sorts focusable elements by tab index.
    /// Elements with explicit TabIndex come first (sorted by value),
    /// followed by elements with null TabIndex (in declaration order).
    /// </summary>
    private static List<IFocusable> SortByTabIndex(List<IFocusable> elements)
    {
        // Separate elements with and without explicit tab index
        var withTabIndex = elements
            .Where(e => e.TabIndex.HasValue)
            .OrderBy(e => e.TabIndex!.Value)
            .ToList();

        var withoutTabIndex = elements
            .Where(e => !e.TabIndex.HasValue)
            .ToList();

        // Explicit tab indices first, then auto-ordered elements
        var result = new List<IFocusable>(withTabIndex.Count + withoutTabIndex.Count);
        result.AddRange(withTabIndex);
        result.AddRange(withoutTabIndex);

        return result;
    }

    /// <summary>
    /// Scrolls the element into view if it's inside a ScrollView.
    /// </summary>
    private void ScrollIntoView(UiElement element)
    {
        // Find the parent ScrollView
        var scrollView = FindParentScrollView(element);
        if (scrollView == null)
        {
            return;
        }

        // Get element bounds relative to ScrollView content
        var elementTop = element.Position.Y - scrollView.Position.Y + scrollView.VerticalOffset;
        var elementBottom = elementTop + element.ElementSize.Height;
        var elementLeft = element.Position.X - scrollView.Position.X + scrollView.HorizontalOffset;
        var elementRight = elementLeft + element.ElementSize.Width;

        // Get visible area
        var visibleTop = scrollView.VerticalOffset;
        var visibleBottom = visibleTop + scrollView.ElementSize.Height;
        var visibleLeft = scrollView.HorizontalOffset;
        var visibleRight = visibleLeft + scrollView.ElementSize.Width;

        // Calculate new scroll offset
        var newScrollY = scrollView.VerticalOffset;
        var newScrollX = scrollView.HorizontalOffset;

        // Vertical scrolling
        if (elementTop < visibleTop)
        {
            // Element is above visible area - scroll up
            newScrollY = elementTop - 10; // 10px padding
        }
        else if (elementBottom > visibleBottom)
        {
            // Element is below visible area - scroll down
            newScrollY = elementBottom - scrollView.ElementSize.Height + 10;
        }

        // Horizontal scrolling
        if (elementLeft < visibleLeft)
        {
            newScrollX = elementLeft - 10;
        }
        else if (elementRight > visibleRight)
        {
            newScrollX = elementRight - scrollView.ElementSize.Width + 10;
        }

        // Apply new scroll offset
        if (Math.Abs(newScrollY - scrollView.VerticalOffset) > 0.1f)
        {
            scrollView.SetVerticalOffset(newScrollY);
        }
        if (Math.Abs(newScrollX - scrollView.HorizontalOffset) > 0.1f)
        {
            scrollView.SetHorizontalOffset(newScrollX);
        }
    }

    /// <summary>
    /// Finds the nearest parent ScrollView of an element.
    /// </summary>
    private ScrollView? FindParentScrollView(UiElement element)
    {
        var current = element.Parent;
        while (current != null)
        {
            if (current is ScrollView scrollView)
            {
                return scrollView;
            }
            current = current.Parent;
        }
        return null;
    }
}
