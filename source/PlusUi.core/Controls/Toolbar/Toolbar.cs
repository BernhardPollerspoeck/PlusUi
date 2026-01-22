using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.UiPropGen;
using PlusUi.core.Services;
using SkiaSharp;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// A toolbar control with support for left icons, right icons, title, icon groups, and overflow handling.
/// </summary>
/// <example>
/// <code>
/// // Material Design style
/// new Toolbar()
///     .SetTitle("My App")
///     .SetDesiredHeight(56)
///     .AddLeft(new Button().SetIcon("menu"))
///     .AddRight(new Button().SetIcon("search"))
///     .SetOverflowBehavior(OverflowBehavior.CollapseToMenu);
///
/// // With icon groups
/// new Toolbar()
///     .SetTitle("Editor")
///     .AddLeftGroup(new ToolbarIconGroup()
///         .AddIcon(new Button().SetIcon("undo"))
///         .AddIcon(new Button().SetIcon("redo"))
///         .SetSeparator(true));
/// </code>
/// </example>
[GenerateShadowMethods]
[UiPropGenPadding]
public partial class Toolbar : UiLayoutElement<Toolbar>
{
    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Toolbar;

    /// <summary>
    /// Returns all child elements including Children, LeftItems, RightItems, CenterContent, title label, overflow button, and overflow menu content.
    /// </summary>
    protected override IEnumerable<UiElement> GetDebugChildrenCore()
    {
        var elements = Children.AsEnumerable();
        elements = elements.Concat(LeftItems);
        elements = elements.Concat(RightItems);
        if (CenterContent != null) elements = elements.Append(CenterContent);
        if (_titleLabel != null) elements = elements.Append(_titleLabel);
        if (_overflowButton != null) elements = elements.Append(_overflowButton);
        if (_overflowMenuContent != null) elements = elements.Append(_overflowMenuContent);
        return elements;
    }

    private Label? _titleLabel;
    internal Button? _overflowButton;
    private List<UiElement> _overflowItems = [];
    private IImageLoaderService? _imageLoaderService;
    private SKImage? _overflowIconImage;
    internal bool _isOverflowMenuOpen;
    internal VStack? _overflowMenuContent;
    private List<Button> _overflowMenuButtons = [];
    private IOverlayService? _overlayService;
    private ToolbarOverflowMenuOverlay? _overflowMenuOverlay;
    private IPlatformService? _platformService;

    #region Title
    internal string? Title
    {
        get => field;
        set
        {
            field = value;
            if (_titleLabel == null)
            {
                _titleLabel = new Label();
                _titleLabel.Parent = this;
            }
            _titleLabel.SetText(value ?? string.Empty);
            InvalidateMeasure();
        }
    }
    public Toolbar SetTitle(string title)
    {
        Title = title;
        return this;
    }
    public Toolbar BindTitle(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Title = getter());
        return this;
    }
    #endregion

    #region TitleFontSize
    internal float TitleFontSize
    {
        get => field;
        set
        {
            field = value;
            _titleLabel?.SetTextSize(value);
            InvalidateMeasure();
        }
    } = 20;
    public Toolbar SetTitleFontSize(float size)
    {
        TitleFontSize = size;
        return this;
    }
    public Toolbar BindTitleFontSize(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TitleFontSize = getter());
        return this;
    }
    #endregion

    #region TitleColor
    internal Color TitleColor
    {
        get => field;
        set
        {
            field = value;
            _titleLabel?.SetTextColor(value);
            InvalidateMeasure();
        }
    } = PlusUiDefaults.TextPrimary;
    public Toolbar SetTitleColor(Color color)
    {
        TitleColor = color;
        return this;
    }
    public Toolbar BindTitleColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TitleColor = getter());
        return this;
    }
    #endregion

    #region TitleAlignment
    internal TitleAlignment TitleAlignment
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = TitleAlignment.Center;
    public Toolbar SetTitleAlignment(TitleAlignment alignment)
    {
        TitleAlignment = alignment;
        return this;
    }
    public Toolbar BindTitleAlignment(Expression<Func<TitleAlignment>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TitleAlignment = getter());
        return this;
    }
    #endregion

    #region ItemSpacing
    internal float ItemSpacing
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = PlusUiDefaults.Spacing;
    public Toolbar SetItemSpacing(float spacing)
    {
        ItemSpacing = spacing;
        return this;
    }
    public Toolbar BindItemSpacing(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ItemSpacing = getter());
        return this;
    }
    #endregion

    #region OverflowBehavior
    internal OverflowBehavior OverflowBehavior
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = OverflowBehavior.None;
    public Toolbar SetOverflowBehavior(OverflowBehavior behavior)
    {
        OverflowBehavior = behavior;
        return this;
    }
    public Toolbar BindOverflowBehavior(Expression<Func<OverflowBehavior>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowBehavior = getter());
        return this;
    }
    #endregion

    #region OverflowThreshold
    internal float OverflowThreshold
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = 600;
    public Toolbar SetOverflowThreshold(float width)
    {
        OverflowThreshold = width;
        return this;
    }
    public Toolbar BindOverflowThreshold(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowThreshold = getter());
        return this;
    }
    #endregion

    #region OverflowIcon
    internal string OverflowIcon
    {
        get => field;
        set
        {
            field = value;
            _imageLoaderService ??= ServiceProviderService.ServiceProvider?.GetRequiredService<IImageLoaderService>();
            var (staticImage, _, _) = _imageLoaderService?.LoadImage(value, OnOverflowIconLoadedFromWeb, null, null) ?? (default, default, default);
            _overflowIconImage = staticImage;
            InvalidateMeasure();
        }
    } = "more_vert";
    public Toolbar SetOverflowIcon(string icon)
    {
        OverflowIcon = icon;
        return this;
    }
    public Toolbar BindOverflowIcon(Expression<Func<string>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowIcon = getter());
        return this;
    }

    private void OnOverflowIconLoadedFromWeb(SKImage? image)
    {
        if (image != null)
        {
            _overflowIconImage = image;
            InvalidateMeasure();
        }
    }
    #endregion

    #region OverflowMenuBackground
    internal Color OverflowMenuBackground { get; set; } = PlusUiDefaults.BackgroundPrimary;
    public Toolbar SetOverflowMenuBackground(Color color)
    {
        OverflowMenuBackground = color;
        return this;
    }
    public Toolbar BindOverflowMenuBackground(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowMenuBackground = getter());
        return this;
    }
    #endregion

    #region OverflowMenuItemBackground
    internal Color OverflowMenuItemBackground { get; set; } = PlusUiDefaults.BackgroundSecondary;
    public Toolbar SetOverflowMenuItemBackground(Color color)
    {
        OverflowMenuItemBackground = color;
        return this;
    }
    public Toolbar BindOverflowMenuItemBackground(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowMenuItemBackground = getter());
        return this;
    }
    #endregion

    #region OverflowMenuItemHoverBackground
    internal Color OverflowMenuItemHoverBackground { get; set; } = new Color(70, 70, 70);
    public Toolbar SetOverflowMenuItemHoverBackground(Color color)
    {
        OverflowMenuItemHoverBackground = color;
        return this;
    }
    public Toolbar BindOverflowMenuItemHoverBackground(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowMenuItemHoverBackground = getter());
        return this;
    }
    #endregion

    #region OverflowMenuItemTextColor
    internal Color OverflowMenuItemTextColor { get; set; } = PlusUiDefaults.TextPrimary;
    public Toolbar SetOverflowMenuItemTextColor(Color color)
    {
        OverflowMenuItemTextColor = color;
        return this;
    }
    public Toolbar BindOverflowMenuItemTextColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OverflowMenuItemTextColor = getter());
        return this;
    }
    #endregion

    #region Sections
    internal List<UiElement> LeftItems { get; } = [];
    internal UiElement? CenterContent { get; set; }
    internal List<UiElement> RightItems { get; } = [];
    #endregion

    public Toolbar()
    {
    }

    #region Fluent API - Left Section
    /// <summary>
    /// Adds an element to the left section of the toolbar.
    /// </summary>
    public Toolbar AddLeft(UiElement element)
    {
        element.Parent = this;
        LeftItems.Add(element);
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Adds an icon group to the left section of the toolbar.
    /// </summary>
    public Toolbar AddLeftGroup(ToolbarIconGroup group)
    {
        group.Parent = this;
        LeftItems.Add(group);
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Fluent API - Center Section
    /// <summary>
    /// Sets the center content of the toolbar (replaces title if set).
    /// </summary>
    public Toolbar SetCenterContent(UiElement content)
    {
        content.Parent = this;
        CenterContent = content;
        InvalidateMeasure();
        return this;
    }

    public Toolbar BindCenterContent(Expression<Func<UiElement?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            var content = getter();
            if (content != null) SetCenterContent(content);
            else CenterContent = null;
        });
        return this;
    }
    #endregion

    #region Fluent API - Right Section
    /// <summary>
    /// Adds an element to the right section of the toolbar.
    /// </summary>
    public Toolbar AddRight(UiElement element)
    {
        element.Parent = this;
        RightItems.Add(element);
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Adds an icon group to the right section of the toolbar.
    /// </summary>
    public Toolbar AddRightGroup(ToolbarIconGroup group)
    {
        group.Parent = this;
        RightItems.Add(group);
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Measure/Arrange
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _overflowItems.Clear();

        var effectiveWidth = availableSize.Width - Padding.Horizontal;
        var effectiveHeight = availableSize.Height - Padding.Vertical;
        var childAvailableSize = new Size(effectiveWidth, effectiveHeight);

        // Use overflow handling when CollapseToMenu is enabled
        // This ensures items never overlap with centered titles
        if (OverflowBehavior == OverflowBehavior.CollapseToMenu)
        {
            return MeasureWithOverflow(availableSize, childAvailableSize);
        }
        else
        {
            return MeasureNormal(availableSize, childAvailableSize);
        }
    }

    private Size MeasureNormal(Size availableSize, Size childAvailableSize)
    {
        var remainingWidth = childAvailableSize.Width;
        var maxHeight = 0f;

        // Measure right items first (from right to left)
        foreach (var item in RightItems)
        {
            var itemSize = item.Measure(new Size(remainingWidth, childAvailableSize.Height), false);
            remainingWidth -= itemSize.Width + ItemSpacing;
            maxHeight = Math.Max(maxHeight, itemSize.Height);
        }

        // Measure left items
        foreach (var item in LeftItems)
        {
            var itemSize = item.Measure(new Size(remainingWidth, childAvailableSize.Height), false);
            remainingWidth -= itemSize.Width + ItemSpacing;
            maxHeight = Math.Max(maxHeight, itemSize.Height);
        }

        // Measure title or center content
        if (!string.IsNullOrEmpty(Title) && _titleLabel != null)
        {
            _titleLabel.SetTextSize(TitleFontSize);
            _titleLabel.SetTextColor(TitleColor);
            var titleSize = _titleLabel.Measure(new Size(remainingWidth, childAvailableSize.Height), false);
            maxHeight = Math.Max(maxHeight, titleSize.Height);
        }
        else if (CenterContent != null)
        {
            var centerSize = CenterContent.Measure(new Size(remainingWidth, childAvailableSize.Height), false);
            maxHeight = Math.Max(maxHeight, centerSize.Height);
        }

        return new Size(availableSize.Width, maxHeight + Padding.Vertical);
    }

    private Size MeasureWithOverflow(Size availableSize, Size childAvailableSize)
    {
        var maxHeight = 0f;
        _overflowItems.Clear();

        // Reserve space for overflow button
        if (_overflowButton == null)
        {
            _overflowButton = new Button()
                .SetText("...")
                .SetPadding(new Margin(8, 4))
                .SetBackground(new SolidColorBackground(Colors.Transparent))
                .SetTextColor(TitleColor)
                .SetOnClick(ShowOverflowMenu);
            _overflowButton.Parent = this;
        }

        var overflowButtonSize = _overflowButton.Measure(childAvailableSize, false);
        maxHeight = Math.Max(maxHeight, overflowButtonSize.Height);

        // Measure title first (always visible)
        float titleWidth = 0;
        if (!string.IsNullOrEmpty(Title) && _titleLabel != null)
        {
            _titleLabel.SetTextSize(TitleFontSize);
            _titleLabel.SetTextColor(TitleColor);
            var titleSize = _titleLabel.Measure(new Size(childAvailableSize.Width * 0.5f, childAvailableSize.Height), false);
            titleWidth = titleSize.Width;
            maxHeight = Math.Max(maxHeight, titleSize.Height);
        }

        // Calculate available space for left and right items based on title alignment
        float leftAvailableWidth;
        float rightAvailableWidth;

        if (TitleAlignment == TitleAlignment.Center && titleWidth > 0)
        {
            // For centered title: calculate where title will be positioned
            // Title will be at: (contentWidth - titleWidth) / 2
            var titleStartX = (childAvailableSize.Width - titleWidth) / 2;
            var titleEndX = titleStartX + titleWidth;

            // Left items can use space from left edge to title start (with spacing)
            leftAvailableWidth = titleStartX - ItemSpacing;

            // Right items can use space from title end to right edge (minus overflow button)
            rightAvailableWidth = childAvailableSize.Width - titleEndX - overflowButtonSize.Width - (ItemSpacing * 2);
        }
        else
        {
            // For left-aligned title or no title: use total remaining width
            var totalRemaining = childAvailableSize.Width - titleWidth - overflowButtonSize.Width - (ItemSpacing * 2);
            leftAvailableWidth = totalRemaining / 2;
            rightAvailableWidth = totalRemaining / 2;
        }

        // Measure and filter left items
        var visibleLeftItems = new List<UiElement>();
        var leftUsedWidth = 0f;

        // Sort left items by priority (highest first)
        var sortedLeftItems = LeftItems
            .Select(item => (Item: item, Priority: item is ToolbarIconGroup g ? g.Priority : 0))
            .OrderByDescending(x => x.Priority)
            .ToList();

        foreach (var (item, priority) in sortedLeftItems)
        {
            var itemSize = item.Measure(new Size(leftAvailableWidth - leftUsedWidth, childAvailableSize.Height), false);

            if (leftUsedWidth + itemSize.Width + ItemSpacing <= leftAvailableWidth)
            {
                visibleLeftItems.Add(item);
                leftUsedWidth += itemSize.Width + ItemSpacing;
                maxHeight = Math.Max(maxHeight, itemSize.Height);
            }
            else
            {
                _overflowItems.Add(item);
            }
        }

        // Measure and filter right items
        var visibleRightItems = new List<UiElement>();
        var rightUsedWidth = 0f;

        // Sort right items by priority (highest first)
        var sortedRightItems = RightItems
            .Select(item => (Item: item, Priority: item is ToolbarIconGroup g ? g.Priority : 0))
            .OrderByDescending(x => x.Priority)
            .ToList();

        foreach (var (item, priority) in sortedRightItems)
        {
            var itemSize = item.Measure(new Size(rightAvailableWidth - rightUsedWidth, childAvailableSize.Height), false);

            if (rightUsedWidth + itemSize.Width + ItemSpacing <= rightAvailableWidth)
            {
                visibleRightItems.Add(item);
                rightUsedWidth += itemSize.Width + ItemSpacing;
                maxHeight = Math.Max(maxHeight, itemSize.Height);
            }
            else
            {
                _overflowItems.Add(item);
            }
        }

        // Update visible/hidden items
        foreach (var item in LeftItems)
        {
            item.SetIsVisible(visibleLeftItems.Contains(item));
        }
        foreach (var item in RightItems)
        {
            item.SetIsVisible(visibleRightItems.Contains(item));
        }

        // Note: Overflow menu is measured/arranged by ToolbarOverflowMenuOverlay

        return new Size(availableSize.Width, maxHeight + Padding.Vertical);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = bounds.Left;
        var positionY = bounds.Top;

        var contentBounds = new Rect(
            bounds.Left + Padding.Left,
            bounds.Top + Padding.Top,
            bounds.Width - Padding.Horizontal,
            bounds.Height - Padding.Vertical);

        // Recalculate visibility based on actual arrange bounds (may differ from measure bounds)
        if (OverflowBehavior == OverflowBehavior.CollapseToMenu)
        {
            RecalculateVisibilityForBounds(contentBounds.Width);
        }

        // Arrange left items
        var leftX = contentBounds.Left;
        foreach (var item in LeftItems.Where(i => i.IsVisible))
        {
            var itemY = contentBounds.Top + (contentBounds.Height - item.ElementSize.Height) / 2;
            item.Arrange(new Rect(leftX, itemY, item.ElementSize.Width, item.ElementSize.Height));
            leftX += item.ElementSize.Width + ItemSpacing;
        }

        // Arrange right items (including overflow button if present)
        var rightX = contentBounds.Right;

        if (_overflowButton != null && _overflowItems.Count > 0)
        {
            rightX -= _overflowButton.ElementSize.Width;
            var overflowY = contentBounds.Top + (contentBounds.Height - _overflowButton.ElementSize.Height) / 2;
            _overflowButton.Arrange(new Rect(rightX, overflowY, _overflowButton.ElementSize.Width, _overflowButton.ElementSize.Height));
            rightX -= ItemSpacing;
        }

        foreach (var item in RightItems.Where(i => i.IsVisible).Reverse())
        {
            rightX -= item.ElementSize.Width;
            var itemY = contentBounds.Top + (contentBounds.Height - item.ElementSize.Height) / 2;
            item.Arrange(new Rect(rightX, itemY, item.ElementSize.Width, item.ElementSize.Height));
            rightX -= ItemSpacing;
        }

        // Arrange title or center content
        if (!string.IsNullOrEmpty(Title) && _titleLabel != null)
        {
            if (TitleAlignment == TitleAlignment.Center)
            {
                var titleX = contentBounds.Left + (contentBounds.Width - _titleLabel.ElementSize.Width) / 2;
                var titleY = contentBounds.Top + (contentBounds.Height - _titleLabel.ElementSize.Height) / 2;
                _titleLabel.Arrange(new Rect(titleX, titleY, _titleLabel.ElementSize.Width, _titleLabel.ElementSize.Height));
            }
            else // Left alignment
            {
                var titleY = contentBounds.Top + (contentBounds.Height - _titleLabel.ElementSize.Height) / 2;
                _titleLabel.Arrange(new Rect(leftX, titleY, _titleLabel.ElementSize.Width, _titleLabel.ElementSize.Height));
            }
        }
        else if (CenterContent != null)
        {
            var centerX = contentBounds.Left + (contentBounds.Width - CenterContent.ElementSize.Width) / 2;
            var centerY = contentBounds.Top + (contentBounds.Height - CenterContent.ElementSize.Height) / 2;
            CenterContent.Arrange(new Rect(centerX, centerY, CenterContent.ElementSize.Width, CenterContent.ElementSize.Height));
        }

        // Note: Overflow menu is measured/arranged by ToolbarOverflowMenuOverlay

        return new Point(positionX, positionY);
    }

    private void RecalculateVisibilityForBounds(float contentWidth)
    {
        _overflowItems.Clear();

        // Get title width
        var titleWidth = _titleLabel?.ElementSize.Width ?? 0;

        // Get overflow button width
        var overflowButtonWidth = _overflowButton?.ElementSize.Width ?? 0;

        // Calculate available space for left and right items
        float leftAvailableWidth;
        float rightAvailableWidth;

        if (TitleAlignment == TitleAlignment.Center && titleWidth > 0)
        {
            var titleStartX = (contentWidth - titleWidth) / 2;
            var titleEndX = titleStartX + titleWidth;
            leftAvailableWidth = titleStartX - ItemSpacing;
            rightAvailableWidth = contentWidth - titleEndX - overflowButtonWidth - (ItemSpacing * 2);
        }
        else
        {
            var totalRemaining = contentWidth - titleWidth - overflowButtonWidth - (ItemSpacing * 2);
            leftAvailableWidth = totalRemaining / 2;
            rightAvailableWidth = totalRemaining / 2;
        }

        // Determine visible left items
        var leftUsedWidth = 0f;
        foreach (var item in LeftItems)
        {
            if (leftUsedWidth + item.ElementSize.Width + ItemSpacing <= leftAvailableWidth)
            {
                item.SetIsVisible(true);
                leftUsedWidth += item.ElementSize.Width + ItemSpacing;
            }
            else
            {
                item.SetIsVisible(false);
                _overflowItems.Add(item);
            }
        }

        // Determine visible right items
        var rightUsedWidth = 0f;
        foreach (var item in RightItems)
        {
            if (rightUsedWidth + item.ElementSize.Width + ItemSpacing <= rightAvailableWidth)
            {
                item.SetIsVisible(true);
                rightUsedWidth += item.ElementSize.Width + ItemSpacing;
            }
            else
            {
                item.SetIsVisible(false);
                _overflowItems.Add(item);
            }
        }
    }

    public override void InvalidateMeasure()
    {
        base.InvalidateMeasure();

        // Invalidate items that are not in Children collection
        foreach (var item in LeftItems)
        {
            item.InvalidateMeasure();
        }
        foreach (var item in RightItems)
        {
            item.InvalidateMeasure();
        }
        _titleLabel?.InvalidateMeasure();
        CenterContent?.InvalidateMeasure();
        _overflowButton?.InvalidateMeasure();
        // Note: _overflowMenuContent is not invalidated here as it's rendered via OverlayService
        // and has Parent=this which would cause infinite recursion
    }
    #endregion

    #region Rendering
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        if (!IsVisible)
        {
            return;
        }

        // Render left items
        foreach (var item in LeftItems.Where(i => i.IsVisible))
        {
            var childOriginalOffset = item.VisualOffset;
            item.SetVisualOffset(new Point(
                childOriginalOffset.X + VisualOffset.X,
                childOriginalOffset.Y + VisualOffset.Y));
            item.Render(canvas);
            item.SetVisualOffset(childOriginalOffset);
        }

        // Render right items
        foreach (var item in RightItems.Where(i => i.IsVisible))
        {
            var childOriginalOffset = item.VisualOffset;
            item.SetVisualOffset(new Point(
                childOriginalOffset.X + VisualOffset.X,
                childOriginalOffset.Y + VisualOffset.Y));
            item.Render(canvas);
            item.SetVisualOffset(childOriginalOffset);
        }

        // Render overflow button
        if (_overflowButton != null && _overflowItems.Count > 0)
        {
            var childOriginalOffset = _overflowButton.VisualOffset;
            _overflowButton.SetVisualOffset(new Point(
                childOriginalOffset.X + VisualOffset.X,
                childOriginalOffset.Y + VisualOffset.Y));
            _overflowButton.Render(canvas);
            _overflowButton.SetVisualOffset(childOriginalOffset);
        }

        // Render title
        if (!string.IsNullOrEmpty(Title) && _titleLabel != null)
        {
            var childOriginalOffset = _titleLabel.VisualOffset;
            _titleLabel.SetVisualOffset(new Point(
                childOriginalOffset.X + VisualOffset.X,
                childOriginalOffset.Y + VisualOffset.Y));
            _titleLabel.Render(canvas);
            _titleLabel.SetVisualOffset(childOriginalOffset);
        }
        else if (CenterContent != null)
        {
            var childOriginalOffset = CenterContent.VisualOffset;
            CenterContent.SetVisualOffset(new Point(
                childOriginalOffset.X + VisualOffset.X,
                childOriginalOffset.Y + VisualOffset.Y));
            CenterContent.Render(canvas);
            CenterContent.SetVisualOffset(childOriginalOffset);
        }

        // Overflow menu is rendered via OverlayService (above all page content)
    }
    #endregion

    #region Overflow Menu
    /// <summary>
    /// Toggles the overflow menu visibility.
    /// </summary>
    private void ShowOverflowMenu()
    {
        _isOverflowMenuOpen = !_isOverflowMenuOpen;

        if (_isOverflowMenuOpen)
        {
            BuildOverflowMenuContent();
            RegisterOverflowMenuOverlay();
        }
        else
        {
            CloseOverflowMenu();
        }

        InvalidateMeasure();
    }

    /// <summary>
    /// Closes the overflow menu.
    /// </summary>
    internal void CloseOverflowMenu()
    {
        _isOverflowMenuOpen = false;
        UnregisterOverflowMenuOverlay();
        _overflowMenuContent = null;
        _overflowMenuButtons.Clear();
    }

    private void RegisterOverflowMenuOverlay()
    {
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null || _overflowMenuContent == null)
            return;

        _overflowMenuOverlay = new ToolbarOverflowMenuOverlay(this);
        _overlayService.RegisterOverlay(_overflowMenuOverlay);
    }

    private void UnregisterOverflowMenuOverlay()
    {
        if (_overlayService != null && _overflowMenuOverlay != null)
        {
            _overlayService.UnregisterOverlay(_overflowMenuOverlay);
            _overflowMenuOverlay = null;
        }
    }

    /// <summary>
    /// Builds the overflow menu content from overflow items.
    /// </summary>
    private void BuildOverflowMenuContent()
    {
        _overflowMenuButtons.Clear();
        _overflowMenuContent = new VStack();
        _overflowMenuContent.Parent = this;
        // Ensure VStack has no background (overlay draws the background) and ignore styling
        _overflowMenuContent.SetBackground(new SolidColorBackground(Colors.Transparent));
        _overflowMenuContent.IgnoreStyling();

        foreach (var item in _overflowItems)
        {
            if (item is Button btn)
            {
                // Create a menu button that mirrors the overflow item
                var menuButton = new Button()
                    .SetText(btn.Text ?? string.Empty)
                    .SetTextSize(14)
                    .SetPadding(new Margin(16, 12))
                    .SetBackground(new SolidColorBackground(OverflowMenuItemBackground))
                    .SetHoverBackground(new SolidColorBackground(OverflowMenuItemHoverBackground))
                    .SetTextColor(OverflowMenuItemTextColor)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch);

                if (!string.IsNullOrEmpty(btn.Icon))
                {
                    menuButton.SetIcon(btn.Icon);
                }

                // Set click handler to close menu, and pass through original command
                menuButton.SetOnClick(() =>
                {
                    CloseOverflowMenu();
                    InvalidateMeasure();
                });

                if (btn.Command != null)
                {
                    menuButton.SetCommand(btn.Command);
                    menuButton.SetCommandParameter(btn.CommandParameter);
                }

                menuButton.Parent = _overflowMenuContent;
                _overflowMenuButtons.Add(menuButton);
                _overflowMenuContent.AddChild(menuButton);
            }
            else if (item is ToolbarIconGroup group)
            {
                // For icon groups, create menu items for each child button
                foreach (var groupChild in group.Children)
                {
                    if (groupChild is Button groupBtn)
                    {
                        var menuButton = new Button()
                            .SetText(groupBtn.Text ?? string.Empty)
                            .SetTextSize(14)
                            .SetPadding(new Margin(16, 12))
                            .SetBackground(new SolidColorBackground(OverflowMenuItemBackground))
                            .SetHoverBackground(new SolidColorBackground(OverflowMenuItemHoverBackground))
                            .SetTextColor(OverflowMenuItemTextColor)
                            .SetHorizontalAlignment(HorizontalAlignment.Stretch);

                        if (!string.IsNullOrEmpty(groupBtn.Icon))
                        {
                            menuButton.SetIcon(groupBtn.Icon);
                        }

                        // Set click handler to close menu, and pass through original command
                        menuButton.SetOnClick(() =>
                        {
                            CloseOverflowMenu();
                            InvalidateMeasure();
                        });

                        if (groupBtn.Command != null)
                        {
                            menuButton.SetCommand(groupBtn.Command);
                            menuButton.SetCommandParameter(groupBtn.CommandParameter);
                        }

                        menuButton.Parent = _overflowMenuContent;
                        _overflowMenuButtons.Add(menuButton);
                        _overflowMenuContent.AddChild(menuButton);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Measures the overflow menu if open.
    /// </summary>
    private void MeasureOverflowMenu()
    {
        if (!_isOverflowMenuOpen || _overflowMenuContent == null || _overflowButton == null)
            return;

        // Measure the menu content with reasonable constraints
        var menuWidth = Math.Max(150, _overflowButton.ElementSize.Width * 4);
        var availableMenuSize = new Size(menuWidth, 400);
        _overflowMenuContent.Measure(availableMenuSize, dontStretch: true);
    }

    /// <summary>
    /// Arranges the overflow menu with intelligent positioning.
    /// Opens upward if there's not enough space below.
    /// </summary>
    private void ArrangeOverflowMenu()
    {
        if (!_isOverflowMenuOpen || _overflowMenuContent == null || _overflowButton == null)
            return;

        var menuWidth = _overflowMenuContent.ElementSize.Width;
        var menuHeight = _overflowMenuContent.ElementSize.Height;
        var buttonBottom = _overflowButton.Position.Y + _overflowButton.ElementSize.Height;
        var buttonTop = _overflowButton.Position.Y;

        // Get window size to check available space
        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowHeight = _platformService?.WindowSize.Height ?? 800f;

        // Check if menu fits below
        var spaceBelow = windowHeight - buttonBottom - 4;
        var opensUpward = spaceBelow < menuHeight && buttonTop > spaceBelow;

        // Position menu aligned to the right of the button
        var menuX = _overflowButton.Position.X + _overflowButton.ElementSize.Width - menuWidth;

        float menuY;
        if (opensUpward)
        {
            // Open upward
            menuY = buttonTop - menuHeight - 4;
        }
        else
        {
            // Open downward (default)
            menuY = buttonBottom + 4;
        }

        _overflowMenuContent.Arrange(new Rect(menuX, menuY, menuWidth, menuHeight));
    }

    #endregion

    #region Hit Testing
    public override UiElement? HitTest(Point point)
    {
        // Overflow menu clicks are handled by the overlay
        // Click outside is handled by InputService dismissing overlays

        // Test overflow button
        if (_overflowButton != null && _overflowItems.Count > 0)
        {
            var result = _overflowButton.HitTest(point);
            if (result != null) return result;
        }

        // Test left items
        foreach (var item in LeftItems.Where(i => i.IsVisible))
        {
            var result = item.HitTest(point);
            if (result != null) return result;
        }

        // Test right items
        foreach (var item in RightItems.Where(i => i.IsVisible))
        {
            var result = item.HitTest(point);
            if (result != null) return result;
        }

        // Test title
        if (_titleLabel != null)
        {
            var result = _titleLabel.HitTest(point);
            if (result != null) return result;
        }

        // Test center content
        if (CenterContent != null)
        {
            var result = CenterContent.HitTest(point);
            if (result != null) return result;
        }

        return base.HitTest(point);
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var item in LeftItems)
            {
                item.Dispose();
            }
            foreach (var item in RightItems)
            {
                item.Dispose();
            }
            foreach (var button in _overflowMenuButtons)
            {
                button.Dispose();
            }
            _titleLabel?.Dispose();
            CenterContent?.Dispose();
            _overflowButton?.Dispose();
            _overflowIconImage?.Dispose();
            _overflowMenuContent?.Dispose();
        }
        base.Dispose(disposing);
    }
}
