using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Models;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Image : UiElement
{
    private IImageLoaderService? _imageLoaderService;

    protected internal override bool IsFocusable => false;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Image;

    public override void BuildContent()
    {
        base.BuildContent();
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? "Image";
    }

    #region ImageSource
    internal string? ImageSource
    {
        get => field;
        set
        {
            field = value;

            // Clean up previous animation if any
            StopAnimation();

            _imageLoaderService ??= ServiceProviderService.ServiceProvider?.GetRequiredService<IImageLoaderService>();
            var (staticImage, animatedImage, svgImage) = _imageLoaderService?.LoadImage(value, OnImageLoadedFromWeb, OnAnimatedImageLoadedFromWeb, OnSvgImageLoadedFromWeb) ?? (default, default, default);

            if (svgImage != null)
            {
                _svgImageInfo = svgImage;
                _image = null;
                _animatedImageInfo = null;
            }
            else if (animatedImage != null)
            {
                _animatedImageInfo = animatedImage;
                _image = null;
                _svgImageInfo = null;
                StartAnimation();
            }
            else
            {
                _image = staticImage;
                _animatedImageInfo = null;
                _svgImageInfo = null;
            }

            // Force re-render when image source changes
            InvalidateMeasure();
        }
    }
    public Image SetImageSource(string imageSource)
    {
        ImageSource = imageSource;
        return this;
    }
    public Image BindImageSource(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ImageSource = getter());
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
    public Image BindAspect(Expression<Func<Aspect>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Aspect = getter());
        return this;
    }

    #endregion

    #region render cache and animation
    private SKImage? _image;
    private AnimatedImageInfo? _animatedImageInfo;
    private SvgImageInfo? _svgImageInfo;
    private SKImage? _renderedSvgImage;
    private Size _lastSvgRenderSize;
    private int _currentFrameIndex = 0;
    private System.Threading.Timer? _animationTimer;

    private void OnImageLoadedFromWeb(SKImage? image)
    {
        // Update the image if this is still the active source
        if (image != null)
        {
            StopAnimation();
            _image = image;
            _animatedImageInfo = null;
            _svgImageInfo = null;

            // Trigger UI update
            InvalidateMeasure();
        }
    }

    private void OnAnimatedImageLoadedFromWeb(AnimatedImageInfo? animatedImage)
    {
        // Update the animated image if this is still the active source
        if (animatedImage != null)
        {
            StopAnimation();
            _animatedImageInfo = animatedImage;
            _image = null;
            _svgImageInfo = null;
            StartAnimation();

            // Trigger UI update
            InvalidateMeasure();
        }
    }

    private void OnSvgImageLoadedFromWeb(SvgImageInfo? svgImage)
    {
        // Update the SVG image if this is still the active source
        if (svgImage != null)
        {
            StopAnimation();
            _svgImageInfo = svgImage;
            _image = null;
            _animatedImageInfo = null;
            _renderedSvgImage = null;

            // Trigger UI update
            InvalidateMeasure();
        }
    }

    private void StartAnimation()
    {
        if (_animatedImageInfo == null || _animatedImageInfo.FrameCount == 0)
            return;

        _currentFrameIndex = 0;

        // Create timer for frame updates
        _animationTimer = new System.Threading.Timer(
   callback: _ => OnAnimationTick(),
            state: null,
  dueTime: _animatedImageInfo.FrameDelays[0],
            period: System.Threading.Timeout.Infinite);
    }

    private void StopAnimation()
    {
        _animationTimer?.Dispose();
        _animationTimer = null;
        _currentFrameIndex = 0;
    }

    private void OnAnimationTick()
    {
        if (_animatedImageInfo == null)
            return;

        // Move to next frame
        _currentFrameIndex = (_currentFrameIndex + 1) % _animatedImageInfo.FrameCount;

        // Trigger UI invalidation to re-render with new frame
        // This is critical for animated GIFs to actually show animation
        InvalidateMeasure();

        // Schedule next frame
        var delay = _animatedImageInfo.FrameDelays[_currentFrameIndex];
        _animationTimer?.Change(delay, System.Threading.Timeout.Infinite);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAnimation();
            _animatedImageInfo?.Dispose();
            _svgImageInfo?.Dispose();
            _renderedSvgImage?.Dispose();
        }
        base.Dispose(disposing);
    }
    #endregion

    #region TintColor
    internal Color? TintColor { get; set; }

    public Image SetTintColor(Color tintColor)
    {
        TintColor = tintColor;
        _renderedSvgImage = null; // Force re-render with new tint
        InvalidateMeasure();
        return this;
    }

    public Image BindTintColor(Expression<Func<Color?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            TintColor = getter();
            _renderedSvgImage = null;
            InvalidateMeasure();
        });
        return this;
    }
    #endregion

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        if (!IsVisible)
        {
            return;
        }

        SKImage? imageToRender = null;

        // Determine which image to render (SVG, animation frame, or static)
        if (_svgImageInfo != null)
        {
            // For SVG, render at the current element size for best quality
            var targetSize = new Size(ElementSize.Width, ElementSize.Height);
            if (_renderedSvgImage == null || Math.Abs(_lastSvgRenderSize.Width - targetSize.Width) > 0.1f || Math.Abs(_lastSvgRenderSize.Height - targetSize.Height) > 0.1f)
            {
                _renderedSvgImage?.Dispose();
                _renderedSvgImage = _svgImageInfo.RenderToImage(targetSize.Width, targetSize.Height, TintColor);
                _lastSvgRenderSize = targetSize;
            }
            imageToRender = _renderedSvgImage;
        }
        else if (_animatedImageInfo != null && _animatedImageInfo.FrameCount > 0)
        {
            imageToRender = _animatedImageInfo.Frames[_currentFrameIndex];
        }
        else if (_image != null)
        {
            imageToRender = _image;
        }

        if (imageToRender == null)
        {
            return;
        }

        var destRect = new SKRect(
            Position.X + VisualOffset.X,
                 Position.Y + VisualOffset.Y,
                 Position.X + VisualOffset.X + ElementSize.Width,
             Position.Y + VisualOffset.Y + ElementSize.Height);
        var srcRect = new SKRect(0, 0, imageToRender.Width, imageToRender.Height);
        var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

        switch (Aspect)
        {
            case Aspect.Fill:
                canvas.DrawImage(imageToRender, destRect, samplingOptions);
                break;
            case Aspect.AspectFit:
                var aspectFitRect = SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height);
                aspectFitRect = aspectFitRect.AspectFit(srcRect.Size);

                canvas.DrawImage(imageToRender, srcRect, aspectFitRect, samplingOptions);
                break;
            case Aspect.AspectFill:
                var aspectFillRect = SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height);
                aspectFillRect = aspectFillRect.AspectFill(srcRect.Size);
                canvas.DrawImage(imageToRender, srcRect, aspectFillRect, samplingOptions);
                break;
        }
    }
}
