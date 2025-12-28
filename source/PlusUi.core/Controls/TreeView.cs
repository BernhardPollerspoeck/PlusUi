using SkiaSharp;
using System.Collections.Specialized;

namespace PlusUi.core;

/// <summary>
/// Node representing an item in the tree hierarchy.
/// </summary>
public class TreeViewNode
{
    public object Item { get; }
    public int Depth { get; }
    public bool IsExpanded { get; set; }
    public float ExpandedHeight { get; private set; }
    public List<TreeViewNode>? Children { get; set; }
    public TreeViewNode? Parent { get; }
    public bool HasChildren { get; set; }

    public TreeViewNode(object item, int depth, TreeViewNode? parent = null)
    {
        Item = item;
        Depth = depth;
        Parent = parent;
        IsExpanded = false;
        HasChildren = false;
    }

    /// <summary>
    /// Updates this node's ExpandedHeight and propagates the change to parents.
    /// </summary>
    public void UpdateExpandedHeight(float itemHeight)
    {
        float childrenHeight = 0;
        if (IsExpanded && Children != null)
        {
            foreach (var child in Children)
            {
                childrenHeight += child.ExpandedHeight;
            }
        }

        var newHeight = itemHeight + childrenHeight;
        var heightDelta = newHeight - ExpandedHeight;
        ExpandedHeight = newHeight;

        // Propagate to parent
        if (heightDelta != 0 && Parent != null)
        {
            Parent.UpdateExpandedHeight(itemHeight);
        }
    }
}

/// <summary>
/// A hierarchical tree view control for displaying nested data structures.
/// Supports heterogeneous child types via type-specific children selectors.
/// </summary>
public class TreeView : UiLayoutElement<TreeView>, IScrollableControl, IInputControl, IHoverableControl
{
    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Tree;

    private readonly Dictionary<Type, Func<object, IEnumerable<object>>> _childrenSelectors = new();
    private readonly List<TreeViewNode> _rootNodes = new();
    private readonly Dictionary<object, TreeViewNode> _nodesByItem = new();

    #region ItemsSource

    private IEnumerable<object>? _itemsSource;

    /// <summary>
    /// Gets the data source for the root items.
    /// </summary>
    public IEnumerable<object>? ItemsSource => _itemsSource;

    /// <summary>
    /// Sets the data source for the root items.
    /// </summary>
    public TreeView SetItemsSource(IEnumerable<object> items)
    {
        _itemsSource = items;
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Binds the data source to a property.
    /// </summary>
    public TreeView BindItemsSource(string propertyName, Func<IEnumerable<object>?> getter)
    {
        RegisterBinding(propertyName, () =>
        {
            var items = getter();
            if (items != null)
                SetItemsSource(items);
        });
        return this;
    }

    #endregion

    #region SelectedItem

    private object? _selectedItem;

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public object? SelectedItem => _selectedItem;

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    public TreeView SetSelectedItem(object? item)
    {
        if (_selectedItem != item)
        {
            _selectedItem = item;
            _selectedItemSetter?.Invoke(item);
            InvalidateMeasure();
        }
        return this;
    }

    private Action<object?>? _selectedItemSetter;

    /// <summary>
    /// Binds the selected item to a property with two-way binding.
    /// </summary>
    public TreeView BindSelectedItem(string propertyName, Func<object?> getter, Action<object?> setter)
    {
        _selectedItemSetter = setter;
        RegisterBinding(propertyName, () => SetSelectedItem(getter()));
        return this;
    }

    #endregion

    #region ItemTemplate

    private Func<object, int, UiElement>? _itemTemplate;

    /// <summary>
    /// Gets the template function for creating item visuals.
    /// </summary>
    public Func<object, int, UiElement>? ItemTemplate => _itemTemplate;

    /// <summary>
    /// Sets the template function for creating item visuals.
    /// </summary>
    /// <param name="template">Function that takes (item, depth) and returns a UI element.</param>
    public TreeView SetItemTemplate(Func<object, int, UiElement> template)
    {
        _itemTemplate = template;
        InvalidateMeasure();
        return this;
    }

    #endregion

    #region Indentation

    private float _indentation = 20f;

    /// <summary>
    /// Gets the horizontal indentation per tree level.
    /// </summary>
    public float Indentation => _indentation;

    /// <summary>
    /// Sets the horizontal indentation per tree level.
    /// </summary>
    public TreeView SetIndentation(float indentation)
    {
        _indentation = indentation;
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Binds the indentation to a property.
    /// </summary>
    public TreeView BindIndentation(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetIndentation(getter()));
        return this;
    }

    #endregion

    #region ItemHeight

    private float _itemHeight = 32f;

    /// <summary>
    /// Gets the height of each tree item row.
    /// </summary>
    public float ItemHeight => _itemHeight;

    /// <summary>
    /// Sets the height of each tree item row.
    /// </summary>
    public TreeView SetItemHeight(float height)
    {
        _itemHeight = height;
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Binds the item height to a property.
    /// </summary>
    public TreeView BindItemHeight(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetItemHeight(getter()));
        return this;
    }

    #endregion

    #region ExpanderSize

    private float _expanderSize = 16f;

    /// <summary>
    /// Gets the size of the expand/collapse icon.
    /// </summary>
    public float ExpanderSize => _expanderSize;

    /// <summary>
    /// Sets the size of the expand/collapse icon.
    /// </summary>
    public TreeView SetExpanderSize(float size)
    {
        _expanderSize = size;
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Binds the expander size to a property.
    /// </summary>
    public TreeView BindExpanderSize(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetExpanderSize(getter()));
        return this;
    }

    #endregion

    #region Children Selectors

    /// <summary>
    /// Registers a children selector for a specific item type.
    /// The selector is used to get child items when expanding a node.
    /// </summary>
    /// <typeparam name="TItem">The type of item this selector handles.</typeparam>
    /// <param name="selector">Function that returns child items for a given parent.</param>
    public TreeView SetChildrenSelector<TItem>(Func<TItem, IEnumerable<object>> selector)
    {
        _childrenSelectors[typeof(TItem)] = item => selector((TItem)item);
        return this;
    }

    /// <summary>
    /// Checks if a children selector is registered for the specified type.
    /// </summary>
    public bool HasChildrenSelectorFor<TItem>()
    {
        return _childrenSelectors.ContainsKey(typeof(TItem));
    }

    /// <summary>
    /// Gets the children selector for a given item type, or null if none registered.
    /// </summary>
    internal Func<object, IEnumerable<object>>? GetChildrenSelector(Type itemType)
    {
        return _childrenSelectors.GetValueOrDefault(itemType);
    }

    #endregion

    #region Node Management

    /// <summary>
    /// Gets the number of root nodes.
    /// </summary>
    public int RootNodeCount => _rootNodes.Count;

    /// <summary>
    /// Gets the total height of all visible content.
    /// </summary>
    public float TotalHeight
    {
        get
        {
            float total = 0;
            foreach (var node in _rootNodes)
            {
                total += node.ExpandedHeight;
            }
            return total;
        }
    }

    /// <summary>
    /// Builds the node structure from the current ItemsSource.
    /// </summary>
    public void BuildNodes()
    {
        _rootNodes.Clear();
        _nodesByItem.Clear();

        if (_itemsSource == null)
            return;

        foreach (var item in _itemsSource)
        {
            var node = new TreeViewNode(item, 0);
            node.HasChildren = HasChildrenFor(item);
            node.UpdateExpandedHeight(_itemHeight);
            _rootNodes.Add(node);
            _nodesByItem[item] = node;
        }
    }

    /// <summary>
    /// Checks if the specified item has any children according to the registered selectors.
    /// </summary>
    private bool HasChildrenFor(object item)
    {
        var selector = _childrenSelectors.GetValueOrDefault(item.GetType());
        if (selector == null)
            return false;

        var children = selector(item);
        return children.Any();
    }

    /// <summary>
    /// Gets the expanded height for a specific item's node.
    /// </summary>
    public float GetNodeExpandedHeight(object item)
    {
        if (_nodesByItem.TryGetValue(item, out var node))
        {
            return node.ExpandedHeight;
        }
        return 0;
    }

    /// <summary>
    /// Checks if the specified item's node is expanded.
    /// </summary>
    public bool IsNodeExpanded(object item)
    {
        if (_nodesByItem.TryGetValue(item, out var node))
        {
            return node.IsExpanded;
        }
        return false;
    }

    /// <summary>
    /// Checks if the specified item has a node registered.
    /// </summary>
    public bool HasNode(object item)
    {
        return _nodesByItem.ContainsKey(item);
    }

    /// <summary>
    /// Gets the depth of the node for the specified item.
    /// </summary>
    public int GetNodeDepth(object item)
    {
        if (_nodesByItem.TryGetValue(item, out var node))
        {
            return node.Depth;
        }
        return -1;
    }

    /// <summary>
    /// Expands the node for the specified item, loading children lazily.
    /// </summary>
    public void ExpandNode(object item)
    {
        if (!_nodesByItem.TryGetValue(item, out var node))
            return;

        if (node.IsExpanded)
            return;

        // Lazy load children if not already loaded
        if (node.Children == null)
        {
            var selector = _childrenSelectors.GetValueOrDefault(item.GetType());
            if (selector != null)
            {
                var childItems = selector(item);
                node.Children = new List<TreeViewNode>();

                foreach (var childItem in childItems)
                {
                    var childNode = new TreeViewNode(childItem, node.Depth + 1, node);
                    childNode.HasChildren = HasChildrenFor(childItem);
                    childNode.UpdateExpandedHeight(_itemHeight);
                    node.Children.Add(childNode);
                    _nodesByItem[childItem] = childNode;
                }
            }
        }

        node.IsExpanded = true;
        node.UpdateExpandedHeight(_itemHeight);
        InvalidateMeasure();
    }

    /// <summary>
    /// Collapses the node for the specified item.
    /// </summary>
    public void CollapseNode(object item)
    {
        if (!_nodesByItem.TryGetValue(item, out var node))
            return;

        if (!node.IsExpanded)
            return;

        node.IsExpanded = false;
        node.UpdateExpandedHeight(_itemHeight);
        InvalidateMeasure();
    }

    /// <summary>
    /// Toggles the expand/collapse state of the node for the specified item.
    /// </summary>
    public void ToggleNode(object item)
    {
        if (!_nodesByItem.TryGetValue(item, out var node))
            return;

        if (node.IsExpanded)
        {
            CollapseNode(item);
        }
        else
        {
            ExpandNode(item);
        }
    }

    #endregion

    #region Visible Nodes Iterator

    /// <summary>
    /// Returns an enumerable of visible nodes within the given viewport.
    /// This is the hierarchical iterator that skips collapsed subtrees.
    /// </summary>
    /// <param name="scrollOffset">The current scroll position.</param>
    /// <param name="viewportHeight">The height of the visible viewport.</param>
    public IEnumerable<TreeViewNode> GetVisibleNodes(float scrollOffset, float viewportHeight)
    {
        var result = new List<TreeViewNode>();
        float currentY = 0;
        float viewportEnd = scrollOffset + viewportHeight;

        foreach (var rootNode in _rootNodes)
        {
            WalkVisible(rootNode, result, ref currentY, scrollOffset, viewportEnd);

            if (currentY > viewportEnd)
                break;
        }

        return result;
    }

    private void WalkVisible(TreeViewNode node, List<TreeViewNode> result, ref float y, float viewportStart, float viewportEnd)
    {
        // Height-based skip optimization: if this entire subtree is above the viewport, skip it
        if (y + node.ExpandedHeight <= viewportStart)
        {
            y += node.ExpandedHeight;
            return;
        }

        // If we're past the viewport, stop entirely
        if (y >= viewportEnd)
        {
            return;
        }

        // Check if this node is within the viewport
        if (y + _itemHeight > viewportStart && y < viewportEnd)
        {
            result.Add(node);
        }

        y += _itemHeight;

        // Walk into children if expanded
        if (node.IsExpanded && node.Children != null)
        {
            foreach (var child in node.Children)
            {
                WalkVisible(child, result, ref y, viewportStart, viewportEnd);

                if (y >= viewportEnd)
                    return;
            }
        }
    }

    #endregion

    #region IScrollableControl

    private float _scrollOffset;

    /// <summary>
    /// Gets or sets the current scroll offset.
    /// </summary>
    public float ScrollOffset
    {
        get => _scrollOffset;
        set
        {
            var maxOffset = Math.Max(0, TotalHeight - ElementSize.Height);
            _scrollOffset = Math.Clamp(value, 0, maxOffset);
        }
    }

    /// <summary>
    /// Gets or sets whether the control is currently being scrolled.
    /// </summary>
    public bool IsScrolling { get; set; }

    /// <summary>
    /// Handles scroll input.
    /// </summary>
    public void HandleScroll(float deltaX, float deltaY)
    {
        var newOffset = _scrollOffset + deltaY;
        var maxOffset = Math.Max(0, TotalHeight);
        _scrollOffset = Math.Clamp(newOffset, 0, maxOffset);
        InvalidateMeasure();
    }

    #endregion

    #region IInputControl

    /// <summary>
    /// Invokes the command when an item is clicked.
    /// If clicked on expander, toggles expand/collapse.
    /// If clicked on item content, selects the item.
    /// </summary>
    public void InvokeCommand()
    {
        if (_hoveredItem == null)
            return;

        if (_isExpanderHit)
        {
            // Toggle expand/collapse
            ToggleNode(_hoveredItem);
        }
        else
        {
            // Select the item
            SetSelectedItem(_hoveredItem);
        }
    }

    #endregion

    #region IHoverableControl

    /// <summary>
    /// Gets or sets whether the control is currently hovered.
    /// </summary>
    public bool IsHovered { get; set; }

    #endregion

    #region Hit Detection

    private object? _hoveredItem;
    private bool _isExpanderHit;

    /// <summary>
    /// Gets the item currently under the mouse cursor (set by HitTest).
    /// </summary>
    public object? HoveredItem => _hoveredItem;

    /// <summary>
    /// Gets whether the last hit test was on the expander area.
    /// </summary>
    public bool IsExpanderHit => _isExpanderHit;

    /// <summary>
    /// Performs hit testing and tracks which node/area was hit.
    /// </summary>
    public override UiElement? HitTest(Point point)
    {
        _hoveredItem = null;
        _isExpanderHit = false;

        // Check if point is within bounds
        if (!(point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
              point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height))
        {
            return null;
        }

        // Calculate which row was hit based on Y position
        float relativeY = (float)(point.Y - Position.Y) + _scrollOffset;

        // Walk the visible nodes to find the hit node
        float currentY = 0;
        TreeViewNode? hitNode = null;

        foreach (var rootNode in _rootNodes)
        {
            hitNode = FindNodeAtY(rootNode, relativeY, ref currentY);
            if (hitNode != null)
                break;
        }

        if (hitNode != null)
        {
            _hoveredItem = hitNode.Item;

            // Check if click was on expander area
            float relativeX = (float)(point.X - Position.X);
            float indentOffset = hitNode.Depth * _indentation;
            float expanderStart = indentOffset;
            float expanderEnd = indentOffset + _expanderSize;

            // Only nodes with children have an expander
            if (hitNode.HasChildren && relativeX >= expanderStart && relativeX < expanderEnd)
            {
                _isExpanderHit = true;
            }
        }

        return this;
    }

    private TreeViewNode? FindNodeAtY(TreeViewNode node, float targetY, ref float currentY)
    {
        // Check if this node is at the target Y
        if (targetY >= currentY && targetY < currentY + _itemHeight)
        {
            return node;
        }

        currentY += _itemHeight;

        // Check children if expanded
        if (node.IsExpanded && node.Children != null)
        {
            foreach (var child in node.Children)
            {
                var found = FindNodeAtY(child, targetY, ref currentY);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    #endregion

    #region Layout

    /// <inheritdoc />
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // TODO: Implement in Phase 5
        return availableSize;
    }

    /// <inheritdoc />
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

        return new Point(positionX, positionY);
    }

    #endregion

    #region Rendering

    /// <inheritdoc />
    public override void Render(SKCanvas canvas)
    {
        // TODO: Implement in Phase 5
        base.Render(canvas);
    }

    #endregion
}
