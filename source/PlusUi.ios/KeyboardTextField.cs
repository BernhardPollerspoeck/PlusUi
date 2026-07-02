using PlusUi.core;
using PlusKeyboardType = PlusUi.core.KeyboardType;
using PlusReturnKeyType = PlusUi.core.ReturnKeyType;

namespace PlusUi.ios;

public class KeyboardTextField : UITextField, IKeyboardHandler, IUITextFieldDelegate
{
    private string _previousText = string.Empty;

    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;
    public event EventHandler<bool>? ShiftStateChanged;
    public event EventHandler<bool>? CtrlStateChanged;
    public event EventHandler<PlusKey>? RawKeyDown;
    public event EventHandler<PlusKey>? RawKeyUp;

    public KeyboardTextField()
    {
        Hidden = false;
        Alpha = 0;
        BackgroundColor = UIColor.Clear;
        AutocorrectionType = UITextAutocorrectionType.No;
        AutocapitalizationType = UITextAutocapitalizationType.None;
        Delegate = this;

        // Subscribe to text changed notifications
        AddTarget(OnTextChanged, UIControlEvent.EditingChanged);
    }

    public void Hide()
    {
        Text = string.Empty;
        _previousText = string.Empty;
        ResignFirstResponder();
    }

    public void Show()
    {
        Show(PlusKeyboardType.Default, PlusReturnKeyType.Default, false);
    }

    public void Show(PlusKeyboardType keyboardType, PlusReturnKeyType returnKeyType, bool isPassword)
    {
        // Set keyboard type
        KeyboardType = keyboardType switch
        {
            PlusKeyboardType.Numeric => UIKeyboardType.NumberPad,
            PlusKeyboardType.Email => UIKeyboardType.EmailAddress,
            PlusKeyboardType.Telephone => UIKeyboardType.PhonePad,
            PlusKeyboardType.Url => UIKeyboardType.Url,
            _ => UIKeyboardType.Default
        };

        // Set return key type
        ReturnKeyType = returnKeyType switch
        {
            PlusReturnKeyType.Go => UIReturnKeyType.Go,
            PlusReturnKeyType.Send => UIReturnKeyType.Send,
            PlusReturnKeyType.Search => UIReturnKeyType.Search,
            PlusReturnKeyType.Next => UIReturnKeyType.Next,
            PlusReturnKeyType.Done => UIReturnKeyType.Done,
            _ => UIReturnKeyType.Default
        };

        // Set secure text entry for password
        SecureTextEntry = isPassword;

        BecomeFirstResponder();
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        var currentText = Text ?? string.Empty;

        if (currentText.Length > _previousText.Length)
        {
            // Characters were added
            var addedChar = currentText[^1];

            if (addedChar == '\n')
            {
                KeyInput?.Invoke(this, PlusKey.Enter);
            }
            else
            {
                CharInput?.Invoke(this, addedChar);
            }
        }
        else if (currentText.Length < _previousText.Length)
        {
            // Character was deleted
            KeyInput?.Invoke(this, PlusKey.Backspace);
        }

        _previousText = currentText;
    }

    [Export("textFieldShouldReturn:")]
    public new bool ShouldReturn(UITextField textField)
    {
        KeyInput?.Invoke(this, PlusKey.Enter);
        return true;
    }

    [Export("textField:shouldChangeCharactersInRange:replacementString:")]
    public new bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
    {
        return true;
    }

    public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        foreach (var press in presses)
        {
            if (press?.Key?.KeyCode != null)
            {
                // Raw, unfiltered key-down for the global input bus (full key set incl. modifiers).
                var rawKey = MapHidUsageToRawPlusKey(press.Key.KeyCode);
                if (rawKey != PlusKey.Unknown)
                {
                    RawKeyDown?.Invoke(this, rawKey);
                }

                // Track modifier key states
                if (press.Key.KeyCode == UIKeyboardHidUsage.KeyboardLeftShift ||
                    press.Key.KeyCode == UIKeyboardHidUsage.KeyboardRightShift)
                {
                    ShiftStateChanged?.Invoke(this, true);
                    return;
                }
                if (press.Key.KeyCode == UIKeyboardHidUsage.KeyboardLeftControl ||
                    press.Key.KeyCode == UIKeyboardHidUsage.KeyboardRightControl)
                {
                    CtrlStateChanged?.Invoke(this, true);
                    return;
                }

                var plusKey = press.Key.KeyCode switch
                {
                    UIKeyboardHidUsage.KeyboardTab when press.Key.ModifierFlags.HasFlag(UIKeyModifierFlags.Shift) => PlusKey.ShiftTab,
                    UIKeyboardHidUsage.KeyboardTab => PlusKey.Tab,
                    UIKeyboardHidUsage.KeyboardEscape => PlusKey.Escape,
                    UIKeyboardHidUsage.KeyboardUpArrow => PlusKey.ArrowUp,
                    UIKeyboardHidUsage.KeyboardDownArrow => PlusKey.ArrowDown,
                    UIKeyboardHidUsage.KeyboardLeftArrow => PlusKey.ArrowLeft,
                    UIKeyboardHidUsage.KeyboardRightArrow => PlusKey.ArrowRight,
                    _ => PlusKey.Unknown
                };

                if (plusKey != PlusKey.Unknown)
                {
                    KeyInput?.Invoke(this, plusKey);
                    return;
                }
            }
        }

        base.PressesBegan(presses, evt);
    }

    public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        foreach (var press in presses)
        {
            if (press?.Key?.KeyCode != null)
            {
                // Raw, unfiltered key-up for the global input bus.
                var rawKey = MapHidUsageToRawPlusKey(press.Key.KeyCode);
                if (rawKey != PlusKey.Unknown)
                {
                    RawKeyUp?.Invoke(this, rawKey);
                }

                if (press.Key.KeyCode == UIKeyboardHidUsage.KeyboardLeftShift ||
                    press.Key.KeyCode == UIKeyboardHidUsage.KeyboardRightShift)
                {
                    ShiftStateChanged?.Invoke(this, false);
                    return;
                }
                if (press.Key.KeyCode == UIKeyboardHidUsage.KeyboardLeftControl ||
                    press.Key.KeyCode == UIKeyboardHidUsage.KeyboardRightControl)
                {
                    CtrlStateChanged?.Invoke(this, false);
                    return;
                }
            }
        }

        base.PressesEnded(presses, evt);
    }

    /// <summary>
    /// Maps a UIKit HID usage to the full <see cref="PlusKey"/> set for the raw global input bus.
    /// </summary>
    private static PlusKey MapHidUsageToRawPlusKey(UIKeyboardHidUsage usage)
    {
        if (usage >= UIKeyboardHidUsage.KeyboardA && usage <= UIKeyboardHidUsage.KeyboardZ)
            return PlusKey.A + (int)(usage - UIKeyboardHidUsage.KeyboardA);
        // HID orders the number row as 1-9 then 0.
        if (usage >= UIKeyboardHidUsage.Keyboard1 && usage <= UIKeyboardHidUsage.Keyboard9)
            return PlusKey.D1 + (int)(usage - UIKeyboardHidUsage.Keyboard1);
        if (usage >= UIKeyboardHidUsage.KeyboardF1 && usage <= UIKeyboardHidUsage.KeyboardF12)
            return PlusKey.F1 + (int)(usage - UIKeyboardHidUsage.KeyboardF1);

        return usage switch
        {
            UIKeyboardHidUsage.Keyboard0 => PlusKey.D0,
            UIKeyboardHidUsage.KeyboardSpacebar => PlusKey.Space,
            UIKeyboardHidUsage.KeyboardReturnOrEnter or UIKeyboardHidUsage.KeypadEnter => PlusKey.Enter,
            UIKeyboardHidUsage.KeyboardTab => PlusKey.Tab,
            UIKeyboardHidUsage.KeyboardEscape => PlusKey.Escape,
            UIKeyboardHidUsage.KeyboardDeleteOrBackspace => PlusKey.Backspace,
            UIKeyboardHidUsage.KeyboardDeleteForward => PlusKey.Delete,
            UIKeyboardHidUsage.KeyboardHome => PlusKey.Home,
            UIKeyboardHidUsage.KeyboardEnd => PlusKey.End,
            UIKeyboardHidUsage.KeyboardUpArrow => PlusKey.ArrowUp,
            UIKeyboardHidUsage.KeyboardDownArrow => PlusKey.ArrowDown,
            UIKeyboardHidUsage.KeyboardLeftArrow => PlusKey.ArrowLeft,
            UIKeyboardHidUsage.KeyboardRightArrow => PlusKey.ArrowRight,
            UIKeyboardHidUsage.KeyboardLeftShift => PlusKey.LeftShift,
            UIKeyboardHidUsage.KeyboardRightShift => PlusKey.RightShift,
            UIKeyboardHidUsage.KeyboardLeftControl => PlusKey.LeftCtrl,
            UIKeyboardHidUsage.KeyboardRightControl => PlusKey.RightCtrl,
            UIKeyboardHidUsage.KeyboardLeftAlt => PlusKey.LeftAlt,
            UIKeyboardHidUsage.KeyboardRightAlt => PlusKey.RightAlt,
            _ => PlusKey.Unknown
        };
    }
}
