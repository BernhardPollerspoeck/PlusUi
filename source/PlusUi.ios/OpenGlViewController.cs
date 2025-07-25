using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using Silk.NET.Maths;
using SkiaSharp.Views.iOS;

namespace PlusUi.ios;

public abstract class PlusUiAppDelegate : UIApplicationDelegate
{
    private IHost? _host;

    protected abstract IAppConfiguration CreateApp(HostApplicationBuilder builder);


    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        _host = CreateAndStartHost();


        Window = new UIWindow(UIScreen.MainScreen.Bounds)
        {
            RootViewController = _host.Services.GetRequiredService<OpenGlViewController>()
        };
        Window.MakeKeyAndVisible();
        return true;
    }

    private IHost CreateAndStartHost()
    {
        var builder = Host.CreateApplicationBuilder();

        var app = CreateApp(builder);

        builder.UsePlusUiInternal(app, []);
        builder.Services.AddSingleton<OpenGlViewController>();
        //builder.Services.AddSingleton<TapGestureListener>();
        //builder.Services.AddSingleton<KeyCaptureEditText>();
        //builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<KeyCaptureEditText>());

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.Start();
        return host;
    }
}

public class OpenGlViewController(
    RenderService renderService,
    ILogger<OpenGlViewController> logger)
    : UIViewController
{
    private SKCanvasView? _canvasView;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        if (View is null)
        {
            logger.LogWarning("View is null. Cannot initialize the canvas view.");
            return;
        }

        renderService.DisplayDensity = (float)UIScreen.MainScreen.Scale;

        _canvasView = new SKCanvasView(View.Bounds)
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            Opaque = false
        };
        _canvasView.PaintSurface += OnCanvasPaintSurface;

        View.AddSubview(_canvasView);
        View.BackgroundColor = UIColor.White;
    }

    private void OnCanvasPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var canvasSize = new Vector2D<int>(e.Info.Width, e.Info.Height);
        renderService.Render(null, canvas, null, canvasSize);
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();
        if (this is { View: not null, _canvasView: not null })
        {
            _canvasView.Frame = View.Bounds;
        }
    }

    public void Invalidate()
    {
        _canvasView?.SetNeedsDisplay();
    }
}
