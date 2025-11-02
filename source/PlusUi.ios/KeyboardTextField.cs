using PlusUi.core;
using UIKit;
using Foundation;
using KeyboardType = PlusUi.core.KeyboardType;

namespace PlusUi.ios;

public class KeyboardTextField : UITextField, IKeyboardHandler, IUITextFieldDelegate
{
    private string _previousText = string.Empty;

    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;

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
        Show(KeyboardType.Default, ReturnKeyType.Default, false);
    }

    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword)
    {
        // Set keyboard type
        KeyboardType = keyboardType switch
        {
            KeyboardType.Numeric => UIKeyboardType.NumberPad,
            KeyboardType.Email => UIKeyboardType.EmailAddress,
            KeyboardType.Telephone => UIKeyboardType.PhonePad,
            KeyboardType.Url => UIKeyboardType.Url,
            _ => UIKeyboardType.Default
        };

        // Set return key type
        ReturnKeyType = returnKeyType switch
        {
            ReturnKeyType.Go => UIReturnKeyType.Go,
            ReturnKeyType.Send => UIReturnKeyType.Send,
            ReturnKeyType.Search => UIReturnKeyType.Search,
            ReturnKeyType.Next => UIReturnKeyType.Next,
            ReturnKeyType.Done => UIReturnKeyType.Done,
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
    public bool ShouldReturn(UITextField textField)
    {
        KeyInput?.Invoke(this, PlusKey.Enter);
        return true;
    }

    [Export("textField:shouldChangeCharactersInRange:replacementString:")]
    public bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
    {
        return true;
    }
}
