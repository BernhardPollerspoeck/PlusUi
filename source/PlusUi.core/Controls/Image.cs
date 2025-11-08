using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Models;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Displays static and animated images from various sources (embedded resources, files, URLs).
/// Supports automatic GIF animation playback and aspect ratio control.
/// </summary>
/// <remarks>
/// Image sources can use different prefixes:
/// - No prefix: Embedded resource in any loaded assembly
/// - "file:": Local file path (e.g., "file:/path/to/image.png")
/// - "http://" or "https://": Web image (loaded asynchronously)
/// </remarks>
/// <example>
/// <code>
/// // Embedded resource
/// new Image().SetImageSource("logo.png");
///
/// // Local file
/// new Image().SetImageSource("file:/images/photo.jpg");
///
/// // Web image
/// new Image().SetImageSource("https://example.com/image.png");
///
/// // With aspect ratio
/// new Image()
///     .SetImageSource("photo.jpg")
///     .SetAspect(Aspect.AspectFill);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Image : UiElement
{
    private IImageLoaderService? _imageLoaderService;

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
            var (staticImage, animatedImage) = _imageLoaderService?.LoadImage(value, OnImageLoadedFromWeb, OnAnimatedImageLoadedFromWeb) ?? (default, default);

            if (animatedImage != null)
            {
                _animatedImageInfo = animatedImage;
                _image = null;
                StartAnimation();
            }
            else
            {
                _image = staticImage;
                _animatedImageInfo = null;
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

    #region render cache and animation
    private SKImage? _image;
    private AnimatedImageInfo? _animatedImageInfo;
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
            StartAnimation();

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
        }
        base.Dispose(disposing);
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

        // Determine which image to render (static or current animation frame)
        if (_animatedImageInfo != null && _animatedImageInfo.FrameCount > 0)
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
