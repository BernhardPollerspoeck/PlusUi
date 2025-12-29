using SkiaSharp;
using PlusUi.core.Models;

namespace PlusUi.core;

/// <summary>
/// Defines a service for loading images from a specified source, supporting static, animated, and SVG images. Provides
/// synchronous loading for local resources and files, and asynchronous loading for web images via callbacks.
/// </summary>
/// <remarks>When loading images from web sources (URLs), the method returns immediately with all tuple values
/// set to null. The actual image data is provided asynchronously through the supplied callback parameters. For local
/// resources and files, the image is loaded synchronously and returned in the tuple. Callbacks are optional and only
/// invoked for asynchronous web image loads. This interface abstracts image loading to support various image types and
/// sources in a platform-agnostic manner.</remarks>
public interface IImageLoaderService
{
    /// <summary>
    /// Loads an image from the specified source (resource, file, or URL).
    /// Returns a tuple with either a static image, animated image info, or SVG image info.
    /// Returns (null, null, null) immediately for web images (which load asynchronously).
    /// </summary>
    /// <param name="imageSource">Image source string (resource name, file: path, or http(s):// URL)</param>
    /// <param name="onImageLoaded">Optional callback when async web image loads</param>
    /// <param name="onAnimatedImageLoaded">Optional callback when async animated web image loads</param>
    /// <param name="onSvgImageLoaded">Optional callback when async SVG web image loads</param>
    /// <returns>Tuple of (static image, animated image info, SVG image info)</returns>
    public (SKImage? staticImage, AnimatedImageInfo? animatedImage, SvgImageInfo? svgImage) LoadImage(
        string? imageSource,
        Action<SKImage?>? onImageLoaded = null,
        Action<AnimatedImageInfo?>? onAnimatedImageLoaded = null,
        Action<SvgImageInfo?>? onSvgImageLoaded = null);
}
