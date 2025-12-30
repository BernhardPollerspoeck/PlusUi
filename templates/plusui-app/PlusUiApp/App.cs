using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUiApp.Pages;

namespace PlusUiApp;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "PlusUiApp";
        configuration.Size = new SizeI(1200, 800);

        configuration.EnableNavigationStack = true;
        configuration.PreservePageState = true;

        // Accessibility features
        configuration.EnableHighContrastSupport = true;
        configuration.RespectReducedMotion = true;
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.StylePlusUi<DefaultStyle>();

        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
