using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class OutroPage(OutroViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Image()
                .SetImageSource("plusui.png")
                .SetDesiredHeight(192)
                .SetDesiredWidth(192)
                .SetHorizontalAlignment(HorizontalAlignment.Center),

            new Solid().SetDesiredHeight(16),

            new Label()
                .SetText("One codebase. Every screen. What will you create?")
                .SetTextSize(42)
                .SetTextColor(new Color(200, 200, 200))
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetHorizontalAlignment(HorizontalAlignment.Center),

            new Solid().SetDesiredHeight(32),

            new Grid()
                .AddRow(Row.Auto)
                .AddRow(Row.Auto)
                .AddRow(Row.Auto)
                .AddRow(Row.Auto)
                .AddColumn(Column.Star, 2)
                .AddColumn(Column.Star)
                .AddChild(LinkCard("GitHub", "github.com/BernhardPollerspoeck/PlusUi", new Color(110, 180, 250)), 0, 0)
                .AddChild(LinkCard("Docs", "plusui.qsp.app", new Color(78, 201, 176)), 1, 0)
                .AddChild(LinkCard("Discord", "discord.gg/Je3kNpcmqn", new Color(140, 120, 220)), 2, 0)
                .AddChild(LinkCard("NuGet", "nuget.org/profiles/BernhardPollerspoeck", new Color(200, 130, 50)), 3, 0)
        )
        .SetSpacing(0)
        .SetHorizontalAlignment(HorizontalAlignment.Left)
        .SetVerticalAlignment(VerticalAlignment.Top)
        .SetMargin(new Margin(80, 40, 350, 40))
        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }

    private static UiElement LinkCard(string name, string url, Color accentColor)
    {
        return new Border()
            .SetStrokeColor(accentColor)
            .SetStrokeThickness(2)
            .SetCornerRadius(12)
            .SetMargin(new Margin(12))
            .AddChild(
                new VStack(
                    new Label()
                        .SetText(name)
                        .SetTextSize(32)
                        .SetTextColor(accentColor)
                        .SetFontWeight(FontWeight.Bold)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetHorizontalAlignment(HorizontalAlignment.Center),

                    new Solid().SetDesiredHeight(8),

                    new Label()
                        .SetText(url)
                        .SetTextSize(16)
                        .SetTextColor(new Color(140, 140, 140))
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetHorizontalAlignment(HorizontalAlignment.Center)
                )
                .SetSpacing(0)
                .SetMargin(new Margin(40, 24))
            );
    }
}
