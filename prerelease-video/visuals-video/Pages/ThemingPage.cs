using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class ThemingPage(
    ThemingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<ThemingViewModel, AccessibilityPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "7. Theming";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeBlock(Comment("// App-wide styles\n")),
            CodeBlock(Keyword("public class "), Type("AppStyle"), Code("("), Type("IThemeService"), Code(" themeService) : "), Type("Style"), Code("(themeService)\n")),
            CodeBlock(Code("{\n")),
            CodeBlock(Code("    "), Keyword("public "), Type("AppStyle"), Code(" Configure()\n")),
            CodeBlock(Code("    {\n")),
            CodeBlock(Code("        "), Comment("// Base style\n")),
            CodeBlock(Code("        "), Method("AddStyle"), Code("<"), Type("Button"), Code(">(b => b\n")),
            CodeBlock(Code("            ."), Method("SetCornerRadius"), Code("(8)\n")),
            CodeBlock(Code("            ."), Method("SetPadding"), Code("("), Keyword("new "), Type("Margin"), Code("(16, 8)));\n")),

            new Solid().SetDesiredHeight(10),

            CodeBlock(Code("        "), Comment("// Theme overrides\n")),
            CodeBlock(Code("        "), Method("AddStyle"), Code("<"), Type("Button"), Code(">(Theme.Dark, b => b\n")),
            CodeBlock(Code("            ."), Method("SetBackground"), Code("("), Keyword("new "), Type("SolidColorBackground"), Code("(Colors.DarkGray)));\n")),

            new Solid().SetDesiredHeight(10),

            CodeBlock(Code("        "), Comment("// Custom theme\n")),
            CodeBlock(Code("        "), Method("AddStyle"), Code("<"), Type("Button"), Code(">("), String("\"corporate-blue\""), Code(", b => b\n")),
            CodeBlock(Code("            ."), Method("SetBackground"), Code("("), Keyword("new "), Type("SolidColorBackground"), Code("(Colors.Navy)));\n")),
            CodeBlock(Code("        "), Keyword("return this"), Code(";\n")),
            CodeBlock(Code("    }\n")),
            CodeBlock(Code("}")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// Switch at runtime\n")),
            CodeBlock(Code("themeService."), Method("SetTheme"), Code("(Theme.Dark);\n")),
            CodeBlock(Code("themeService."), Method("SetTheme"), Code("("), String("\"corporate-blue\""), Code(");"))
        )
        .SetSpacing(2)
        .SetHorizontalAlignment(HorizontalAlignment.Center);
    }
}
