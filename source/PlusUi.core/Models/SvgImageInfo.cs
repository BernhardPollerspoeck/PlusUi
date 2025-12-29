using SkiaSharp;

namespace PlusUi.core.Models;

/// <summary>
/// Contains information about an SVG image that can be rendered at any size.
/// </summary>
public class SvgImageInfo : IDisposable
{
    /// <summary>
    /// The SKPicture that can be rendered at any size.
    /// </summary>
    public SKPicture Picture { get; }

    /// <summary>
    /// Original width of the SVG.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Original height of the SVG.
    /// </summary>
    public float Height { get; }

    public SvgImageInfo(SKPicture picture, float width, float height)
    {
        Picture = picture;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Renders the SVG to an SKImage at the specified size with optional tint color.
    /// </summary>
    public SKImage? RenderToImage(float width, float height, Color? tintColor = null)
    {
        if (width <= 0 || height <= 0)
            return null;

        var info = new SKImageInfo((int)Math.Ceiling(width), (int)Math.Ceiling(height));
        using var surface = SKSurface.Create(info);
        if (surface == null)
            return null;

        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        // Scale to fit the target size
        var scaleX = width / Width;
        var scaleY = height / Height;
        canvas.Scale(scaleX, scaleY);

        if (tintColor.HasValue)
        {
            // Apply tint color using color filter
            using var paint = new SKPaint
            {
                ColorFilter = SKColorFilter.CreateBlendMode(tintColor.Value, SKBlendMode.SrcIn)
            };
            canvas.DrawPicture(Picture, paint);
        }
        else
        {
            canvas.DrawPicture(Picture);
        }

        return surface.Snapshot();
    }

    public void Dispose()
    {
        Picture?.Dispose();
        GC.SuppressFinalize(this);
    }
}
