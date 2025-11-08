using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using SkiaSharp;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "PlusUi.Regression.Core";
        configuration.Size = new SizeI(800, 600);
        configuration.LoadImagesSynchronously = true;
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

public class DefaultStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        style.AddStyle<UiPageElement>(s =>
        {
            s.SetBackground(new LinearGradient(new(25, 25, 25), new(60, 60, 60), 90));
        });

        style.AddStyle<Border>(s =>
        {
            s.SetBackground(new SKColor(15, 15, 15))
            .SetStrokeColor(new SKColor(200, 200, 200))
            .SetStrokeThickness(2)
            .SetCornerRadius(5);
        });

    }
}

public partial class MainPageViewModel : ObservableObject
{

}

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new HStack(
            BuildNavBar(),
            BuildWelcomeSection());
    }

    private UiElement BuildNavBar()
    {
        return new Border()
            .AddChild(new Solid()
                .SetDesiredWidth(200)
                )
            .SetMargin(new(10, 10, 5, 10));
    }

    private UiElement BuildWelcomeSection()
    {
        return new Border()
            .AddChild(new VStack(
                new Label()
                    .SetText("Welcome to the PlusUi Control Demo")
                    .SetTextColor(SKColors.White)
                    .SetTextSize(20),
                new Image()
                    .SetImageSource("plusuiv2.png")
                    .SetAspect(Aspect.AspectFit)
                    .SetDesiredSize(new(250, 250))
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                )
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .SetVerticalAlignment(VerticalAlignment.Stretch)
            .SetMargin(new(5, 10, 10, 10));
    }

}