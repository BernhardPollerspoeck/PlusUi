using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Service for exporting UI elements as images.
/// </summary>
internal class ImageExportService : IImageExportService
{
    public void ExportToFile(UiElement element, string filePath, ImageExportFormat format = ImageExportFormat.Png, int quality = 100)
    {
        using var stream = File.Create(filePath);
        ExportToStream(element, stream, format, quality);
    }

    public void ExportToStream(UiElement element, Stream stream, ImageExportFormat format = ImageExportFormat.Png, int quality = 100)
    {
        using var bitmap = RenderToBitmap(element);
        using var data = bitmap.Encode(ToSkiaFormat(format), quality);
        data.SaveTo(stream);
    }

    public byte[] ExportToBytes(UiElement element, ImageExportFormat format = ImageExportFormat.Png, int quality = 100)
    {
        using var stream = new MemoryStream();
        ExportToStream(element, stream, format, quality);
        return stream.ToArray();
    }

    private static SKBitmap RenderToBitmap(UiElement element)
    {
        var width = (int)Math.Ceiling(element.ElementSize.Width);
        var height = (int)Math.Ceiling(element.ElementSize.Height);

        if (width <= 0 || height <= 0)
        {
            throw new InvalidOperationException(
                $"Element has invalid size ({width}x{height}). Ensure the element has been measured and arranged before exporting.");
        }

        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.Transparent);

        // Translate canvas so element renders at (0,0) regardless of its position
        canvas.Translate(-element.Position.X, -element.Position.Y);
        element.Render(canvas);
        canvas.Flush();

        return bitmap;
    }

    private static SKEncodedImageFormat ToSkiaFormat(ImageExportFormat format) => format switch
    {
        ImageExportFormat.Png => SKEncodedImageFormat.Png,
        ImageExportFormat.Jpeg => SKEncodedImageFormat.Jpeg,
        ImageExportFormat.Webp => SKEncodedImageFormat.Webp,
        _ => SKEncodedImageFormat.Png
    };
}
