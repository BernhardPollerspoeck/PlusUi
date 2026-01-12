using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Services;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Overlay element that renders the toolbar overflow menu above all page content.
/// </summary>
internal class ToolbarOverflowMenuOverlay(Toolbar toolbar) : UiElement, IDismissableOverlay, IDebugInspectable
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Menu;

    /// <summary>
    /// Returns the overflow menu content for debug inspection.
    /// </summary>
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() =>
        toolbar._overflowMenuContent != null ? [toolbar._overflowMenuContent] : [];

    private SKRect _menuRect;
    private bool _measured;
    private IPlatformService? _platformService;

    public override void Render(SKCanvas canvas)
    {
        if (toolbar._overflowMenuContent == null || !toolbar._isOverflowMenuOpen || toolbar._overflowButton == null)
            return;

        var menuContent = toolbar._overflowMenuContent;
        var button = toolbar._overflowButton;

        // Calculate menu width based on button width
        var menuWidth = Math.Max(150, button.ElementSize.Width * 4);

        // Always measure and arrange fresh to ensure children are properly laid out
        if (!_measured)
        {
            // Force re-measure by invalidating
            menuContent.InvalidateMeasure();
            menuContent.Measure(new Size(menuWidth, 400));
            _measured = true;
        }

        // Calculate button's visual position (accounting for scroll via Toolbar's VisualOffset)
        var buttonVisualX = button.Position.X + toolbar.VisualOffset.X;
        var buttonVisualY = button.Position.Y + toolbar.VisualOffset.Y;
        var buttonBottom = buttonVisualY + button.ElementSize.Height;
        var buttonTop = buttonVisualY;

        var menuHeight = menuContent.ElementSize.Height;
        var actualMenuWidth = menuContent.ElementSize.Width;

        // Get window size to check available space
        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowHeight = _platformService?.WindowSize.Height ?? 800f;

        // Check if menu fits below
        var spaceBelow = windowHeight - buttonBottom - 4;
        var opensUpward = spaceBelow < menuHeight && buttonTop > spaceBelow;

        // Position menu aligned to the right of the button
        var menuX = buttonVisualX + button.ElementSize.Width - actualMenuWidth;

        float menuY;
        if (opensUpward)
        {
            menuY = buttonTop - menuHeight - 4;
        }
        else
        {
            menuY = buttonBottom + 4;
        }

        // Always re-arrange to update positions
        menuContent.InvalidateArrange();
        menuContent.Arrange(new Rect(menuX, menuY, actualMenuWidth, menuHeight));

        _menuRect = new SKRect(
            menuX,
            menuY,
            menuX + actualMenuWidth,
            menuY + menuHeight);

        // Draw shadow
        using var shadowPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 80),
            ImageFilter = SKImageFilter.CreateDropShadow(2, 2, 4, 4, new SKColor(0, 0, 0, 80))
        };
        canvas.DrawRoundRect(_menuRect, 4, 4, shadowPaint);

        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = toolbar.OverflowMenuBackground,
            IsAntialias = true
        };
        canvas.DrawRoundRect(_menuRect, 4, 4, bgPaint);

        // Render menu content (VStack with buttons)
        menuContent.Render(canvas);
    }

    public override UiElement? HitTest(Point point)
    {
        if (toolbar._overflowMenuContent == null || !toolbar._isOverflowMenuOpen)
            return null;

        // Check if point is within menu bounds
        if (point.X >= _menuRect.Left && point.X <= _menuRect.Right &&
            point.Y >= _menuRect.Top && point.Y <= _menuRect.Bottom)
        {
            // Delegate to the menu content for button hit testing
            return toolbar._overflowMenuContent.HitTest(point) ?? this;
        }

        return null;
    }

    public void Dismiss()
    {
        toolbar.CloseOverflowMenu();
        _measured = false;
    }
}
