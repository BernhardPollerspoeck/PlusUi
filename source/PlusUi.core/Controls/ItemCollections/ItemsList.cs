using PlusUi.core.Attributes;
using PlusUi.core.Binding;
using SkiaSharp;
using System.Collections;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A high-performance virtualized list control for displaying large collections of data.
/// Only renders visible items to optimize memory and performance.
/// Supports vertical and horizontal orientations and observable collections.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
/// <remarks>
/// ItemsList uses UI virtualization to render only visible items, making it efficient for
/// large data sets (thousands of items). Automatically responds to collection changes when
/// using ObservableCollection.
/// </remarks>
/// <example>
/// <code>
/// // Simple list
/// new ItemsList<Person>()
///     .SetItemsSource(people)
///     .SetItemTemplate((person, index) =>
///         new Label().SetText(person.Name)
///     );
///
/// // Horizontal list
/// new ItemsList<Product>()
///     .SetItemsSource(products)
///     .SetOrientation(Orientation.Horizontal)
///     .SetItemTemplate((product, index) =>
///         new VStack(
///             new Image().SetImageSource(product.ImageUrl),
///             new Label().SetText(product.Name)
///         )
///     );
/// </code>
/// </example>
public class ItemsList<T> : UiLayoutElement<ItemsList<T>>, IScrollableControl
{
    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.List;

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        if (AccessibilityLabel != null)
            return AccessibilityLabel;

        var itemCount = _itemsSource?.Count() ?? 0;
        return $"List with {itemCount} item{(itemCount == 1 ? "" : "s")}";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        if (AccessibilityValue != null)
            return AccessibilityValue;

        var itemCount = _itemsSource?.Count() ?? 0;
        var visibleRange = _lastVisibleIndex - _firstVisibleIndex + 1;
        return itemCount > 0 ? $"Showing {visibleRange} of {itemCount}" : "Empty list";
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        return base.GetComputedAccessibilityTraits();
    }

    private IEnumerable<T>? _itemsSource;
    private Func<T, int, UiElement>? _itemTemplate;
    private readonly Dictionary<int, UiElement> _realizedItems = [];
    private readonly Dictionary<int, float> _itemSizes = []; // Track individual item sizes
    private readonly Dictionary<int, float> _itemPositions = []; // Track individual item positions
    private int _firstVisibleIndex;
    private int _lastVisibleIndex;
    private float _estimatedItemSize; // For items we haven't measured yet
    private Size _lastMeasuredAvailableSize; // For virtualization when ElementSize is not yet set
    private float _measuredContentWidth; // For limiting Stretch children to content width
    private float _measuredContentHeight;
    private float _totalContentSize; // Total size of all items (for scrollbar calculation)

    /// <summary>
    /// The scrollbar control for this ItemsList.
    /// </summary>
    public Scrollbar Scrollbar { get; }

    /// <summary>
    /// Returns Children plus the scrollbar for debug inspection.
    /// </summary>
    protected override IEnumerable<UiElement> GetDebugChildrenCore() =>
        Children.Concat([Scrollbar]);

    #region Orientation
    internal Orientation Orientation
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }

    public ItemsList<T> SetOrientation(Orientation orientation)
    {
        Orientation = orientation;
        return this;
    }

    public ItemsList<T> BindOrientation(Expression<Func<Orientation>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Orientation = getter());
        return this;
    }
    #endregion

    #region ItemsSource
    internal IEnumerable<T>? ItemsSource
    {
        get => field;
        set
        {
            // Unsubscribe from old collection
            if (_itemsSource is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnCollectionChanged;
            }

            field = value;
            _itemsSource = value;

            // Subscribe to new collection
            if (_itemsSource is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnCollectionChanged;
            }

            InvalidateMeasure();
            RebuildItems();
        }
    }

    public ItemsList<T> SetItemsSource(IEnumerable<T>? items)
    {
        ItemsSource = items;
        return this;
    }

    public ItemsList<T> BindItemsSource(Expression<Func<IEnumerable<T>?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ItemsSource = getter());
        return this;
    }
    #endregion

    #region ItemTemplate
    internal Func<T, int, UiElement>? ItemTemplate
    {
        get => field;
        set
        {
            field = value;
            _itemTemplate = value;
            InvalidateMeasure();
            RebuildItems();
        }
    }

    public ItemsList<T> SetItemTemplate(Func<T, int, UiElement> template)
    {
        ItemTemplate = template;
        return this;
    }

    public ItemsList<T> BindItemTemplate(Expression<Func<Func<T, int, UiElement>?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ItemTemplate = getter());
        return this;
    }
    #endregion

    #region ScrollFactor
    internal float ScrollFactor
    {
        get => field;
        set => field = value;
    }

    public ItemsList<T> SetScrollFactor(float factor)
    {
        ScrollFactor = factor;
        return this;
    }

    public ItemsList<T> BindScrollFactor(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ScrollFactor = getter());
        return this;
    }
    #endregion

    #region ScrollOffset
    internal float ScrollOffset
    {
        get => field;
        set
        {
            var totalSize = CalculateTotalSize();
            var maxOffset = Orientation == Orientation.Vertical
                ? Math.Max(0, totalSize.Height - ElementSize.Height)
                : Math.Max(0, totalSize.Width - ElementSize.Width);
            field = Math.Clamp(value, 0, maxOffset);
            InvalidateMeasure();
            UpdateVisibleRange();
        }
    }

    public ItemsList<T> SetScrollOffset(float offset)
    {
        ScrollOffset = offset;
        return this;
    }

    public ItemsList<T> BindScrollOffset(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ScrollOffset = getter());
        return this;
    }
    #endregion

    #region ShowScrollbar
    internal bool ShowScrollbar
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = true;

    public ItemsList<T> SetShowScrollbar(bool show)
    {
        ShowScrollbar = show;
        return this;
    }

    public ItemsList<T> BindShowScrollbar(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ShowScrollbar = getter());
        return this;
    }
    #endregion

    public ItemsList()
    {
        Orientation = PlusUiDefaults.ListOrientation;
        ScrollFactor = PlusUiDefaults.ScrollFactor;

        // Initialize scrollbar
        Scrollbar = new Scrollbar()
            .SetOrientation(ScrollbarOrientation.Vertical)
            .SetOnValueChanged(OnScrollbarValueChanged);
    }

    private void OnScrollbarValueChanged(float newOffset)
    {
        ScrollOffset = newOffset;
    }

    private void UpdateScrollbarState()
    {
        var viewportSize = Orientation == Orientation.Vertical
            ? ElementSize.Height
            : ElementSize.Width;

        var maxOffset = Math.Max(0, _totalContentSize - viewportSize);

        Scrollbar.UpdateScrollState(
            ScrollOffset,
            maxOffset,
            viewportSize,
            _totalContentSize);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateMeasure();
        RebuildItems();
    }

    private void RebuildItems()
    {
        // Clear existing realized items and cached sizes
        _realizedItems.Clear();
        _itemSizes.Clear();
        _itemPositions.Clear();
        Children.Clear();

        if (_itemsSource == null || _itemTemplate == null)
        {
            return;
        }

        // Calculate visible range and realize only visible items
        UpdateVisibleRange();
    }

    private void UpdateVisibleRange()
    {
        if (_itemsSource == null || _itemTemplate == null)
        {
            return;
        }

        var items = _itemsSource.ToList();
        if (items.Count == 0)
        {
            return;
        }

        // Estimate item size from first item if not yet calculated
        if (_estimatedItemSize == 0)
        {
            var firstItem = _itemTemplate(items[0], 0);
            firstItem.Measure(new Size(float.MaxValue, float.MaxValue), true);
            _estimatedItemSize = Orientation == Orientation.Vertical
                ? firstItem.ElementSize.Height + firstItem.Margin.Top + firstItem.Margin.Bottom
                : firstItem.ElementSize.Width + firstItem.Margin.Left + firstItem.Margin.Right;
        }

        // Calculate visible range based on orientation
        int newFirstVisible = 0;
        int newLastVisible = 0;

        // Use ElementSize if available, otherwise fall back to last measured available size
        var viewportSize = Orientation == Orientation.Vertical
            ? (ElementSize.Height > 0 ? ElementSize.Height : _lastMeasuredAvailableSize.Height)
            : (ElementSize.Width > 0 ? ElementSize.Width : _lastMeasuredAvailableSize.Width);

        // If we still don't have a viewport size, show all items
        var showAllItems = viewportSize <= 0;

        if (Orientation == Orientation.Vertical)
        {
            // Find first visible item
            float currentPosition = 0;
            for (int i = 0; i < items.Count; i++)
            {
                float itemSize = _itemSizes.TryGetValue(i, out var size) ? size : _estimatedItemSize;
                _itemPositions[i] = currentPosition;

                if (currentPosition + itemSize >= ScrollOffset)
                {
                    newFirstVisible = i;
                    break;
                }

                currentPosition += itemSize;
            }

            // Find last visible item
            currentPosition = _itemPositions.TryGetValue(newFirstVisible, out var pos) ? pos : 0;
            for (int i = newFirstVisible; i < items.Count; i++)
            {
                float itemSize = _itemSizes.TryGetValue(i, out var size) ? size : _estimatedItemSize;
                _itemPositions[i] = currentPosition;

                if (!showAllItems && currentPosition >= ScrollOffset + viewportSize)
                {
                    newLastVisible = i;
                    break;
                }

                currentPosition += itemSize;
                newLastVisible = i;
            }
        }
        else
        {
            // Find first visible item
            float currentPosition = 0;
            for (int i = 0; i < items.Count; i++)
            {
                float itemSize = _itemSizes.TryGetValue(i, out var size) ? size : _estimatedItemSize;
                _itemPositions[i] = currentPosition;

                if (currentPosition + itemSize >= ScrollOffset)
                {
                    newFirstVisible = i;
                    break;
                }

                currentPosition += itemSize;
            }

            // Find last visible item
            currentPosition = _itemPositions.TryGetValue(newFirstVisible, out var pos) ? pos : 0;
            for (int i = newFirstVisible; i < items.Count; i++)
            {
                float itemSize = _itemSizes.TryGetValue(i, out var size) ? size : _estimatedItemSize;
                _itemPositions[i] = currentPosition;

                if (!showAllItems && currentPosition >= ScrollOffset + viewportSize)
                {
                    newLastVisible = i;
                    break;
                }

                currentPosition += itemSize;
                newLastVisible = i;
            }
        }

        // Realize new items in visible range
        for (int i = newFirstVisible; i <= newLastVisible; i++)
        {
            if (!_realizedItems.ContainsKey(i))
            {
                var item = _itemTemplate(items[i], i)
                    ?? throw new InvalidOperationException($"ItemTemplate returned null for index {i}");
                item.Parent = this;

                // Measure the item to get its actual size
                item.Measure(new Size(float.MaxValue, float.MaxValue), true);
                float itemSize = Orientation == Orientation.Vertical
                    ? item.ElementSize.Height + item.Margin.Top + item.Margin.Bottom
                    : item.ElementSize.Width + item.Margin.Left + item.Margin.Right;

                _itemSizes[i] = itemSize;
                _realizedItems[i] = item;
            }
        }

        // Remove items outside visible range - snapshot keys first to avoid enumeration issues
        var allKeys = _realizedItems.Keys.ToArray();
        foreach (var key in allKeys)
        {
            if (key < newFirstVisible || key > newLastVisible)
                _realizedItems.Remove(key);
        }

        // Update children list - snapshot to array for stable enumeration
        Children.Clear();
        var sortedItems = _realizedItems.ToArray();
        Array.Sort(sortedItems, (a, b) => a.Key.CompareTo(b.Key));
        foreach (var kvp in sortedItems)
        {
            if (kvp.Value != null)
                Children.Add(kvp.Value);
        }

        _firstVisibleIndex = newFirstVisible;
        _lastVisibleIndex = newLastVisible;
    }

    private Size CalculateTotalSize()
    {
        if (_itemsSource == null)
        {
            return new Size(0, 0);
        }

        var itemCount = _itemsSource.Count();

        // Calculate total size based on measured items and estimates for unmeasured items
        float totalSize = 0;
        for (int i = 0; i < itemCount; i++)
        {
            totalSize += _itemSizes.TryGetValue(i, out var size) ? size : _estimatedItemSize;
        }

        if (Orientation == Orientation.Vertical)
        {
            return new Size(0, totalSize);
        }
        else
        {
            return new Size(totalSize, 0);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (_itemsSource == null || _itemTemplate == null)
        {
            return new Size(0, 0);
        }

        var items = _itemsSource.ToList();
        if (items.Count == 0)
        {
            return new Size(0, 0);
        }

        var childAvailableSize = new Size(
            Math.Max(0, availableSize.Width - Margin.Horizontal),
            Math.Max(0, availableSize.Height - Margin.Vertical));

        _lastMeasuredAvailableSize = childAvailableSize;

        // Measure ALL items to find max width/height (prevents UI jumping when scrolling)
        float maxWidth = 0;
        float maxHeight = 0;
        float totalHeight = 0;
        float totalWidth = 0;

        for (var i = 0; i < items.Count; i++)
        {
            var item = _itemTemplate(items[i], i);
            item.Measure(childAvailableSize, true);

            var itemWidth = item.ElementSize.Width + item.Margin.Left + item.Margin.Right;
            var itemHeight = item.ElementSize.Height + item.Margin.Top + item.Margin.Bottom;

            maxWidth = Math.Max(maxWidth, itemWidth);
            maxHeight = Math.Max(maxHeight, itemHeight);
            totalHeight += itemHeight;
            totalWidth += itemWidth;

            // Cache the size for virtualization
            _itemSizes[i] = Orientation == Orientation.Vertical ? itemHeight : itemWidth;
        }

        _estimatedItemSize = Orientation == Orientation.Vertical
            ? totalHeight / items.Count
            : totalWidth / items.Count;

        // Store total content size for scrollbar
        _totalContentSize = Orientation == Orientation.Vertical ? totalHeight : totalWidth;

        // Update visible range after measuring
        UpdateVisibleRange();

        // Store measured content size (widest/tallest child including margins)
        _measuredContentWidth = maxWidth;
        _measuredContentHeight = maxHeight;

        // Measure visible items for rendering
        // Use a constrained size so Stretch children expand to the widest child, not the full available
        var stretchConstrainedSize = Orientation == Orientation.Vertical
            ? new Size(maxWidth, childAvailableSize.Height)
            : new Size(childAvailableSize.Width, maxHeight);

        foreach (var child in Children.ToArray())
        {
            child.Measure(stretchConstrainedSize, false);
        }

        // Measure scrollbar
        Scrollbar.Parent = this;
        var scrollbarOrientation = Orientation == Orientation.Vertical
            ? ScrollbarOrientation.Vertical
            : ScrollbarOrientation.Horizontal;
        Scrollbar.SetOrientation(scrollbarOrientation);

        if (Orientation == Orientation.Vertical)
        {
            Scrollbar.Measure(new Size(Scrollbar.Width, childAvailableSize.Height), true);
        }
        else
        {
            Scrollbar.Measure(new Size(childAvailableSize.Width, Scrollbar.Width), true);
        }

        // Return actual needed size based on orientation
        // Width/Height = widest/tallest child + scrollbar, scrolling dimension = min of content and available
        var needsScrollbar = ShowScrollbar && (Orientation == Orientation.Vertical
            ? totalHeight > availableSize.Height
            : totalWidth > availableSize.Width);
        var scrollbarSize = needsScrollbar ? Scrollbar.Width : 0;

        if (Orientation == Orientation.Vertical)
        {
            var contentHeight = totalHeight;
            return new Size(maxWidth + scrollbarSize, Math.Min(contentHeight, availableSize.Height));
        }
        else
        {
            var contentWidth = totalWidth;
            return new Size(Math.Min(contentWidth, availableSize.Width), maxHeight + scrollbarSize);
        }
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => bounds.Left + ((bounds.Width - ElementSize.Width) / 2),
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.Left + Margin.Left,
        };
        var positionY = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.Top + ((bounds.Height - ElementSize.Height) / 2),
            VerticalAlignment.Bottom => bounds.Bottom - ElementSize.Height - Margin.Bottom,
            _ => bounds.Top + Margin.Top,
        };

        // Check if scrollbar should be shown
        var viewportSize = Orientation == Orientation.Vertical ? ElementSize.Height : ElementSize.Width;
        var needsScrollbar = ShowScrollbar && _totalContentSize > viewportSize;

        if (Orientation == Orientation.Vertical)
        {
            // Use the cached position for the first visible item
            var startPosition = _itemPositions.TryGetValue(_firstVisibleIndex, out var pos) ? pos : 0;
            var y = positionY - ScrollOffset + startPosition;

            foreach (var child in Children.ToArray())
            {
                // Pass the full content width as available space - child's ArrangeInternal handles alignment/margins
                var contentWidth = _measuredContentWidth > 0 ? _measuredContentWidth : ElementSize.Width;

                child.Arrange(new Rect(
                    positionX,
                    y,
                    contentWidth,
                    child.ElementSize.Height + child.Margin.Vertical));
                y += child.ElementSize.Height + child.Margin.Top + child.Margin.Bottom;
            }

            // Arrange scrollbar on the right side
            if (needsScrollbar)
            {
                var scrollbarX = positionX + ElementSize.Width - Scrollbar.Width;
                Scrollbar.Arrange(new Rect(scrollbarX, positionY, Scrollbar.Width, ElementSize.Height));
            }
        }
        else
        {
            // Use the cached position for the first visible item
            var startPosition = _itemPositions.TryGetValue(_firstVisibleIndex, out var pos) ? pos : 0;
            var x = positionX - ScrollOffset + startPosition;

            foreach (var child in Children.ToArray())
            {
                // Pass the full content height as available space - child's ArrangeInternal handles alignment/margins
                var contentHeight = _measuredContentHeight > 0 ? _measuredContentHeight : ElementSize.Height;

                child.Arrange(new Rect(
                    x,
                    positionY,
                    child.ElementSize.Width + child.Margin.Horizontal,
                    contentHeight));
                x += child.ElementSize.Width + child.Margin.Left + child.Margin.Right;
            }

            // Arrange scrollbar at the bottom
            if (needsScrollbar)
            {
                var scrollbarY = positionY + ElementSize.Height - Scrollbar.Width;
                Scrollbar.Arrange(new Rect(positionX, scrollbarY, ElementSize.Width, Scrollbar.Width));
            }
        }

        // Update scrollbar state (must be after ElementSize is set via Arrange)
        UpdateScrollbarState();

        return new Point(positionX, positionY);
    }

    public override void Render(SKCanvas canvas)
    {
        if (IsVisible)
        {
            // Save canvas state and apply clipping
            canvas.Save();
            var rect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);

            // Apply clipping with corner radius if needed
            if (CornerRadius > 0)
            {
                canvas.ClipRoundRect(new SKRoundRect(rect, CornerRadius, CornerRadius));
            }
            else
            {
                canvas.ClipRect(rect);
            }
        }

        // Call base to render background (now clipped)
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Render visible children
        foreach (var child in Children.ToArray())
        {
            // Save the current VisualOffset
            var childOriginalOffset = child.VisualOffset;

            // Apply parent's VisualOffset to child
            child.SetVisualOffset(new Point(
                childOriginalOffset.X + VisualOffset.X,
                childOriginalOffset.Y + VisualOffset.Y
            ));

            // Render the child
            child.Render(canvas);

            // Restore original VisualOffset
            child.SetVisualOffset(childOriginalOffset);
        }

        // Render scrollbar (after children, so it's on top)
        var viewportSize = Orientation == Orientation.Vertical ? ElementSize.Height : ElementSize.Width;
        if (ShowScrollbar && _totalContentSize > viewportSize)
        {
            var scrollbarOriginalOffset = Scrollbar.VisualOffset;
            Scrollbar.SetVisualOffset(new Point(
                scrollbarOriginalOffset.X + VisualOffset.X,
                scrollbarOriginalOffset.Y + VisualOffset.Y
            ));
            Scrollbar.Render(canvas);
            Scrollbar.SetVisualOffset(scrollbarOriginalOffset);
        }

        // Restore canvas state
        canvas.Restore();
    }

    public override UiElement? HitTest(Point point)
    {
        // First check if the point is within our bounds
        if (!(point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
              point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height))
        {
            return null;
        }

        // Check scrollbar first (it's rendered on top)
        var viewportSize = Orientation == Orientation.Vertical ? ElementSize.Height : ElementSize.Width;
        if (ShowScrollbar && _totalContentSize > viewportSize)
        {
            var scrollbarHit = Scrollbar.HitTest(point);
            if (scrollbarHit != null)
            {
                return scrollbarHit;
            }
        }

        // Check if any child was hit
        // No need to adjust the point - the child's Position was already adjusted during Arrange
        var childHit = Children.ToArray().Select(c => c?.HitTest(point)).FirstOrDefault(hit => hit != null);

        // If no child hit, return this ItemsList
        if (childHit == null)
        {
            return this;
        }

        // Return interactive controls directly
        if (childHit is IInputControl || childHit is ITextInputControl || childHit is IToggleButtonControl)
        {
            return childHit;
        }

        // For non-interactive controls, return this ItemsList to handle scrolling
        return this;
    }

    #region IScrollableControl Implementation
    public bool IsScrolling { get; set; }

    public void HandleScroll(float deltaX, float deltaY)
    {
        if (Orientation == Orientation.Vertical)
        {
            ScrollOffset += (deltaY + deltaX) * ScrollFactor;
        }
        else
        {
            ScrollOffset += (deltaX + deltaY) * ScrollFactor;
        }
    }
    #endregion
}
