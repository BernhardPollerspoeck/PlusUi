using Android.Content;
using Android.Views;
using PlusUi.core;
using System.Numerics;

namespace PlusUi.droid;

internal class TapGestureListener(InputService inputService, RenderService renderService)
    : Java.Lang.Object, View.IOnTouchListener
{
    public bool OnTouch(View? v, MotionEvent? e)
    {
        var density = renderService.DisplayDensity;

        if (e?.Action is MotionEventActions.Up)
        {
            inputService.MouseUp(new Vector2(e.GetX() / density, e.GetY() / density));
            return true;
        }
        else if (e?.Action is MotionEventActions.Down)
        {
            inputService.MouseDown(new Vector2(e.GetX() / density, e.GetY() / density));
            return true;
        }
        return false;
    }
}