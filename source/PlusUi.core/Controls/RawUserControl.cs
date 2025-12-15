using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Creates custom controls with direct SkiaSharp rendering.
/// Use this for custom drawing, charts, graphs, or any UI that requires direct canvas access.
/// </summary>
/// <remarks>
/// RawUserControl gives you direct access to the SkiaSharp bitmap for custom rendering.
/// Use this for performance-critical rendering or when you need full control over drawing.
/// </remarks>
/// <example>
/// <code>
/// public class CircleControl : RawUserControl
/// {
///     public override Size Size => new(100, 100);
///
///     public override void RenderControl(SKBitmap bitmap)
///     {
///         using var canvas = new SKCanvas(bitmap);
///         using var paint = new SKPaint
///         {
///             Color = SKColors.Blue,
///             IsAntialias = true
///         };
///         canvas.DrawCircle(50, 50, 40, paint);
///     }
/// }
/// </code>
/// </example>
[GenerateGenericWrapper]
public abstract class RawUserControl : UiElement
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

    public abstract Size Size { get; }
    public abstract void RenderControl(SKBitmap bitmap);

    public override void BuildContent()
    {
        InvalidateMeasure();
    }
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        var bitmap = new SKBitmap((int)Size.Width, (int)Size.Height);
        RenderControl(bitmap);
        canvas.DrawBitmap(bitmap, Position.X, Position.Y);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        return Size;
    }
}