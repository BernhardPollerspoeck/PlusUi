using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class DataBindingPage(
    DataBindingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<DataBindingViewModel, FluentApiPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "4. Data Binding";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeLine("// Type-safe binding", isComment: true),
            CodeLine("new Label()"),
            CodeLine("    .BindText(() => vm.Username)"),
            Spacer(),
            CodeLine("// Mit Formatter", isComment: true),
            CodeLine("new Label()"),
            CodeLine("    .BindText(() => vm.ItemCount, c => $\"You have {c} items in your cart\")"),
            Spacer(),
            CodeLine("// Two-way binding", isComment: true),
            CodeLine("new Entry()"),
            CodeLine("    .BindText(() => vm.SearchQuery, v => vm.SearchQuery = v)"),
            Spacer(),
            CodeLine("// Command binding", isComment: true),
            CodeLine("new Button()"),
            CodeLine("    .SetText(\"Save\")"),
            CodeLine("    .BindCommand(() => vm.SaveCommand)"),
            Spacer(),
            CodeLine("// Visibility binding", isComment: true),
            CodeLine("new Label()"),
            CodeLine("    .SetText(\"Error!\")"),
            CodeLine("    .BindIsVisible(() => vm.HasError)")
        )
        .SetSpacing(4);
    }

    private static Label CodeLine(string text, bool isComment = false)
    {
        return new Label()
            .SetText(text)
            .SetTextSize(20)
            .SetTextColor(isComment ? new Color(106, 153, 85) : Colors.White)
            .SetFontFamily("Consolas");
    }

    private static Solid Spacer()
    {
        return new Solid().SetDesiredHeight(12);
    }
}
