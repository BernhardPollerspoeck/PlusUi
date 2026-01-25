using PlusUi.core;
using PrereleaseVideo.ViewModels;

public abstract class PlaceholderPage<TViewModel, TNextPage>(
    TViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider) : UiPageElement(vm)
    where TViewModel : PlaceholderViewModel
    where TNextPage : UiPageElement
{
    private TimeSpan _startTime;

    protected abstract string SectionTitle { get; }

    public override void Appearing()
    {
        vm.SectionTitle = SectionTitle;
        _startTime = timeProvider.GetUtcNow().TimeOfDay;
    }

    protected override void PostRender()
    {
        var elapsed = timeProvider.GetUtcNow().TimeOfDay - _startTime;
        if (elapsed >= vm.Duration)
        {
            navigationService.NavigateTo<TNextPage>();
        }
    }

    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .BindText(() => vm.SectionTitle)
                .SetTextSize(200)
                .SetTextColor(Colors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center)
        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }
}
