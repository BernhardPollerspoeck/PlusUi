using SkiaSharp;
using System.Collections.Concurrent;

namespace PlusUi.core;

public class Image : UiElement<Image>
{
    private static readonly ConcurrentDictionary<string, SKImage?> _imageCache = new();
    private static readonly System.Net.Http.HttpClient _httpClient = new();

    #region ImageSource
    internal string? ImageSource
    {
        get => field;
        set
        {
            field = value;
            _image = CreateImage();
        }
    }
    public UiElement SetImageSource(string imageSource)
    {
        ImageSource = imageSource;
        return this;
    }
    public UiElement BindImageSource(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => ImageSource = propertyGetter());
        return this;
    }
    #endregion

    #region Aspect
    internal Aspect Aspect
    {
        get => field;
        set
        {
            field = value;
        }
    }
    public Image SetAspect(Aspect aspect)
    {
        Aspect = aspect;
        return this;
    }
    public Image BindAspect(string propertyName, Func<Aspect> propertyGetter)
    {
        RegisterBinding(propertyName, () => Aspect = propertyGetter());
        return this;
    }

    #endregion

    #region render cache
    private SKImage? _image;
    private SKImage? CreateImage()
    {
        if (string.IsNullOrEmpty(ImageSource))
        {
            return null;
        }

        if (_imageCache.TryGetValue(ImageSource, out var cachedImage))
        {
            return cachedImage;
        }

        SKImage? image = null;

        // Check for http:// or https:// prefix
        if (ImageSource.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            ImageSource.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            image = TryLoadImageFromWeb(ImageSource);
        }
        // Check for file: prefix
        else if (ImageSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            const string filePrefix = "file:";
            var filePath = ImageSource.Substring(filePrefix.Length);
            image = TryLoadImageFromFile(filePath);
        }
        // Default: Load from embedded resources
        else
        {
            image = TryLoadImageFromResources(ImageSource);
        }

        // Cache the result (even if null) to avoid repeated failed attempts
        _imageCache[ImageSource] = image;
        return image;
    }

    private SKImage? TryLoadImageFromWeb(string url)
    {
        try
        {
            using var stream = _httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
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

    private SKImage? TryLoadImageFromFile(string filePath)
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

    private SKImage? TryLoadImageFromResources(string resourceName)
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
    #endregion

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (_image == null)
        {
            return;
        }
        if (!IsVisible)
        {
            return;
        }

        var destRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);
        var srcRect = new SKRect(0, 0, _image.Width, _image.Height);
        var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

        switch (Aspect)
        {
            case Aspect.Fill:
                canvas.DrawImage(_image, destRect, samplingOptions);
                break;
            case Aspect.AspectFit:
                var aspectFitRect = SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height);
                aspectFitRect = aspectFitRect.AspectFit(srcRect.Size);

                canvas.DrawImage(_image, srcRect, aspectFitRect, samplingOptions);
                break;
            case Aspect.AspectFill:
                var aspectFillRect = SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height);
                aspectFillRect = aspectFillRect.AspectFill(srcRect.Size);
                canvas.DrawImage(_image, srcRect, aspectFillRect, samplingOptions);
                break;
        }
    }
}
