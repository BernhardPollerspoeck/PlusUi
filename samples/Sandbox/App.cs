using Microsoft.Extensions.Hosting;
using PlusUi.core;
using Sandbox.Pages.ControlsGrid;
using Sandbox.Pages.Main;
using Sandbox.Pages.TextRendering;
using Sandbox.Services;

namespace Sandbox;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "Sandbox";
        configuration.Size = new SizeI(800, 600);
    }
    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.StylePlusUi<DefaultStyle>();

        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
        builder.AddPage<ControlsGridPage>().WithViewModel<ControlsGridPageViewModel>();
        builder.AddPage<TextRenderPage>().WithViewModel<TextRenderPageViewModel>();
    }
}
