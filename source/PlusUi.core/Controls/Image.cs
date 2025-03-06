using SkiaSharp;
using System.Collections.Concurrent;

namespace PlusUi.core;

public class Image : UiElement<Image>
{
    private static readonly ConcurrentDictionary<string, SKImage?> _imageCache = new();

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

        // First try the entry assembly
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            var image = TryLoadImageFromAssembly(assembly, ImageSource);
            if (image != null)
            {
                _imageCache[ImageSource] = image;
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

            var image = TryLoadImageFromAssembly(loadedAssembly, ImageSource);
            if (image != null)
            {
                _imageCache[ImageSource] = image;
                return image;
            }
        }

        throw new InvalidOperationException($"Resource '{ImageSource}' not found in any assembly.");
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

        var destRect = new SKRect(
            Position.X,
            Position.Y,
            Position.X + ElementSize.Width,
            Position.Y + ElementSize.Height);
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
