using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class DataBindingPage(
    DataBindingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : CodePage<DataBindingViewModel, FluentApiPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "Data Binding";

    protected override UiElement BuildCodeContent()
    {
        return new VStack(
            CodeBlock(Comment("// Type-safe binding")),
            CodeBlock(Keyword("new "), Type("Label"), Code("()")),
            CodeBlock(Code("    ."), Method("BindText"), Code("(() => "), Variable("vm"), Code(".Username)")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// With formatter")),
            CodeBlock(Keyword("new "), Type("Label"), Code("()")),
            CodeBlock(Code("    ."), Method("BindText"), Code("(() => "), Variable("vm"), Code(".ItemCount, c => "), String("$\"You have {c} items\""), Code(")")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// Two-way binding")),
            CodeBlock(Keyword("new "), Type("Entry"), Code("()")),
            CodeBlock(Code("    ."), Method("BindText"), Code("(() => "), Variable("vm"), Code(".SearchQuery, v => "), Variable("vm"), Code(".SearchQuery = v)")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// Command binding")),
            CodeBlock(Keyword("new "), Type("Button"), Code("()")),
            CodeBlock(Code("    ."), Method("SetText"), Code("("), String("\"Save\""), Code(")")),
            CodeBlock(Code("    ."), Method("BindCommand"), Code("(() => "), Variable("vm"), Code(".SaveCommand)")),

            new Solid().SetDesiredHeight(24),

            CodeBlock(Comment("// Visibility binding")),
            CodeBlock(Keyword("new "), Type("Label"), Code("()")),
            CodeBlock(Code("    ."), Method("SetText"), Code("("), String("\"Error!\""), Code(")")),
            CodeBlock(Code("    ."), Method("BindIsVisible"), Code("(() => "), Variable("vm"), Code(".HasError)"))
        )
        .SetSpacing(8);
    }
}
