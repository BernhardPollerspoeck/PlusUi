﻿using System.Numerics;

namespace PlusUi.core;

public class InputService
{
    private bool _isMousePressed;
    private ITextInputControl? _textInputControl;
    private readonly NavigationContainer _navigationContainer;
    private readonly PlusUiPopupService _popupService;
    private readonly IKeyboardHandler _keyboardHandler;

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

        //we have a down action
    }
    public void MouseUp(Vector2 location)
    {
        if (!_isMousePressed)
        {
            return;
        }
        _isMousePressed = false;

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

}
