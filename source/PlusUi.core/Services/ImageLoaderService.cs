using SkiaSharp;
using System.Collections.Concurrent;

namespace PlusUi.core;

/// <summary>
/// Internal service for loading images from various sources (resources, files, URLs).
/// Uses weak reference caching to prevent memory bloat in long-running applications.
/// </summary>
internal static class ImageLoaderService
{
    private static readonly ConcurrentDictionary<string, WeakReference<SKImage>> _imageCache = new();
    private static readonly System.Net.Http.HttpClient _httpClient = new();

    /// <summary>
    /// Loads an image from the specified source (resource, file, or URL).
    /// Returns null immediately for web images (which load asynchronously).
    /// </summary>
    /// <param name="imageSource">Image source string (resource name, file: path, or http(s):// URL)</param>
    /// <param name="onImageLoaded">Optional callback when async web image loads</param>
    /// <returns>SKImage if immediately available, null otherwise</returns>
    public static SKImage? LoadImage(string? imageSource, Action<SKImage?>? onImageLoaded = null)
    {
        if (string.IsNullOrEmpty(imageSource))
        {
            return null;
        }

        // Try to get from cache (weak reference)
        if (_imageCache.TryGetValue(imageSource, out var weakRef) && weakRef.TryGetTarget(out var cachedImage))
        {
            return cachedImage;
        }

        SKImage? image = null;

        // Check for http:// or https:// prefix
        if (imageSource.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            imageSource.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // Web images are loaded asynchronously, don't cache null here
            image = TryLoadImageFromWeb(imageSource, onImageLoaded);
        }
        // Check for file: prefix
        else if (imageSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            const string filePrefix = "file:";
            var filePath = imageSource.Substring(filePrefix.Length);
            image = TryLoadImageFromFile(filePath);
            // Cache using weak reference if image was loaded successfully
            if (image != null)
            {
                _imageCache[imageSource] = new WeakReference<SKImage>(image);
            }
        }
        // Default: Load from embedded resources
        else
        {
            image = TryLoadImageFromResources(imageSource);
            // Cache using weak reference if image was loaded successfully
            if (image != null)
            {
                _imageCache[imageSource] = new WeakReference<SKImage>(image);
            }
        }

        return image;
    }

    private static SKImage? TryLoadImageFromWeb(string url, Action<SKImage?>? onImageLoaded)
    {
        // Start loading the image asynchronously in the background
        // Return null immediately to avoid blocking the UI thread
        _ = LoadImageFromWebAsync(url, onImageLoaded);
        return null;
    }

    private static async Task LoadImageFromWebAsync(string url, Action<SKImage?>? onImageLoaded)
    {
        try
        {
            using var stream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            using var codec = SKCodec.Create(stream);
            if (codec == null)
            {
                onImageLoaded?.Invoke(null);
                return;
            }

            var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
            using var bitmap = new SKBitmap(info);
            codec.GetPixels(bitmap.Info, bitmap.GetPixels());
            var image = SKImage.FromBitmap(bitmap);

            // Cache using weak reference
            _imageCache[url] = new WeakReference<SKImage>(image);
            onImageLoaded?.Invoke(image);
        }
        catch
        {
            onImageLoaded?.Invoke(null);
        }
    }

    private static SKImage? TryLoadImageFromFile(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
                return null;

            using var stream = System.IO.File.OpenRead(filePath);
            using var codec = SKCodec.Create(stream);
            if (codec == null)
                return null;

            var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
            using var bitmap = new SKBitmap(info);
            codec.GetPixels(bitmap.Info, bitmap.GetPixels());
            return SKImage.FromBitmap(bitmap);
        }
        catch
        {
            return null;
        }
    }

    private static SKImage? TryLoadImageFromResources(string resourceName)
    {
        // First try the entry assembly
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            var image = TryLoadImageFromAssembly(assembly, resourceName);
            if (image != null)
            {
                return image;
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

            var image = TryLoadImageFromAssembly(loadedAssembly, resourceName);
            if (image != null)
            {
                return image;
            }
        }

        // Return null instead of throwing exception
        return null;
    }

    private static SKImage? TryLoadImageFromAssembly(System.Reflection.Assembly assembly, string resourceName)
    {
        var resourceNames = assembly.GetManifestResourceNames();
        var fullResourceName = resourceNames.FirstOrDefault(name =>
            name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (fullResourceName == null)
        {
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
            return null;

        using var codec = SKCodec.Create(stream);
        if (codec == null)
            return null;

        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
        using var bitmap = new SKBitmap(info);
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        return SKImage.FromBitmap(bitmap);
    }
}
