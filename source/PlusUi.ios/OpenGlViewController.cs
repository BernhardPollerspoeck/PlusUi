using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using SkiaSharp.Views.iOS;
using System.Numerics;

namespace PlusUi.ios;

public class OpenGlViewController(
    RenderService renderService,
    PlusUiNavigationService plusUiNavigationService,
    InputService inputService,
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

    }

    private void OnCanvasPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var canvasSize = new Vector2(e.Info.Width, e.Info.Height);
        renderService.Render(clearAction: null, canvas, grContext: null, canvasSize);
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
    }

    public void Invalidate()
    {
        _canvasView?.SetNeedsDisplay();
    }
}

