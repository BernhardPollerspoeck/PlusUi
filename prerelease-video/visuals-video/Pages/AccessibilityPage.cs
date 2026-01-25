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
            CodeBlock(Comment("// Screen reader support\n")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()\n")),
            CodeBlock(Code("    ."), Method("SetIcon"), Code("("), String("\"delete.svg\""), Code(")\n")),
            CodeBlock(Code("    ."), Method("SetAccessibilityLabel"), Code("("), String("\"Delete item\""), Code(")\n")),
            CodeBlock(Code("    ."), Method("SetAccessibilityHint"), Code("("), String("\"Removes this item from your cart\""), Code(")")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// Keyboard navigation\n")),
            CodeBlock(Keyword("new "), Type("Entry"), Code("()\n")),
            CodeBlock(Code("    ."), Method("SetPlaceholder"), Code("("), String("\"First name\""), Code(")\n")),
            CodeBlock(Code("    ."), Method("SetTabIndex"), Code("(1)")),

            new Solid().SetDesiredHeight(8),

            CodeBlock(Keyword("new "), Type("Entry"), Code("()\n")),
            CodeBlock(Code("    ."), Method("SetPlaceholder"), Code("("), String("\"Last name\""), Code(")\n")),
            CodeBlock(Code("    ."), Method("SetTabIndex"), Code("(2)")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// Focus ring customization\n")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()\n")),
            CodeBlock(Code("    ."), Method("SetFocusRingColor"), Code("(Colors.Blue)\n")),
            CodeBlock(Code("    ."), Method("SetFocusRingWidth"), Code("(3)\n")),
            CodeBlock(Code("    ."), Method("SetFocusRingOffset"), Code("(2)")),

            new Solid().SetDesiredHeight(16),

            CodeBlock(Comment("// High contrast mode\n")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()\n")),
            CodeBlock(Code("    ."), Method("SetText"), Code("("), String("\"Submit\""), Code(")\n")),
            CodeBlock(Code("    ."), Method("SetHighContrastBackground"), Code("("), Keyword("new "), Type("SolidColorBackground"), Code("(Colors.Black))\n")),
            CodeBlock(Code("    ."), Method("SetHighContrastForeground"), Code("(Colors.White)"))
        )
        .SetSpacing(4)
        .SetHorizontalAlignment(HorizontalAlignment.Center);
    }
}
