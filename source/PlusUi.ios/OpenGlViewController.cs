using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.Rendering;
using PlusUi.ios.Accessibility;
using SkiaSharp.Views.iOS;
using System.Numerics;

namespace PlusUi.ios;

public abstract class PlusUiAppDelegate : UIApplicationDelegate
{
    private IHost? _host;

    protected abstract IAppConfiguration CreateApp(HostApplicationBuilder builder);


    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
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
        builder.Services.AddSingleton<IosPlatformService>();
        builder.Services.AddSingleton<IPlatformService>(sp => sp.GetRequiredService<IosPlatformService>());
        builder.Services.AddSingleton<IosHapticService>();
        builder.Services.AddSingleton<IHapticService>(sp => sp.GetRequiredService<IosHapticService>());
        builder.Services.AddSingleton<OpenGlViewController>();
        builder.Services.AddSingleton<KeyboardTextField>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<KeyboardTextField>());

        // Register iOS accessibility bridge (VoiceOver support)
        builder.Services.AddSingleton<IAccessibilityBridge, IosAccessibilityBridge>();

        // Register iOS accessibility settings service (Dynamic Type, high contrast, etc.)
        builder.Services.AddSingleton<IAccessibilitySettingsService, IosAccessibilitySettingsService>();

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.InitializePlusUi();
        host.Start();
        return host;
    }
}

public class OpenGlViewController(
    RenderService renderService,
    PlusUiNavigationService plusUiNavigationService,
    InputService inputService,
    InvalidationTracker invalidationTracker,
    RenderLoopService renderLoopService,
    KeyboardTextField keyboardTextField,
    IosPlatformService platformService,
    NavigationContainer navigationContainer,
    IAccessibilityService accessibilityService,
    ILogger<OpenGlViewController> logger)
    : UIViewController
{
    private SKCanvasView? _canvasView;
    private TouchGestureRecognizer? _gestureRecognizer;
    private LongPressGestureRecognizer? _longPressRecognizer;
    private PinchGestureRecognizer? _pinchRecognizer;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        if (View is null)
        {
            logger.LogWarning("View is null. Cannot initialize the canvas view.");
            return;
        }

        renderService.DisplayDensity = (float)UIScreen.MainScreen.Scale;
        platformService.SetWindowSize((float)View.Bounds.Width, (float)View.Bounds.Height);

        _canvasView = new SKCanvasView(View.Bounds)
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            Opaque = false
        };
        _canvasView.PaintSurface += OnCanvasPaintSurface;

        View.AddSubview(_canvasView);
        View.BackgroundColor = UIColor.White;

        // Add touch gesture recognizer
        _gestureRecognizer = new TouchGestureRecognizer(inputService, renderService);
        View.AddGestureRecognizer(_gestureRecognizer);

        // Add long press gesture recognizer
        _longPressRecognizer = new LongPressGestureRecognizer(inputService, renderService);
        View.AddGestureRecognizer(_longPressRecognizer);

        // Add pinch gesture recognizer
        _pinchRecognizer = new PinchGestureRecognizer(inputService, renderService);
        View.AddGestureRecognizer(_pinchRecognizer);

        // Add invisible keyboard text field
        keyboardTextField.Frame = new CGRect(0, 0, 1, 1);
        View.AddSubview(keyboardTextField);

        // Initialize navigation service
        plusUiNavigationService.Initialize();

        // Initialize accessibility with root provider that returns current page
        accessibilityService.Initialize(() => navigationContainer.CurrentPage);

        // Subscribe to cross-platform render loop for continuous rendering (animations, transitions, etc.)
        renderLoopService.RenderRequested += (_, _) => _canvasView?.SetNeedsDisplay();
    }

    private void OnCanvasPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        // Skip rendering if nothing needs to be rendered
        if (!invalidationTracker.NeedsRendering)
        {
            return;
        }

        var canvas = e.Surface.Canvas;
        var canvasSize = new Vector2(e.Info.Width, e.Info.Height);
        renderService.Render(clearAction: null, canvas, grContext: null, canvasSize);

        // Notify tracker that we rendered a frame (clears manual render requests)
        invalidationTracker.FrameRendered();
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();
        if (this is { View: not null, _canvasView: not null })
        {
            _canvasView.Frame = View.Bounds;
            platformService.SetWindowSize((float)View.Bounds.Width, (float)View.Bounds.Height);

            // Invalidate layout when view bounds change (rotation, size change)
            navigationContainer.CurrentPage.InvalidateMeasure();

            // Request render to display layout changes
            invalidationTracker.RequestRender();
        }
    }

    public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
    {
        base.TraitCollectionDidChange(previousTraitCollection);

        // Trait collection changed (DPI scale, size class, dark mode, etc.)
        logger.LogDebug("Trait collection changed");

        // Update display density if scale changed
        var newScale = (float)UIScreen.MainScreen.Scale;
        if (Math.Abs(renderService.DisplayDensity - newScale) > 0.01f)
        {
            renderService.DisplayDensity = newScale;
            navigationContainer.CurrentPage.InvalidateMeasure();
        }

        // Request render for trait changes
        invalidationTracker.RequestRender();
    }

    public void Invalidate()
    {
        _canvasView?.SetNeedsDisplay();
    }
}

