using SkiaSharp;

namespace PlusUi.core;

public class Image : UiElement<Image>
{
    #region ImageSource
    public string? ImageSource
    {
        get => field;
        protected set
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
    public Aspect Aspect
    {
        get => field;
        protected set
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

        var assembly = System.Reflection.Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Entry assembly not found.");

        var resourceNames = assembly.GetManifestResourceNames();
        var resourceName = resourceNames.FirstOrDefault(name => name.EndsWith(ImageSource, StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException($"Resource '{ImageSource}' not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");

        using var codec = SKCodec.Create(stream);
        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
        using var bitmap = new SKBitmap(info);
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        return SKImage.FromBitmap(bitmap);
    }
    #endregion


    public override void Render(SKCanvas canvas)
    {
        if (_image == null)
        {
            return;
        }

        var destRect = new SKRect(Position.X, Position.Y, Position.X + ElementSize.Width, Position.Y + ElementSize.Height);
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
