using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class IntroPage(
    IntroViewModel vm,
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
            navigationService.NavigateTo<PlatformsPage>();
        }
    }

    protected override UiElement Build()
    {
        return new VStack(
                new Image()
                    .SetImageSource("plusui.png")
                    .SetDesiredHeight(384)
                    .SetDesiredWidth(384)
                    .SetHorizontalAlignment(HorizontalAlignment.Center),

                new Solid().SetDesiredHeight(24),

                new Label()
                    .SetText("PlusUi")
                    .SetTextSize(80)
                    .SetTextColor(Colors.White)
                    .SetFontWeight(FontWeight.Bold)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
            )
            .SetSpacing(0)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(0, 0, 350, 0))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }
}
