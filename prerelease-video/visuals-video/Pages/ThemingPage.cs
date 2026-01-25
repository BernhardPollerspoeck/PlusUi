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
            CodeLine("// App-wide styles", isComment: true),
            CodeLine("public class AppStyle(IThemeService themeService) : Style(themeService)"),
            CodeLine("{"),
            CodeLine("    public AppStyle Configure()"),
            CodeLine("    {"),
            CodeLine("        // Base style", isComment: true),
            CodeLine("        AddStyle<Button>(b => b"),
            CodeLine("            .SetCornerRadius(8)"),
            CodeLine("            .SetPadding(new Margin(16, 8)));"),
            Spacer(),
            CodeLine("        // Theme overrides", isComment: true),
            CodeLine("        AddStyle<Button>(Theme.Dark, b => b"),
            CodeLine("            .SetBackground(new SolidColorBackground(Colors.DarkGray)));"),
            Spacer(),
            CodeLine("        // Custom theme", isComment: true),
            CodeLine("        AddStyle<Button>(\"corporate-blue\", b => b"),
            CodeLine("            .SetBackground(new SolidColorBackground(Colors.Navy)));"),
            CodeLine("        return this;"),
            CodeLine("    }"),
            CodeLine("}"),
            Spacer(),
            CodeLine("// Per-page override", isComment: true),
            CodeLine("public override Style? PageStyle => new Style(ThemeService)"),
            CodeLine("    .AddStyle<Button>(b => b.SetCornerRadius(16));"),
            Spacer(),
            CodeLine("// Switch at runtime", isComment: true),
            CodeLine("themeService.SetTheme(Theme.Dark);"),
            CodeLine("themeService.SetTheme(\"corporate-blue\");")
        )
        .SetSpacing(2);
    }

    private static Label CodeLine(string text, bool isComment = false)
    {
        return new Label()
            .SetText(text)
            .SetTextSize(16)
            .SetTextColor(isComment ? new Color(106, 153, 85) : Colors.White)
            .SetFontFamily("Consolas");
    }

    private static Solid Spacer()
    {
        return new Solid().SetDesiredHeight(10);
    }
}
