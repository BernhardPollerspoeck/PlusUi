using System.Diagnostics.CodeAnalysis;
using PlusUi.core.Attributes;
using SkiaSharp;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// A container control that organizes content into tabbed views.
/// Users can switch between tabs to display different content.
/// </summary>
/// <example>
/// <code>
/// new TabControl()
///     .AddTab(new TabItem()
///         .SetHeader("General")
///         .SetContent(new VStack().Children(...)))
///     .AddTab(new TabItem()
///         .SetHeader("Advanced")
///         .SetContent(new VStack().Children(...)))
///     .SetSelectedIndex(0)
///     .SetTabPosition(TabPosition.Top);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class TabControl : UiLayoutElement, IInputControl, IFocusable, IKeyboardInputHandler, IHoverableControl
{
    private readonly List<TabItem> _tabs = [];
    private int _hoveredTabIndex = -1;
    private SKFont _headerFont;
    private SKPaint _headerPaint;
    private SKPaint _activeHeaderPaint;
    private SKPaint _disabledHeaderPaint;

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Tab;

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        if (AccessibilityLabel != null)
            return AccessibilityLabel;

        var tabCount = _tabs.Count;
        return $"Tab control with {tabCount} tab{(tabCount == 1 ? "" : "s")}";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        if (AccessibilityValue != null)
            return AccessibilityValue;

        return SelectedTab != null ? $"Tab {SelectedIndex + 1}: {SelectedTab.Header}" : "No tab selected";
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (SelectedTab != null)
        {
            traits |= AccessibilityTrait.Selected;
        }
        return traits;
    }

    /// <summary>
    /// Returns all tab contents for debug inspection (not just the active tab).
    /// </summary>
    protected override IEnumerable<UiElement> GetDebugChildrenCore() =>
        _tabs.Select(t => t.Content).Where(c => c != null)!;

    public TabControl()
    {
        UpdatePaints();
    }

    [MemberNotNull(nameof(_headerFont), nameof(_headerPaint), nameof(_activeHeaderPaint), nameof(_disabledHeaderPaint))]
    private void UpdatePaints()
    {
        // Release old paints if exists (for property changes)
        if (_headerPaint is not null && _headerFont is not null && _activeHeaderPaint is not null && _disabledHeaderPaint is not null)
        {
            PaintRegistry.Release(_headerPaint, _headerFont);
            PaintRegistry.Release(_activeHeaderPaint, _headerFont);
            PaintRegistry.Release(_disabledHeaderPaint, _headerFont);
        }

        // Get or create all three paints from registry (same font, different colors)
        (_headerPaint, _headerFont) = PaintRegistry.GetOrCreate(
            color: HeaderTextColor,
            size: HeaderTextSize
        );

        // Active and disabled use same font instance but different colors
        (_activeHeaderPaint, _) = PaintRegistry.GetOrCreate(
            color: ActiveHeaderTextColor,
            size: HeaderTextSize
        );

        (_disabledHeaderPaint, _) = PaintRegistry.GetOrCreate(
            color: DisabledHeaderTextColor,
            size: HeaderTextSize
        );
    }

    #region IFocusable
    bool IFocusable.IsFocusable => IsFocusable;
    int? IFocusable.TabIndex => TabIndex;
    bool IFocusable.TabStop => TabStop;
    bool IFocusable.IsFocused
    {
        get => IsFocused;
        set => IsFocused = value;
    }
    void IFocusable.OnFocus() => OnFocus();
    void IFocusable.OnBlur() => OnBlur();
    #endregion

    #region IHoverableControl
    private bool _isHovered;
    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            if (_isHovered != value)
            {
                _isHovered = value;
                if (!value)
                {
                    _hoveredTabIndex = -1;
                }
            }
        }
    }
    #endregion

    #region IInputControl
    public void InvokeCommand()
    {
        // Use the hovered tab index to select the tab
        if (_hoveredTabIndex >= 0 && _hoveredTabIndex < _tabs.Count && _tabs[_hoveredTabIndex].IsEnabled)
        {
            SelectedIndex = _hoveredTabIndex;
            NotifySetter(nameof(SelectedIndex), _hoveredTabIndex);
        }
    }
    #endregion

    #region IKeyboardInputHandler
    public bool HandleKeyboardInput(PlusKey key)
    {
        if (_tabs.Count == 0) return false;

        var isHorizontal = TabPosition == TabPosition.Top || TabPosition == TabPosition.Bottom;
        var prevKey = isHorizontal ? PlusKey.ArrowLeft : PlusKey.ArrowUp;
        var nextKey = isHorizontal ? PlusKey.ArrowRight : PlusKey.ArrowDown;

        if (key == prevKey)
        {
            SelectPreviousTab();
            return true;
        }
        if (key == nextKey)
        {
            SelectNextTab();
            return true;
        }

        return false;
    }
    #endregion

    #region Tabs
    public IReadOnlyList<TabItem> Tabs => _tabs;

    private INotifyCollectionChanged? _boundCollection;

    public TabControl AddTab(TabItem tab)
    {
        _tabs.Add(tab);
        if (tab.Content != null)
        {
            tab.Content.Parent = this;
        }
        if (_tabs.Count == 1)
        {
            SelectedIndex = 0;
        }
        InvalidateMeasure();
        return this;
    }

    public TabControl RemoveTab(TabItem tab)
    {
        var index = _tabs.IndexOf(tab);
        if (index >= 0)
        {
            _tabs.RemoveAt(index);
            if (SelectedIndex >= _tabs.Count)
            {
                SelectedIndex = _tabs.Count - 1;
            }
            InvalidateMeasure();
        }
        return this;
    }

    public TabControl ClearTabs()
    {
        _tabs.Clear();
        SelectedIndex = -1;
        InvalidateMeasure();
        return this;
    }

    public TabControl SetTabs(IEnumerable<TabItem> tabs)
    {
        // Unsubscribe from old collection
        if (_boundCollection != null)
        {
            _boundCollection.CollectionChanged -= OnBoundCollectionChanged;
            _boundCollection = null;
        }

        _tabs.Clear();
        foreach (var tab in tabs)
        {
            _tabs.Add(tab);
            if (tab.Content != null)
            {
                tab.Content.Parent = this;
            }
        }
        if (_tabs.Count > 0 && SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }

        // Subscribe to new collection if it's observable
        if (tabs is INotifyCollectionChanged observable)
        {
            _boundCollection = observable;
            _boundCollection.CollectionChanged += OnBoundCollectionChanged;
        }

        InvalidateMeasure();
        return this;
    }

    public TabControl BindTabs(Expression<Func<IEnumerable<TabItem>>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SetTabs(getter()));
        return this;
    }

    private void OnBoundCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not IEnumerable<TabItem> tabs) return;

        // Clear and rebuild from collection
        _tabs.Clear();
        foreach (var tab in tabs)
        {
            _tabs.Add(tab);
            if (tab.Content != null)
            {
                tab.Content.Parent = this;
                tab.Content.BuildContent();
            }
        }
        if (_tabs.Count > 0 && SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }
        else if (SelectedIndex >= _tabs.Count)
        {
            SelectedIndex = _tabs.Count - 1;
        }

        InvalidateMeasure();
    }
    #endregion

    #region SelectedIndex
    private int _selectedIndex = -1;

    internal int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value) return;
            if (value >= 0 && value < _tabs.Count && !_tabs[value].IsEnabled) return;

            _selectedIndex = value;
            OnSelectedIndexChanged?.Invoke(value);
            SelectionChangedCommand?.Execute(value);

            // Invalidate the new content so it gets re-measured and arranged
            if (SelectedTab?.Content != null)
            {
                SelectedTab.Content.InvalidateMeasure();
            }
            InvalidateMeasure();
        }
    }

    public TabControl SetSelectedIndex(int index)
    {
        SelectedIndex = index;
        return this;
    }

    public TabControl BindSelectedIndex(Expression<Func<int>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedIndex = getter());
        return this;
    }

    public TabControl BindSelectedIndex(Expression<Func<int>> propertyExpression, Action<int> setter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedIndex = getter());
        foreach (var segment in path)
        {
            RegisterSetter<int>(segment, setter);
        }
        return this;
    }
    #endregion

    #region SelectedTab
    internal TabItem? SelectedTab => SelectedIndex >= 0 && SelectedIndex < _tabs.Count ? _tabs[SelectedIndex] : null;

    public TabControl SetSelectedTab(TabItem tab)
    {
        var index = _tabs.IndexOf(tab);
        if (index >= 0)
        {
            SelectedIndex = index;
        }
        return this;
    }

    public TabControl BindSelectedTab(Expression<Func<TabItem?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            var tab = getter();
            if (tab != null) SetSelectedTab(tab);
        });
        return this;
    }
    #endregion

    #region TabPosition
    internal TabPosition TabPosition
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    } = TabPosition.Top;

    public TabControl SetTabPosition(TabPosition position)
    {
        TabPosition = position;
        return this;
    }

    public TabControl BindTabPosition(Expression<Func<TabPosition>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TabPosition = getter());
        return this;
    }
    #endregion

    #region Events/Commands
    internal Action<int>? OnSelectedIndexChanged { get; set; }

    public TabControl SetOnSelectedIndexChanged(Action<int> action)
    {
        OnSelectedIndexChanged = action;
        return this;
    }

    public TabControl BindOnSelectedIndexChanged(Expression<Func<Action<int>?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OnSelectedIndexChanged = getter());
        return this;
    }

    internal ICommand? SelectionChangedCommand { get; set; }

    public TabControl SetSelectionChangedCommand(ICommand command)
    {
        SelectionChangedCommand = command;
        return this;
    }

    public TabControl BindSelectionChangedCommand(Expression<Func<ICommand?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectionChangedCommand = getter());
        return this;
    }
    #endregion

    #region Styling
    internal float HeaderTextSize
    {
        get => field;
        set
        {
            field = value;
            UpdatePaints();
            InvalidateMeasure();
        }
    } = PlusUiDefaults.FontSize;

    public TabControl SetHeaderTextSize(float size)
    {
        HeaderTextSize = size;
        return this;
    }

    public TabControl BindHeaderTextSize(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HeaderTextSize = getter());
        return this;
    }

    internal SKColor HeaderTextColor
    {
        get => field;
        set
        {
            field = value;
            UpdatePaints();
        }
    } = PlusUiDefaults.TextPrimary;

    public TabControl SetHeaderTextColor(SKColor color)
    {
        HeaderTextColor = color;
        return this;
    }

    public TabControl BindHeaderTextColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HeaderTextColor = getter());
        return this;
    }

    internal SKColor ActiveHeaderTextColor
    {
        get => field;
        set
        {
            field = value;
            UpdatePaints();
        }
    } = PlusUiDefaults.AccentSuccess;

    public TabControl SetActiveHeaderTextColor(SKColor color)
    {
        ActiveHeaderTextColor = color;
        return this;
    }

    public TabControl BindActiveHeaderTextColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ActiveHeaderTextColor = getter());
        return this;
    }

    internal SKColor DisabledHeaderTextColor
    {
        get => field;
        set
        {
            field = value;
            UpdatePaints();
        }
    } = PlusUiDefaults.BorderColor;

    public TabControl SetDisabledHeaderTextColor(SKColor color)
    {
        DisabledHeaderTextColor = color;
        return this;
    }

    public TabControl BindDisabledHeaderTextColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DisabledHeaderTextColor = getter());
        return this;
    }

    internal SKColor HeaderBackgroundColor { get; set; } = PlusUiDefaults.BackgroundPrimary;

    public TabControl SetHeaderBackgroundColor(SKColor color)
    {
        HeaderBackgroundColor = color;
        return this;
    }

    public TabControl BindHeaderBackgroundColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HeaderBackgroundColor = getter());
        return this;
    }

    internal SKColor ActiveTabBackgroundColor { get; set; } = PlusUiDefaults.BackgroundHover;

    public TabControl SetActiveTabBackgroundColor(SKColor color)
    {
        ActiveTabBackgroundColor = color;
        return this;
    }

    public TabControl BindActiveTabBackgroundColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ActiveTabBackgroundColor = getter());
        return this;
    }

    internal SKColor HoverTabBackgroundColor { get; set; } = PlusUiDefaults.BackgroundSecondary;

    public TabControl SetHoverTabBackgroundColor(SKColor color)
    {
        HoverTabBackgroundColor = color;
        return this;
    }

    public TabControl BindHoverTabBackgroundColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HoverTabBackgroundColor = getter());
        return this;
    }

    internal SKColor TabIndicatorColor { get; set; } = PlusUiDefaults.AccentSuccess;

    public TabControl SetTabIndicatorColor(SKColor color)
    {
        TabIndicatorColor = color;
        return this;
    }

    public TabControl BindTabIndicatorColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TabIndicatorColor = getter());
        return this;
    }

    internal float TabIndicatorHeight { get; set; } = 3f;

    public TabControl SetTabIndicatorHeight(float height)
    {
        TabIndicatorHeight = height;
        return this;
    }

    public TabControl BindTabIndicatorHeight(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TabIndicatorHeight = getter());
        return this;
    }

    internal Margin TabPadding { get; set; } = new Margin(16, 10);

    public TabControl SetTabPadding(Margin padding)
    {
        TabPadding = padding;
        InvalidateMeasure();
        return this;
    }

    public TabControl BindTabPadding(Expression<Func<Margin>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SetTabPadding(getter()));
        return this;
    }

    internal float TabSpacing { get; set; } = 0f;

    public TabControl SetTabSpacing(float spacing)
    {
        TabSpacing = spacing;
        InvalidateMeasure();
        return this;
    }

    public TabControl BindTabSpacing(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SetTabSpacing(getter()));
        return this;
    }
    #endregion

    #region Tab Navigation
    private void SelectNextTab()
    {
        for (var i = SelectedIndex + 1; i < _tabs.Count; i++)
        {
            if (_tabs[i].IsEnabled)
            {
                SelectedIndex = i;
                NotifySetter(nameof(SelectedIndex), i);
                return;
            }
        }
    }

    private void SelectPreviousTab()
    {
        for (var i = SelectedIndex - 1; i >= 0; i--)
        {
            if (_tabs[i].IsEnabled)
            {
                SelectedIndex = i;
                NotifySetter(nameof(SelectedIndex), i);
                return;
            }
        }
    }

    private void NotifySetter(string propertyName, object value)
    {
        if (_setter.TryGetValue(propertyName, out var setters))
        {
            foreach (var setter in setters)
            {
                setter(value);
            }
        }
    }
    #endregion

    #region Measure/Arrange
    private float _headerHeight;
    private float _headerWidth;
    private readonly List<SKRect> _tabRects = [];
    private TabPosition _measuredTabPosition;

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _tabRects.Clear();
        _measuredTabPosition = TabPosition;

        // Calculate header dimensions
        var isHorizontal = TabPosition == TabPosition.Top || TabPosition == TabPosition.Bottom;

        if (isHorizontal)
        {
            MeasureHorizontalTabs(availableSize);
        }
        else
        {
            MeasureVerticalTabs(availableSize);
        }

        // Measure content - measure all tab contents so they're ready when selected
        var contentAvailable = isHorizontal
            ? new Size(availableSize.Width, availableSize.Height - _headerHeight)
            : new Size(availableSize.Width - _headerWidth, availableSize.Height);

        // Use snapshot to avoid collection modified exception during shutdown
        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.Measure(contentAvailable, dontStretch);
        }

        return new Size(availableSize.Width, availableSize.Height);
    }

    private void MeasureHorizontalTabs(Size availableSize)
    {
        _headerHeight = 0;
        var x = 0f;

        if (_headerFont == null) return;

        foreach (var tab in _tabs.ToList())
        {
            var textWidth = _headerFont.MeasureText(tab.Header);
            var tabWidth = textWidth + TabPadding.Left + TabPadding.Right;

            _headerFont.GetFontMetrics(out var metrics);
            var textHeight = metrics.Descent - metrics.Ascent;
            var tabHeight = textHeight + TabPadding.Top + TabPadding.Bottom + TabIndicatorHeight;

            _headerHeight = Math.Max(_headerHeight, tabHeight);
            _tabRects.Add(new SKRect(x, 0, x + tabWidth, tabHeight));

            x += tabWidth + TabSpacing;
        }
    }

    private void MeasureVerticalTabs(Size availableSize)
    {
        _headerWidth = 0;
        var y = 0f;

        if (_headerFont == null) return;

        foreach (var tab in _tabs.ToList())
        {
            var textWidth = _headerFont.MeasureText(tab.Header);
            var tabWidth = textWidth + TabPadding.Left + TabPadding.Right + TabIndicatorHeight;

            _headerFont.GetFontMetrics(out var metrics);
            var textHeight = metrics.Descent - metrics.Ascent;
            var tabHeight = textHeight + TabPadding.Top + TabPadding.Bottom;

            _headerWidth = Math.Max(_headerWidth, tabWidth);
            _tabRects.Add(new SKRect(0, y, tabWidth, y + tabHeight));

            y += tabHeight + TabSpacing;
        }
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var contentBounds = GetContentBounds(bounds);

        // Arrange all tab contents so they have correct positions when scrolling
        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.Arrange(contentBounds);
        }

        return new Point(bounds.Left + Margin.Left, bounds.Top + Margin.Top);
    }

    private Rect GetContentBounds(Rect bounds)
    {
        return TabPosition switch
        {
            TabPosition.Top => new Rect(
                bounds.Left,
                bounds.Top + _headerHeight,
                bounds.Width,
                bounds.Height - _headerHeight),
            TabPosition.Bottom => new Rect(
                bounds.Left,
                bounds.Top,
                bounds.Width,
                bounds.Height - _headerHeight),
            TabPosition.Left => new Rect(
                bounds.Left + _headerWidth,
                bounds.Top,
                bounds.Width - _headerWidth,
                bounds.Height),
            TabPosition.Right => new Rect(
                bounds.Left,
                bounds.Top,
                bounds.Width - _headerWidth,
                bounds.Height),
            _ => bounds
        };
    }
    #endregion

    #region Rendering
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible) return;

        var isHorizontal = TabPosition == TabPosition.Top || TabPosition == TabPosition.Bottom;

        // Render header background
        RenderHeaderBackground(canvas, isHorizontal);

        // Render tabs
        RenderTabs(canvas, isHorizontal);

        // Render content
        RenderContent(canvas);
    }

    private void RenderHeaderBackground(SKCanvas canvas, bool isHorizontal)
    {
        using var bgPaint = new SKPaint { Color = HeaderBackgroundColor };

        var headerRect = GetHeaderRect(isHorizontal);
        canvas.DrawRect(headerRect, bgPaint);
    }

    private SKRect GetHeaderRect(bool isHorizontal)
    {
        var x = Position.X + VisualOffset.X;
        var y = Position.Y + VisualOffset.Y;

        return TabPosition switch
        {
            TabPosition.Top => new SKRect(x, y, x + ElementSize.Width, y + _headerHeight),
            TabPosition.Bottom => new SKRect(x, y + ElementSize.Height - _headerHeight, x + ElementSize.Width, y + ElementSize.Height),
            TabPosition.Left => new SKRect(x, y, x + _headerWidth, y + ElementSize.Height),
            TabPosition.Right => new SKRect(x + ElementSize.Width - _headerWidth, y, x + ElementSize.Width, y + ElementSize.Height),
            _ => SKRect.Empty
        };
    }

    private void RenderTabs(SKCanvas canvas, bool isHorizontal)
    {
        if (_headerFont == null) return;
        if (_tabRects.Count != _tabs.Count) return; // Not yet measured
        if (_measuredTabPosition != TabPosition) return; // TabPosition changed, waiting for re-measure

        var baseX = Position.X + VisualOffset.X;
        var baseY = Position.Y + VisualOffset.Y;

        // Adjust base position based on TabPosition
        if (TabPosition == TabPosition.Bottom)
        {
            baseY += ElementSize.Height - _headerHeight;
        }
        else if (TabPosition == TabPosition.Right)
        {
            baseX += ElementSize.Width - _headerWidth;
        }

        for (var i = 0; i < _tabs.Count; i++)
        {
            var tab = _tabs[i];
            var tabRect = _tabRects[i];
            var isActive = i == SelectedIndex;
            var isHovered = i == _hoveredTabIndex;

            // Offset tab rect by base position
            var renderRect = new SKRect(
                tabRect.Left + baseX,
                tabRect.Top + baseY,
                tabRect.Right + baseX,
                tabRect.Bottom + baseY);

            // Draw tab background
            if (isActive)
            {
                using var activeBgPaint = new SKPaint { Color = ActiveTabBackgroundColor };
                canvas.DrawRect(renderRect, activeBgPaint);
            }
            else if (isHovered && tab.IsEnabled)
            {
                using var hoverBgPaint = new SKPaint { Color = HoverTabBackgroundColor };
                canvas.DrawRect(renderRect, hoverBgPaint);
            }

            // Draw indicator
            if (isActive)
            {
                RenderTabIndicator(canvas, renderRect, isHorizontal);
            }

            // Draw text
            var paint = !tab.IsEnabled ? _disabledHeaderPaint
                : isActive ? _activeHeaderPaint
                : _headerPaint;

            _headerFont.GetFontMetrics(out var metrics);
            var textX = renderRect.Left + TabPadding.Left;
            var textY = renderRect.MidY - (metrics.Ascent + metrics.Descent) / 2;

            if (!isHorizontal && TabPosition == TabPosition.Left)
            {
                textX = renderRect.Left + TabPadding.Left;
            }
            else if (!isHorizontal && TabPosition == TabPosition.Right)
            {
                textX = renderRect.Left + TabIndicatorHeight + TabPadding.Left;
            }

            canvas.DrawText(tab.Header, textX, textY, _headerFont, paint);
        }
    }

    private void RenderTabIndicator(SKCanvas canvas, SKRect tabRect, bool isHorizontal)
    {
        using var indicatorPaint = new SKPaint { Color = TabIndicatorColor };

        SKRect indicatorRect;
        if (isHorizontal)
        {
            indicatorRect = TabPosition == TabPosition.Top
                ? new SKRect(tabRect.Left, tabRect.Bottom - TabIndicatorHeight, tabRect.Right, tabRect.Bottom)
                : new SKRect(tabRect.Left, tabRect.Top, tabRect.Right, tabRect.Top + TabIndicatorHeight);
        }
        else
        {
            indicatorRect = TabPosition == TabPosition.Left
                ? new SKRect(tabRect.Right - TabIndicatorHeight, tabRect.Top, tabRect.Right, tabRect.Bottom)
                : new SKRect(tabRect.Left, tabRect.Top, tabRect.Left + TabIndicatorHeight, tabRect.Bottom);
        }

        canvas.DrawRect(indicatorRect, indicatorPaint);
    }

    private void RenderContent(SKCanvas canvas)
    {
        if (SelectedTab?.Content == null) return;

        var content = SelectedTab.Content;
        var originalOffset = content.VisualOffset;

        content.SetVisualOffset(new Point(
            originalOffset.X + VisualOffset.X,
            originalOffset.Y + VisualOffset.Y));

        content.Render(canvas);

        content.SetVisualOffset(originalOffset);
    }
    #endregion

    #region Hit Testing
    public override UiElement? HitTest(Point point)
    {
        // Check if clicking on a tab header
        var tabIndex = GetTabIndexAtPoint(point);
        if (tabIndex >= 0)
        {
            // Store the hovered tab index for use in InvokeCommand
            _hoveredTabIndex = tabIndex;
            return this; // Return self to handle tab clicks
        }

        // Not on a tab header - clear hover
        _hoveredTabIndex = -1;

        // Check content
        if (SelectedTab?.Content != null)
        {
            var result = SelectedTab.Content.HitTest(point);
            if (result != null)
            {
                return result;
            }
        }

        return base.HitTest(point);
    }

    private int GetTabIndexAtPoint(Point point)
    {
        var baseX = Position.X + VisualOffset.X;
        var baseY = Position.Y + VisualOffset.Y;

        if (TabPosition == TabPosition.Bottom)
        {
            baseY += ElementSize.Height - _headerHeight;
        }
        else if (TabPosition == TabPosition.Right)
        {
            baseX += ElementSize.Width - _headerWidth;
        }

        for (var i = 0; i < _tabRects.Count; i++)
        {
            var tabRect = _tabRects[i];
            var renderRect = new SKRect(
                tabRect.Left + baseX,
                tabRect.Top + baseY,
                tabRect.Right + baseX,
                tabRect.Bottom + baseY);

            if (point.X >= renderRect.Left && point.X <= renderRect.Right &&
                point.Y >= renderRect.Top && point.Y <= renderRect.Bottom)
            {
                return i;
            }
        }

        return -1;
    }

    #endregion

    #region BuildContent
    public override void BuildContent()
    {
        base.BuildContent();
        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.BuildContent();
        }
    }
    #endregion

    #region InvalidateMeasure
    public override void InvalidateMeasure()
    {
        ForceInvalidateMeasureToRoot();

        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.InvalidateMeasure();
        }
    }
    #endregion

    #region Bindings
    protected override void UpdateBindingsInternal()
    {
        base.UpdateBindingsInternal();
        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.UpdateBindings();
        }
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        base.UpdateBindingsInternal(propertyName);
        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.UpdateBindings(propertyName);
        }
    }
    #endregion

    #region ApplyStyles
    public override void ApplyStyles()
    {
        base.ApplyStyles();
        foreach (var tab in _tabs.ToList())
        {
            tab.Content?.ApplyStyles();
        }
    }
    #endregion

    #region Dispose
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from bound collection
            if (_boundCollection != null)
            {
                _boundCollection.CollectionChanged -= OnBoundCollectionChanged;
                _boundCollection = null;
            }

            // Release paints from registry
            if (_headerPaint is not null && _headerFont is not null && _activeHeaderPaint is not null && _disabledHeaderPaint is not null)
            {
                PaintRegistry.Release(_headerPaint, _headerFont);
                PaintRegistry.Release(_activeHeaderPaint, _headerFont);
                PaintRegistry.Release(_disabledHeaderPaint, _headerFont);
            }

            foreach (var tab in _tabs.ToList())
            {
                tab.Content?.Dispose();
            }
        }
        base.Dispose(disposing);
    }
    #endregion
}
