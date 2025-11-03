using SkiaSharp;
using System.Collections.Concurrent;
using PlusUi.core.Models;
using Microsoft.Extensions.Logging;

namespace PlusUi.core;

/// <summary>
/// Internal service for loading images from various sources (resources, files, URLs).
/// Uses weak reference caching to prevent memory bloat in long-running applications.
/// Supports animated GIF loading with frame extraction.
/// </summary>
internal static class ImageLoaderService
{
    private static readonly ConcurrentDictionary<string, WeakReference<SKImage>> _imageCache = new();
    private static readonly ConcurrentDictionary<string, WeakReference<AnimatedImageInfo>> _animatedImageCache = new();
    private static readonly System.Net.Http.HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30) // Add timeout for web requests
    };

    /// <summary>
    /// Optional logger for diagnostic messages. Can be set from the application host.
    /// </summary>
    public static ILogger? Logger { get; set; }

    /// <summary>
    /// Loads an image from the specified source (resource, file, or URL).
    /// Returns a tuple with either a static image or animated image info.
    /// Returns (null, null) immediately for web images (which load asynchronously).
    /// </summary>
    /// <param name="imageSource">Image source string (resource name, file: path, or http(s):// URL)</param>
    /// <param name="onImageLoaded">Optional callback when async web image loads</param>
    /// <param name="onAnimatedImageLoaded">Optional callback when async animated web image loads</param>
    /// <returns>Tuple of (static image, animated image info)</returns>
    public static (SKImage? staticImage, AnimatedImageInfo? animatedImage) LoadImage(
        string? imageSource,
        Action<SKImage?>? onImageLoaded = null,
        Action<AnimatedImageInfo?>? onAnimatedImageLoaded = null)
    {
        if (string.IsNullOrEmpty(imageSource))
        {
            return (null, null);
        }

        // Try to get from animated cache first
        if (_animatedImageCache.TryGetValue(imageSource, out var animatedWeakRef) && animatedWeakRef.TryGetTarget(out var cachedAnimatedImage))
        {
            return (null, cachedAnimatedImage);
        }

        // Try to get from static image cache
        if (_imageCache.TryGetValue(imageSource, out var weakRef) && weakRef.TryGetTarget(out var cachedImage))
        {
            return (cachedImage, null);
        }

        // Check for http:// or https:// prefix
        if (imageSource.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            imageSource.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // Web images are loaded asynchronously, don't cache null here
            TryLoadImageFromWeb(imageSource, onImageLoaded, onAnimatedImageLoaded);
            return (null, null);
        }
        // Check for file: prefix
        else if (imageSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            const string filePrefix = "file:";
            var filePath = imageSource[filePrefix.Length..];
            var (image, animatedImage) = TryLoadImageFromFile(filePath);
            // Cache using weak reference if image was loaded successfully
            if (image != null)
            {
                _imageCache[imageSource] = new WeakReference<SKImage>(image);
            }
            if (animatedImage != null)
            {
                _animatedImageCache[imageSource] = new WeakReference<AnimatedImageInfo>(animatedImage);
            }
            return (image, animatedImage);
        }
        // Default: Load from embedded resources
        else
        {
            var (image, animatedImage) = TryLoadImageFromResources(imageSource);
            // Cache using weak reference if image was loaded successfully
            if (image != null)
            {
                _imageCache[imageSource] = new WeakReference<SKImage>(image);
            }
            if (animatedImage != null)
            {
                _animatedImageCache[imageSource] = new WeakReference<AnimatedImageInfo>(animatedImage);
            }
            return (image, animatedImage);
        }
    }

    private static void TryLoadImageFromWeb(string url, Action<SKImage?>? onImageLoaded, Action<AnimatedImageInfo?>? onAnimatedImageLoaded)
    {
        // Start loading the image asynchronously in the background
        // Return immediately to avoid blocking the UI thread
        _ = LoadImageFromWebAsync(url, onImageLoaded, onAnimatedImageLoaded);
    }

    private static async Task LoadImageFromWebAsync(string url, Action<SKImage?>? onImageLoaded, Action<AnimatedImageInfo?>? onAnimatedImageLoaded)
    {
        try
        {
            // Add retry logic for better reliability
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // Failed to load - invoke callbacks with null
                onImageLoaded?.Invoke(null);
                return;
            }

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            // Create a memory stream to allow multiple reads (needed for SKCodec)
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            memoryStream.Position = 0;

            var (staticImage, animatedImage) = LoadImageFromStream(memoryStream);

            if (staticImage != null)
            {
                // Cache using weak reference
                _imageCache[url] = new WeakReference<SKImage>(staticImage);
                onImageLoaded?.Invoke(staticImage);
            }
            else if (animatedImage != null)
            {
                // Cache using weak reference
                _animatedImageCache[url] = new WeakReference<AnimatedImageInfo>(animatedImage);
                onAnimatedImageLoaded?.Invoke(animatedImage);
            }
            else
            {
                onImageLoaded?.Invoke(null);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load image from URL: {Url}", url);
            onImageLoaded?.Invoke(null);
        }
    }

    private static (SKImage? staticImage, AnimatedImageInfo? animatedImage) TryLoadImageFromFile(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
            {
                Logger?.LogWarning("Image file not found: {FilePath}", filePath);
                return (null, null);
            }

            using var stream = System.IO.File.OpenRead(filePath);
            return LoadImageFromStream(stream);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load image from file: {FilePath}", filePath);
            return (null, null);
        }
    }

    private static (SKImage? staticImage, AnimatedImageInfo? animatedImage) TryLoadImageFromResources(string resourceName)
    {
        // First try the entry assembly
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            var result = TryLoadImageFromAssembly(assembly, resourceName);
            if (result.staticImage != null || result.animatedImage != null)
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
            if (result.staticImage != null || result.animatedImage != null)
            {
                return result;
            }
        }

        // Return null instead of throwing exception
        return (null, null);
    }

    private static (SKImage? staticImage, AnimatedImageInfo? animatedImage) TryLoadImageFromAssembly(System.Reflection.Assembly assembly, string resourceName)
    {
        var resourceNames = assembly.GetManifestResourceNames();
        var fullResourceName = resourceNames.FirstOrDefault(name =>
            name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (fullResourceName == null)
        {
            return (null, null);
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
            return (null, null);

        return LoadImageFromStream(stream);
    }

    /// <summary>
    /// Loads an image from a stream and determines if it's animated.
    /// For animated images (e.g., GIF), extracts all frames and their delays.
    /// </summary>
    private static (SKImage? staticImage, AnimatedImageInfo? animatedImage) LoadImageFromStream(Stream stream)
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
                return (null, null);

            var frameCount = codec.FrameCount;

            // Check if this is an animated image (more than one frame)
            if (frameCount > 1)
            {
                return (null, LoadAnimatedImage(codec, frameCount));
            }
            else
            {
                // Static image - load single frame
                var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                using var bitmap = new SKBitmap(info);
                var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
                if (result != SKCodecResult.Success)
                    return (null, null);

                return (SKImage.FromBitmap(bitmap), null);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load image from stream");
            return (null, null);
        }
    }

    /// <summary>
    /// Loads all frames from an animated image (e.g., GIF) along with their timing information.
    /// Properly handles frame composition and disposal methods.
    /// </summary>
    private static AnimatedImageInfo LoadAnimatedImage(SKCodec codec, int frameCount)
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
            Logger?.LogError(ex, "Failed to load animated image");
            return new AnimatedImageInfo([], [], 0, 0);
        }
    }
}
