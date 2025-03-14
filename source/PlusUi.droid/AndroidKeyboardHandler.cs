using Android.Content;
using Android.Views.InputMethods;
using PlusUi.core;

namespace PlusUi.droid;

internal class AndroidKeyboardHandler(
    Context context, 
    Activity activity) 
    : IKeyboardHandler
{
    public event EventHandler<Silk.NET.Input.Key>? KeyInput;
    public event EventHandler<char>? CharInput;

    public void Hide()
    {
        var window = activity?.Window?.DecorView?.WindowToken;
        if (context.GetSystemService(Context.InputMethodService) is InputMethodManager imm 
            && window != null)
        {
            imm.HideSoftInputFromWindow(window, HideSoftInputFlags.ImplicitOnly);
        }
    }
    public void Show()
    {
        var window = activity?.Window?.DecorView;
        if (context.GetSystemService(Context.InputMethodService) is InputMethodManager imm
            && window != null)
        {
            imm.ShowSoftInput(window, ShowFlags.Forced);
        }
    }


}
