using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class AccessibilityPage(
    AccessibilityViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<AccessibilityViewModel, HotReloadPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "Accessibility";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeBlock(Comment("// Screen reader support")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()")),
            CodeBlock(Code("    ."), Method("SetIcon"), Code("("), String("\"delete.svg\""), Code(")")),
            CodeBlock(Code("    ."), Method("SetAccessibilityLabel"), Code("("), String("\"Delete item\""), Code(")")),
            CodeBlock(Code("    ."), Method("SetAccessibilityHint"), Code("("), String("\"Removes this item from your cart\""), Code(")")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// Keyboard navigation")),
            CodeBlock(Keyword("new "), Type("Entry"), Code("()")),
            CodeBlock(Code("    ."), Method("SetPlaceholder"), Code("("), String("\"First name\""), Code(")")),
            CodeBlock(Code("    ."), Method("SetTabIndex"), Code("(1)")),

            new Solid().SetDesiredHeight(8),

            CodeBlock(Keyword("new "), Type("Entry"), Code("()")),
            CodeBlock(Code("    ."), Method("SetPlaceholder"), Code("("), String("\"Last name\""), Code(")")),
            CodeBlock(Code("    ."), Method("SetTabIndex"), Code("(2)")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// Focus ring customization")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()")),
            CodeBlock(Code("    ."), Method("SetFocusRingColor"), Code("("), Type("Colors"), Code(".Blue)")),
            CodeBlock(Code("    ."), Method("SetFocusRingWidth"), Code("(3)")),
            CodeBlock(Code("    ."), Method("SetFocusRingOffset"), Code("(2)")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// High contrast mode")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()")),
            CodeBlock(Code("    ."), Method("SetText"), Code("("), String("\"Submit\""), Code(")")),
            CodeBlock(Code("    ."), Method("SetHighContrastBackground"), Code("("), Keyword("new "), Type("SolidColorBackground"), Code("("), Type("Colors"), Code(".Black))")),
            CodeBlock(Code("    ."), Method("SetHighContrastForeground"), Code("("), Type("Colors"), Code(".White)"))
        )
        .SetSpacing(4);
    }
}
