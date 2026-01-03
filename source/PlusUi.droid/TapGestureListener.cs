using Android.Views;
using PlusUi.core;
using PlusUi.core.Services.Rendering;
using System.Numerics;

namespace PlusUi.droid;

internal class TapGestureListener : Java.Lang.Object,
    View.IOnTouchListener,
    GestureDetector.IOnGestureListener,
    ScaleGestureDetector.IOnScaleGestureListener
{
    private readonly InputService _inputService;
    private readonly RenderService _renderService;
    private readonly InvalidationTracker _invalidationTracker;
    private readonly GestureDetector _gestureDetector;
    private readonly ScaleGestureDetector _scaleDetector;
    private bool _isScaling;

    public TapGestureListener(
        InputService inputService,
        RenderService renderService,
        InvalidationTracker invalidationTracker,
        Android.Content.Context context)
    {
        _inputService = inputService;
        _renderService = renderService;
        _invalidationTracker = invalidationTracker;
        _gestureDetector = new GestureDetector(context, this);
        _scaleDetector = new ScaleGestureDetector(context, this);
    }

    public bool OnTouch(View? v, MotionEvent? e)
    {
        if (e is null) return false;

        var density = _renderService.DisplayDensity;

        _scaleDetector.OnTouchEvent(e);
        _gestureDetector.OnTouchEvent(e);

        if (_isScaling)
        {
            return true;
        }

        if (e.Action is MotionEventActions.Up)
        {
            _inputService.MouseUp(new Vector2(e.GetX() / density, e.GetY() / density));
            _invalidationTracker.RequestRender();
            return true;
        }
        else if (e.Action is MotionEventActions.Down)
        {
            _inputService.MouseDown(new Vector2(e.GetX() / density, e.GetY() / density));
            _invalidationTracker.RequestRender();
            return true;
        }
        else if (e.Action is MotionEventActions.Move)
        {
            _inputService.MouseMove(new Vector2(e.GetX() / density, e.GetY() / density));
            _invalidationTracker.RequestRender();
            return true;
        }
        return false;
    }

    #region GestureDetector.IOnGestureListener
    public bool OnDown(MotionEvent e) => true;

    public bool OnFling(MotionEvent? e1, MotionEvent e2, float velocityX, float velocityY) => false;

    public void OnLongPress(MotionEvent e)
    {
        var density = _renderService.DisplayDensity;
        _inputService.LongPress(new Vector2(e.GetX() / density, e.GetY() / density));
        _invalidationTracker.RequestRender();
    }

    public bool OnScroll(MotionEvent? e1, MotionEvent e2, float distanceX, float distanceY) => false;

    public void OnShowPress(MotionEvent e) { }

    public bool OnSingleTapUp(MotionEvent e) => false;
    #endregion

    #region ScaleGestureDetector.IOnScaleGestureListener
    public bool OnScale(ScaleGestureDetector detector)
    {
        var density = _renderService.DisplayDensity;
        var location = new Vector2(detector.FocusX / density, detector.FocusY / density);
        _inputService.HandlePinch(location, detector.ScaleFactor);
        _invalidationTracker.RequestRender();
        return true;
    }

    public bool OnScaleBegin(ScaleGestureDetector detector)
    {
        _isScaling = true;
        return true;
    }

    public void OnScaleEnd(ScaleGestureDetector detector)
    {
        _isScaling = false;
    }
    #endregion
}
