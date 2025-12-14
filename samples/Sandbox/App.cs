using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using Sandbox.Pages.BgTest;
using Sandbox.Pages.ButtonDemo;
using Sandbox.Pages.ControlsGrid;
using Sandbox.Pages.FontDemo;
using Sandbox.Pages.Form;
using Sandbox.Pages.ItemsListDemo;
using Sandbox.Pages.LinkDemo;
using Sandbox.Pages.Main;
using Sandbox.Pages.RawControl;
using Sandbox.Pages.ScrollViewDemo;
using Sandbox.Pages.ShadowDemo;
using Sandbox.Pages.TextRendering;
using Sandbox.Pages.TextWrapDemo;
using Sandbox.Pages.NewControlsDemo;
using Sandbox.Pages.ToolbarDemo;
using Sandbox.Popups;
using Sandbox.Services;

namespace Sandbox;

public class App(bool loadImagesSynchronously = false) : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "Sandbox";
        configuration.Size = new SizeI(1200, 800);

        configuration.LoadImagesSynchronously = loadImagesSynchronously;
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
        builder.AddPage<TextWrapDemoPage>().WithViewModel<TextWrapDemoPageViewModel>();
        builder.AddPage<FormDemoPage>().WithViewModel<FormDemoPageViewModel>();
        builder.AddPage<ScrollViewExamplePage>().WithViewModel<ScrollViewExamplePageViewModel>();
        builder.AddPage<RawUserControlPage>().WithViewModel<RawUserControlPageViewModel>();
        builder.AddPage<ItemsListDemoPage>().WithViewModel<ItemsListDemoPageViewModel>();
        builder.AddPage<ButtonDemoPage>().WithViewModel<ButtonDemoPageViewModel>();
        builder.AddPage<LinkDemoPage>().WithViewModel<LinkDemoPageViewModel>();
        builder.AddPage<FontDemoPage>().WithViewModel<FontDemoPageViewModel>();
        builder.AddPage<ShadowDemoPage>().WithViewModel<ShadowDemoPageViewModel>();
        builder.AddPage<NewControlsDemoPage>().WithViewModel<NewControlsDemoPageViewModel>();
        builder.AddPage<ToolbarDemoPage>().WithViewModel<ToolbarDemoPageViewModel>();

        builder.AddPage<BgTestPage>().WithViewModel<BgTestPageViewModel>();

        builder.AddPopup<TestPopup>().WithViewModel<TestPopupViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
