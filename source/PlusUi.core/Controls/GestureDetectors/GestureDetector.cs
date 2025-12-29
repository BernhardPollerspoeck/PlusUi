using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Abstract base class for gesture detector controls.
/// Wraps content and detects specific gestures.
/// </summary>
/// <typeparam name="T">The concrete gesture detector type for method chaining.</typeparam>
public abstract class GestureDetector<T> : UiLayoutElement where T : GestureDetector<T>
{
    protected UiElement Content { get; }

    protected GestureDetector(UiElement content)
    {
        Content = content;
        content.Parent = this;
        Children.Add(content);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Content gets available size minus our margin
        var contentAvailable = new Size(
            Math.Max(0, availableSize.Width - Margin.Horizontal),
            Math.Max(0, availableSize.Height - Margin.Vertical));
        Content.Measure(contentAvailable, dontStretch);
        return new Size(
            Content.ElementSize.Width + Margin.Horizontal,
            Content.ElementSize.Height + Margin.Vertical);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        // Let base handle our positioning (respects alignment and margin)
        var position = base.ArrangeInternal(bounds);

        // Content fills our entire element area (no additional margin between us and content)
        var contentBounds = new Rect(
            position.X,
            position.Y,
            ElementSize.Width,
            ElementSize.Height);
        Content.Arrange(contentBounds);

        return position;
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
    }
}
