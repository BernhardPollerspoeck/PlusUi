using Microsoft.Extensions.Hosting;
using PlusUi.core;
using Sandbox.Pages.ControlsGrid;
using Sandbox.Pages.DataGridDemo;
using Sandbox.Pages.Form;
using Sandbox.Pages.Main;
using Sandbox.Pages.ScrollViewDemo;
using Sandbox.Pages.TextRendering;
using Sandbox.Popups;
using Sandbox.Services;

namespace Sandbox;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "Sandbox";
        configuration.Size = new SizeI(800, 600);
        //configuration.IsWindowTransparent = true;
        //configuration.WindowBorder = Silk.NET.Windowing.WindowBorder.Hidden;
        //configuration.WindowState = Silk.NET.Windowing.WindowState.Maximized;
    }
    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.StylePlusUi<DefaultStyle>();

        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
        builder.AddPage<ControlsGridPage>().WithViewModel<ControlsGridPageViewModel>();
        builder.AddPage<TextRenderPage>().WithViewModel<TextRenderPageViewModel>();
        builder.AddPage<FormDemoPage>().WithViewModel<FormDemoPageViewModel>();
        builder.AddPage<ScrollViewExamplePage>().WithViewModel<ScrollViewExamplePageViewModel>();
        builder.AddPage<DataGridDemoPage>().WithViewModel<DataGridDemoPageViewModel>();

        builder.AddPopup<TestPopup>().WithViewModel<TestPopupViewModel>();
    }

    public Type ConfigureRootPage()
    {
        return typeof(MainPage);
    }
}
