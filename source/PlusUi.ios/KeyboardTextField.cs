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
        // Handle hardware keyboard keys for Tab navigation
        foreach (var press in presses)
        {
            if (press?.Key?.KeyCode != null)
            {
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
}
