using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.DebugServer.Pages;

namespace PlusUi.DebugServer;

public class DebugServerApp : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "PlusUi Debug Server";
        configuration.Size = new SizeI(1400, 900);
        configuration.EnableNavigationStack = false;
        configuration.PreservePageState = false;
        configuration.WindowIcon = "plusui.svg";
        configuration.RememberWindowPosition = true;
        configuration.ApplicationId = "PlusUi.DebugServer";
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        // Register MainViewModel as Singleton to prevent duplicate event subscriptions
        builder.Services.AddSingleton<MainViewModel>();
        builder.AddPage<MainPage>().WithViewModel<MainViewModel>();

        // Register PropertyEditor Popup
        builder.Services.AddTransient<PropertyEditorPopupViewModel>();
        builder.Services.AddTransient<PropertyEditorPopup>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
