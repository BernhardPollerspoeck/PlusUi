namespace PlusUi.core;

/// <summary>
/// Service for exporting UI elements as images.
/// </summary>
public interface IImageExportService
{
    /// <summary>
    /// Exports a UI element to a file.
    /// </summary>
    /// <param name="element">The UI element to export.</param>
    /// <param name="filePath">The path where the image will be saved.</param>
    /// <param name="format">The image format to use.</param>
    /// <param name="quality">The quality level (1-100). Only applies to lossy formats like JPEG and WebP.</param>
    void ExportToFile(UiElement element, string filePath, ImageExportFormat format = ImageExportFormat.Png, int quality = 100);

    /// <summary>
    /// Exports a UI element to a stream.
    /// </summary>
    /// <param name="element">The UI element to export.</param>
    /// <param name="stream">The stream to write the image data to.</param>
    /// <param name="format">The image format to use.</param>
    /// <param name="quality">The quality level (1-100). Only applies to lossy formats like JPEG and WebP.</param>
    void ExportToStream(UiElement element, Stream stream, ImageExportFormat format = ImageExportFormat.Png, int quality = 100);

    /// <summary>
    /// Exports a UI element to a byte array.
    /// </summary>
    /// <param name="element">The UI element to export.</param>
    /// <param name="format">The image format to use.</param>
    /// <param name="quality">The quality level (1-100). Only applies to lossy formats like JPEG and WebP.</param>
    /// <returns>The image data as a byte array.</returns>
    byte[] ExportToBytes(UiElement element, ImageExportFormat format = ImageExportFormat.Png, int quality = 100);
}
