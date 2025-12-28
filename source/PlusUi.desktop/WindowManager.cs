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
using MouseButton = Silk.NET.Input.MouseButton;

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
    #endregion

    #region IHostedService
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var options = WindowOptions.Default with
        {
            Size = new(uiOptions.Value.Size.Width, uiOptions.Value.Size.Height),
            Title = uiOptions.Value.Title,
            Position = new(uiOptions.Value.Position.Width, uiOptions.Value.Position.Height),
            WindowState = (Silk.NET.Windowing.WindowState)uiOptions.Value.WindowState,
            WindowBorder = (Silk.NET.Windowing.WindowBorder)uiOptions.Value.WindowBorder,
            TopMost = uiOptions.Value.IsWindowTopMost,
            TransparentFramebuffer = uiOptions.Value.IsWindowTransparent,
        };

        _window = Window.Create(options);
        platformService.SetWindow(_window);

        _window.Closing += HandleWindowClosing;
        _window.Render += HandleWindowRender;
        _window.Resize += HandleWindowResize;
        _window.Load += HandleWindowLoad;

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
    }
    private void HandleWindowUpdate(double delta)
    {
        if (_mouse is not null)
        {
            if (_mouse.IsButtonPressed(MouseButton.Left))
            {
                inputService.MouseDown(_mouse.Position / renderService.DisplayDensity);
                return;
            }

            if (!_mouse.IsButtonPressed(MouseButton.Left))
            {
                inputService.MouseUp(_mouse.Position / renderService.DisplayDensity);
                return;
            }
        }
    }
    private void HandleWindowClosing()
    {
        if (_window is not null)
        {
            _window.Update -= HandleWindowUpdate;
        }

        try
        {
            _surface?.Dispose();
        }
        catch
        {
        }

        try
        {
            _grContext?.Dispose();
        }
        catch
        {
        }

        try
        {
            _inputContext?.Dispose();
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
            _mouse.MouseMove += (_, position) =>
                inputService.MouseMove(position / renderService.DisplayDensity);

            // Right-click for LongPress gesture
            _mouse.MouseDown += (_, button) =>
            {
                if (button == MouseButton.Right && _mouse is not null)
                {
                    inputService.RightClick(_mouse.Position / renderService.DisplayDensity);
                }
            };

            // Add mouse wheel event handler
            _mouse.Scroll += (_, scrollDelta) =>
            {
                if (_mouse is null) return;

                // Scale the scroll delta and invert Y for natural scrolling direction
                // Multiply by a scroll speed factor (e.g., 20) for better UX
                float scrollSpeed = 20f;
                float deltaX = scrollDelta.X * scrollSpeed;
                float deltaY = -scrollDelta.Y * scrollSpeed; // Invert Y for natural scrolling

                inputService.MouseWheel(
                    _mouse.Position / renderService.DisplayDensity,
                    deltaX,
                    deltaY);
            };
        }

        // Setup keyboard if available
        if (_inputContext.Keyboards.Count > 0)
        {
            _keyboard = _inputContext.Keyboards[0];
            desktopKeyboardHandler.SetKeyboard(_keyboard);

            // Track Ctrl key for Pinch gesture
            _keyboard.KeyDown += (_, key, _) =>
            {
                if (key == Key.ControlLeft || key == Key.ControlRight)
                {
                    inputService.SetCtrlPressed(true);
                }
            };
            _keyboard.KeyUp += (_, key, _) =>
            {
                if (key == Key.ControlLeft || key == Key.ControlRight)
                {
                    inputService.SetCtrlPressed(false);
                }
            };
        }

        // Only subscribe to Update if we have a mouse
        if (_mouse is not null)
        {
            _window.Update += HandleWindowUpdate;
        }
    }
    #endregion
}
