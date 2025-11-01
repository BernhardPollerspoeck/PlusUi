using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Solid color background implementation.
/// Immutable and thread-safe.
/// </summary>
public sealed class SolidColorBackground : IBackground
{
    /// <summary>
    /// The color to fill the background with.
    /// </summary>
    public SKColor Color { get; init; }

    /// <summary>
    /// Creates a solid color background with the specified color.
    /// </summary>
    /// <param name="color">The color to use</param>
    public SolidColorBackground(SKColor color)
    {
        Color = color;
    }

    /// <summary>
    /// Parameterless constructor for object initializer syntax.
    /// Color must be set via object initializer.
    /// </summary>
    public SolidColorBackground()
    {
    }

    /// <summary>
    /// Implicit conversion from SKColor for convenience.
    /// </summary>
    /// <param name="color">The color to convert</param>
    public static implicit operator SolidColorBackground(SKColor color)
    {
        return new SolidColorBackground(color);
    }

    /// <summary>
    /// Renders the solid color background.
    /// </summary>
    public void Render(SKCanvas canvas, SKRect bounds, float cornerRadius)
    {
        using var paint = new SKPaint
        {
            Color = Color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        if (cornerRadius > 0)
        {
            canvas.DrawRoundRect(bounds, cornerRadius, cornerRadius, paint);
        }
        else
        {
            canvas.DrawRect(bounds, paint);
        }
    }
}
