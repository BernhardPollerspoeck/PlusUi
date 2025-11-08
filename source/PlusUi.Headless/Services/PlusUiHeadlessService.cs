using PlusUi.core;
using PlusUi.Headless.Enumerations;
using SkiaSharp;
using System.Numerics;

namespace PlusUi.Headless.Services;

/// <summary>
/// Internal headless service implementation.
/// Handles on-demand frame rendering and input event propagation.
/// </summary>
internal class PlusUiHeadlessService
{
    private readonly RenderService _renderService;
    private readonly InputService _inputService;
    private readonly HeadlessPlatformService _platformService;
    private readonly HeadlessKeyboardHandler _keyboardHandler;
    private readonly PlusUiNavigationService _navigationService;

    private Vector2 _currentMousePosition;
    private Size _frameSize;
    private ImageFormat _format;

    public PlusUiHeadlessService(
        RenderService renderService,
        InputService inputService,
        HeadlessPlatformService platformService,
        HeadlessKeyboardHandler keyboardHandler,
        PlusUiNavigationService navigationService)
    {
        _renderService = renderService;
        _inputService = inputService;
        _platformService = platformService;
        _keyboardHandler = keyboardHandler;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Initializes the service with frame size and image format.
    /// Called by wrapper after construction.
    /// </summary>
    internal void Initialize(Size frameSize, ImageFormat format)
    {
        _frameSize = frameSize;
        _format = format;

        // Initialize navigation to set up the page
        _navigationService.Initialize();
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
        _renderService.Render(
            gl: null,  // No OpenGL
            canvas: canvas,
            grContext: null,  // No GPU context
            canvasSize: new Vector2(_frameSize.Width, _frameSize.Height)
        );

        // 3. Encode to format
        return Task.FromResult(EncodeToFormat(bitmap, _format));
    }

    public void MouseMove(float x, float y)
    {
        _currentMousePosition = new Vector2(x, y);
        _inputService.MouseMove(_currentMousePosition);
    }

    public void MouseDown()
    {
        _inputService.MouseDown(_currentMousePosition);
    }

    public void MouseUp()
    {
        _inputService.MouseUp(_currentMousePosition);
    }

    public void MouseWheel(float deltaX, float deltaY)
    {
        _inputService.MouseWheel(_currentMousePosition, deltaX, deltaY);
    }

    public void KeyPress(PlusKey key)
    {
        _keyboardHandler.RaiseKeyInput(key);
    }

    public void CharInput(char c)
    {
        _keyboardHandler.RaiseCharInput(c);
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
