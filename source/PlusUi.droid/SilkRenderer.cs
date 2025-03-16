using Android.Content;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace PlusUi.droid;

internal class SilkRenderer(
    PlusUiNavigationService plusUiNavigationService,
    RenderService renderService,
    ILogger<SilkRenderer> logger,
    Context context)
    : Java.Lang.Object, GLSurfaceView.IRenderer
{
    private GL? _glContext;
    private GRContext? _grContext;
    private SKSurface? _surface;
    private SKCanvas? _canvas;
    private Vector2D<int>? _size;
    private bool _initialized = false;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
    [DllImport("libEGL.so")]
    static extern IntPtr eglGetProcAddress(string procname);
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
#pragma warning restore IDE0079 // Remove unnecessary suppression

    public void OnSurfaceCreated(IGL10? _, Javax.Microedition.Khronos.Egl.EGLConfig? config)
    {
        _glContext = GL.GetApi(eglGetProcAddress);
        _glContext.ClearColor(0, 0, 0, 1.0f);
        _initialized = true;
    }

    public void OnDrawFrame(IGL10? _)
    {
        if (this is not { _initialized: true, _glContext: not null, _canvas: not null, _size: not null })
        {
            logger.LogWarning("Render skipped: GL context, canvas, GR context, or window is not initialized.");
            return;
        }

        renderService.Render(
            _glContext,
            _canvas,
            _grContext,
            _size.Value);
    }

    public void OnSurfaceChanged(IGL10? _, int width, int height)
    {
        if (this is not { _initialized: true, _glContext: not null })
        {
            return;
        }

        var density = context.Resources?.DisplayMetrics?.Density ?? 1;
        renderService.DisplayDensity = density;

        _size = new Vector2D<int>(width, height);
        _glContext.Viewport(0, 0, (uint)width, (uint)height);
        var glInterface = GRGlInterface.Create();
        _grContext = GRContext.CreateGl(glInterface);

        CreateSurface(new Vector2D<int>(width, height));

        plusUiNavigationService.Initialize();
    }

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


}