using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class ControlsPage(
    ControlsViewModel vm,
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
            navigationService.NavigateTo<DataBindingPage>();
        }
    }

    protected override UiElement Build()
    {
        return new VStack(
                new Label()
                    .SetText("Controls that matter:")
                    .SetTextSize(40)
                    .SetTextColor(Colors.White)
                    .SetFontWeight(FontWeight.Bold),

                new Solid().SetDesiredHeight(32),

                HighlightItem("DataGrid", "11 column types"),
                HighlightItem("ItemsList", "virtualized"),
                HighlightItem("TreeView", null),
                HighlightItem("DatePicker / TimePicker", null),

                new Solid().SetDesiredHeight(24),

                new Label()
                    .SetText("... and 25 more")
                    .SetTextSize(28)
                    .SetTextColor(new Color(120, 120, 120))
                    .SetFontStyle(FontStyle.Italic)
            )
            .SetSpacing(12)
            .SetMargin(new Margin(80, 0, 350, 0))
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }

    private static UiElement HighlightItem(string name, string? detail)
    {
        var label = new RichTextLabel()
            .SetTextSize(32)
            .AddRun(new TextRun(name).SetColor(new Color(78, 201, 176)).SetFontWeight(FontWeight.Bold));

        if (detail != null)
        {
            label.AddRun(new TextRun($"  ({detail})").SetColor(new Color(160, 160, 160)));
        }

        return label;
    }
}
