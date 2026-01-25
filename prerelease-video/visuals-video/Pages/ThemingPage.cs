using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class ThemingPage(
    ThemingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<ThemingViewModel, AccessibilityPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "Theming";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeBlock(Comment("// App-wide styles")),
            CodeBlock(Keyword("public class "), Type("AppStyle"), Code("("), Type("IThemeService"), Code(" "), Variable("themeService"), Code(") : "), Type("Style"), Code("("), Variable("themeService"), Code(")")),
            CodeBlock(Code("{")),
            CodeBlock(Code("    "), Keyword("public "), Type("AppStyle"), Code(" Configure()")),
            CodeBlock(Code("    {")),
            CodeBlock(Code("        "), Comment("// Base style")),
            CodeBlock(Code("        "), Method("AddStyle"), Code("<"), Type("Button"), Code(">(b => b")),
            CodeBlock(Code("            ."), Method("SetCornerRadius"), Code("(8)")),
            CodeBlock(Code("            ."), Method("SetPadding"), Code("("), Keyword("new "), Type("Margin"), Code("(16, 8)));")),

            new Solid().SetDesiredHeight(10),

            CodeBlock(Code("        "), Comment("// Theme overrides")),
            CodeBlock(Code("        "), Method("AddStyle"), Code("<"), Type("Button"), Code(">("), Enum("Theme"), Code(".Dark, b => b")),
            CodeBlock(Code("            ."), Method("SetBackground"), Code("("), Keyword("new "), Type("SolidColorBackground"), Code("("), Type("Colors"), Code(".DarkGray)));")),

            new Solid().SetDesiredHeight(10),

            CodeBlock(Code("        "), Comment("// Custom theme")),
            CodeBlock(Code("        "), Method("AddStyle"), Code("<"), Type("Button"), Code(">("), String("\"corporate-blue\""), Code(", b => b")),
            CodeBlock(Code("            ."), Method("SetBackground"), Code("("), Keyword("new "), Type("SolidColorBackground"), Code("("), Type("Colors"), Code(".Navy)));")),
            CodeBlock(Code("        "), Keyword("return this"), Code(";")),
            CodeBlock(Code("    }")),
            CodeBlock(Code("}")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// Switch at runtime")),
            CodeBlock(Variable("themeService"), Code("."), Method("SetTheme"), Code("("), Enum("Theme"), Code(".Dark);")),
            CodeBlock(Variable("themeService"), Code("."), Method("SetTheme"), Code("("), String("\"corporate-blue\""), Code(");"))
        )
        .SetSpacing(4);
    }
}
