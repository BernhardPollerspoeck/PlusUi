using PlusUi.core;
using PlusUi.Headless.Enumerations;
using SkiaSharp;
using System.Numerics;

namespace PlusUi.Headless.Services;

/// <summary>
/// Internal headless service implementation.
/// Handles on-demand frame rendering and input event propagation.
/// </summary>
internal class PlusUiHeadlessService(
    RenderService renderService,
    InputService inputService,
    HeadlessKeyboardHandler keyboardHandler,
    PlusUiNavigationService navigationService)
{
    private Vector2 _currentMousePosition;
    private Size _frameSize;
    private ImageFormat _format;

    /// <summary>
    /// Initializes the service with frame size and image format.
    /// Called by wrapper after construction.
    /// </summary>
    internal void Initialize(Size frameSize, ImageFormat format)
    {
        _frameSize = frameSize;
        _format = format;

        // Initialize navigation to set up the page
        navigationService.Initialize();
    }

    public Task<byte[]> GetCurrentFrameAsync()
    {
        // 1. Create offscreen surface
        var bitmap = new SKBitmap(
            (int)_frameSize.Width,
            (int)_frameSize.Height,
            SKColorType.Rgba8888,
            SKAlphaType.Premul
        );

        var canvas = new SKCanvas(bitmap);

        // 2. Render current frame (Measure + Arrange + Render)
        renderService.Render(
            clearAction: null,
            canvas: canvas,
            grContext: null,
            canvasSize: new Vector2(_frameSize.Width, _frameSize.Height)
        );

        // 3. Encode to format
        return Task.FromResult(EncodeToFormat(bitmap, _format));
    }

    public void MouseMove(float x, float y)
    {
        _currentMousePosition = new Vector2(x, y);
        inputService.MouseMove(_currentMousePosition);
    }

    public void MouseDown()
    {
        inputService.MouseDown(_currentMousePosition);
    }

    public void MouseUp()
    {
        inputService.MouseUp(_currentMousePosition);
    }

    public void MouseWheel(float deltaX, float deltaY)
    {
        inputService.MouseWheel(_currentMousePosition, deltaX, deltaY);
    }

    public void KeyPress(PlusKey key)
    {
        keyboardHandler.RaiseKeyInput(key);
    }

    public void CharInput(char c)
    {
        keyboardHandler.RaiseCharInput(c);
    }

    /// <summary>
    /// Encodes the bitmap to the specified image format.
    /// </summary>
    private static byte[] EncodeToFormat(SKBitmap bitmap, ImageFormat format)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = format switch
        {
            ImageFormat.Png => image.Encode(SKEncodedImageFormat.Png, 100),
            ImageFormat.Jpeg => image.Encode(SKEncodedImageFormat.Jpeg, 90),
            ImageFormat.WebP => image.Encode(SKEncodedImageFormat.Webp, 90),
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };

        return data.ToArray();
    }
}
