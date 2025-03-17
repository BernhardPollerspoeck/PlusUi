namespace PlusUi.core;

public interface IPopupService
{
    void ShowPopup<TPopup, TArg>(
        TArg? arg = default,
        Action? onClosed = null,//TODO: is there a good way to have this with a result type?
        Action<IPopupConfiguration>? configure = null)
        where TPopup : UiPopupElement<TArg>;

    void ClosePopup(bool success = true);
}
