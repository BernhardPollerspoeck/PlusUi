using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class StylingPage(
    StylingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<StylingViewModel, ThemingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "Styling";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeBlock(Comment("// Gradient Background")),
            CodeBlock(Keyword("new "), Type("Solid"), Code("()")),
            CodeBlock(Code("    ."), Method("SetBackground"), Code("("), Keyword("new "), Type("LinearGradient"), Code("(Colors.Blue, Colors.Purple, 45))")),
            CodeBlock(Code("    ."), Method("SetCornerRadius"), Code("(12)")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// Shadow")),
            CodeBlock(Keyword("new "), Type("Solid"), Code("()")),
            CodeBlock(Code("    ."), Method("SetShadowColor"), Code("(Colors.Black)")),
            CodeBlock(Code("    ."), Method("SetShadowBlur"), Code("(8)")),
            CodeBlock(Code("    ."), Method("SetShadowOffset"), Code("("), Keyword("new "), Type("Point"), Code("(2, 4))")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// Border")),
            CodeBlock(Keyword("new "), Type("Border"), Code("()")),
            CodeBlock(Code("    ."), Method("SetStrokeColor"), Code("(Colors.Red)")),
            CodeBlock(Code("    ."), Method("SetStrokeThickness"), Code("(2)")),
            CodeBlock(Code("    ."), Method("SetStrokeType"), Code("(StrokeType.Dashed)")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// Dynamic styling")),
            CodeBlock(Keyword("new "), Type("Solid"), Code("()")),
            CodeBlock(Code("    ."), Method("BindBackground"), Code("(() => "), Variable("vm"), Code(".IsActive")),
            CodeBlock(Code("        ? "), Keyword("new "), Type("SolidColorBackground"), Code("(Colors.Green)")),
            CodeBlock(Code("        : "), Keyword("new "), Type("SolidColorBackground"), Code("(Colors.Gray))"))
        )
        .SetSpacing(8);
    }
}
