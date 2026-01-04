using SkiaSharp;

namespace PlusUi.core.Services;

/// <summary>
/// Immutable key for PaintRegistry dictionary lookup based on all paint/font properties.
/// </summary>
public readonly record struct PaintCacheKey(
    SKColor Color,
    float Size,
    SKTypeface? Typeface,
    bool IsAntialias,
    bool Subpixel,
    SKFontEdging Edging,
    SKFontHinting Hinting,
    SKTextAlign TextAlign
);
