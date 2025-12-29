using SkiaSharp;
using System.Collections.Concurrent;
using PlusUi.core.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Svg.Skia;

namespace PlusUi.core;

/// <summary>
/// Internal service for loading images from various sources (resources, files, URLs).
/// Uses weak reference caching to prevent memory bloat in long-running applications.
/// Supports animated GIF loading with frame extraction and SVG images.
/// </summary>
internal class ImageLoaderService(IOptions<PlusUiConfiguration> configuration, ILogger<ImageLoaderService>? logger = null) : IImageLoaderService
{
    private static readonly ConcurrentDictionary<string, WeakReference<SKImage>> _imageCache = new();
    private static readonly ConcurrentDictionary<string, WeakReference<AnimatedImageInfo>> _animatedImageCache = new();
    private static readonly ConcurrentDictionary<string, WeakReference<SvgImageInfo>> _svgImageCache = new();
    private static readonly System.Net.Http.HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30), // Add timeout for web requests
        DefaultRequestHeaders =
        {
            { "User-Agent", "PlusUi/1.0" }
        }
    };
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
        Action<SvgImageInfo?>? onSvgImageLoaded = null)
    {
        if (string.IsNullOrEmpty(imageSource))
        {
            return (null, null, null);
        }

        var isSvg = imageSource.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);

        // Try to get from SVG cache first
        if (isSvg && _svgImageCache.TryGetValue(imageSource, out var svgWeakRef) && svgWeakRef.TryGetTarget(out var cachedSvgImage))
        {
            return (null, null, cachedSvgImage);
        }

        // Try to get from animated cache
        if (_animatedImageCache.TryGetValue(imageSource, out var animatedWeakRef) && animatedWeakRef.TryGetTarget(out var cachedAnimatedImage))
        {
            return (null, cachedAnimatedImage, null);
        }

        // Try to get from static image cache
        if (_imageCache.TryGetValue(imageSource, out var weakRef) && weakRef.TryGetTarget(out var cachedImage))
        {
            return (cachedImage, null, null);
        }

        // Check for http:// or https:// prefix
        if (imageSource.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            imageSource.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // Web images are loaded asynchronously, don't cache null here
            return TryLoadImageFromWeb(imageSource, onImageLoaded, onAnimatedImageLoaded, onSvgImageLoaded);
        }
        // Check for file: prefix
        else if (imageSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            const string filePrefix = "file:";
            var filePath = imageSource[filePrefix.Length..];
            var (image, animatedImage, svgImage) = TryLoadImageFromFile(filePath);
            CacheResult(imageSource, image, animatedImage, svgImage);
            return (image, animatedImage, svgImage);
        }
        // Default: Load from embedded resources
        else
        {
            var (image, animatedImage, svgImage) = TryLoadImageFromResources(imageSource);
            CacheResult(imageSource, image, animatedImage, svgImage);
            return (image, animatedImage, svgImage);
        }
    }

    private static void CacheResult(string imageSource, SKImage? image, AnimatedImageInfo? animatedImage, SvgImageInfo? svgImage)
    {
        if (image != null)
        {
            _imageCache[imageSource] = new WeakReference<SKImage>(image);
        }
        if (animatedImage != null)
        {
            _animatedImageCache[imageSource] = new WeakReference<AnimatedImageInfo>(animatedImage);
        }
        if (svgImage != null)
        {
            _svgImageCache[imageSource] = new WeakReference<SvgImageInfo>(svgImage);
        }
    }

    private (SKImage?, AnimatedImageInfo?, SvgImageInfo?) TryLoadImageFromWeb(string url, Action<SKImage?>? onImageLoaded, Action<AnimatedImageInfo?>? onAnimatedImageLoaded, Action<SvgImageInfo?>? onSvgImageLoaded)
    {
        if (configuration.Value.LoadImagesSynchronously)
        {
            // Synchronous loading (not recommended for UI thread - but this is explicitly set by the user)
            var task = LoadImageFromWebAsync(url, null, null, null);
            task.Wait();
            return task.Result;
        }
        // Start loading the image asynchronously in the background
        // Return immediately to avoid blocking the UI thread
        _ = LoadImageFromWebAsync(url, onImageLoaded, onAnimatedImageLoaded, onSvgImageLoaded);
        return (null, null, null);
    }

    private async Task<ValueTuple<SKImage?, AnimatedImageInfo?, SvgImageInfo?>> LoadImageFromWebAsync(string url, Action<SKImage?>? onImageLoaded, Action<AnimatedImageInfo?>? onAnimatedImageLoaded, Action<SvgImageInfo?>? onSvgImageLoaded)
    {
        try
        {
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                if (!configuration.Value.LoadImagesSynchronously)
                {
                    onImageLoaded?.Invoke(null);
                }
                return (null, null, null);
            }

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            memoryStream.Position = 0;

            var isSvg = url.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
            var (staticImage, animatedImage, svgImage) = isSvg
                ? (null, null, LoadSvgFromStream(memoryStream))
                : LoadImageFromStream(memoryStream);

            if (svgImage != null)
            {
                _svgImageCache[url] = new WeakReference<SvgImageInfo>(svgImage);
                if (!configuration.Value.LoadImagesSynchronously)
                {
                    onSvgImageLoaded?.Invoke(svgImage);
                }
            }
            else if (staticImage != null)
            {
                _imageCache[url] = new WeakReference<SKImage>(staticImage);
                if (!configuration.Value.LoadImagesSynchronously)
                {
                    onImageLoaded?.Invoke(staticImage);
                }
            }
            else if (animatedImage != null)
            {
                _animatedImageCache[url] = new WeakReference<AnimatedImageInfo>(animatedImage);
                if (!configuration.Value.LoadImagesSynchronously)
                {
                    onAnimatedImageLoaded?.Invoke(animatedImage);
                }
            }
            else if (!configuration.Value.LoadImagesSynchronously)
            {
                onImageLoaded?.Invoke(null);
            }

            return (staticImage, animatedImage, svgImage);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load image from URL: {Url}", url);
            if (!configuration.Value.LoadImagesSynchronously)
            {
                onImageLoaded?.Invoke(null);
            }
            return (null, null, null);
        }
    }

    private (SKImage? staticImage, AnimatedImageInfo? animatedImage, SvgImageInfo? svgImage) TryLoadImageFromFile(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
            {
                logger?.LogWarning("Image file not found: {FilePath}", filePath);
                return (null, null, null);
            }

            using var stream = System.IO.File.OpenRead(filePath);

            if (filePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                return (null, null, LoadSvgFromStream(stream));
            }

            return LoadImageFromStream(stream);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load image from file: {FilePath}", filePath);
            return (null, null, null);
        }
    }

    private (SKImage? staticImage, AnimatedImageInfo? animatedImage, SvgImageInfo? svgImage) TryLoadImageFromResources(string resourceName)
    {
        // First try the entry assembly
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            var result = TryLoadImageFromAssembly(assembly, resourceName);
            if (result.staticImage != null || result.animatedImage != null || result.svgImage != null)
            {
                return result;
            }
        }

        // Try all loaded assemblies in the current AppDomain if not found
        foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Skip system assemblies to improve performance
            if (loadedAssembly.IsDynamic)
            {
                continue;
            }

            var result = TryLoadImageFromAssembly(loadedAssembly, resourceName);
            if (result.staticImage != null || result.animatedImage != null || result.svgImage != null)
            {
                return result;
            }
        }

        // Return null instead of throwing exception
        return (null, null, null);
    }

    private (SKImage? staticImage, AnimatedImageInfo? animatedImage, SvgImageInfo? svgImage) TryLoadImageFromAssembly(System.Reflection.Assembly assembly, string resourceName)
    {
        var resourceNames = assembly.GetManifestResourceNames();
        var fullResourceName = resourceNames.FirstOrDefault(name =>
            name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (fullResourceName == null)
        {
            return (null, null, null);
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
            return (null, null, null);

        if (resourceName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            return (null, null, LoadSvgFromStream(stream));
        }

        return LoadImageFromStream(stream);
    }

    /// <summary>
    /// Loads an SVG from a stream.
    /// </summary>
    private SvgImageInfo? LoadSvgFromStream(Stream stream)
    {
        try
        {
            var svg = new SKSvg();
            var picture = svg.Load(stream);

            if (picture == null)
                return null;

            var bounds = picture.CullRect;
            return new SvgImageInfo(picture, bounds.Width, bounds.Height);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load SVG from stream");
            return null;
        }
    }

    /// <summary>
    /// Loads an image from a stream and determines if it's animated.
    /// For animated images (e.g., GIF), extracts all frames and their delays.
    /// </summary>
    private (SKImage? staticImage, AnimatedImageInfo? animatedImage, SvgImageInfo? svgImage) LoadImageFromStream(Stream stream)
    {
        try
        {
            // Ensure stream is at the beginning
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var codec = SKCodec.Create(stream);
            if (codec == null)
                return (null, null, null);

            var frameCount = codec.FrameCount;

            // Check if this is an animated image (more than one frame)
            if (frameCount > 1)
            {
                return (null, LoadAnimatedImage(codec, frameCount), null);
            }
            else
            {
                // Static image - load single frame
                var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                using var bitmap = new SKBitmap(info);
                var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
                if (result != SKCodecResult.Success)
                    return (null, null, null);

                return (SKImage.FromBitmap(bitmap), null, null);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load image from stream");
            return (null, null, null);
        }
    }

    /// <summary>
    /// Loads all frames from an animated image (e.g., GIF) along with their timing information.
    /// Properly handles frame composition and disposal methods.
    /// </summary>
    private AnimatedImageInfo LoadAnimatedImage(SKCodec codec, int frameCount)
    {
        try
        {
            var frames = new SKImage[frameCount];
            var frameDelays = new int[frameCount];
            var info = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

            // Create a canvas bitmap that will accumulate frames
            using var canvasBitmap = new SKBitmap(info);

            for (int i = 0; i < frameCount; i++)
            {
                // Get frame info for timing and disposal
                codec.GetFrameInfo(i, out var frameInfo);

                // Frame duration in milliseconds (default to 100ms if not specified or invalid)
                var duration = frameInfo.Duration > 0 ? frameInfo.Duration : 100;
                frameDelays[i] = duration;

                // Decode the frame with proper options
                var opts = new SKCodecOptions(i);

                // Handle disposal method from previous frame BEFORE decoding new frame
                if (i > 0)
                {
                    codec.GetFrameInfo(i - 1, out var prevFrameInfo);

                    using var canvas = new SKCanvas(canvasBitmap);

                    // RestorePrevious or RestoreBackground: clear the previous frame's area
                    if (prevFrameInfo.DisposalMethod == SKCodecAnimationDisposalMethod.RestorePrevious ||
                        prevFrameInfo.DisposalMethod == SKCodecAnimationDisposalMethod.RestoreBackgroundColor)
                    {
                        // Clear the area where the previous frame was drawn
                        var clearRect = new SKRectI(
                            prevFrameInfo.FrameRect.Left,
                            prevFrameInfo.FrameRect.Top,
                            prevFrameInfo.FrameRect.Right,
                            prevFrameInfo.FrameRect.Bottom);

                        using var clearPaint = new SKPaint { BlendMode = SKBlendMode.Clear };
                        canvas.DrawRect(clearRect, clearPaint);
                    }
                    // Keep: keep the canvas as-is (do nothing)
                }
                else
                {
                    // First frame: clear to transparent
                    using var canvas = new SKCanvas(canvasBitmap);
                    canvas.Clear(SKColors.Transparent);
                }

                // Decode the frame onto the canvas
                var result = codec.GetPixels(canvasBitmap.Info, canvasBitmap.GetPixels(), opts);

                if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                {
                    // Create an image from the accumulated canvas bitmap
                    var frameCopy = new SKBitmap(info);
                    canvasBitmap.CopyTo(frameCopy);
                    frames[i] = SKImage.FromBitmap(frameCopy);
                    frameCopy.Dispose();
                }
                else
                {
                    // If frame failed to load, use previous frame or blank
                    if (i > 0 && frames[i - 1] != null)
                    {
                        // Reuse previous frame
                        var prevBitmap = new SKBitmap(info);
                        canvasBitmap.CopyTo(prevBitmap);
                        frames[i] = SKImage.FromBitmap(prevBitmap);
                        prevBitmap.Dispose();
                    }
                    else
                    {
                        // Create blank frame
                        var blankBitmap = new SKBitmap(info);
                        using var canvas = new SKCanvas(blankBitmap);
                        canvas.Clear(SKColors.Transparent);
                        frames[i] = SKImage.FromBitmap(blankBitmap);
                        blankBitmap.Dispose();
                    }
                }
            }

            return new AnimatedImageInfo(frames, frameDelays, info.Width, info.Height);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load animated image");
            return new AnimatedImageInfo([], [], 0, 0);
        }
    }
}
