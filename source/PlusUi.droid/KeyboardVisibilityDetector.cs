using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.droid;

internal class KeyboardVisibilityDetector(
    Activity activity, 
    InputService inputService) 
    : IHostedService
{
    public event EventHandler<bool>? KeyboardVisibilityChanged;

    private bool _isKeyboardVisible = false;
    private float _previousHeight = 0;
    private Android.Content.Res.Orientation? _previousOrientation;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var rootView = activity.Window?.DecorView.RootView;
        if (rootView?.ViewTreeObserver is not null)
        {
            rootView.ViewTreeObserver.GlobalLayout += OnGlobalLayout;
        }
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        var rootView = activity.Window?.DecorView.RootView;
        if (rootView?.ViewTreeObserver is not null)
        {
            rootView.ViewTreeObserver.GlobalLayout -= OnGlobalLayout;
        }
        return Task.CompletedTask;
    }

    private void OnGlobalLayout(object? sender, EventArgs e)
    {
        var rect = new Android.Graphics.Rect();
        activity.Window?.DecorView.GetWindowVisibleDisplayFrame(rect);
        var screenHeight = activity.Window?.DecorView.RootView?.Height;

        var keyboardHeight = screenHeight - rect.Bottom;
        var currentOrientation = activity.Resources?.Configuration?.Orientation;

        if (keyboardHeight.HasValue
            && Math.Abs(keyboardHeight.Value - _previousHeight) > 200
            && currentOrientation == _previousOrientation)
        {
            _previousHeight = keyboardHeight.Value;
            var isKeyboardNowVisible = keyboardHeight > screenHeight * 0.15; 

            if (_isKeyboardVisible != isKeyboardNowVisible)
            {
                _isKeyboardVisible = isKeyboardNowVisible;
                if(!_isKeyboardVisible)
                {
                    inputService.KeyboardExternallyClosed();
                }

            }
        }

        _previousOrientation = currentOrientation;
    }

}