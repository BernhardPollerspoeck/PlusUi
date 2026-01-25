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
                .SetDesiredHeight(192)
                .SetDesiredWidth(192),
            new Label()
                .BindText(() => vm.SectionTitle)
                .SetTextSize(32)
                .SetTextColor(Colors.White)
                .SetMargin(new Margin(16, 0, 0, 0))
        )
        .SetSpacing(0)
        .SetMargin(new Margin(24))
        .SetVerticalAlignment(VerticalAlignment.Center);
    }

    protected static RichTextLabel CodeBlock(params TextRun[] runs)
    {
        var label = new RichTextLabel()
            .SetFontFamily("Consolas")
            .SetTextSize(100)
            .SetTextColor(Colors.White);
        foreach (var run in runs)
        {
            label.AddRun(run);
        }
        return label;
    }

    protected static TextRun Comment(string text) => new TextRun(text).SetColor(new Color(106, 153, 85));
    protected static TextRun Keyword(string text) => new TextRun(text).SetColor(new Color(86, 156, 214));
    protected static TextRun Type(string text) => new TextRun(text).SetColor(new Color(78, 201, 176));
    protected static TextRun String(string text) => new TextRun(text).SetColor(new Color(206, 145, 120));
    protected static TextRun Method(string text) => new TextRun(text).SetColor(new Color(220, 220, 170));
    protected static TextRun Variable(string text) => new TextRun(text).SetColor(new Color(156, 220, 254));
    protected static TextRun Code(string text) => new TextRun(text);
}
