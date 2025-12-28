using System.Numerics;
using PlusUi.core.Services.Focus;

namespace PlusUi.core;

public class InputService
{
    private bool _isMousePressed;
    private ITextInputControl? _textInputControl;
    private readonly NavigationContainer _navigationContainer;
    private readonly PlusUiPopupService _popupService;
    private readonly OverlayService _overlayService;
    private readonly IKeyboardHandler _keyboardHandler;
    private readonly ITooltipService _tooltipService;
    private readonly IFocusManager _focusManager;
    private Vector2 _lastMousePosition;
    private IScrollableControl? _activeScrollControl;
    private IDraggableControl? _activeDragControl;
    private UiElement? _hoveredElement;

    // Gesture detection
    private Vector2 _mouseDownPosition;
    private DateTime _lastTapTime;
    private UiElement? _lastTapElement;
    private const double DoubleTapThresholdMs = 300;
    private const float SwipeThreshold = 50f;
    private bool _isCtrlPressed;

    public InputService(
        NavigationContainer navigationContainer,
        PlusUiPopupService popupService,
        OverlayService overlayService,
        IKeyboardHandler keyboardHandler,
        IFocusManager focusManager,
        ITooltipService tooltipService)
    {
        _navigationContainer = navigationContainer;
        _popupService = popupService;
        _overlayService = overlayService;
        _keyboardHandler = keyboardHandler;
        _tooltipService = tooltipService;
        _focusManager = focusManager;
        _keyboardHandler.KeyInput += HandleKeyInput;
        _keyboardHandler.CharInput += HandleCharInput;
    }


    public void MouseDown(Vector2 location)
    {
        if (_isMousePressed)
        {
            return;
        }
        _isMousePressed = true;
        _lastMousePosition = location;
        _mouseDownPosition = location;

        // Check if we're starting a scroll or drag operation
        var currentPopup = _popupService.CurrentPopup;
        var hitControl = (currentPopup) switch
        {
            not null => currentPopup.HitTest(new(location.X, location.Y)),
            _ => _navigationContainer.CurrentPage.HitTest(new(location.X, location.Y))
        };

        // Check for draggable control first (higher priority than scrollable)
        if (hitControl is IDraggableControl dragControl)
        {
            _activeDragControl = dragControl;
            _activeDragControl.IsDragging = true;
        }
        else if (hitControl is IScrollableControl scrollControl)
        {
            _activeScrollControl = scrollControl;
            _activeScrollControl.IsScrolling = true;
        }
    }
    
    public void MouseUp(Vector2 location)
    {
        if (!_isMousePressed)
        {
            return;
        }
        _isMousePressed = false;

        // End any active drag operation
        _activeDragControl?.IsDragging = false;
        _activeDragControl = null;

        // End any active scrolling operation
        _activeScrollControl?.IsScrolling = false;
        _activeScrollControl = null;

        //we have an up action
        var point = new Point(location.X, location.Y);

        // Check overlays first (they render on top)
        UiElement? hitControl = null;
        bool hitOverlay = false;
        foreach (var overlay in _overlayService.Overlays)
        {
            hitControl = overlay.HitTest(point);
            if (hitControl != null)
            {
                hitOverlay = true;
                break;
            }
        }

        // If no overlay was hit, dismiss all dismissable overlays
        if (!hitOverlay && _overlayService.Overlays.Count > 0)
        {
            // Create a copy since Dismiss may modify the collection
            var overlaysToCheck = _overlayService.Overlays.ToList();
            foreach (var overlay in overlaysToCheck)
            {
                if (overlay is IDismissableOverlay dismissable)
                {
                    dismissable.Dismiss();
                }
            }
        }

        // Then check popup if no overlay was hit
        if (hitControl == null)
        {
            var currentPopup = _popupService.CurrentPopup;
            hitControl = (currentPopup) switch
            {
                not null => currentPopup.HitTest(point),
                _ => _navigationContainer.CurrentPage.HitTest(point)
            };
        }

        // Set focus to clicked control if it's focusable
        if (hitControl is IFocusable focusable && focusable.IsFocusable)
        {
            _focusManager.SetFocus(focusable);
        }
        else if (hitControl != null)
        {
            // Clicked on non-focusable control - clear focus
            _focusManager.ClearFocus();
        }

        if (hitControl is IInputControl inputControl)
        {
            inputControl.InvokeCommand();
        }

        if (hitControl is ITextInputControl textInputControl
            && textInputControl != _textInputControl)
        {
            _textInputControl?.SetSelectionStatus(false);
            _textInputControl = textInputControl;
            _textInputControl.SetSelectionStatus(true);
            
            // Pass keyboard configuration if the control is an Entry
            if (textInputControl is Entry entry)
            {
                _keyboardHandler.Show(entry.Keyboard, entry.ReturnKey, entry.IsPassword);
            }
            else
            {
                _keyboardHandler.Show();
            }
        }
        else if (_textInputControl != null && _textInputControl != hitControl)
        {
            _textInputControl.SetSelectionStatus(false);
            _textInputControl = null;
            _keyboardHandler.Hide();
        }

        if (hitControl is IToggleButtonControl toggleButtonControl)
        {
            toggleButtonControl.Toggle();
        }

        // Gesture detection: Swipe
        var deltaX = location.X - _mouseDownPosition.X;
        var deltaY = location.Y - _mouseDownPosition.Y;
        var swipeDirection = DetectSwipeDirection(deltaX, deltaY);
        if (swipeDirection != SwipeDirection.None)
        {
            var swipeControl = FindGestureControl<ISwipeGestureControl>(hitControl);
            if (swipeControl != null && (swipeControl.AllowedDirections & swipeDirection) != 0)
            {
                swipeControl.OnSwipe(swipeDirection);
                _lastTapTime = DateTime.MinValue;
                return;
            }
        }

        // Gesture detection: DoubleTap
        var now = DateTime.UtcNow;
        if (hitControl != null && hitControl == _lastTapElement &&
            (now - _lastTapTime).TotalMilliseconds < DoubleTapThresholdMs)
        {
            var doubleTapControl = FindGestureControl<IDoubleTapGestureControl>(hitControl);
            if (doubleTapControl != null)
            {
                doubleTapControl.OnDoubleTap();
                _lastTapTime = DateTime.MinValue;
                _lastTapElement = null;
                return;
            }
        }

        _lastTapTime = now;
        _lastTapElement = hitControl;
    }

    public void RightClick(Vector2 location)
    {
        var point = new Point(location.X, location.Y);
        var hitControl = HitTestAll(point);

        var longPressControl = FindGestureControl<ILongPressGestureControl>(hitControl);
        longPressControl?.OnLongPress();
    }

    private SwipeDirection DetectSwipeDirection(float deltaX, float deltaY)
    {
        var absX = Math.Abs(deltaX);
        var absY = Math.Abs(deltaY);

        if (absX < SwipeThreshold && absY < SwipeThreshold)
            return SwipeDirection.None;

        if (absX > absY)
            return deltaX > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        else
            return deltaY > 0 ? SwipeDirection.Down : SwipeDirection.Up;
    }

    private T? FindGestureControl<T>(UiElement? element) where T : class, IGestureControl
    {
        while (element != null)
        {
            if (element is T gestureControl)
                return gestureControl;
            element = element.Parent;
        }
        return null;
    }

    private UiElement? HitTestAll(Point point)
    {
        foreach (var overlay in _overlayService.Overlays)
        {
            var hit = overlay.HitTest(point);
            if (hit != null) return hit;
        }

        var popup = _popupService.CurrentPopup;
        if (popup != null)
        {
            var hit = popup.HitTest(point);
            if (hit != null) return hit;
        }

        return _navigationContainer.CurrentPage.HitTest(point);
    }

    public void HandleKeyInput(object? sender, PlusKey key)
    {
        var focused = _focusManager.FocusedElement;

        // Tab always navigates focus
        if (key == PlusKey.Tab)
        {
            _focusManager.MoveFocus(FocusNavigationDirection.Next);
            SyncTextInputWithFocus();
            return;
        }
        if (key == PlusKey.ShiftTab)
        {
            _focusManager.MoveFocus(FocusNavigationDirection.Previous);
            SyncTextInputWithFocus();
            return;
        }

        // If focused element handles keyboard input specially, let it handle arrow keys
        if (focused is IKeyboardInputHandler keyboardHandler)
        {
            if (keyboardHandler.HandleKeyboardInput(key))
            {
                return; // Control handled the key
            }
        }

        // Arrow keys for focus navigation (only if not handled by control and not in text input)
        if (_textInputControl == null)
        {
            switch (key)
            {
                case PlusKey.ArrowUp:
                    _focusManager.MoveFocus(FocusNavigationDirection.Up);
                    SyncTextInputWithFocus();
                    return;
                case PlusKey.ArrowDown:
                    _focusManager.MoveFocus(FocusNavigationDirection.Down);
                    SyncTextInputWithFocus();
                    return;
                case PlusKey.ArrowLeft:
                    _focusManager.MoveFocus(FocusNavigationDirection.Left);
                    SyncTextInputWithFocus();
                    return;
                case PlusKey.ArrowRight:
                    _focusManager.MoveFocus(FocusNavigationDirection.Right);
                    SyncTextInputWithFocus();
                    return;
            }
        }

        // Enter/Space to activate (unless in text input for Space)
        if (key == PlusKey.Enter || (key == PlusKey.Space && _textInputControl == null))
        {
            ActivateFocusedElement();
            return;
        }

        // Forward to text input control
        _textInputControl?.HandleInput(key);
    }

    /// <summary>
    /// Syncs the text input control state with the currently focused element.
    /// </summary>
    private void SyncTextInputWithFocus()
    {
        var focused = _focusManager.FocusedElement;

        // If focused element is a text input, activate it
        if (focused is ITextInputControl textInput)
        {
            if (_textInputControl != textInput)
            {
                _textInputControl?.SetSelectionStatus(false);
                _textInputControl = textInput;
                _textInputControl.SetSelectionStatus(true);

                if (textInput is Entry entry)
                {
                    _keyboardHandler.Show(entry.Keyboard, entry.ReturnKey, entry.IsPassword);
                }
                else
                {
                    _keyboardHandler.Show();
                }
            }
        }
        else if (_textInputControl != null)
        {
            // Focused element is not a text input, deactivate current text input
            _textInputControl.SetSelectionStatus(false);
            _textInputControl = null;
            _keyboardHandler.Hide();
        }
    }

    /// <summary>
    /// Activates the currently focused element (simulates a click).
    /// </summary>
    private void ActivateFocusedElement()
    {
        var focused = _focusManager.FocusedElement;
        if (focused == null)
        {
            return;
        }

        // Handle IInputControl (buttons, links)
        if (focused is IInputControl inputControl)
        {
            inputControl.InvokeCommand();
        }

        // Handle IToggleButtonControl (checkbox, toggle, radio)
        if (focused is IToggleButtonControl toggleControl)
        {
            toggleControl.Toggle();
        }
    }
    
    public void HandleCharInput(object? sender, char chr)
    {
        _textInputControl?.HandleInput(chr);
    }
    
    public void MouseMove(Vector2 location)
    {
        // Handle dragging if active
        if (_isMousePressed && _activeDragControl != null)
        {
            float deltaX = location.X - _lastMousePosition.X;
            float deltaY = location.Y - _lastMousePosition.Y;

            _activeDragControl.HandleDrag(deltaX, deltaY);
        }
        // Handle scrolling if active
        else if (_isMousePressed && _activeScrollControl != null)
        {
            float deltaX = _lastMousePosition.X - location.X;
            float deltaY = _lastMousePosition.Y - location.Y;

            _activeScrollControl.HandleScroll(deltaX, deltaY);
        }

        // Update hover state
        UpdateHoverState(location);

        _lastMousePosition = location;
    }

    private void UpdateHoverState(Vector2 location)
    {
        var point = new Point(location.X, location.Y);

        // Check overlays first (they render on top)
        UiElement? hitElement = null;
        foreach (var overlay in _overlayService.Overlays)
        {
            hitElement = overlay.HitTest(point);
            if (hitElement != null)
                break;
        }

        // Then check popup
        if (hitElement == null)
        {
            var currentPopup = _popupService.CurrentPopup;
            if (currentPopup != null)
            {
                hitElement = currentPopup.HitTest(point);
            }
        }

        // Then check page
        if (hitElement == null)
        {
            hitElement = _navigationContainer.CurrentPage.HitTest(point);
        }

        // Update hover states
        if (hitElement != _hoveredElement)
        {
            var oldElement = _hoveredElement;

            if (_hoveredElement is IHoverableControl oldHoverable)
            {
                oldHoverable.IsHovered = false;
            }

            // Notify tooltip service of hover leave
            _tooltipService.OnHoverLeave(oldElement);

            _hoveredElement = hitElement;

            if (_hoveredElement is IHoverableControl newHoverable)
            {
                newHoverable.IsHovered = true;
            }

            // Notify tooltip service of hover enter
            _tooltipService.OnHoverEnter(_hoveredElement);
        }
    }
    
    public void MouseWheel(Vector2 location, float deltaX, float deltaY)
    {
        var point = new Point(location.X, location.Y);
        var hitControl = HitTestAll(point);

        // Pinch gesture (Ctrl + MouseWheel on desktop)
        if (_isCtrlPressed)
        {
            var pinchControl = FindGestureControl<IPinchGestureControl>(hitControl);
            if (pinchControl != null)
            {
                var scale = 1.0f + (deltaY * 0.01f);
                pinchControl.OnPinch(scale);
                return;
            }
        }

        // Scroll the control if it's scrollable
        if (hitControl is IScrollableControl scrollControl)
        {
            scrollControl.HandleScroll(deltaX, deltaY);
        }
    }

    public void SetCtrlPressed(bool isPressed)
    {
        _isCtrlPressed = isPressed;
    }

    public void LongPress(Vector2 location)
    {
        RightClick(location);
    }

    public void HandlePinch(Vector2 location, float scale)
    {
        var point = new Point(location.X, location.Y);
        var hitControl = HitTestAll(point);

        var pinchControl = FindGestureControl<IPinchGestureControl>(hitControl);
        pinchControl?.OnPinch(scale);
    }
}
