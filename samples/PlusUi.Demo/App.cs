using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.Demo.Pages.Controls;
using PlusUi.Demo.Pages.Main;
using PlusUi.Demo.Pages.Shared;

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
        configuration.ApplicationId = "PlusUi.Demo";
        configuration.WindowIcon = "plusui.svg";
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
        builder.AddPage<EntryPage>().WithViewModel<EntryPageViewModel>();
        builder.AddPage<RichTextLabelPage>().WithViewModel<RichTextLabelPageViewModel>();
        builder.AddPage<CodeEditorPage>().WithViewModel<CodeEditorPageViewModel>();

        // Shared view model for simple demo pages that only need back-navigation.
        builder.WithViewModel<DemoPageViewModel>();

        builder.AddPage<LabelPage>();
        builder.AddPage<ButtonPage>().WithViewModel<ButtonPageViewModel>();
        builder.AddPage<CheckboxPage>().WithViewModel<CheckboxPageViewModel>();
        builder.AddPage<TogglePage>().WithViewModel<TogglePageViewModel>();
        builder.AddPage<RadioButtonPage>();
        builder.AddPage<SliderPage>().WithViewModel<SliderPageViewModel>();
        builder.AddPage<SeparatorPage>();
        builder.AddPage<ProgressBarPage>();
        builder.AddPage<ActivityIndicatorPage>();
        builder.AddPage<BorderPage>();
        builder.AddPage<ImagePage>();
        builder.AddPage<SolidPage>();
        builder.AddPage<LinkPage>();
        builder.AddPage<TabControlPage>();
        builder.AddPage<MenuPage>().WithViewModel<MenuPageViewModel>();
        builder.AddPage<ContextMenuPage>().WithViewModel<ContextMenuPageViewModel>();
        builder.AddPage<ToolbarPage>();
        builder.AddPage<TooltipPage>();
        builder.AddPage<LineGraphPage>();
        builder.AddPage<GameCanvasPage>().WithViewModel<GameCanvasPageViewModel>();
        builder.AddPage<ScrollbarPage>();
        builder.AddPage<GesturesPage>().WithViewModel<GesturesPageViewModel>();
        builder.AddPage<HoverPage>().WithViewModel<HoverPageViewModel>();
        builder.AddPage<UserControlPage>();
        builder.AddPage<VStackPage>();
        builder.AddPage<HStackPage>();
        builder.AddPage<GridPage>();
        builder.AddPage<UniformGridPage>();
        builder.AddPage<ScrollViewPage>();
        builder.AddPage<ComboBoxPage>().WithViewModel<ComboBoxPageViewModel>();
        builder.AddPage<DatePickerPage>();
        builder.AddPage<TimePickerPage>();
        builder.AddPage<ItemsListPage>();
        builder.AddPage<TreeViewPage>();
        builder.AddPage<DataGridPage>().WithViewModel<DataGridPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
