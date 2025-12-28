using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Multi-stop gradient background implementation supporting multiple color stops.
/// Immutable and thread-safe.
/// </summary>
public sealed class MultiStopGradient : IBackground
{
    /// <summary>
    /// The list of gradient stops defining the gradient.
    /// </summary>
    public IReadOnlyList<GradientStop> Stops { get; init; }

    /// <summary>
    /// The angle of the gradient in degrees (0-360).
    /// 0 = left to right, 90 = top to bottom, 180 = right to left, 270 = bottom to top.
    /// </summary>
    public float Angle { get; init; } = 0;

    /// <summary>
    /// Creates a multi-stop gradient with the specified angle and stops.
    /// </summary>
    /// <param name="angle">Gradient angle in degrees</param>
    /// <param name="stops">Variable number of gradient stops</param>
    public MultiStopGradient(float angle, params GradientStop[] stops)
    {
        Angle = angle;
        Stops = stops.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a multi-stop gradient with the specified angle and stops collection.
    /// </summary>
    /// <param name="angle">Gradient angle in degrees</param>
    /// <param name="stops">Collection of gradient stops</param>
    public MultiStopGradient(float angle, IEnumerable<GradientStop> stops)
    {
        Angle = angle;
        Stops = stops.ToList().AsReadOnly();
    }

    /// <summary>
    /// Parameterless constructor for object initializer syntax.
    /// </summary>
    public MultiStopGradient()
    {
        Stops = new List<GradientStop>().AsReadOnly();
    }

    /// <summary>
    /// Renders the multi-stop gradient background.
    /// </summary>
    public void Render(SKCanvas canvas, SKRect bounds, float cornerRadius)
    {
        if (Stops.Count < 2)
        {
            // Need at least 2 stops for a gradient - render transparent as fallback
            return;
        }

        // Calculate gradient start and end points based on angle
        var angleRad = Angle * (float)Math.PI / 180f;
        var centerX = bounds.MidX;
        var centerY = bounds.MidY;

        // Calculate the diagonal length to ensure gradient covers entire bounds
        var diagonal = (float)Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height) / 2f;

        var startX = centerX - (float)Math.Cos(angleRad) * diagonal;
        var startY = centerY - (float)Math.Sin(angleRad) * diagonal;
        var endX = centerX + (float)Math.Cos(angleRad) * diagonal;
        var endY = centerY + (float)Math.Sin(angleRad) * diagonal;

        var startPoint = new SKPoint(startX, startY);
        var endPoint = new SKPoint(endX, endY);

        // Extract colors and positions from stops
        var colors = Stops.Select(s => (SKColor)s.Color).ToArray();
        var positions = Stops.Select(s => s.Position).ToArray();

        using var shader = SKShader.CreateLinearGradient(
            startPoint,
            endPoint,
            colors,
            positions,
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
