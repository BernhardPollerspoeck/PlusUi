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
        return new VStack(
            BuildCodeContent()
        )
        .SetVerticalAlignment(VerticalAlignment.Center)
        .SetMargin(new Margin(60, 40, 350, 40))
        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }

    protected static RichTextLabel CodeBlock(params TextRun[] runs)
    {
        var label = new RichTextLabel()
            .SetFontFamily("Consolas")
            .SetTextSize(18)
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
    protected static TextRun Enum(string text) => new TextRun(text).SetColor(new Color(184, 215, 163));
    protected static TextRun String(string text) => new TextRun(text).SetColor(new Color(206, 145, 120));
    protected static TextRun Method(string text) => new TextRun(text).SetColor(new Color(220, 220, 170));
    protected static TextRun Variable(string text) => new TextRun(text).SetColor(new Color(156, 220, 254));
    protected static TextRun Code(string text) => new TextRun(text);
}
