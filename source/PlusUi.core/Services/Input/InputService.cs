using System.Numerics;

namespace PlusUi.core;

public class InputService
{
    private bool _isMousePressed;
    private ITextInputControl? _textInputControl;
    private readonly NavigationContainer _navigationContainer;
    private readonly PlusUiPopupService _popupService;
    private readonly OverlayService _overlayService;
    private readonly IKeyboardHandler _keyboardHandler;
    private readonly ITooltipService? _tooltipService;
    private Vector2 _lastMousePosition;
    private IScrollableControl? _activeScrollControl;
    private IDraggableControl? _activeDragControl;
    private UiElement? _hoveredElement;

    public InputService(
        NavigationContainer navigationContainer,
        PlusUiPopupService popupService,
        OverlayService overlayService,
        IKeyboardHandler keyboardHandler,
        ITooltipService? tooltipService = null)
    {
        _navigationContainer = navigationContainer;
        _popupService = popupService;
        _overlayService = overlayService;
        _keyboardHandler = keyboardHandler;
        _tooltipService = tooltipService;
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

        //we have a down action
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

    }

    public void HandleKeyInput(object? sender, PlusKey key)
    {
        _textInputControl?.HandleInput(key);
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
            _tooltipService?.OnHoverLeave(oldElement);

            _hoveredElement = hitElement;

            if (_hoveredElement is IHoverableControl newHoverable)
            {
                newHoverable.IsHovered = true;
            }

            // Notify tooltip service of hover enter
            _tooltipService?.OnHoverEnter(_hoveredElement);
        }
    }
    
    public void MouseWheel(Vector2 location, float deltaX, float deltaY)
    {
        // Find the scrollable control under the mouse cursor
        var currentPopup = _popupService.CurrentPopup;
        var hitControl = (currentPopup) switch
        {
            not null => currentPopup.HitTest(new(location.X, location.Y)),
            _ => _navigationContainer.CurrentPage.HitTest(new(location.X, location.Y))
        };
        
        // Scroll the control if it's scrollable
        if (hitControl is IScrollableControl scrollControl)
        {
            scrollControl.HandleScroll(deltaX, deltaY);
        }
    }
}
