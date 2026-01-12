using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A horizontal menu bar control that displays top-level menu items.
/// Clicking on an item opens a dropdown menu overlay.
/// </summary>
/// <example>
/// <code>
/// new Menu()
///     .AddItem(new MenuItem()
///         .SetText("File")
///         .AddItem(new MenuItem().SetText("New").SetShortcut("Ctrl+N").SetCommand(vm.NewCommand))
///         .AddItem(new MenuItem().SetText("Open").SetShortcut("Ctrl+O").SetCommand(vm.OpenCommand))
///         .AddSeparator()
///         .AddItem(new MenuItem().SetText("Exit").SetCommand(vm.ExitCommand)))
///     .AddItem(new MenuItem()
///         .SetText("Edit")
///         .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X"))
///         .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C"))
///         .AddItem(new MenuItem().SetText("Paste").SetShortcut("Ctrl+V")));
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Menu : UiLayoutElement, IInputControl, IHoverableControl
{
    private static readonly Color DefaultBackgroundColor = PlusUiDefaults.BackgroundPrimary;

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Menu;

    /// <summary>
    /// Returns the active menu overlay for debug inspection.
    /// </summary>
    protected override IEnumerable<UiElement> GetDebugChildrenCore() =>
        _activeOverlay != null ? Children.Append(_activeOverlay) : Children;

    public Menu()
    {
        SetBackground(DefaultBackgroundColor);
        UpdatePaints();
    }

    [MemberNotNull(nameof(_font), nameof(_textPaint), nameof(_disabledTextPaint))]
    private void UpdatePaints()
    {
        // Release old paints if exists (for property changes)
        if (_textPaint is not null && _font is not null && _disabledTextPaint is not null)
        {
            PaintRegistry.Release(_textPaint, _font);
            PaintRegistry.Release(_disabledTextPaint, _font);
        }

        // Get or create paints from registry
        (_textPaint, _font) = PaintRegistry.GetOrCreate(
            color: TextColor,
            size: TextSize
        );

        (_disabledTextPaint, _) = PaintRegistry.GetOrCreate(
            color: new SKColor(128, 128, 128),
            size: TextSize
        );
    }

    #region Constants
    private static readonly float ItemPaddingHorizontal = PlusUiDefaults.PaddingHorizontal;
    private static readonly float MenuHeight = PlusUiDefaults.ItemHeight;
    #endregion

    #region Colors
    internal Color HoverBackgroundColor
    {
        get => field;
        set { field = value; InvalidateMeasure(); }
    } = PlusUiDefaults.BackgroundSecondary;

    public Menu SetHoverBackgroundColor(Color color)
    {
        HoverBackgroundColor = color;
        return this;
    }

    public Menu BindHoverBackgroundColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HoverBackgroundColor = getter());
        return this;
    }

    internal Color ActiveBackgroundColor
    {
        get => field;
        set { field = value; InvalidateMeasure(); }
    } = PlusUiDefaults.BackgroundHover;

    public Menu SetActiveBackgroundColor(Color color)
    {
        ActiveBackgroundColor = color;
        return this;
    }

    public Menu BindActiveBackgroundColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ActiveBackgroundColor = getter());
        return this;
    }

    internal Color TextColor
    {
        get => field;
        set
        {
            field = value;
            UpdatePaints();
            InvalidateMeasure();
        }
    } = PlusUiDefaults.TextPrimary;

    public Menu SetTextColor(Color color)
    {
        TextColor = color;
        return this;
    }

    public Menu BindTextColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextColor = getter());
        return this;
    }

    internal float TextSize
    {
        get => field;
        set
        {
            field = value;
            UpdatePaints();
            InvalidateMeasure();
        }
    } = PlusUiDefaults.FontSize;

    public Menu SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public Menu BindTextSize(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextSize = getter());
        return this;
    }
    #endregion

    #region Items
    private readonly List<MenuItem> _items = [];

    public Menu AddItem(MenuItem item)
    {
        _items.Add(item);
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region State
    private int _hoveredIndex = -1;
    private int _openMenuIndex = -1;
    private int _hitIndex = -1;
    private MenuOverlay? _activeOverlay;
    private IOverlayService? _overlayService;
    private readonly List<SKRect> _itemRects = [];
    #endregion

    #region IHoverableControl
    public bool IsHovered { get; set; }
    #endregion

    private SKFont _font;
    private SKPaint _textPaint;
    private SKPaint _disabledTextPaint;

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        float totalWidth = 0;

        foreach (var item in _items.ToList())
        {
            var textWidth = _font.MeasureText(item.Text);
            totalWidth += textWidth + ItemPaddingHorizontal * 2;
        }

        return new Size(
            dontStretch ? totalWidth : availableSize.Width,
            MenuHeight);
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible) return;

        var menuRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + MenuHeight);

        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = (Background as SolidColorBackground)?.Color ?? DefaultBackgroundColor,
            IsAntialias = true
        };
        canvas.DrawRect(menuRect, bgPaint);

        // Calculate and draw menu items
        _itemRects.Clear();
        float currentX = Position.X + VisualOffset.X;
        _font.GetFontMetrics(out var metrics);
        var textY = Position.Y + VisualOffset.Y + MenuHeight / 2 - (metrics.Ascent + metrics.Descent) / 2;

        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            var textWidth = _font.MeasureText(item.Text);
            var itemWidth = textWidth + ItemPaddingHorizontal * 2;

            var itemRect = new SKRect(
                currentX,
                Position.Y + VisualOffset.Y,
                currentX + itemWidth,
                Position.Y + VisualOffset.Y + MenuHeight);
            _itemRects.Add(itemRect);

            // Draw item background (hover or active)
            if (i == _openMenuIndex)
            {
                using var activePaint = new SKPaint { Color = ActiveBackgroundColor, IsAntialias = true };
                canvas.DrawRect(itemRect, activePaint);
            }
            else if (i == _hoveredIndex)
            {
                using var hoverPaint = new SKPaint { Color = HoverBackgroundColor, IsAntialias = true };
                canvas.DrawRect(itemRect, hoverPaint);
            }

            // Draw text (use appropriate paint based on enabled state)
            var paint = item.IsEnabled ? _textPaint : _disabledTextPaint;
            canvas.DrawText(item.Text, currentX + ItemPaddingHorizontal, textY, SKTextAlign.Left, _font, paint);

            currentX += itemWidth;
        }
    }

    public override UiElement? HitTest(Point point)
    {
        if (!IsVisible) return null;

        // Check if point is within menu bar bounds
        var menuRect = new SKRect(
            Position.X,
            Position.Y,
            Position.X + ElementSize.Width,
            Position.Y + MenuHeight);

        if (point.X >= menuRect.Left && point.X <= menuRect.Right &&
            point.Y >= menuRect.Top && point.Y <= menuRect.Bottom)
        {
            // Find which item was hit
            for (int i = 0; i < _itemRects.Count; i++)
            {
                var rect = _itemRects[i];
                if (point.X >= rect.Left && point.X <= rect.Right)
                {
                    _hitIndex = i;
                    _hoveredIndex = i;

                    // If a menu is already open and we hover a different item, switch menus
                    if (_openMenuIndex >= 0 && _openMenuIndex != i)
                    {
                        CloseMenu();
                        OpenMenu(i);
                    }

                    return this;
                }
            }

            _hitIndex = -1;
            _hoveredIndex = -1;
            return this;
        }

        // Clear hover when mouse leaves menu bar (unless menu is open)
        if (_openMenuIndex < 0)
        {
            _hoveredIndex = -1;
        }
        _hitIndex = -1;
        return null;
    }

    public void InvokeCommand()
    {
        if (_hitIndex < 0 || _hitIndex >= _items.Count) return;

        var item = _items[_hitIndex];
        if (!item.IsEnabled) return;

        if (_openMenuIndex == _hitIndex)
        {
            // Clicking on already open menu - close it
            CloseMenu();
        }
        else
        {
            // Open the menu
            CloseMenu();
            OpenMenu(_hitIndex);
        }
    }

    private void OpenMenu(int index)
    {
        if (index < 0 || index >= _items.Count) return;

        var item = _items[index];
        if (!item.HasSubItems) return;

        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null) return;

        _openMenuIndex = index;

        // Calculate position below the menu item
        var rect = _itemRects[index];
        var anchorPosition = new Point(rect.Left, rect.Bottom);

        _activeOverlay = new MenuOverlay(
            item.Items,
            anchorPosition,
            openToRight: true,
            parentOverlay: null,
            onDismiss: () =>
            {
                _openMenuIndex = -1;
                _activeOverlay = null;
            });

        _overlayService.RegisterOverlay(_activeOverlay);
    }

    private void CloseMenu()
    {
        if (_activeOverlay != null)
        {
            _overlayService?.UnregisterOverlay(_activeOverlay);
            _activeOverlay = null;
        }
        _openMenuIndex = -1;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CloseMenu();

            // Release paints from registry
            if (_textPaint is not null && _font is not null && _disabledTextPaint is not null)
            {
                PaintRegistry.Release(_textPaint, _font);
                PaintRegistry.Release(_disabledTextPaint, _font);
            }
        }
        base.Dispose(disposing);
    }
}
