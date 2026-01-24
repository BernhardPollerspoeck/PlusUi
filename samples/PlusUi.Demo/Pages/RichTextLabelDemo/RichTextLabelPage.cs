using PlusUi.core;

namespace PlusUi.Demo.Pages.RichTextLabelDemo;

public class RichTextLabelPage(RichTextLabelPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
                new VStack()
                    .SetSpacing(24)
                    .SetMargin(new Margin(40))
                    .AddChild(BuildHeader())
                    .AddChild(BuildBasicExample())
                    .AddChild(BuildSyntaxHighlightingExample())
                    .AddChild(BuildMixedFontSizesExample())
                    .AddChild(BuildColorfulExample())
                    .AddChild(BuildDataBindingExample())
                    .AddChild(BuildWrappingExample())
                    .AddChild(BuildBackButton()))
            .SetCanScrollHorizontally(false);
    }

    private UiElement BuildHeader()
    {
        return new VStack()
            .SetSpacing(8)
            .AddChild(
                new RichTextLabel()
                    .SetTextSize(32)
                    .AddRun(new TextRun("Rich").SetColor(new Color(255, 100, 100)))
                    .AddRun(new TextRun("Text").SetColor(new Color(100, 255, 100)))
                    .AddRun(new TextRun("Label").SetColor(new Color(100, 100, 255))))
            .AddChild(
                new Label()
                    .SetText("Display multiple styled text segments inline - for syntax highlighting, mixed formatting, and rich text display.")
                    .SetTextColor(PlusUiDefaults.TextSecondary)
                    .SetTextWrapping(TextWrapping.WordWrap));
    }

    private UiElement BuildBasicExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Basic Usage"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("Hello ").SetColor(Colors.White))
                                    .AddRun(new TextRun("World").SetColor(Colors.Blue).SetFontWeight(FontWeight.Bold)))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("Normal, "))
                                    .AddRun(new TextRun("Bold, ").SetFontWeight(FontWeight.Bold))
                                    .AddRun(new TextRun("Italic, ").SetFontStyle(FontStyle.Italic))
                                    .AddRun(new TextRun("Both").SetFontWeight(FontWeight.Bold).SetFontStyle(FontStyle.Italic)))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("Status: "))
                                    .AddRun(new TextRun("Online").SetColor(PlusUiDefaults.AccentSuccess).SetFontWeight(FontWeight.Bold))
                                    .AddRun(new TextRun(" | "))
                                    .AddRun(new TextRun("3 Warnings").SetColor(PlusUiDefaults.AccentWarning))
                                    .AddRun(new TextRun(" | "))
                                    .AddRun(new TextRun("1 Error").SetColor(PlusUiDefaults.AccentError)))));
    }

    private UiElement BuildSyntaxHighlightingExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Syntax Highlighting"))
            .AddChild(
                new Border()
                    .SetBackground(new Color(30, 30, 30))
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(4)
                            .SetMargin(new Margin(16))
                            .AddChild(BuildCodeLine("public", " ", "class", " ", "RichTextLabel", " : ", "UiElement"))
                            .AddChild(BuildCodeLine("{"))
                            .AddChild(new RichTextLabel()
                                .SetFontFamily("Consolas")
                                .AddRun(new TextRun("    ").SetColor(Colors.White))
                                .AddRun(new TextRun("private").SetColor(new Color(86, 156, 214)))
                                .AddRun(new TextRun(" ").SetColor(Colors.White))
                                .AddRun(new TextRun("readonly").SetColor(new Color(86, 156, 214)))
                                .AddRun(new TextRun(" List<").SetColor(Colors.White))
                                .AddRun(new TextRun("TextRun").SetColor(new Color(78, 201, 176)))
                                .AddRun(new TextRun("> _runs = [];").SetColor(Colors.White)))
                            .AddChild(new RichTextLabel()
                                .SetFontFamily("Consolas")
                                .AddRun(new TextRun("    ").SetColor(Colors.White))
                                .AddRun(new TextRun("public").SetColor(new Color(86, 156, 214)))
                                .AddRun(new TextRun(" ").SetColor(Colors.White))
                                .AddRun(new TextRun("RichTextLabel").SetColor(new Color(78, 201, 176)))
                                .AddRun(new TextRun(" ").SetColor(Colors.White))
                                .AddRun(new TextRun("AddRun").SetColor(new Color(220, 220, 170)))
                                .AddRun(new TextRun("(").SetColor(Colors.White))
                                .AddRun(new TextRun("TextRun").SetColor(new Color(78, 201, 176)))
                                .AddRun(new TextRun(" run)").SetColor(Colors.White)))
                            .AddChild(BuildCodeLine("    {"))
                            .AddChild(new RichTextLabel()
                                .SetFontFamily("Consolas")
                                .AddRun(new TextRun("        _runs.").SetColor(Colors.White))
                                .AddRun(new TextRun("Add").SetColor(new Color(220, 220, 170)))
                                .AddRun(new TextRun("(run);").SetColor(Colors.White)))
                            .AddChild(new RichTextLabel()
                                .SetFontFamily("Consolas")
                                .AddRun(new TextRun("        ").SetColor(Colors.White))
                                .AddRun(new TextRun("return").SetColor(new Color(86, 156, 214)))
                                .AddRun(new TextRun(" ").SetColor(Colors.White))
                                .AddRun(new TextRun("this").SetColor(new Color(86, 156, 214)))
                                .AddRun(new TextRun(";").SetColor(Colors.White)))
                            .AddChild(BuildCodeLine("    }"))
                            .AddChild(BuildCodeLine("}"))));
    }

    private RichTextLabel BuildCodeLine(params string[] parts)
    {
        var label = new RichTextLabel().SetFontFamily("Consolas");
        var colors = new Dictionary<string, Color>
        {
            { "public", new Color(86, 156, 214) },
            { "private", new Color(86, 156, 214) },
            { "readonly", new Color(86, 156, 214) },
            { "class", new Color(86, 156, 214) },
            { "return", new Color(86, 156, 214) },
            { "this", new Color(86, 156, 214) },
            { "RichTextLabel", new Color(78, 201, 176) },
            { "UiElement", new Color(78, 201, 176) },
            { "List", new Color(78, 201, 176) },
            { "TextRun", new Color(78, 201, 176) }
        };

        foreach (var part in parts)
        {
            var color = colors.GetValueOrDefault(part, Colors.White);
            label.AddRun(new TextRun(part).SetColor(color));
        }

        return label;
    }

    private UiElement BuildMixedFontSizesExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Mixed Font Sizes"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new RichTextLabel()
                                    .SetTextSize(14)
                                    .AddRun(new TextRun("Price: "))
                                    .AddRun(new TextRun("$99").SetFontSize(28).SetFontWeight(FontWeight.Bold).SetColor(PlusUiDefaults.AccentSuccess))
                                    .AddRun(new TextRun(".99").SetFontSize(16).SetColor(PlusUiDefaults.AccentSuccess))
                                    .AddRun(new TextRun(" /month")))
                            .AddChild(
                                new RichTextLabel()
                                    .SetTextSize(14)
                                    .AddRun(new TextRun("H").SetFontSize(24).SetFontWeight(FontWeight.Bold).SetColor(new Color(255, 100, 100)))
                                    .AddRun(new TextRun("E").SetFontSize(24).SetFontWeight(FontWeight.Bold).SetColor(new Color(255, 165, 0)))
                                    .AddRun(new TextRun("L").SetFontSize(24).SetFontWeight(FontWeight.Bold).SetColor(new Color(255, 255, 0)))
                                    .AddRun(new TextRun("L").SetFontSize(24).SetFontWeight(FontWeight.Bold).SetColor(new Color(0, 255, 0)))
                                    .AddRun(new TextRun("O").SetFontSize(24).SetFontWeight(FontWeight.Bold).SetColor(new Color(0, 100, 255)))
                                    .AddRun(new TextRun(" Rainbow!").SetFontSize(14)))
                            .AddChild(
                                new RichTextLabel()
                                    .SetTextSize(12)
                                    .AddRun(new TextRun("Use "))
                                    .AddRun(new TextRun("SUPERSCRIPT").SetFontSize(8))
                                    .AddRun(new TextRun(" and "))
                                    .AddRun(new TextRun("BIG").SetFontSize(20).SetFontWeight(FontWeight.Bold))
                                    .AddRun(new TextRun(" text together")))));
    }

    private UiElement BuildColorfulExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Colorful Text"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(BuildGradientText("Gradient Text Effect", 20))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("[INFO] ").SetColor(new Color(100, 180, 255)))
                                    .AddRun(new TextRun("Application started successfully at "))
                                    .AddRun(new TextRun("2024-01-15 10:30:45").SetColor(new Color(180, 180, 180))))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("[WARN] ").SetColor(PlusUiDefaults.AccentWarning))
                                    .AddRun(new TextRun("Memory usage is at "))
                                    .AddRun(new TextRun("85%").SetColor(PlusUiDefaults.AccentWarning).SetFontWeight(FontWeight.Bold))
                                    .AddRun(new TextRun(" - consider optimizing")))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("[ERROR] ").SetColor(PlusUiDefaults.AccentError))
                                    .AddRun(new TextRun("Failed to connect to "))
                                    .AddRun(new TextRun("database.server.com").SetFontWeight(FontWeight.Bold).SetColor(Colors.White))
                                    .AddRun(new TextRun(" - ").SetColor(PlusUiDefaults.AccentError))
                                    .AddRun(new TextRun("Connection refused").SetFontStyle(FontStyle.Italic).SetColor(PlusUiDefaults.AccentError)))));
    }

    private RichTextLabel BuildGradientText(string text, float fontSize)
    {
        var label = new RichTextLabel().SetTextSize(fontSize);
        var colors = new[]
        {
            new Color(255, 0, 128),
            new Color(255, 50, 100),
            new Color(255, 100, 50),
            new Color(255, 150, 0),
            new Color(255, 200, 0),
            new Color(200, 255, 0),
            new Color(100, 255, 50),
            new Color(0, 255, 100),
            new Color(0, 255, 200),
            new Color(0, 200, 255),
            new Color(0, 100, 255),
            new Color(100, 50, 255),
            new Color(200, 0, 255)
        };

        for (var i = 0; i < text.Length; i++)
        {
            var colorIndex = (int)((float)i / text.Length * colors.Length);
            colorIndex = Math.Min(colorIndex, colors.Length - 1);
            label.AddRun(new TextRun(text[i].ToString())
                .SetColor(colors[colorIndex])
                .SetFontWeight(FontWeight.Bold));
        }

        return label;
    }

    private UiElement BuildDataBindingExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Data Binding"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new RichTextLabel()
                                    .AddRun(new TextRun("Static text with "))
                                    .AddRun(new TextRun("").BindText(() => vm.DynamicText).BindColor(() => vm.HighlightColor))
                                    .AddRun(new TextRun(" content")))
                            .AddChild(
                                new HStack()
                                    .SetSpacing(8)
                                    .AddChild(
                                        new Button()
                                            .SetText("Toggle Color")
                                            .SetCommand(vm.ToggleHighlightCommand))
                                    .AddChild(
                                        new Button()
                                            .SetText("Update Text")
                                            .SetCommand(vm.UpdateTextCommand)))));
    }

    private UiElement BuildWrappingExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Text Wrapping"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new Label()
                                    .SetText("Word wrap preserves styling across line breaks:")
                                    .SetTextColor(PlusUiDefaults.TextSecondary)
                                    .SetTextSize(12))
                            .AddChild(
                                new RichTextLabel()
                                    .SetTextWrapping(TextWrapping.WordWrap)
                                    .SetDesiredWidth(400)
                                    .AddRun(new TextRun("This is a "))
                                    .AddRun(new TextRun("long paragraph ").SetFontWeight(FontWeight.Bold))
                                    .AddRun(new TextRun("with multiple styled runs that will "))
                                    .AddRun(new TextRun("wrap to multiple lines ").SetColor(PlusUiDefaults.AccentPrimary))
                                    .AddRun(new TextRun("while preserving the correct "))
                                    .AddRun(new TextRun("colors ").SetColor(PlusUiDefaults.AccentSuccess))
                                    .AddRun(new TextRun("and "))
                                    .AddRun(new TextRun("font styles").SetFontStyle(FontStyle.Italic))
                                    .AddRun(new TextRun(" across the line boundaries. "))
                                    .AddRun(new TextRun("Pretty cool, right?").SetFontWeight(FontWeight.Bold).SetColor(new Color(255, 200, 100))))
                            .AddChild(
                                new Label()
                                    .SetText("With MaxLines=2 truncation:")
                                    .SetTextColor(PlusUiDefaults.TextSecondary)
                                    .SetTextSize(12)
                                    .SetMargin(new Margin(0, 12, 0, 0)))
                            .AddChild(
                                new RichTextLabel()
                                    .SetTextWrapping(TextWrapping.WordWrap)
                                    .SetMaxLines(2)
                                    .SetDesiredWidth(400)
                                    .AddRun(new TextRun("This paragraph will be "))
                                    .AddRun(new TextRun("truncated ").SetColor(PlusUiDefaults.AccentWarning).SetFontWeight(FontWeight.Bold))
                                    .AddRun(new TextRun("after two lines because we set MaxLines=2. All the extra content that would appear on line three or beyond will simply not be rendered.")))));
    }

    private UiElement BuildSectionTitle(string title)
    {
        return new Label()
            .SetText(title)
            .SetTextSize(18)
            .SetFontWeight(FontWeight.SemiBold)
            .SetTextColor(PlusUiDefaults.AccentPrimary);
    }

    private UiElement BuildBackButton()
    {
        return new Button()
            .SetText("Back to Controls")
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetCommand(vm.GoBackCommand);
    }
}
