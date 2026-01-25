using PlusUi.core;
using PrereleaseVideo.ViewModels;

public abstract class CodePage<TViewModel, TNextPage>(
    TViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider) : UiPageElement(vm)
    where TViewModel : PlaceholderViewModel
    where TNextPage : UiPageElement
{
    private bool _initialized;

    protected abstract string SectionTitle { get; }
    protected abstract UiElement BuildCodeContent();

    public override void OnNavigatedTo(object? parameter)
    {
        vm.SectionTitle = SectionTitle;
        vm.StartTime = timeProvider.GetUtcNow().TimeOfDay;
        _initialized = true;
    }

    protected override UiElement Build()
    {
        if (_initialized)
        {
            var elapsed = timeProvider.GetUtcNow().TimeOfDay - vm.StartTime;
            if (elapsed >= vm.Duration)
            {
                navigationService.NavigateTo<TNextPage>();
            }
        }

        return new Grid()
            .AddRow(Row.Auto)
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
            .AddChild(BuildHeader(), 0, 0)
            .AddChild(BuildCodeContent()
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetHorizontalAlignment(HorizontalAlignment.Center), 1, 0);
    }

    private UiElement BuildHeader()
    {
        return new HStack(
            new Image()
                .SetImageSource("plusui.png")
                .SetDesiredHeight(48)
                .SetDesiredWidth(48),
            new Label()
                .SetText(vm.SectionTitle)
                .SetTextSize(32)
                .SetTextColor(Colors.White)
                .SetMargin(new Margin(16, 0, 0, 0))
        )
        .SetSpacing(0)
        .SetMargin(new Margin(24))
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
