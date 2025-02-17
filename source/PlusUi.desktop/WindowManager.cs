using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PlusUi.core;
using PlusUi.core.Services;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;

namespace PlusUi.desktop;

internal class WindowManager(
    IOptions<PlusUiConfiguration> uiOptions,
    RenderService renderService,
    UpdateService updateService,
    CurrentPage currentPage,
    IHostApplicationLifetime appLifetime)
    : IHostedService
{
    private IWindow? _window;
    private GL? _glContext;
    private GRContext? _grContext;
    private SKSurface? _surface;
    private SKCanvas? _canvas;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var options = WindowOptions.Default with
        {
            Size = uiOptions.Value.Size,
            Title = uiOptions.Value.Title,
            FramesPerSecond = 0,//TODO: trigger render
            UpdatesPerSecond = 0//TODO: trigger update
        };

        _window = Window.Create(options);

        _window.Closing += HandleWindowClosing;
        _window.Render += (d)
            => renderService.Render(
                _glContext!,
                _canvas!,
                _grContext!,
                _window.Size);


        _window.Load += () =>
            {
                _glContext = _window.CreateOpenGL();

                var glInterface = GRGlInterface.Create();
                _grContext = GRContext.CreateGl(glInterface);

                var frameBufferInfo = new GRGlFramebufferInfo(0, 0x8058); // 0x8058 is GL_RGBA8
                var backendRenderTarget = new GRBackendRenderTarget(
                _window.Size.X,
                _window.Size.Y,
                0, // Sample count
                0, // Stencil bits
                frameBufferInfo);

                _surface = SKSurface.Create(
                _grContext,
                backendRenderTarget,
                GRSurfaceOrigin.BottomLeft,
                SKColorType.Rgba8888);
                _canvas = _surface.Canvas;
                currentPage.Page.ViewModel.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName is not null)
                    {
                        currentPage.Page.UpdateBindings(e.PropertyName);
                    }
                };
                currentPage.Page.BuildPage();


                var mouse = _window.CreateInput().Mice.FirstOrDefault();
                if (mouse is not null)
                {
                    _window.Update += (d)
                        => updateService.Update(mouse);
                }

            };

        _window.Run();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _window?.Close();
        return Task.CompletedTask;
    }

    private void HandleWindowClosing()
    {
        _surface?.Dispose();
        _grContext?.Dispose();
        appLifetime.StopApplication();
    }
}
