using PlusUi.core;

namespace PlusUi.Demo.Pages.EntryDemo;

public class EntryPage(EntryPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
                new VStack()
                    .SetSpacing(24)
                    .SetMargin(new Margin(40))
                    .AddChild(BuildHeader())
                    .AddChild(BuildBasicExamples())
                    .AddChild(BuildMultiLineExample())
                    .AddChild(BuildKeyboardTypesExample())
                    .AddChild(BuildDataBindingExample())
                    .AddChild(BuildInteractiveExample())
                    .AddChild(BuildBackButton()))
            .SetCanScrollHorizontally(false);
    }

    private UiElement BuildHeader()
    {
        return new VStack()
            .SetSpacing(8)
            .AddChild(
                new Label()
                    .SetText("Entry Control")
                    .SetTextSize(32)
                    .SetFontWeight(FontWeight.Bold)
                    .SetTextColor(PlusUiDefaults.AccentPrimary))
            .AddChild(
                new Label()
                    .SetText("A versatile text input control with cursor navigation, text selection, copy/paste support, and multi-line capability.")
                    .SetTextColor(PlusUiDefaults.TextSecondary)
                    .SetTextWrapping(TextWrapping.WordWrap));
    }

    private UiElement BuildBasicExamples()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Basic Entry"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Username")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetPlaceholder("Enter username...")
                                            .SetDesiredWidth(300)
                                            .BindText(() => vm.Username, v => vm.Username = v)))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Password")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetIsPassword(true)
                                            .SetPlaceholder("Enter password...")
                                            .SetDesiredWidth(300)
                                            .BindText(() => vm.Password, v => vm.Password = v)))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Email (Max 50 characters)")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetPlaceholder("email@example.com")
                                            .SetMaxLength(50)
                                            .SetDesiredWidth(300)
                                            .BindText(() => vm.Email, v => vm.Email = v)))));
    }

    private UiElement BuildMultiLineExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Multi-Line Entry"))
            .AddChild(
                new Label()
                    .SetText("Use SetIsMultiLine(true) to enable multi-line input. Default: 5 lines. Press Enter to add new lines.")
                    .SetTextColor(PlusUiDefaults.TextSecondary)
                    .SetTextSize(12))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Notes (Default: 5 lines)")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetIsMultiLine(true)
                                            .SetPlaceholder("Enter your notes here...")
                                            .SetDesiredWidth(500)
                                            .BindText(() => vm.Notes, v => vm.Notes = v)))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Custom Lines (MinLines: 3, MaxLines: 10)")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetIsMultiLine(true)
                                            .SetMinLines(3)
                                            .SetMaxLines(10)
                                            .SetPlaceholder("Type as much as you want...")
                                            .SetDesiredWidth(400)))));
    }

    private UiElement BuildKeyboardTypesExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Keyboard Types (Mobile)"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new HStack()
                            .SetSpacing(16)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Numeric")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetKeyboard(KeyboardType.Numeric)
                                            .SetPlaceholder("123...")
                                            .SetDesiredWidth(120)))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Email")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetKeyboard(KeyboardType.Email)
                                            .SetPlaceholder("email@...")
                                            .SetDesiredWidth(150)))
                            .AddChild(
                                new VStack()
                                    .SetSpacing(4)
                                    .AddChild(
                                        new Label()
                                            .SetText("Phone")
                                            .SetTextSize(12)
                                            .SetTextColor(PlusUiDefaults.TextSecondary))
                                    .AddChild(
                                        new Entry()
                                            .SetKeyboard(KeyboardType.Telephone)
                                            .SetPlaceholder("+1...")
                                            .SetDesiredWidth(120)))));
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
                                new Entry()
                                    .SetPlaceholder("Type something...")
                                    .SetDesiredWidth(300)
                                    .BindText(() => vm.BoundText, v => vm.BoundText = v))
                            .AddChild(
                                new HStack()
                                    .SetSpacing(8)
                                    .AddChild(
                                        new Label()
                                            .SetText("You typed: "))
                                    .AddChild(
                                        new Label()
                                            .BindText(() => vm.BoundText)
                                            .SetFontWeight(FontWeight.Bold)))
                            .AddChild(
                                new HStack()
                                    .SetSpacing(8)
                                    .AddChild(
                                        new Label()
                                            .SetText("Character count: "))
                                    .AddChild(
                                        new Label()
                                            .BindText(() => vm.CharacterCount.ToString())
                                            .SetTextColor(PlusUiDefaults.AccentPrimary)
                                            .SetFontWeight(FontWeight.Bold)))));
    }

    private UiElement BuildInteractiveExample()
    {
        return new VStack()
            .SetSpacing(12)
            .AddChild(BuildSectionTitle("Features"))
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundSecondary)
                    .SetCornerRadius(8)
                    .AddChild(
                        new VStack()
                            .SetSpacing(12)
                            .SetMargin(new Margin(16))
                            .AddChild(
                                new Label()
                                    .SetText("Try these keyboard shortcuts:")
                                    .SetFontWeight(FontWeight.SemiBold))
                            .AddChild(BuildFeatureRow("Ctrl+A", "Select all text"))
                            .AddChild(BuildFeatureRow("Ctrl+C", "Copy selection"))
                            .AddChild(BuildFeatureRow("Ctrl+X", "Cut selection"))
                            .AddChild(BuildFeatureRow("Ctrl+V", "Paste from clipboard"))
                            .AddChild(BuildFeatureRow("Ctrl+Left/Right", "Jump to word boundary"))
                            .AddChild(BuildFeatureRow("Shift+Arrow", "Extend selection"))
                            .AddChild(BuildFeatureRow("Home/End", "Jump to line start/end"))
                            .AddChild(BuildFeatureRow("Ctrl+Home/End", "Jump to text start/end (multi-line)"))
                            .AddChild(BuildFeatureRow("Arrow Up/Down", "Navigate lines (multi-line)"))
                            .AddChild(
                                new Button()
                                    .SetText("Clear All Fields")
                                    .SetCommand(vm.ClearAllCommand)
                                    .SetMargin(new Margin(0, 8, 0, 0)))));
    }

    private UiElement BuildFeatureRow(string shortcut, string description)
    {
        return new HStack()
            .SetSpacing(12)
            .AddChild(
                new Border()
                    .SetBackground(new Color(60, 60, 60))
                    .SetCornerRadius(4)
                    .AddChild(
                        new Label()
                            .SetText(shortcut)
                            .SetTextSize(12)
                            .SetFontFamily("Consolas")
                            .SetMargin(new Margin(6, 2))))
            .AddChild(
                new Label()
                    .SetText(description)
                    .SetTextColor(PlusUiDefaults.TextSecondary));
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
