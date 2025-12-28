using PlusUi.core;
using System.Numerics;
using UIKit;

namespace PlusUi.ios;

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
