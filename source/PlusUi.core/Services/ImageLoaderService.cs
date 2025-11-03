using SkiaSharp;
using System.Collections.Concurrent;
using PlusUi.core.Models;

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
    private static readonly System.Net.Http.HttpClient _httpClient = new();

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
            var filePath = imageSource.Substring(filePrefix.Length);
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
            using var stream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var (staticImage, animatedImage) = LoadImageFromStream(stream);

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
        catch
        {
            onImageLoaded?.Invoke(null);
        }
    }

    private static (SKImage? staticImage, AnimatedImageInfo? animatedImage) TryLoadImageFromFile(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
                return (null, null);

            using var stream = System.IO.File.OpenRead(filePath);
            return LoadImageFromStream(stream);
        }
        catch
        {
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
        catch
        {
            return (null, null);
        }
    }

    /// <summary>
    /// Loads all frames from an animated image (e.g., GIF) along with their timing information.
    /// </summary>
    private static AnimatedImageInfo LoadAnimatedImage(SKCodec codec, int frameCount)
    {
        try
        {
            var frames = new SKImage[frameCount];
            var frameDelays = new int[frameCount];
            var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);

            for (int i = 0; i < frameCount; i++)
            {
                // Get frame info for timing
                codec.GetFrameInfo(i, out var frameInfo);

                // Frame duration in milliseconds (default to 100ms if not specified or invalid)
                var duration = frameInfo.Duration > 0 ? frameInfo.Duration : 100;
                frameDelays[i] = duration;

                // Decode the frame
                using var bitmap = new SKBitmap(info);
                var opts = new SKCodecOptions(i);
                var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(), opts);

                if (result == SKCodecResult.Success)
                {
                    frames[i] = SKImage.FromBitmap(bitmap);
                }
                else
                {
                    // If frame failed to load, use a blank image
                    frames[i] = SKImage.FromBitmap(new SKBitmap(info));
                }
            }

            return new AnimatedImageInfo(frames, frameDelays, info.Width, info.Height);
        }
        catch
        {
            return new AnimatedImageInfo(Array.Empty<SKImage>(), Array.Empty<int>(), 0, 0);
        }
    }
}
