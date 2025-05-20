﻿using System.Numerics;

namespace PlusUi.core;

public class InputService
{
    private bool _isMousePressed;
    private ITextInputControl? _textInputControl;
    private readonly NavigationContainer _navigationContainer;
    private readonly PlusUiPopupService _popupService;
    private readonly IKeyboardHandler _keyboardHandler;
    private Vector2 _lastMousePosition;
    private IScrollableControl? _activeScrollControl;

    public InputService(
        NavigationContainer navigationContainer,
        PlusUiPopupService popupService,
        IKeyboardHandler keyboardHandler)
    {
        _navigationContainer = navigationContainer;
        _popupService = popupService;
        _keyboardHandler = keyboardHandler;
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

        // Check if we're starting a scroll operation
        var currentPopup = _popupService.CurrentPopup;
        var hitControl = (currentPopup) switch
        {
            not null => currentPopup.HitTest(new(location.X, location.Y)),
            _ => _navigationContainer.Page.HitTest(new(location.X, location.Y))
        };
        
        if (hitControl is IScrollableControl scrollControl)
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
        
        // End any active scrolling operation
        if (_activeScrollControl != null)
        {
            _activeScrollControl.IsScrolling = false;
            _activeScrollControl = null;
        }

        //we have an up action
        var currentPopup = _popupService.CurrentPopup;

        var hitControl = (currentPopup) switch
        {
            not null => currentPopup.HitTest(new(location.X, location.Y)),
            _ => _navigationContainer.Page.HitTest(new(location.X, location.Y))
        };
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
            _keyboardHandler.Show();
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
        //TODO: keyInputControl
        _textInputControl?.HandleInput(key);
    }
    public void HandleCharInput(object? sender, char chr)
    {
        _textInputControl?.HandleInput(chr);
    }
    
    public void MouseMove(Vector2 location)
    {
        // Handle scrolling if active
        if (_isMousePressed && _activeScrollControl != null)
        {
            float deltaX = _lastMousePosition.X - location.X;
            float deltaY = _lastMousePosition.Y - location.Y;
            
            _activeScrollControl.HandleScroll(deltaX, deltaY);
        }
        
        _lastMousePosition = location;
    }

}
