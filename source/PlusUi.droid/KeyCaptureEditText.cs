using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Java.Lang;
using PlusUi.core;

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
        if (_context.GetSystemService(Context.InputMethodService) is InputMethodManager imm)
        {
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

}


