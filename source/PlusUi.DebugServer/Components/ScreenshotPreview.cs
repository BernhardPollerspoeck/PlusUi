using PlusUi.core;
using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.DebugServer.Components;

[GenerateShadowMethods]
internal partial class ScreenshotPreview : UiElement
{
    private SKImage? _image;
    private byte[]? _imageData;

    protected internal override bool IsFocusable => false;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Image;

    public byte[]? ImageData
    {
        get => _imageData;
        set
        {
            if (_imageData == value) return;
            _imageData = value;
            LoadImage();
            InvalidateMeasure();
        }
    }

    public ScreenshotPreview SetImageData(byte[]? data)
    {
        ImageData = data;
        return this;
    }

    public ScreenshotPreview BindImageData(Expression<Func<byte[]?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ImageData = getter());
        return this;
    }

    private void LoadImage()
    {
        _image?.Dispose();
        _image = null;

        if (_imageData == null || _imageData.Length == 0)
            return;

        try
        {
            using var stream = new MemoryStream(_imageData);
            using var codec = SKCodec.Create(stream);
            if (codec == null) return;

            var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
            using var bitmap = new SKBitmap(info);
            var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
            if (result == SKCodecResult.Success)
            {
                _image = SKImage.FromBitmap(bitmap);
            }
        }
        catch
        {
            _image = null;
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (_image == null)
            return new Size(0, 0);

        var imageAspect = (float)_image.Width / _image.Height;
        var desiredW = DesiredSize?.Width ?? -1;
        var desiredH = DesiredSize?.Height ?? -1;

        // Both dimensions set
        if (desiredW > 0 && desiredH > 0)
            return new Size(desiredW, desiredH);

        // Only height set - calculate width from aspect ratio
        if (desiredH > 0)
            return new Size(desiredH * imageAspect, desiredH);

        // Only width set - calculate height from aspect ratio
        if (desiredW > 0)
            return new Size(desiredW, desiredW / imageAspect);

        // Stretching vertically (no fixed height) - use available height to calculate width
        if (VerticalAlignment == VerticalAlignment.Stretch && availableSize.Height < 1e6f)
            return new Size(availableSize.Height * imageAspect, availableSize.Height);

        // Stretching horizontally (no fixed width) - use available width to calculate height
        if (HorizontalAlignment == HorizontalAlignment.Stretch && availableSize.Width < 1e6f)
            return new Size(availableSize.Width, availableSize.Width / imageAspect);

        // Neither set, no stretch - use image size capped to available
        var width = Math.Min(_image.Width, availableSize.Width);
        var height = width / imageAspect;
        return new Size(width, height);
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        if (!IsVisible || _image == null)
            return;

        var destRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        var srcRect = new SKRect(0, 0, _image.Width, _image.Height);

        // AspectFit scaling
        var aspectFitRect = SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height);
        aspectFitRect = aspectFitRect.AspectFit(srcRect.Size);

        var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
        canvas.DrawImage(_image, srcRect, aspectFitRect, samplingOptions);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _image?.Dispose();
            _image = null;
        }
        base.Dispose(disposing);
    }
}
