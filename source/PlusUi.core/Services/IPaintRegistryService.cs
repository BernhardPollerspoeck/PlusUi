using SkiaSharp;

namespace PlusUi.core.Services;

/// <summary>
/// Service for centralized SKPaint/SKFont management with caching and reference counting.
/// </summary>
public interface IPaintRegistryService
{
    /// <summary>
    /// Gets or creates a cached paint/font pair with the specified properties.
    /// Increments reference count if cached entry exists.
    /// </summary>
    (SKPaint paint, SKFont font) GetOrCreate(
        SKColor color,
        float size,
        SKTypeface? typeface = null,
        bool isAntialias = true,
        bool subpixel = true,
        SKFontEdging edging = SKFontEdging.Antialias,
        SKFontHinting hinting = SKFontHinting.Full,
        SKTextAlign textAlign = SKTextAlign.Left
    );

    /// <summary>
    /// Releases a paint/font pair. Decrements reference count and disposes if count reaches zero.
    /// Used when control properties change.
    /// </summary>
    void Release(SKPaint paint, SKFont font);

    /// <summary>
    /// Disposes all cached paints and clears the cache.
    /// Called on navigation to clean up paints from the previous page.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Gets the number of cached paint entries (for testing/diagnostics).
    /// </summary>
    int CacheCount { get; }
}
