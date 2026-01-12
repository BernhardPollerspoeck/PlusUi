using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Services;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Overlay element that renders a menu dropdown with items.
/// Supports nested submenus, keyboard navigation, and hover tracking.
/// </summary>
internal class MenuOverlay : UiElement, IInputControl, IDismissableOverlay, IKeyboardInputHandler, IDebugInspectable
{
    private static readonly Color DefaultBackgroundColor = PlusUiDefaults.BackgroundPrimary;

    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Menu;

    /// <summary>
    /// Returns the active submenu for debug inspection.
    /// </summary>
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() =>
        _activeSubmenu != null ? [_activeSubmenu] : [];

    #region Constants
    private static readonly float ItemHeight = PlusUiDefaults.ItemHeight;
    private const float SeparatorHeight = 9f;
    private static readonly float CheckmarkWidth = PlusUiDefaults.CheckboxSize;
    private static readonly float IconAreaWidth = PlusUiDefaults.IconSizeLarge;
    private const float ShortcutMinWidth = 60f;
    private static readonly float SubmenuArrowWidth = PlusUiDefaults.IconSize;
    private static readonly float HorizontalPadding = PlusUiDefaults.Spacing;
    private const float MinMenuWidth = 150f;
    private static readonly float MenuCornerRadius = PlusUiDefaults.CornerRadius;
    private const float MenuShadowOffset = 2f;
    private const float MenuShadowBlur = 4f;
    #endregion

    #region Colors
    internal Color HoverBackgroundColor { get; set; } = PlusUiDefaults.BackgroundHover;
    internal Color TextColor { get; set; } = PlusUiDefaults.TextPrimary;
    internal Color DisabledTextColor { get; set; } = PlusUiDefaults.TextPlaceholder;
    internal Color ShortcutColor { get; set; } = PlusUiDefaults.TextSecondary;
    internal Color SeparatorColor { get; set; } = PlusUiDefaults.BorderColor;
    internal Color CheckmarkColor { get; set; } = PlusUiDefaults.AccentPrimary;
    #endregion

    private readonly List<object> _items;
    private readonly Point _anchorPosition;
    private readonly bool _openToRight;
    private readonly MenuOverlay? _parentOverlay;
    private readonly Action? _onDismiss;

    private SKRect _menuRect;
    private int _hoveredIndex = -1;
    private int _hitIndex = -1;
    private MenuOverlay? _activeSubmenu;
    private IOverlayService? _overlayService;
    private IPlatformService? _platformService;
    private float _calculatedWidth;
    private float _calculatedHeight;
    private bool _measured;

    private SKFont _font;
    private SKPaint _textPaint;

    /// <summary>
    /// Creates a new MenuOverlay.
    /// </summary>
    /// <param name="items">The menu items to display.</param>
    /// <param name="anchorPosition">The position to anchor the menu (top-left corner).</param>
    /// <param name="openToRight">Whether to prefer opening submenus to the right.</param>
    /// <param name="parentOverlay">Parent overlay for submenu chain management.</param>
    /// <param name="onDismiss">Callback when menu is dismissed.</param>
    public MenuOverlay(
        List<object> items,
        Point anchorPosition,
        bool openToRight = true,
        MenuOverlay? parentOverlay = null,
        Action? onDismiss = null)
    {
        _items = items;
        _anchorPosition = anchorPosition;
        _openToRight = openToRight;
        _parentOverlay = parentOverlay;
        _onDismiss = onDismiss;
        SetBackground(DefaultBackgroundColor);
        UpdatePaint();
    }

    [MemberNotNull(nameof(_font), nameof(_textPaint))]
    private void UpdatePaint()
    {
        // Release old paint if exists (for property changes)
        if (_textPaint is not null && _font is not null)
        {
            PaintRegistry.Release(_textPaint, _font);
        }

        // Get or create paint from registry
        (_textPaint, _font) = PaintRegistry.GetOrCreate(
            color: TextColor,
            size: PlusUiDefaults.FontSize
        );
    }

    private void EnsureMeasured()
    {
        if (_measured) return;

        // Calculate menu dimensions
        float maxTextWidth = 0;
        float maxShortcutWidth = 0;
        bool hasIcons = false;
        bool hasSubmenus = false;
        bool hasCheckmarks = false;

        foreach (var item in _items.ToList())
        {
            if (item is MenuItem menuItem)
            {
                var textWidth = _font.MeasureText(menuItem.Text);
                maxTextWidth = Math.Max(maxTextWidth, textWidth);

                if (!string.IsNullOrEmpty(menuItem.Shortcut))
                {
                    var shortcutWidth = _font.MeasureText(menuItem.Shortcut);
                    maxShortcutWidth = Math.Max(maxShortcutWidth, shortcutWidth);
                }

                if (!string.IsNullOrEmpty(menuItem.Icon)) hasIcons = true;
                if (menuItem.HasSubItems) hasSubmenus = true;
                if (menuItem.IsChecked) hasCheckmarks = true;
            }
        }

        // Calculate total width
        _calculatedWidth = HorizontalPadding; // Left padding
        if (hasCheckmarks) _calculatedWidth += CheckmarkWidth;
        if (hasIcons) _calculatedWidth += IconAreaWidth;
        _calculatedWidth += maxTextWidth;
        _calculatedWidth += HorizontalPadding * 2; // Spacing
        if (maxShortcutWidth > 0) _calculatedWidth += Math.Max(maxShortcutWidth, ShortcutMinWidth);
        if (hasSubmenus) _calculatedWidth += SubmenuArrowWidth;
        _calculatedWidth += HorizontalPadding; // Right padding
        _calculatedWidth = Math.Max(_calculatedWidth, MinMenuWidth);

        // Calculate total height
        _calculatedHeight = 0;
        foreach (var item in _items.ToList())
        {
            _calculatedHeight += item is MenuSeparator ? SeparatorHeight : ItemHeight;
        }

        _measured = true;
    }

    public override void Render(SKCanvas canvas)
    {
        EnsureMeasured();

        // Get window bounds for positioning
        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowWidth = _platformService?.WindowSize.Width ?? 800f;
        var windowHeight = _platformService?.WindowSize.Height ?? 600f;

        // Calculate position with boundary checking
        var menuX = _anchorPosition.X;
        var menuY = _anchorPosition.Y;

        // Check horizontal bounds
        if (menuX + _calculatedWidth > windowWidth - 4)
        {
            menuX = windowWidth - _calculatedWidth - 4;
        }
        if (menuX < 4) menuX = 4;

        // Check vertical bounds - open upward if not enough space below
        if (menuY + _calculatedHeight > windowHeight - 4)
        {
            menuY = windowHeight - _calculatedHeight - 4;
        }
        if (menuY < 4) menuY = 4;

        _menuRect = new SKRect(menuX, menuY, menuX + _calculatedWidth, menuY + _calculatedHeight);

        // Draw shadow
        using var shadowPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 80),
            ImageFilter = SKImageFilter.CreateDropShadow(MenuShadowOffset, MenuShadowOffset, MenuShadowBlur, MenuShadowBlur, new SKColor(0, 0, 0, 80))
        };
        canvas.DrawRoundRect(_menuRect, MenuCornerRadius, MenuCornerRadius, shadowPaint);

        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = (Background as SolidColorBackground)?.Color ?? DefaultBackgroundColor,
            IsAntialias = true
        };
        canvas.DrawRoundRect(_menuRect, MenuCornerRadius, MenuCornerRadius, bgPaint);

        // Draw border
        using var borderPaint = new SKPaint
        {
            Color = SeparatorColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true
        };
        canvas.DrawRoundRect(_menuRect, MenuCornerRadius, MenuCornerRadius, borderPaint);

        // Draw items
        float currentY = menuY;
        int index = 0;

        foreach (var item in _items.ToList())
        {
            if (item is MenuSeparator)
            {
                RenderSeparator(canvas, menuX, currentY);
                currentY += SeparatorHeight;
            }
            else if (item is MenuItem menuItem)
            {
                var isHovered = index == _hoveredIndex;
                RenderMenuItem(canvas, menuItem, menuX, currentY, isHovered);
                currentY += ItemHeight;
            }
            index++;
        }
    }

    private void RenderSeparator(SKCanvas canvas, float x, float y)
    {
        using var paint = new SKPaint
        {
            Color = SeparatorColor,
            StrokeWidth = 1,
            IsAntialias = true
        };

        var lineY = y + SeparatorHeight / 2;
        canvas.DrawLine(x + HorizontalPadding, lineY, x + _calculatedWidth - HorizontalPadding, lineY, paint);
    }

    private void RenderMenuItem(SKCanvas canvas, MenuItem item, float x, float y, bool isHovered)
    {
        var itemRect = new SKRect(x, y, x + _calculatedWidth, y + ItemHeight);

        // Draw hover background
        if (isHovered && item.IsEnabled)
        {
            using var hoverPaint = new SKPaint
            {
                Color = HoverBackgroundColor,
                IsAntialias = true
            };
            canvas.DrawRect(itemRect, hoverPaint);
        }

        var textColor = item.IsEnabled ? TextColor : DisabledTextColor;

        float currentX = x + HorizontalPadding;
        _font.GetFontMetrics(out var metrics);
        var textY = y + ItemHeight / 2 - (metrics.Ascent + metrics.Descent) / 2;

        // Create temporary paints for different colors instead of mutating _textPaint
        using var textPaint = new SKPaint { Color = textColor, IsAntialias = true };

        // Draw checkmark if checked
        if (item.IsChecked)
        {
            var checkColor = item.IsEnabled ? CheckmarkColor : DisabledTextColor;
            using var checkPaint = new SKPaint { Color = checkColor, IsAntialias = true };
            canvas.DrawText("\u2713", currentX, textY, SKTextAlign.Left, _font, checkPaint);
        }
        currentX += CheckmarkWidth;

        // Draw icon area (skip for now, would need image loading)
        currentX += IconAreaWidth;

        // Draw text
        canvas.DrawText(item.Text, currentX, textY, SKTextAlign.Left, _font, textPaint);

        // Draw shortcut (right-aligned)
        if (!string.IsNullOrEmpty(item.Shortcut))
        {
            var shortcutColor = item.IsEnabled ? ShortcutColor : DisabledTextColor;
            using var shortcutPaint = new SKPaint { Color = shortcutColor, IsAntialias = true };
            var shortcutX = x + _calculatedWidth - HorizontalPadding - (item.HasSubItems ? SubmenuArrowWidth : 0);
            canvas.DrawText(item.Shortcut, shortcutX, textY, SKTextAlign.Right, _font, shortcutPaint);
        }

        // Draw submenu arrow
        if (item.HasSubItems)
        {
            var arrowX = x + _calculatedWidth - HorizontalPadding - 8;
            canvas.DrawText("\u25B6", arrowX, textY, SKTextAlign.Center, _font, textPaint);
        }
    }

    public override UiElement? HitTest(Point point)
    {
        // First check if we hit an active submenu
        if (_activeSubmenu != null)
        {
            var submenuHit = _activeSubmenu.HitTest(point);
            if (submenuHit != null) return submenuHit;
        }

        // Check if point is within our menu bounds
        if (point.X >= _menuRect.Left && point.X <= _menuRect.Right &&
            point.Y >= _menuRect.Top && point.Y <= _menuRect.Bottom)
        {
            // Calculate which item was hit
            var relativeY = point.Y - _menuRect.Top;
            float currentY = 0;
            int index = 0;

            foreach (var item in _items.ToList())
            {
                float itemHeight = item is MenuSeparator ? SeparatorHeight : ItemHeight;

                if (relativeY >= currentY && relativeY < currentY + itemHeight)
                {
                    if (item is MenuItem menuItem && menuItem.IsEnabled)
                    {
                        _hitIndex = index;
                        UpdateHoverAndSubmenu(index);
                        return this;
                    }
                    else
                    {
                        _hitIndex = -1;
                        _hoveredIndex = -1;
                        return this;
                    }
                }

                currentY += itemHeight;
                if (item is not MenuSeparator) index++;
                else index++;
            }

            _hitIndex = -1;
            return this;
        }

        // Clear hover when mouse leaves
        _hoveredIndex = -1;
        _hitIndex = -1;
        return null;
    }

    private void UpdateHoverAndSubmenu(int index)
    {
        if (_hoveredIndex == index) return;

        _hoveredIndex = index;

        // Close any existing submenu if hovering a different item
        CloseSubmenu();

        // Open submenu if the hovered item has sub-items
        if (index >= 0 && index < _items.Count)
        {
            int actualIndex = 0;
            foreach (var item in _items.ToList())
            {
                if (actualIndex == index && item is MenuItem menuItem && menuItem.HasSubItems)
                {
                    OpenSubmenu(menuItem, index);
                    break;
                }
                actualIndex++;
            }
        }
    }

    private void OpenSubmenu(MenuItem menuItem, int itemIndex)
    {
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null) return;

        // Calculate submenu position (to the right of this item)
        float itemY = _menuRect.Top;
        int idx = 0;
        foreach (var item in _items.ToList())
        {
            if (idx == itemIndex) break;
            itemY += item is MenuSeparator ? SeparatorHeight : ItemHeight;
            idx++;
        }

        var submenuX = _menuRect.Right - 4; // Slight overlap
        var submenuAnchor = new Point(submenuX, itemY);

        _activeSubmenu = new MenuOverlay(
            menuItem.Items,
            submenuAnchor,
            _openToRight,
            this,
            null);

        _overlayService.RegisterOverlay(_activeSubmenu);
    }

    private void CloseSubmenu()
    {
        if (_activeSubmenu != null)
        {
            _activeSubmenu.CloseSubmenu(); // Close nested submenus first
            _overlayService?.UnregisterOverlay(_activeSubmenu);
            _activeSubmenu = null;
        }
    }

    public void InvokeCommand()
    {
        if (_hitIndex < 0) return;

        int actualIndex = 0;
        foreach (var item in _items.ToList())
        {
            if (actualIndex == _hitIndex)
            {
                if (item is MenuItem menuItem)
                {
                    if (menuItem.HasSubItems)
                    {
                        // Don't dismiss, just open submenu
                        OpenSubmenu(menuItem, _hitIndex);
                    }
                    else
                    {
                        // Execute command and dismiss entire menu chain
                        menuItem.Execute();
                        DismissAll();
                    }
                }
                break;
            }
            actualIndex++;
        }
    }

    public bool HandleKeyboardInput(PlusKey key)
    {
        switch (key)
        {
            case PlusKey.ArrowUp:
                NavigateUp();
                return true;

            case PlusKey.ArrowDown:
                NavigateDown();
                return true;

            case PlusKey.ArrowRight:
                if (_hoveredIndex >= 0)
                {
                    int idx = 0;
                    foreach (var item in _items.ToList())
                    {
                        if (idx == _hoveredIndex && item is MenuItem menuItem && menuItem.HasSubItems)
                        {
                            OpenSubmenu(menuItem, _hoveredIndex);
                            _activeSubmenu?._hoveredIndex = 0; // Select first item in submenu
                            return true;
                        }
                        idx++;
                    }
                }
                return false;

            case PlusKey.ArrowLeft:
                if (_parentOverlay != null)
                {
                    // Close this submenu and return to parent
                    _parentOverlay.CloseSubmenu();
                    return true;
                }
                return false;

            case PlusKey.Enter:
            case PlusKey.Space:
                if (_hoveredIndex >= 0)
                {
                    _hitIndex = _hoveredIndex;
                    InvokeCommand();
                    return true;
                }
                return false;

            case PlusKey.Escape:
                DismissAll();
                return true;

            default:
                return false;
        }
    }

    private void NavigateUp()
    {
        if (_items.Count == 0) return;

        int newIndex = _hoveredIndex - 1;
        while (newIndex >= 0)
        {
            if (_items[newIndex] is MenuItem menuItem && menuItem.IsEnabled)
            {
                _hoveredIndex = newIndex;
                CloseSubmenu();
                return;
            }
            newIndex--;
        }

        // Wrap to bottom
        newIndex = _items.Count - 1;
        while (newIndex > _hoveredIndex)
        {
            if (_items[newIndex] is MenuItem menuItem && menuItem.IsEnabled)
            {
                _hoveredIndex = newIndex;
                CloseSubmenu();
                return;
            }
            newIndex--;
        }
    }

    private void NavigateDown()
    {
        if (_items.Count == 0) return;

        int newIndex = _hoveredIndex + 1;
        while (newIndex < _items.Count)
        {
            if (_items[newIndex] is MenuItem menuItem && menuItem.IsEnabled)
            {
                _hoveredIndex = newIndex;
                CloseSubmenu();
                return;
            }
            newIndex++;
        }

        // Wrap to top
        newIndex = 0;
        while (newIndex < _hoveredIndex)
        {
            if (_items[newIndex] is MenuItem menuItem && menuItem.IsEnabled)
            {
                _hoveredIndex = newIndex;
                CloseSubmenu();
                return;
            }
            newIndex++;
        }
    }

    public void Dismiss()
    {
        CloseSubmenu();
        // Unregister self from overlay service
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        _overlayService?.UnregisterOverlay(this);
        _onDismiss?.Invoke();
    }

    private void DismissAll()
    {
        // Walk up the parent chain to dismiss from root
        if (_parentOverlay != null)
        {
            _parentOverlay.DismissAll();
        }
        else
        {
            // This is the root - dismiss through overlay service
            _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
            _overlayService?.UnregisterOverlay(this);
            Dismiss();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Release paint from registry
            if (_textPaint is not null && _font is not null)
            {
                PaintRegistry.Release(_textPaint, _font);
            }
        }
        base.Dispose(disposing);
    }
}
