using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using SkiaSharp;
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
///     .SetHeight(56)
///     .AddLeftIcon(new Button().SetIcon("menu"))
///     .AddRightIcon(new Button().SetIcon("search"))
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
public partial class Toolbar : UiLayoutElement<Toolbar>
{
    private Label? _titleLabel;
    private Button? _overflowButton;
    private UiPopupElement? _overflowPopup;
    private List<UiElement> _overflowItems = new();
    private IImageLoaderService? _imageLoaderService;
    private SKImage? _overflowIconImage;

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
    public Toolbar BindTitle(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Title = propertyGetter());
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
            _titleLabel?.SetFontSize(value);
            InvalidateMeasure();
        }
    } = 20;
    public Toolbar SetTitleFontSize(float size)
    {
        TitleFontSize = size;
        return this;
    }
    public Toolbar BindTitleFontSize(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => TitleFontSize = propertyGetter());
        return this;
    }
    #endregion

    #region TitleColor
    internal SKColor TitleColor
    {
        get => field;
        set
        {
            field = value;
            _titleLabel?.SetTextColor(value);
            InvalidateMeasure();
        }
    } = SKColors.Black;
    public Toolbar SetTitleColor(SKColor color)
    {
        TitleColor = color;
        return this;
    }
    public Toolbar BindTitleColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => TitleColor = propertyGetter());
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
    public Toolbar BindTitleAlignment(string propertyName, Func<TitleAlignment> propertyGetter)
    {
        RegisterBinding(propertyName, () => TitleAlignment = propertyGetter());
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
    } = 8;
    public Toolbar SetItemSpacing(float spacing)
    {
        ItemSpacing = spacing;
        return this;
    }
    public Toolbar BindItemSpacing(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => ItemSpacing = propertyGetter());
        return this;
    }
    #endregion

    #region ContentPadding
    internal Margin ContentPadding
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = new Margin(16, 0, 16, 0);
    public Toolbar SetContentPadding(Margin padding)
    {
        ContentPadding = padding;
        return this;
    }
    public Toolbar BindContentPadding(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => ContentPadding = propertyGetter());
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
    public Toolbar BindOverflowBehavior(string propertyName, Func<OverflowBehavior> propertyGetter)
    {
        RegisterBinding(propertyName, () => OverflowBehavior = propertyGetter());
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
    public Toolbar BindOverflowThreshold(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => OverflowThreshold = propertyGetter());
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
            var (staticImage, _) = _imageLoaderService?.LoadImage(value, OnOverflowIconLoadedFromWeb, null) ?? (default, default);
            _overflowIconImage = staticImage;
            InvalidateMeasure();
        }
    } = "more_vert";
    public Toolbar SetOverflowIcon(string icon)
    {
        OverflowIcon = icon;
        return this;
    }
    public Toolbar BindOverflowIcon(string propertyName, Func<string> propertyGetter)
    {
        RegisterBinding(propertyName, () => OverflowIcon = propertyGetter());
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

    #region Sections
    internal List<UiElement> LeftItems { get; } = new();
    internal UiElement? CenterContent { get; set; }
    internal List<UiElement> RightItems { get; } = new();
    #endregion

    public Toolbar()
    {
    }

    #region Fluent API - Left Section
    public Toolbar AddLeftIcon(UiElement icon)
    {
        icon.Parent = this;
        LeftItems.Add(icon);
        InvalidateMeasure();
        return this;
    }

    public Toolbar AddLeftGroup(ToolbarIconGroup group)
    {
        group.Parent = this;
        LeftItems.Add(group);
        InvalidateMeasure();
        return this;
    }

    public Toolbar AddLeftContent(UiElement content)
    {
        content.Parent = this;
        LeftItems.Add(content);
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Fluent API - Center Section
    public Toolbar SetCenterContent(UiElement content)
    {
        content.Parent = this;
        CenterContent = content;
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Fluent API - Right Section
    public Toolbar AddRightIcon(UiElement icon)
    {
        icon.Parent = this;
        RightItems.Add(icon);
        InvalidateMeasure();
        return this;
    }

    public Toolbar AddRightGroup(ToolbarIconGroup group)
    {
        group.Parent = this;
        RightItems.Add(group);
        InvalidateMeasure();
        return this;
    }

    public Toolbar AddRightContent(UiElement content)
    {
        content.Parent = this;
        RightItems.Add(content);
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Measure/Arrange
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _overflowItems.Clear();

        var effectiveWidth = availableSize.Width - ContentPadding.Horizontal;
        var effectiveHeight = availableSize.Height - ContentPadding.Vertical;
        var childAvailableSize = new Size(effectiveWidth, effectiveHeight);

        // Determine if we need overflow handling
        var needsOverflow = OverflowBehavior == OverflowBehavior.CollapseToMenu && effectiveWidth < OverflowThreshold;

        if (needsOverflow)
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
            _titleLabel.SetFontSize(TitleFontSize);
            _titleLabel.SetTextColor(TitleColor);
            var titleSize = _titleLabel.Measure(new Size(remainingWidth, childAvailableSize.Height), false);
            maxHeight = Math.Max(maxHeight, titleSize.Height);
        }
        else if (CenterContent != null)
        {
            var centerSize = CenterContent.Measure(new Size(remainingWidth, childAvailableSize.Height), false);
            maxHeight = Math.Max(maxHeight, centerSize.Height);
        }

        return new Size(availableSize.Width, maxHeight + ContentPadding.Vertical);
    }

    private Size MeasureWithOverflow(Size availableSize, Size childAvailableSize)
    {
        var remainingWidth = childAvailableSize.Width;
        var maxHeight = 0f;

        // Reserve space for overflow button
        if (_overflowButton == null)
        {
            _overflowButton = new Button()
                .SetIcon(OverflowIcon)
                .SetCommand(new RelayCommand(ShowOverflowMenu));
            _overflowButton.Parent = this;
        }

        var overflowButtonSize = _overflowButton.Measure(childAvailableSize, false);
        remainingWidth -= overflowButtonSize.Width + ItemSpacing;
        maxHeight = Math.Max(maxHeight, overflowButtonSize.Height);

        // Measure title first (always visible)
        if (!string.IsNullOrEmpty(Title) && _titleLabel != null)
        {
            _titleLabel.SetFontSize(TitleFontSize);
            _titleLabel.SetTextColor(TitleColor);
            var titleSize = _titleLabel.Measure(new Size(remainingWidth * 0.5f, childAvailableSize.Height), false);
            remainingWidth -= titleSize.Width + ItemSpacing;
            maxHeight = Math.Max(maxHeight, titleSize.Height);
        }

        // Collect all items with their priorities
        var allItems = new List<(UiElement Item, int Priority)>();

        foreach (var item in LeftItems)
        {
            var priority = item is ToolbarIconGroup group ? group.Priority : 0;
            allItems.Add((item, priority));
        }

        foreach (var item in RightItems)
        {
            var priority = item is ToolbarIconGroup group ? group.Priority : 0;
            allItems.Add((item, priority));
        }

        // Sort by priority (highest first)
        allItems = allItems.OrderByDescending(x => x.Priority).ToList();

        var visibleItems = new List<UiElement>();

        foreach (var (item, priority) in allItems)
        {
            var itemSize = item.Measure(new Size(remainingWidth, childAvailableSize.Height), false);

            if (itemSize.Width + ItemSpacing <= remainingWidth)
            {
                visibleItems.Add(item);
                remainingWidth -= itemSize.Width + ItemSpacing;
                maxHeight = Math.Max(maxHeight, itemSize.Height);
            }
            else
            {
                _overflowItems.Add(item);
            }
        }

        // Update visible/hidden items
        foreach (var item in LeftItems.Concat(RightItems))
        {
            item.SetIsVisible(visibleItems.Contains(item));
        }

        return new Size(availableSize.Width, maxHeight + ContentPadding.Vertical);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = bounds.Left;
        var positionY = bounds.Top;

        var contentBounds = new Rect(
            bounds.Left + ContentPadding.Left,
            bounds.Top + ContentPadding.Top,
            bounds.Width - ContentPadding.Horizontal,
            bounds.Height - ContentPadding.Vertical);

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

        return new Point(positionX, positionY);
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
    }
    #endregion

    #region Overflow Menu
    private void ShowOverflowMenu()
    {
        // Create overflow menu popup
        var menuContent = new VStack();

        foreach (var item in _overflowItems)
        {
            // Create a menu item with icon and text
            var menuItem = new HStack()
                .SetSpacing(12)
                .SetHeight(48)
                .SetBackground(new SolidColorBackground(SKColors.White))
                .SetPadding(new Margin(16, 12));

            // Extract icon and text from the item if it's a button
            if (item is Button btn)
            {
                if (!string.IsNullOrEmpty(btn.Icon))
                {
                    menuItem.AddChild(new Image().SetSource(btn.Icon).SetWidth(24).SetHeight(24));
                }
                if (!string.IsNullOrEmpty(btn.Text))
                {
                    menuItem.AddChild(new Label().SetText(btn.Text));
                }

                // Copy command
                if (btn.Command != null)
                {
                    var menuButton = new Button()
                        .SetCommand(btn.Command)
                        .SetCommandParameter(btn.CommandParameter);
                    menuButton.SetWidth(menuItem.Width);
                    menuButton.SetHeight(menuItem.Height);
                }
            }

            menuContent.AddChild(menuItem);
        }

        // Show popup (simplified - would need proper popup implementation)
        // _overflowPopup = new UiPopupElement();
        // _overflowPopup.Show(menuContent);
    }
    #endregion

    #region Hit Testing
    public override UiElement? HitTest(Point point)
    {
        // Test overflow button first
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
            _titleLabel?.Dispose();
            CenterContent?.Dispose();
            _overflowButton?.Dispose();
            _overflowPopup?.Dispose();
        }
        base.Dispose(disposing);
    }
}

// Helper command for overflow button
internal class RelayCommand : ICommand
{
    private readonly Action _execute;

    public RelayCommand(Action execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _execute();
}
