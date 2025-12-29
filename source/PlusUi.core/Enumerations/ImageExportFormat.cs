namespace PlusUi.core;

/// <summary>
/// Specifies the image format for export operations.
/// </summary>
public enum ImageExportFormat
{
    /// <summary>
    /// PNG format (lossless, supports transparency).
    /// </summary>
    Png,

    /// <summary>
    /// JPEG format (lossy, smaller file size).
    /// </summary>
    Jpeg,

    /// <summary>
    /// WebP format (modern format with good compression).
    /// </summary>
    Webp
}
