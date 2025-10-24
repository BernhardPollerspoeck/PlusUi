using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.CoreElements;

namespace PlusUi.core;

//TODO: internal
public class PlusUiPopupService(IServiceProvider serviceProvider) : IPopupService
{
    internal UiPopupElement? CurrentPopup { get; private set; }

    public void ShowPopup<TPopup, TArg>(
        TArg? arg = default,
        Action? onClosed = null,
        Action<IPopupConfiguration>? configure = null)
        where TPopup : UiPopupElement<TArg>
    {
        if (CurrentPopup is not null)
        {
            ClosePopup();
        }

        var configuration = new PopupConfiguration();
        configure?.Invoke(configuration);

        var popup = serviceProvider.GetRequiredService<TPopup>();
        popup.SetConfiguration(configuration);
        popup.SetArgument(arg);
        popup.SetOnClosed(onClosed);
        popup.BuildPopup();

        CurrentPopup = popup;
    }

    public void ClosePopup(bool success = true)
    {
        if (CurrentPopup is not null)
        {
            CurrentPopup.Close(success);
            CurrentPopup.Disappearing();
            CurrentPopup = null;
        }
    }

    internal void Build()
    {
        CurrentPopup?.BuildPopup();
    }
}
