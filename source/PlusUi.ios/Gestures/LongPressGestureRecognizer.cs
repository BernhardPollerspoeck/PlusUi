using PlusUi.core;
using System.Numerics;
using UIKit;

namespace PlusUi.ios;

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
