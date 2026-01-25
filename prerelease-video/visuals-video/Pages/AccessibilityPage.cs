using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class AccessibilityPage(
    AccessibilityViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<AccessibilityViewModel, HotReloadPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "8. Accessibility";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeLine("// Screen reader support", isComment: true),
            CodeLine("new Button()"),
            CodeLine("    .SetIcon(\"delete.svg\")"),
            CodeLine("    .SetAccessibilityLabel(\"Delete item\")"),
            CodeLine("    .SetAccessibilityHint(\"Removes this item from your cart\")"),
            Spacer(),
            CodeLine("// Keyboard navigation", isComment: true),
            CodeLine("new Entry()"),
            CodeLine("    .SetPlaceholder(\"First name\")"),
            CodeLine("    .SetTabIndex(1)"),
            Spacer(),
            CodeLine("new Entry()"),
            CodeLine("    .SetPlaceholder(\"Last name\")"),
            CodeLine("    .SetTabIndex(2)"),
            Spacer(),
            CodeLine("// Focus ring customization", isComment: true),
            CodeLine("new Button()"),
            CodeLine("    .SetFocusRingColor(Colors.Blue)"),
            CodeLine("    .SetFocusRingWidth(3)"),
            CodeLine("    .SetFocusRingOffset(2)"),
            Spacer(),
            CodeLine("// High contrast mode", isComment: true),
            CodeLine("new Button()"),
            CodeLine("    .SetText(\"Submit\")"),
            CodeLine("    .SetHighContrastBackground(new SolidColorBackground(Colors.Black))"),
            CodeLine("    .SetHighContrastForeground(Colors.White)")
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
