using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class DebugServerPage(
    DebugServerViewModel vm,
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
            navigationService.NavigateTo<OutroPage>();
        }
    }

    protected override UiElement Build()
    {
        return new VStack(
                new Label()
                    .SetText("Debug Server")
                    .SetTextSize(48)
                    .SetTextColor(Colors.White)
                    .SetFontWeight(FontWeight.Bold),

                new Solid().SetDesiredHeight(32),

                new Grid()
                    .AddRow(Row.Star)
                    .AddRow(Row.Star)
                    .AddRow(Row.Star)
                    .AddColumn(Column.Star)
                    .AddColumn(Column.Star)
                    .AddChild(FeatureCard("UI Tree", "Live element hierarchy\nReal-time updates", new Color(78, 201, 176)), 0, 0)
                    .AddChild(FeatureCard("Property Grid", "Inspect & edit live\nPin favorites", new Color(86, 156, 214)), 0, 1)
                    .AddChild(FeatureCard("Performance", "FPS, frame time, memory\nRender phase breakdown", new Color(230, 180, 60)), 1, 0)
                    .AddChild(FeatureCard("Screenshots", "Full page or element\nClipboard or disk", new Color(200, 130, 50)), 1, 1)
                    .AddChild(FeatureCard("Logging", "Real-time log stream\nLevel filtering", new Color(140, 120, 220)), 2, 0)
                    .AddChild(FeatureCard("Multi-App", "Debug multiple apps\nEach in its own tab", new Color(110, 180, 250)), 2, 1)
            )
            .SetSpacing(0)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(60, 40, 350, 40))
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }

    private static UiElement FeatureCard(string title, string description, Color accentColor)
    {
        return new Border()
            .SetStrokeColor(accentColor)
            .SetStrokeThickness(2)
            .SetCornerRadius(12)
            .SetMargin(new Margin(8))
            .AddChild(
                new VStack(
                    new Label()
                        .SetText(title)
                        .SetTextSize(28)
                        .SetTextColor(accentColor)
                        .SetFontWeight(FontWeight.Bold),

                    new Solid().SetDesiredHeight(8),

                    new Label()
                        .SetText(description)
                        .SetTextSize(16)
                        .SetTextColor(new Color(160, 160, 160))
                )
                .SetSpacing(0)
                .SetMargin(new Margin(24, 20))
            );
    }
}
