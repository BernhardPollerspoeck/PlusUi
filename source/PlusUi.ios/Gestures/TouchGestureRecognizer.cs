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
