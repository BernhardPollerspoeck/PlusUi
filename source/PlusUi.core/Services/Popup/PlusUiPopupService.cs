using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core.CoreElements;

namespace PlusUi.core;

internal class PlusUiPopupService(IServiceProvider serviceProvider, ILogger<PlusUiPopupService>? logger = null) : IPopupService
{
    internal UiPopupElement? CurrentPopup { get; private set; }
    private readonly ILogger<PlusUiPopupService>? _logger = logger;

    public void ShowPopup<TPopup, TArg>(
        TArg? arg = default,
        Action? onClosed = null,
        Action<IPopupConfiguration>? configure = null)
        where TPopup : UiPopupElement<TArg>
    {
        if (CurrentPopup is not null)
        {
            _logger?.LogDebug("Closing existing popup before showing new popup of type {PopupType}", typeof(TPopup).Name);
            ClosePopup();
        }

        var configuration = new PopupConfiguration();
        configure?.Invoke(configuration);

        try
        {
            var popup = serviceProvider.GetRequiredService<TPopup>();
            popup.SetConfiguration(configuration);
            popup.SetArgument(arg);
            popup.SetOnClosed(onClosed);
            popup.BuildPopup();

            CurrentPopup = popup;
            _logger?.LogDebug("Showing popup of type {PopupType}", typeof(TPopup).Name);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to show popup of type {PopupType}. Ensure the popup is registered in the service collection.", typeof(TPopup).Name);
            throw new InvalidOperationException($"Failed to show popup of type {typeof(TPopup).Name}. Ensure it is registered via services.AddTransient<{typeof(TPopup).Name}>()", ex);
        }
    }

    public void ClosePopup(bool success = true)
    {
        if (CurrentPopup is not null)
        {
            var popupType = CurrentPopup.GetType().Name;
            CurrentPopup.Close(success);
            CurrentPopup.Disappearing();
            CurrentPopup = null;
            _logger?.LogDebug("Closed popup of type {PopupType} with success={Success}", popupType, success);
        }
    }

    internal void Build()
    {
        CurrentPopup?.BuildPopup();
    }
}
