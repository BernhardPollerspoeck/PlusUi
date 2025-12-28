using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Services;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Internal overlay element that renders the tooltip above all page content.
/// </summary>
internal class TooltipOverlay : UiElement
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Tooltip;

    private const float Padding = 8f;
    private const float Spacing = 4f;
    private const float DefaultFontSize = 14f;

    private readonly UiElement _targetElement;
    private readonly TooltipAttachment _attachment;
    private readonly IPlatformService? _platformService;

    private SKPaint? _textPaint;
    private SKFont? _font;
    private UiElement? _contentElement;
    private Size _measuredSize;

    public TooltipOverlay(UiElement targetElement, TooltipAttachment attachment)
    {
        _targetElement = targetElement;
        _attachment = attachment;
        _platformService = ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();

        Background = new SolidColorBackground(new Color(30, 30, 30, 230));
        CornerRadius = 4f;
        ShadowColor = new Color(0, 0, 0, 60);
        ShadowBlur = 8f;
        ShadowOffset = new Point(0, 2);

        InitializeContent();
        MeasureContent();
        CalculatePosition();
    }

    private void InitializeContent()
    {
        if (_attachment.Content is UiElement element)
        {
            _contentElement = element;
        }
        else
        {
            _textPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };
            _font = new SKFont(SKTypeface.Default)
            {
                Size = DefaultFontSize
            };
        }
    }

    private void MeasureContent()
    {
        if (_contentElement != null)
        {
            var windowSize = _platformService?.WindowSize ?? new Size(800, 600);
            var contentSize = _contentElement.Measure(new Size(windowSize.Width / 2, windowSize.Height / 2), true);
            _measuredSize = new Size(
                contentSize.Width + Padding * 2,
                contentSize.Height + Padding * 2);
        }
        else if (_attachment.Content is string text && _font != null)
        {
            var textWidth = _font.MeasureText(text);
            _font.GetFontMetrics(out var metrics);
            var textHeight = metrics.Descent - metrics.Ascent;
            _measuredSize = new Size(
                textWidth + Padding * 2,
                textHeight + Padding * 2);
        }
        else
        {
            _measuredSize = new Size(Padding * 2, Padding * 2);
        }

        ElementSize = _measuredSize;
    }

    private void CalculatePosition()
    {
        var windowSize = _platformService?.WindowSize ?? new Size(800, 600);

        var targetBounds = new Rect(
            _targetElement.Position.X + _targetElement.VisualOffset.X,
            _targetElement.Position.Y + _targetElement.VisualOffset.Y,
            _targetElement.ElementSize.Width,
            _targetElement.ElementSize.Height);

        var placement = ResolvePlacement(_attachment.Placement, targetBounds, _measuredSize, windowSize);
        var position = GetPositionForPlacement(placement, targetBounds, _measuredSize, windowSize);

        Position = position;

        // Arrange content element if present
        if (_contentElement != null)
        {
            var contentBounds = new Rect(
                Position.X + Padding,
                Position.Y + Padding,
                _measuredSize.Width - Padding * 2,
                _measuredSize.Height - Padding * 2);
            _contentElement.Arrange(contentBounds);
        }
    }

    private TooltipPlacement ResolvePlacement(TooltipPlacement placement, Rect targetBounds, Size tooltipSize, Size windowSize)
    {
        if (placement != TooltipPlacement.Auto)
        {
            return placement;
        }

        // Auto: Try Top → Bottom → Right → Left
        if (targetBounds.Y - tooltipSize.Height - Spacing >= 0)
        {
            return TooltipPlacement.Top;
        }
        if (targetBounds.Y + targetBounds.Height + tooltipSize.Height + Spacing <= windowSize.Height)
        {
            return TooltipPlacement.Bottom;
        }
        if (targetBounds.X + targetBounds.Width + tooltipSize.Width + Spacing <= windowSize.Width)
        {
            return TooltipPlacement.Right;
        }
        return TooltipPlacement.Left;
    }

    private Point GetPositionForPlacement(TooltipPlacement placement, Rect targetBounds, Size tooltipSize, Size windowSize)
    {
        float x, y;

        switch (placement)
        {
            case TooltipPlacement.Top:
                x = targetBounds.X + (targetBounds.Width - tooltipSize.Width) / 2;
                y = targetBounds.Y - tooltipSize.Height - Spacing;
                break;

            case TooltipPlacement.Bottom:
                x = targetBounds.X + (targetBounds.Width - tooltipSize.Width) / 2;
                y = targetBounds.Y + targetBounds.Height + Spacing;
                break;

            case TooltipPlacement.Left:
                x = targetBounds.X - tooltipSize.Width - Spacing;
                y = targetBounds.Y + (targetBounds.Height - tooltipSize.Height) / 2;
                break;

            case TooltipPlacement.Right:
            default:
                x = targetBounds.X + targetBounds.Width + Spacing;
                y = targetBounds.Y + (targetBounds.Height - tooltipSize.Height) / 2;
                break;
        }

        // Clamp to screen bounds
        x = Math.Max(4, Math.Min(x, windowSize.Width - tooltipSize.Width - 4));
        y = Math.Max(4, Math.Min(y, windowSize.Height - tooltipSize.Height - 4));

        return new Point(x, y);
    }

    public override void Render(SKCanvas canvas)
    {
        // Render background and shadow
        base.Render(canvas);

        if (!IsVisible)
        {
            return;
        }

        // Render content
        if (_contentElement != null)
        {
            _contentElement.Render(canvas);
        }
        else if (_attachment.Content is string text && _textPaint != null && _font != null)
        {
            _font.GetFontMetrics(out var metrics);
            var textY = Position.Y + Padding + (-metrics.Ascent);
            canvas.DrawText(
                text,
                Position.X + Padding,
                textY,
                SKTextAlign.Left,
                _font,
                _textPaint);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        return _measuredSize;
    }

    public override UiElement? HitTest(Point point)
    {
        // Tooltips are non-interactive
        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _textPaint?.Dispose();
            _font?.Dispose();
            // Note: _contentElement is not disposed here because we don't own it.
            // It was provided by the user through TooltipAttachment and may be reused.
        }
        base.Dispose(disposing);
    }
}
