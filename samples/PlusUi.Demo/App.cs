using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.Demo.Pages.Main;

namespace PlusUi.Demo;

public class App(bool loadImagesSynchronously = false) : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "PlusUi Demo";
        configuration.Size = new SizeI(1200, 800);
        configuration.LoadImagesSynchronously = loadImagesSynchronously;
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
