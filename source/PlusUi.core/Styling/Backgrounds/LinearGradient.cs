using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Linear gradient background implementation.
/// Immutable and thread-safe.
/// </summary>
public sealed class LinearGradient : IBackground
{
    /// <summary>
    /// The starting color of the gradient.
    /// </summary>
    public Color StartColor { get; init; }

    /// <summary>
    /// The ending color of the gradient.
    /// </summary>
    public Color EndColor { get; init; }

    /// <summary>
    /// The angle of the gradient in degrees (0-360).
    /// 0 = left to right, 90 = top to bottom, 180 = right to left, 270 = bottom to top.
    /// </summary>
    public float Angle { get; init; } = 0;

    /// <summary>
    /// Creates a linear gradient with the specified colors and angle.
    /// </summary>
    /// <param name="startColor">Starting color</param>
    /// <param name="endColor">Ending color</param>
    /// <param name="angle">Gradient angle in degrees (default: 0)</param>
    public LinearGradient(Color startColor, Color endColor, float angle = 0)
    {
        StartColor = startColor;
        EndColor = endColor;
        Angle = angle;
    }

    /// <summary>
    /// Parameterless constructor for object initializer syntax.
    /// </summary>
    public LinearGradient()
    {
    }

    /// <summary>
    /// Renders the linear gradient background.
    /// </summary>
    public void Render(SKCanvas canvas, SKRect bounds, float cornerRadius)
    {
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

        using var shader = SKShader.CreateLinearGradient(
            startPoint,
            endPoint,
            [(SKColor)StartColor, (SKColor)EndColor],
            [0f, 1f],
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
