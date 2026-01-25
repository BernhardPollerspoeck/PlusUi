using PlusUi.core;
using PrereleaseVideo.ViewModels;

public abstract class CodePage<TViewModel, TNextPage>(
    TViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider) : UiPageElement(vm)
    where TViewModel : PlaceholderViewModel
    where TNextPage : UiPageElement
{
    private TimeSpan _startTime;

    protected abstract string SectionTitle { get; }
    protected abstract UiElement BuildCodeContent();

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
