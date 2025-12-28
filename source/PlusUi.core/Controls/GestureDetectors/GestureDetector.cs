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
        Content.Measure(availableSize, dontStretch);
        return Content.ElementSize;
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        Content.Arrange(bounds);
        return new Point(bounds.X, bounds.Y);
    }

    public override void Render(SKCanvas canvas)
    {
        Content.Render(canvas);
    }
}
