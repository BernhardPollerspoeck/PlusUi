using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class PlatformsPage(
    PlatformsViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider) : UiPageElement(vm)
{
    private TimeSpan _startTime;

    public override void Appearing()
    {
        _startTime = timeProvider.GetUtcNow().TimeOfDay;
    }

    protected override void PostRender()
    {
        var elapsed = timeProvider.GetUtcNow().TimeOfDay - _startTime;
        if (elapsed >= vm.Duration)
        {
            navigationService.NavigateTo<ControlsPage>();
        }
    }

    protected override UiElement Build()
    {
        return new Grid()
            .AddRow(Row.Star)
            .AddRow(Row.Star)
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)
            .SetMargin(new Margin(60, 40, 350, 40))
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
            .AddChild(PlatformCard("Windows", "windows.svg"), 0, 0)
            .AddChild(PlatformCard("macOS", "apple.svg"), 0, 1)
            .AddChild(PlatformCard("Linux", "linux.svg"), 0, 2)
            .AddChild(PlatformCard("Web", "blazor.svg"), 1, 0)
            .AddChild(PlatformCard("iOS", "ios.svg"), 1, 1)
            .AddChild(PlatformCard("Android", "android.svg"), 1, 2)
            .AddChild(PlatformCard("Headless", "gnubash.svg"), 2, 0)
            .AddChild(PlatformCard("Video", "ffmpeg.svg"), 2, 1);
    }

    private static UiElement PlatformCard(string name, string iconPath)
    {
        return new Border()
            .SetStrokeColor(new Color(60, 60, 60))
            .SetStrokeThickness(1)
            .SetCornerRadius(12)
            .SetMargin(new Margin(10))
            .AddChild(
                new VStack(
                    new Image()
                        .SetImageSource(iconPath)
                        .SetDesiredHeight(64)
                        .SetDesiredWidth(64)
                        .SetHorizontalAlignment(HorizontalAlignment.Center),

                    new Solid().SetDesiredHeight(12),

                    new Label()
                        .SetText(name)
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetFontWeight(FontWeight.Bold)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetHorizontalAlignment(HorizontalAlignment.Center)
                )
                .SetSpacing(0)
                .SetMargin(new Margin(20, 24))
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center)
            );
    }
}
