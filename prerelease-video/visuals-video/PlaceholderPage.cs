using PlusUi.core;

public abstract class PlaceholderPage<TNextPage>(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider) : UiPageElement(vm)
    where TNextPage : UiPageElement
{
    private bool _initialized;

    protected abstract string SectionTitle { get; }

    public override void OnNavigatedTo(object? parameter)
    {
        vm.SectionTitle = SectionTitle;
        vm.StartTime = timeProvider.GetUtcNow().TimeOfDay;
        _initialized = true;
    }

    protected override UiElement Build()
    {
        // Check if we should navigate to next page
        if (_initialized)
        {
            var elapsed = timeProvider.GetUtcNow().TimeOfDay - vm.StartTime;
            if (elapsed >= vm.Duration)
            {
                navigationService.NavigateTo<TNextPage>();
            }
        }

        return new VStack(
            new Label()
                .SetText(vm.SectionTitle)
                .SetTextSize(64)
                .SetTextColor(Colors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center)
        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }
}
