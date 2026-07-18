using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class CodeEditorPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    private string _code = """
        using System;

        namespace Demo;

        /* This block comment spans
           several lines - the highlighter
           carries its state downward. */
        public class Greeter
        {
            private readonly string _name;
            private const string Path = @"C:\temp\greeter.log";

            public Greeter(string name)
            {
                _name = name;
            }

            public string Greet(int times)
            {
                // Repeat the greeting
                var result = "";
                for (var i = 0; i < times; i++)
                {
                    result += $"Hello, {_name}! ";
                }
                return result.Trim();
            }
        }
        """;

    [ObservableProperty]
    private string _lineCount = "";

    [ObservableProperty]
    private string _notes = "TODO: wire up the parser\nFIXME: handle empty input\nNormal note line";

    partial void OnCodeChanged(string value)
        => LineCount = $"{value.Split('\n').Length} lines, {value.Length} characters";

    [RelayCommand]
    private void ResetCode()
    {
        Code = "public class Empty\n{\n}";
    }
}

public class CodeEditorPage(CodeEditorPageViewModel vm) : DemoPage(vm)
{
    private static readonly Color EditorBackground = new(30, 30, 30);

    protected override string ControlName => "CodeEditor";

    protected override string Description =>
        "A multi-line editor for code. The document stays a plain string; a highlighter callback " +
        "turns it into styled spans on every change, so cursor and selection indices never need remapping.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        BuildCSharpSection(),
        BuildHighlighterModelSection(),
        BuildCustomHighlighterSection(),
        BuildOptionsSection(),
        BuildReadOnlySection(),
    ];

    private UiElement BuildCSharpSection() =>
        Section("C# Syntax Highlighting",
            Note("Type into the editor - the highlighter re-runs on every change. Tab and Shift+Tab indent " +
                 "and outdent, Enter keeps the current indentation, Home jumps to the first non-whitespace character."),
            new CodeEditor()
                .SetFontFamily("Consolas")
                .SetTextSize(13)
                .SetDesiredWidth(720)
                .SetDesiredHeight(420)
                .SetBackground(EditorBackground)
                .SetLineHighlighter(CSharpHighlighter.HighlightLine)
                .BindText(() => vm.Code, value => vm.Code = value),
            new Label()
                .SetTextSize(12)
                .SetTextColor(PlusUiDefaults.TextSecondary)
                .BindText(() => vm.LineCount));

    private static UiElement BuildHighlighterModelSection() =>
        Section("How styling works",
            Note("Nothing is stored on the control: styling is recomputed from the text, so inserting in " +
                 "the middle can never desynchronize it. SetLineHighlighter tokenizes one line at a time and " +
                 "carries a state to the next line - that is what makes block comments and verbatim strings " +
                 "spanning several lines work, and it lets the editor cache per line so an edit re-tokenizes " +
                 "only what changed. SetHighlighter is the simpler whole-document variant."),
            new CodeEditor()
                .SetFontFamily("Consolas")
                .SetTextSize(13)
                .SetDesiredWidth(720)
                .SetDesiredHeight(150)
                .SetBackground(EditorBackground)
                .SetIsReadOnly(true)
                .SetShowLineNumbers(false)
                .SetLineHighlighter(CSharpHighlighter.HighlightLine)
                .SetText("""
                    editor.SetLineHighlighter((line, index, state, output) =>
                    {
                        /* state carries across lines,
                           so this comment stays green
                           all the way down here */
                        foreach (var token in Tokenize(line, ref state))
                            output.Add(new StyleSpan(
                                token.Start, token.Length, ColorFor(token.Kind)));
                        return state;
                    });
                    """));

    private UiElement BuildCustomHighlighterSection() =>
        Section("Any highlighter, not just code",
            Note("The same callback drives non-code styling. Here TODO is red, FIXME is orange - a plain " +
                 "keyword scan rather than a language tokenizer."),
            new CodeEditor()
                .SetFontFamily("Consolas")
                .SetTextSize(13)
                .SetDesiredWidth(720)
                .SetDesiredHeight(110)
                .SetBackground(EditorBackground)
                .SetShowLineNumbers(false)
                .SetHighlighter(HighlightMarkers)
                .BindText(() => vm.Notes, value => vm.Notes = value));

    /// <summary>Marks TODO and FIXME occurrences - a deliberately trivial highlighter.</summary>
    private static List<StyleSpan> HighlightMarkers(string text)
    {
        var spans = new List<StyleSpan>();
        AddAll(spans, text, "TODO", new Color(240, 90, 90));
        AddAll(spans, text, "FIXME", new Color(230, 160, 60));
        return spans;

        static void AddAll(List<StyleSpan> target, string text, string marker, Color color)
        {
            var index = text.IndexOf(marker, StringComparison.Ordinal);
            while (index >= 0)
            {
                target.Add(new StyleSpan(index, marker.Length, color, FontWeight.Bold));
                index = text.IndexOf(marker, index + marker.Length, StringComparison.Ordinal);
            }
        }
    }

    private UiElement BuildOptionsSection() =>
        Section("Options",
            Demo("Tab size 2, no line numbers",
                new CodeEditor()
                    .SetFontFamily("Consolas")
                    .SetTextSize(13)
                    .SetTabSize(2)
                    .SetShowLineNumbers(false)
                    .SetDesiredWidth(360)
                    .SetDesiredHeight(120)
                    .SetBackground(EditorBackground)
                    .SetLineHighlighter(CSharpHighlighter.HighlightLine)
                    .SetText("if (ready)\n{\n  Run();\n}")),
            Demo("Auto-indent disabled",
                new CodeEditor()
                    .SetFontFamily("Consolas")
                    .SetTextSize(13)
                    .SetAutoIndent(false)
                    .SetDesiredWidth(360)
                    .SetDesiredHeight(120)
                    .SetBackground(EditorBackground)
                    .SetLineHighlighter(CSharpHighlighter.HighlightLine)
                    .SetText("    // press Enter here")),
            new Button()
                .SetText("Reset first editor")
                .SetCommand(vm.ResetCodeCommand));

    private static UiElement BuildReadOnlySection() =>
        Section("Read-only",
            Note("A read-only editor still supports selection, copy and navigation, and lets Tab move focus " +
                 "on to the next control instead of indenting."),
            new CodeEditor()
                .SetFontFamily("Consolas")
                .SetTextSize(13)
                .SetIsReadOnly(true)
                .SetDesiredWidth(720)
                .SetDesiredHeight(120)
                .SetBackground(EditorBackground)
                .SetLineHighlighter(CSharpHighlighter.HighlightLine)
                .SetText("public static int Add(int a, int b) => a + b;\n\n// try selecting and copying this"));
}
