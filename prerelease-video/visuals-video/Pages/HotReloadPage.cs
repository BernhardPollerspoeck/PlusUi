using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class HotReloadPage(
    HotReloadViewModel vm,
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
            navigationService.NavigateTo<DebugServerPage>();
        }
    }

    protected override UiElement Build()
    {
        return new VStack(
                new Label()
                    .SetText("Hot Reload")
                    .SetTextSize(72)
                    .SetTextColor(new Color(230, 60, 60))
                    .SetFontWeight(FontWeight.Bold)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Center),

                new Solid().SetDesiredHeight(40),

                new Label()
                    .SetText("Fastest inner dev loop for UI iteration")
                    .SetTextSize(28)
                    .SetTextColor(new Color(200, 200, 200))
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Center),

                new Solid().SetDesiredHeight(12),

                new Label()
                    .SetText("Built on .NET Hot Reload")
                    .SetTextSize(22)
                    .SetTextColor(new Color(140, 140, 140))
                    .SetFontStyle(FontStyle.Italic)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
            )
            .SetSpacing(0)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(80, 0, 350, 0))
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }
}
