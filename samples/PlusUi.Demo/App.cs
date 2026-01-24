using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.Demo.Pages.EntryDemo;
using PlusUi.Demo.Pages.Main;
using PlusUi.Demo.Pages.RichTextLabelDemo;

namespace PlusUi.Demo;

public class App(bool loadImagesSynchronously = false) : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "PlusUi Demo";
        configuration.Size = new SizeI(1200, 800);
        configuration.LoadImagesSynchronously = loadImagesSynchronously;
        configuration.EnableNavigationStack = true;
        configuration.RememberWindowPosition = true;
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
        builder.AddPage<EntryPage>().WithViewModel<EntryPageViewModel>();
        builder.AddPage<RichTextLabelPage>().WithViewModel<RichTextLabelPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
