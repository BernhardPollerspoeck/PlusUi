using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class FluentApiPage(
    FluentApiViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<FluentApiViewModel, StylingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "Fluent API";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeBlock(Keyword("public class "), Type("MainPage"), Code("("), Type("MainPageViewModel"), Code(" "), Variable("vm"), Code(") : "), Type("UiPageElement"), Code("("), Variable("vm"), Code(")")),
            CodeBlock(Code("{")),
            CodeBlock(Code("    "), Keyword("protected override "), Type("UiElement"), Code(" Build()")),
            CodeBlock(Code("    {")),
            CodeBlock(Code("        "), Keyword("return new "), Type("VStack"), Code("(")),
            CodeBlock(Code("            "), Keyword("new "), Type("Label"), Code("()")),
            CodeBlock(Code("                ."), Method("SetText"), Code("("), String("\"Welcome to PlusUi\""), Code(")")),
            CodeBlock(Code("                ."), Method("SetTextColor"), Code("("), Type("Colors"), Code(".White),")),

            CodeBlock(Code("            "), Keyword("new "), Type("Entry"), Code("()")),
            CodeBlock(Code("                ."), Method("BindText"), Code("(() => "), Variable("vm"), Code(".Name, v => "), Variable("vm"), Code(".Name = v)")),
            CodeBlock(Code("                ."), Method("SetCornerRadius"), Code("(8),")),

            CodeBlock(Code("            "), Keyword("new "), Type("Button"), Code("()")),
            CodeBlock(Code("                ."), Method("SetText"), Code("("), String("\"Click Me!\""), Code(")")),
            CodeBlock(Code("                ."), Method("SetCommand"), Code("("), Variable("vm"), Code(".SubmitCommand)")),
            CodeBlock(Code("        )")),
            CodeBlock(Code("        ."), Method("SetSpacing"), Code("(16);")),
            CodeBlock(Code("    }")),
            CodeBlock(Code("}"))
        )
        .SetSpacing(4);
    }
}
