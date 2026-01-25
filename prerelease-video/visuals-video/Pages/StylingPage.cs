using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class StylingPage(
    StylingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<StylingViewModel, ThemingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "6. Styling";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeLine("// Gradient Background", isComment: true),
            CodeLine("new Solid()"),
            CodeLine("    .SetBackground(new LinearGradient(Colors.Blue, Colors.Purple, 45))"),
            CodeLine("    .SetCornerRadius(12)"),
            Spacer(),
            CodeLine("// Shadow", isComment: true),
            CodeLine("new Solid()"),
            CodeLine("    .SetBackground(new SolidColorBackground(Colors.White))"),
            CodeLine("    .SetShadowColor(Colors.Black)"),
            CodeLine("    .SetShadowBlur(8)"),
            CodeLine("    .SetShadowOffset(new Point(2, 4))"),
            Spacer(),
            CodeLine("// Border (solid, dashed, dotted)", isComment: true),
            CodeLine("new Border()"),
            CodeLine("    .SetStrokeColor(Colors.Red)"),
            CodeLine("    .SetStrokeThickness(2)"),
            CodeLine("    .SetStrokeType(StrokeType.Dashed)"),
            CodeLine("    .SetCornerRadius(8)"),
            Spacer(),
            CodeLine("// Dynamic styling via binding", isComment: true),
            CodeLine("new Solid()"),
            CodeLine("    .BindBackground(() => vm.IsActive"),
            CodeLine("        ? new SolidColorBackground(Colors.Green)"),
            CodeLine("        : new SolidColorBackground(Colors.Gray))")
        )
        .SetSpacing(4);
    }

    private static Label CodeLine(string text, bool isComment = false)
    {
        return new Label()
            .SetText(text)
            .SetTextSize(18)
            .SetTextColor(isComment ? new Color(106, 153, 85) : Colors.White)
            .SetFontFamily("Consolas");
    }

    private static Solid Spacer()
    {
        return new Solid().SetDesiredHeight(12);
    }
}
