using PlusUi.core;
using System.Numerics;
using UIKit;

namespace PlusUi.ios;

internal class TouchGestureRecognizer : UIGestureRecognizer
{
    private readonly InputService _inputService;
    private readonly RenderService _renderService;

    public TouchGestureRecognizer(InputService inputService, RenderService renderService)
    {
        _inputService = inputService;
        _renderService = renderService;
        CancelsTouchesInView = false;
    }

    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
        base.TouchesBegan(touches, evt);

        if (touches.AnyObject is UITouch touch && View is not null)
        {
            var location = touch.LocationInView(View);
            var density = _renderService.DisplayDensity;
            _inputService.MouseDown(new Vector2((float)location.X / density, (float)location.Y / density));
        }
    }

    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
        base.TouchesMoved(touches, evt);

        if (touches.AnyObject is UITouch touch && View is not null)
        {
            var location = touch.LocationInView(View);
            var density = _renderService.DisplayDensity;
            _inputService.MouseMove(new Vector2((float)location.X / density, (float)location.Y / density));
        }
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
        base.TouchesEnded(touches, evt);

        if (touches.AnyObject is UITouch touch && View is not null)
        {
            var location = touch.LocationInView(View);
            var density = _renderService.DisplayDensity;
            _inputService.MouseUp(new Vector2((float)location.X / density, (float)location.Y / density));
        }
    }

    public override void TouchesCancelled(NSSet touches, UIEvent evt)
    {
        base.TouchesCancelled(touches, evt);

        if (touches.AnyObject is UITouch touch && View is not null)
        {
            var location = touch.LocationInView(View);
            var density = _renderService.DisplayDensity;
            _inputService.MouseUp(new Vector2((float)location.X / density, (float)location.Y / density));
        }
    }
}

internal class LongPressGestureRecognizer : UILongPressGestureRecognizer
{
    private readonly InputService _inputService;
    private readonly RenderService _renderService;

    public LongPressGestureRecognizer(InputService inputService, RenderService renderService)
        : base(HandleLongPress)
    {
        _inputService = inputService;
        _renderService = renderService;
        MinimumPressDuration = 0.5;
        AddTarget(OnLongPress);
    }

    private static void HandleLongPress() { }

    private void OnLongPress()
    {
        if (State == UIGestureRecognizerState.Began && View is not null)
        {
            var location = LocationInView(View);
            var density = _renderService.DisplayDensity;
            _inputService.LongPress(new Vector2((float)location.X / density, (float)location.Y / density));
        }
    }
}

internal class PinchGestureRecognizer : UIPinchGestureRecognizer
{
    private readonly InputService _inputService;
    private readonly RenderService _renderService;

    public PinchGestureRecognizer(InputService inputService, RenderService renderService)
        : base(HandlePinch)
    {
        _inputService = inputService;
        _renderService = renderService;
        AddTarget(OnPinch);
    }

    private static void HandlePinch() { }

    private void OnPinch()
    {
        if (View is null) return;

        if (State == UIGestureRecognizerState.Changed)
        {
            var location = LocationInView(View);
            var density = _renderService.DisplayDensity;
            _inputService.HandlePinch(
                new Vector2((float)location.X / density, (float)location.Y / density),
                (float)Scale);
            Scale = 1;
        }
    }
}
