using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Radial gradient background implementation.
/// Immutable and thread-safe.
/// </summary>
public sealed class RadialGradient : IBackground
{
    /// <summary>
    /// The color at the center of the gradient.
    /// </summary>
    public SKColor CenterColor { get; init; }

    /// <summary>
    /// The color at the edges of the gradient.
    /// </summary>
    public SKColor EdgeColor { get; init; }

    /// <summary>
    /// The center point of the gradient in relative coordinates (0-1).
    /// Default is (0.5, 0.5) which is the center of the bounds.
    /// </summary>
    public Point Center { get; init; } = new Point(0.5f, 0.5f);

    /// <summary>
    /// Creates a radial gradient with the specified colors and center.
    /// </summary>
    /// <param name="centerColor">Color at the center</param>
    /// <param name="edgeColor">Color at the edges</param>
    /// <param name="center">Center point in relative coordinates (optional, default: center)</param>
    public RadialGradient(SKColor centerColor, SKColor edgeColor, Point? center = null)
    {
        CenterColor = centerColor;
        EdgeColor = edgeColor;
        Center = center ?? new Point(0.5f, 0.5f);
    }

    /// <summary>
    /// Parameterless constructor for object initializer syntax.
    /// </summary>
    public RadialGradient()
    {
    }

    /// <summary>
    /// Renders the radial gradient background.
    /// </summary>
    public void Render(SKCanvas canvas, SKRect bounds, float cornerRadius)
    {
        // Calculate absolute center point from relative coordinates
        var centerX = bounds.Left + bounds.Width * Center.X;
        var centerY = bounds.Top + bounds.Height * Center.Y;
        var centerPoint = new SKPoint(centerX, centerY);

        // Calculate radius to cover entire bounds from center point
        var radius = Math.Max(
            Math.Max(
                (float)Math.Sqrt(Math.Pow(centerX - bounds.Left, 2) + Math.Pow(centerY - bounds.Top, 2)),
                (float)Math.Sqrt(Math.Pow(centerX - bounds.Right, 2) + Math.Pow(centerY - bounds.Top, 2))),
            Math.Max(
                (float)Math.Sqrt(Math.Pow(centerX - bounds.Left, 2) + Math.Pow(centerY - bounds.Bottom, 2)),
                (float)Math.Sqrt(Math.Pow(centerX - bounds.Right, 2) + Math.Pow(centerY - bounds.Bottom, 2))));

        using var shader = SKShader.CreateRadialGradient(
            centerPoint,
            radius,
            new[] { CenterColor, EdgeColor },
            new[] { 0f, 1f },
            SKShaderTileMode.Clamp);

        using var paint = new SKPaint
        {
            Shader = shader,
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
