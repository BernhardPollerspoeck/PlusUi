using Android.Content;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using PlusUi.core;
using KeyboardType = PlusUi.core.KeyboardType;

namespace PlusUi.droid;

public class KeyCaptureEditText : EditText, IKeyboardHandler
{
    private readonly Context _context;

    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;


    public KeyCaptureEditText(Context context) : base(context)
    {
        Focusable = true;
        FocusableInTouchMode = true;
        Visibility = ViewStates.Visible;
        Alpha = 0;
        SetBackgroundColor(Android.Graphics.Color.Transparent);
        _context = context;
    }

    public void Hide()
    {
        if (_context.GetSystemService(Context.InputMethodService) is InputMethodManager imm)
        {
            Text = string.Empty;
            imm.HideSoftInputFromWindow(WindowToken, HideSoftInputFlags.None);
        }
    }
    public void Show()
    {
        Show(KeyboardType.Default, ReturnKeyType.Default, false);
    }

    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword)
    {
        if (_context.GetSystemService(Context.InputMethodService) is InputMethodManager imm)
        {
            // Set input type based on keyboard type
            var inputType = keyboardType switch
            {
                KeyboardType.Numeric => InputTypes.ClassNumber,
                KeyboardType.Email => InputTypes.ClassText | InputTypes.TextVariationEmailAddress,
                KeyboardType.Telephone => InputTypes.ClassPhone,
                KeyboardType.Url => InputTypes.ClassText | InputTypes.TextVariationUri,
                _ => InputTypes.ClassText
            };

            // Add password flag if needed
            if (isPassword)
            {
                inputType |= InputTypes.TextVariationPassword;
            }

            InputType = inputType;

            // Set IME options based on return key type
            ImeOptions = returnKeyType switch
            {
                ReturnKeyType.Go => ImeAction.Go,
                ReturnKeyType.Send => ImeAction.Send,
                ReturnKeyType.Search => ImeAction.Search,
                ReturnKeyType.Next => ImeAction.Next,
                ReturnKeyType.Done => ImeAction.Done,
                _ => ImeAction.None
            };

            RequestFocus();
            imm.ShowSoftInput(this, ShowFlags.Forced);
        }
    }

    protected override void OnTextChanged(ICharSequence? text, int start, int lengthBefore, int lengthAfter)
    {
        if (text == null)
        {
            base.OnTextChanged(text, start, lengthBefore, lengthAfter);
            return;
        }


        var newText = text.ToString();

        if (lengthAfter > lengthBefore)
        {
            var addedChar = newText[^1];


            if (addedChar == '\n')
            {
                KeyInput?.Invoke(this, PlusKey.Enter);
            }
            else
            {
                CharInput?.Invoke(this, addedChar);
            }
        }
        else if (lengthBefore > lengthAfter)
        {
            KeyInput?.Invoke(this, PlusKey.Backspace);
        }
        base.OnTextChanged(text, start, lengthBefore, lengthAfter);

    }

    public override bool OnKeyDown(Keycode keyCode, KeyEvent? e)
    {
        // Handle Tab navigation for hardware keyboards
        if (e != null)
        {
            var plusKey = keyCode switch
            {
                Keycode.Tab when e.IsShiftPressed => PlusKey.ShiftTab,
                Keycode.Tab => PlusKey.Tab,
                Keycode.Escape => PlusKey.Escape,
                Keycode.DpadUp => PlusKey.ArrowUp,
                Keycode.DpadDown => PlusKey.ArrowDown,
                Keycode.DpadLeft => PlusKey.ArrowLeft,
                Keycode.DpadRight => PlusKey.ArrowRight,
                _ => PlusKey.Unknown
            };

            if (plusKey != PlusKey.Unknown)
            {
                KeyInput?.Invoke(this, plusKey);
                return true;
            }
        }

        return base.OnKeyDown(keyCode, e);
    }

}


