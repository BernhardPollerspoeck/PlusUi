using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlusUi.core;
using PlusUi.core.Services.Accessibility;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;
using Svg.Skia;
using System.Runtime.InteropServices;
using MouseButton = Silk.NET.Input.MouseButton;
using WindowState = Silk.NET.Windowing.WindowState;

namespace PlusUi.desktop;

internal class WindowManager(
    IOptions<PlusUiConfiguration> uiOptions,
    RenderService renderService,
    InputService inputService,
    DesktopKeyboardHandler desktopKeyboardHandler,
    DesktopPlatformService platformService,
    PlusUiNavigationService plusUiNavigationService,
    NavigationContainer navigationContainer,
    IAccessibilityService accessibilityService,
    WindowSettingsService windowSettingsService,
    IHostApplicationLifetime appLifetime,
    ILogger<WindowManager> logger)
    : IHostedService
{
    #region fields
    private IWindow? _window;
    private GL? _glContext;
    private GRContext? _grContext;
    private SKSurface? _surface;
    private SKCanvas? _canvas;
    private IInputContext? _inputContext;
    private IMouse? _mouse;
    private IKeyboard? _keyboard;
    private bool _isClosing;
    private Vector2D<int> _lastNormalPosition;
    private Vector2D<int> _lastNormalSize;
    #endregion

    #region IHostedService
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var savedSettings = windowSettingsService.Load();

        var size = savedSettings != null
            ? new Vector2D<int>(savedSettings.Width, savedSettings.Height)
            : new Vector2D<int>(uiOptions.Value.Size.Width, uiOptions.Value.Size.Height);

        var position = savedSettings != null
            ? new Vector2D<int>(savedSettings.X, savedSettings.Y)
            : new Vector2D<int>(uiOptions.Value.Position.Width, uiOptions.Value.Position.Height);

        var windowState = savedSettings is { IsMaximized: true }
            ? Silk.NET.Windowing.WindowState.Maximized
            : (Silk.NET.Windowing.WindowState)uiOptions.Value.WindowState;

        var options = WindowOptions.Default with
        {
            Size = size,
            Title = uiOptions.Value.Title,
            Position = position,
            WindowState = windowState,
            WindowBorder = (Silk.NET.Windowing.WindowBorder)uiOptions.Value.WindowBorder,
            TopMost = uiOptions.Value.IsWindowTopMost,
            TransparentFramebuffer = uiOptions.Value.IsWindowTransparent,
            VSync = true,
            FramesPerSecond = 60
        };

        _window = Window.Create(options);
        platformService.SetWindow(_window);

        _lastNormalPosition = position;
        _lastNormalSize = size;

        _window.Closing += HandleWindowClosing;
        _window.Render += HandleWindowRender;
        _window.Resize += HandleWindowResize;
        _window.Load += HandleWindowLoad;
        _window.StateChanged += HandleWindowStateChanged;
        _window.FocusChanged += HandleWindowFocusChanged;
        _window.Move += HandleWindowMove;

        _window.Run();

        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _window?.Close();
        return Task.CompletedTask;
    }
    #endregion

    #region event handling
    private void HandleWindowRender(double delta)
    {
        // Stop rendering immediately when closing to prevent accessing disposed services
        if (_isClosing)
            return;

        if (this is not { _glContext: not null, _canvas: not null, _grContext: not null, _window: not null })
        {
            logger.LogWarning("Render skipped: GL context, canvas, GR context, or window is not initialized.");
            return;
        }

        renderService.Render(
            () => _glContext.Clear((uint)ClearBufferMask.ColorBufferBit),
            _canvas,
            _grContext,
            new(_window.Size.X, _window.Size.Y));
    }
    private void HandleWindowLoad()
    {
        if (_window is null)
        {
            logger.LogError("Window is not initialized during load.");
            throw new Exception("Window is not initialized");
        }

        _glContext = _window.CreateOpenGL();

        var glInterface = GRGlInterface.Create();
        _grContext = GRContext.CreateGl(glInterface);

        float displayDensity;
        unsafe
        {
            var glfw = Glfw.GetApi();
            var monitor = glfw.GetPrimaryMonitor();
            glfw.GetMonitorContentScale(monitor, out var monXscale, out var _);
            displayDensity = monXscale;
        }
        renderService.DisplayDensity = displayDensity;

        CreateSurface(_window.Size);

        plusUiNavigationService.Initialize();

        // Initialize accessibility with root provider that returns current page
        accessibilityService.Initialize(() => navigationContainer.CurrentPage);

        SetupInputHandling();

        SetWindowIcon();
    }
    private void HandleWindowClosing()
    {
        // Set closing flag immediately to stop render loop
        _isClosing = true;

        // Save window position and size if enabled
        if (_window != null && uiOptions.Value.RememberWindowPosition)
        {
            var isMaximized = _window.WindowState == WindowState.Maximized;
            var settings = new WindowSettings(
                _lastNormalPosition.X,
                _lastNormalPosition.Y,
                _lastNormalSize.X,
                _lastNormalSize.Y,
                isMaximized);
            windowSettingsService.Save(settings);
        }

        // Unsubscribe from input events first to prevent ObjectDisposedException
        if (_mouse is not null)
        {
            _mouse.MouseMove -= HandleMouseMove;
            _mouse.MouseDown -= HandleMouseDown;
            _mouse.MouseUp -= HandleMouseUp;
            _mouse.Scroll -= HandleMouseScroll;
            _mouse = null;
        }

        if (_keyboard is not null)
        {
            _keyboard.KeyDown -= HandleKeyDown;
            _keyboard.KeyUp -= HandleKeyUp;
            _keyboard = null;
        }

        // Dispose resources in reverse order of creation
        try
        {
            _surface?.Dispose();
            _surface = null;
        }
        catch
        {
        }

        try
        {
            _grContext?.Dispose();
            _grContext = null;
        }
        catch
        {
        }

        try
        {
            _inputContext?.Dispose();
            _inputContext = null;
        }
        catch
        {
        }

        appLifetime.StopApplication();
    }
    private void HandleWindowResize(Vector2D<int> newSize)
    {
        _surface?.Dispose();
        CreateSurface(newSize);
        navigationContainer.CurrentPage.InvalidateMeasure();

        if (_window?.WindowState == WindowState.Normal)
        {
            _lastNormalSize = newSize;
        }
    }

    private void HandleWindowStateChanged(WindowState state)
    {
        logger.LogDebug("Window state changed: {State}", state);

        // When window is restored or maximized, request a render
        if (state is WindowState.Normal or WindowState.Maximized)
        {
        }
        // When minimized or hidden, no need to render (optimization handled by event-driven mode)
    }

    private void HandleWindowFocusChanged(bool focused)
    {
        logger.LogDebug("Window focus changed: {Focused}", focused);

        // Request render when window regains focus (content might have changed)
        if (focused)
        {
        }
    }

    private void HandleWindowMove(Vector2D<int> position)
    {
        if (_window?.WindowState == WindowState.Normal)
        {
            _lastNormalPosition = position;
        }
    }
    #endregion

    #region private methods
    private void CreateSurface(Vector2D<int> size)
    {
        var frameBufferInfo = new GRGlFramebufferInfo(0, 0x8058); // 0x8058 is GL_RGBA8
        var backendRenderTarget = new GRBackendRenderTarget(
            size.X,
            size.Y,
            0, // Sample count
            0, // Stencil bits
            frameBufferInfo);

        _surface = SKSurface.Create(
            _grContext,
            backendRenderTarget,
            GRSurfaceOrigin.BottomLeft,
            SKColorType.Rgba8888);

        _canvas = _surface.Canvas;
    }
    private void SetupInputHandling()
    {
        if (_window is null)
        {
            return;
        }

        _inputContext = _window.CreateInput();
        if (_inputContext is null)
        {
            return;
        }

        // Setup mouse if available
        if (_inputContext.Mice.Count > 0)
        {
            _mouse = _inputContext.Mice[0];
            _mouse.MouseMove += HandleMouseMove;
            _mouse.MouseDown += HandleMouseDown;
            _mouse.MouseUp += HandleMouseUp;
            _mouse.Scroll += HandleMouseScroll;
        }

        // Setup keyboard if available
        if (_inputContext.Keyboards.Count > 0)
        {
            _keyboard = _inputContext.Keyboards[0];
            desktopKeyboardHandler.SetKeyboard(_keyboard);
            _keyboard.KeyDown += HandleKeyDown;
            _keyboard.KeyUp += HandleKeyUp;
        }
    }

    private void HandleMouseMove(IMouse mouse, System.Numerics.Vector2 position)
    {
        inputService.MouseMove(position / renderService.DisplayDensity);
    }

    private void HandleMouseDown(IMouse mouse, MouseButton button)
    {
        if (_mouse is null) return;

        if (button == MouseButton.Left)
        {
            inputService.MouseDown(_mouse.Position / renderService.DisplayDensity);
        }
        else if (button == MouseButton.Right)
        {
            inputService.RightClick(_mouse.Position / renderService.DisplayDensity);
        }
    }

    private void HandleMouseUp(IMouse mouse, MouseButton button)
    {
        if (_mouse is null) return;

        if (button == MouseButton.Left)
        {
            inputService.MouseUp(_mouse.Position / renderService.DisplayDensity);
        }
    }

    private void HandleMouseScroll(IMouse mouse, ScrollWheel scrollDelta)
    {
        if (_mouse is null) return;

        // Scale the scroll delta and invert Y for natural scrolling direction
        float scrollSpeed = 20f;
        float deltaX = scrollDelta.X * scrollSpeed;
        float deltaY = -scrollDelta.Y * scrollSpeed;

        inputService.MouseWheel(
            _mouse.Position / renderService.DisplayDensity,
            deltaX,
            deltaY);
    }

    private void HandleKeyDown(IKeyboard keyboard, Key key, int scanCode)
    {
        if (key == Key.ControlLeft || key == Key.ControlRight)
        {
            inputService.SetCtrlPressed(true);
        }
    }

    private void HandleKeyUp(IKeyboard keyboard, Key key, int scanCode)
    {
        if (key == Key.ControlLeft || key == Key.ControlRight)
        {
            inputService.SetCtrlPressed(false);
        }
    }

    private void SetWindowIcon()
    {
        var iconPath = uiOptions.Value.WindowIcon;
        if (string.IsNullOrEmpty(iconPath) || _window is null)
            return;

        try
        {
            SKBitmap? bitmap = null;

            if (iconPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                using var svg = new SKSvg();
                svg.Load(iconPath);
                if (svg.Picture is null) return;

                var bounds = svg.Picture.CullRect;
                var size = 64;
                var scale = size / Math.Max(bounds.Width, bounds.Height);

                bitmap = new SKBitmap(size, size);
                using var canvas = new SKCanvas(bitmap);
                canvas.Clear(SKColors.Transparent);
                canvas.Scale(scale);
                canvas.Translate(-bounds.Left, -bounds.Top);
                canvas.DrawPicture(svg.Picture);
            }
            else if (iconPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.OpenRead(iconPath);
                bitmap = SKBitmap.Decode(stream);
            }

            if (bitmap is null) return;

            var pixels = bitmap.Pixels;
            var pixelData = new byte[bitmap.Width * bitmap.Height * 4];

            for (var i = 0; i < pixels.Length; i++)
            {
                pixelData[i * 4 + 0] = pixels[i].Red;
                pixelData[i * 4 + 1] = pixels[i].Green;
                pixelData[i * 4 + 2] = pixels[i].Blue;
                pixelData[i * 4 + 3] = pixels[i].Alpha;
            }

            unsafe
            {
                var glfw = Glfw.GetApi();
                var nativeWindow = _window.Native?.Glfw;
                if (nativeWindow is null) return;

                fixed (byte* ptr = pixelData)
                {
                    var image = new Silk.NET.GLFW.Image
                    {
                        Width = bitmap.Width,
                        Height = bitmap.Height,
                        Pixels = ptr
                    };
                    glfw.SetWindowIcon((WindowHandle*)nativeWindow.Value, 1, &image);
                }
            }

            bitmap.Dispose();
            logger.LogDebug("Window icon set from {IconPath}", iconPath);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to set window icon from {IconPath}", iconPath);
        }
    }
    #endregion
}
