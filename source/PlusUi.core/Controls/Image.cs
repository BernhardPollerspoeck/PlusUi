using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Image : UiElement
{
    #region ImageSource
    internal string? ImageSource
    {
        get => field;
        set
        {
            field = value;
            _image = ImageLoaderService.LoadImage(value, OnImageLoadedFromWeb);
        }
    }
    public Image SetImageSource(string imageSource)
    {
        ImageSource = imageSource;
        return this;
    }
    public Image BindImageSource(string propertyName, Func<string?> propertyGetter)
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

    private void OnImageLoadedFromWeb(SKImage? image)
    {
        // Update the image if this is still the active source
        if (image != null)
        {
            _image = image;
        }
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
