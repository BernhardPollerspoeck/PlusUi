using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PlusUi.core.UiElements;
using PlusUi.core.ViewModel;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;

namespace PlusUi;

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder UsePlusUi<TRootPage>(this HostApplicationBuilder builder)
        where TRootPage : UiPage
    {
        return builder.UsePlusUi<TRootPage>(_ => { });
    }
    public static HostApplicationBuilder UsePlusUi<TRootPage>(
        this HostApplicationBuilder builder,
        Action<PlusUiConfiguration> configurationAction)
        where TRootPage : UiPage
    {
        builder.Services.Configure(configurationAction);

        builder.Services.AddSingleton<RenderService>();

        builder.Services.AddHostedService<WindowManager>();

        builder.Services.AddSingleton(sp => new CurrentPage
        {
            Page = sp.GetRequiredService<TRootPage>()
        });

        return builder;
    }


    public static HostApplicationBuilder AddPage<TPage>(this HostApplicationBuilder builder)
        where TPage : UiPage
    {
        builder.Services.AddTransient<TPage>();
        return builder;
    }
    public static HostApplicationBuilder WithViewModel<TViewModel>(this HostApplicationBuilder builder)
        where TViewModel : ViewModelBase
    {
        builder.Services.AddTransient<TViewModel>();
        return builder;
    }
}
public class CurrentPage
{
    public required UiPage Page { get; set; }
}
public class PlusUiConfiguration
{
    public Vector2D<int> Size { get; set; } = new Vector2D<int>(800, 600);
    public string Title { get; set; } = "Plus Ui Application";

}

internal class WindowManager(
    IOptions<PlusUiConfiguration> uiOptions,
    RenderService renderService,
    CurrentPage currentPage)
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
        _window.Render += (d) => renderService.Render(_glContext!, _canvas!, _grContext!);

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
    }
}

internal class RenderService(CurrentPage rootPage)
{
    public void Render(GL gl, SKCanvas canvas, GRContext grContext)
    {
        // Clear the OpenGL buffer
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        // Clear the canvas
        canvas.Clear(SKColors.Black);

        // Render the UI
        rootPage.Page.Render(canvas, new SKPoint(0, 0));



        // Flush the surface
        canvas.Flush();
        grContext.Flush();
    }

}