using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class FluentApiPage(
    FluentApiViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<FluentApiViewModel, StylingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "5. Fluent API";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeLine("public class MainPage(MainPageViewModel vm) : UiPageElement(vm)"),
            CodeLine("{"),
            CodeLine("    protected override UiElement Build()"),
            CodeLine("    {"),
            CodeLine("        return new VStack("),
            CodeLine("            new Label()"),
            CodeLine("                .SetText(\"Welcome to PlusUi\")"),
            CodeLine("                .SetTextSize(32)"),
            CodeLine("                .SetTextColor(Colors.White)"),
            CodeLine("                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),"),
            Spacer(),
            CodeLine("            new Entry()"),
            CodeLine("                .SetPlaceholder(\"Enter your name...\")"),
            CodeLine("                .BindText(() => vm.Name, v => vm.Name = v)"),
            CodeLine("                .SetPadding(new Margin(12, 8))"),
            CodeLine("                .SetCornerRadius(8),"),
            Spacer(),
            CodeLine("            new Button()"),
            CodeLine("                .SetText(\"Click Me!\")"),
            CodeLine("                .SetPadding(new Margin(20, 10))"),
            CodeLine("                .SetCornerRadius(8)"),
            CodeLine("                .SetCommand(vm.SubmitCommand)"),
            CodeLine("        )"),
            CodeLine("        .SetSpacing(16)"),
            CodeLine("        .SetPadding(new Margin(24))"),
            CodeLine("        .SetHorizontalAlignment(HorizontalAlignment.Center)"),
            CodeLine("        .SetVerticalAlignment(VerticalAlignment.Center);"),
            CodeLine("    }"),
            CodeLine("}")
        )
        .SetSpacing(2);
    }

    private static Label CodeLine(string text)
    {
        return new Label()
            .SetText(text)
            .SetTextSize(16)
            .SetTextColor(Colors.White)
            .SetFontFamily("Consolas");
    }

    private static Solid Spacer()
    {
        return new Solid().SetDesiredHeight(8);
    }
}
