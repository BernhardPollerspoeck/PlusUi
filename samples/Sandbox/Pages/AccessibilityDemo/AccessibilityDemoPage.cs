using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.AccessibilityDemo;

public class AccessibilityDemoPage(AccessibilityDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with back button
                new HStack(
                    new Button()
                        .SetText("<- Back")
                        .SetTextSize(16)
                        .SetCommand(vm.GoBackCommand)
                        .SetTextColor(SKColors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("Accessibility Demo")
                        .SetTextSize(24)
                        .SetTextColor(SKColors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 20)),

                // Status Display
                new Label()
                    .SetText("Letzte Aktion:")
                    .SetTextSize(12)
                    .SetTextColor(SKColors.Gray),
                new Label()
                    .BindText(nameof(vm.LastAction), () => vm.LastAction)
                    .SetTextSize(16)
                    .SetTextColor(SKColors.LimeGreen)
                    .SetMargin(new Margin(0, 0, 0, 20)),

                // Section 1: Tab Navigation
                new Label()
                    .SetText("1. Focus & Tab Navigation")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.White)
                    .SetAccessibilityLabel("Abschnitt: Focus und Tab Navigation"),
                new Label()
                    .SetText("Drucke TAB um durch die Controls zu navigieren")
                    .SetTextSize(12)
                    .SetTextColor(SKColors.Gray)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                // TabIndex Demo
                new HStack(
                    new Button()
                        .SetText("Tab 1")
                        .SetTabIndex(1)
                        .SetAccessibilityLabel("Erster Button")
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("Tab 3")
                        .SetTabIndex(3)
                        .SetAccessibilityLabel("Dritter Button")
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("Tab 2")
                        .SetTabIndex(2)
                        .SetAccessibilityLabel("Zweiter Button")
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("Kein TabStop")
                        .SetTabStop(false)
                        .SetAccessibilityLabel("Dieser wird uebersprungen")
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8))
                ).SetMargin(new Margin(0, 0, 0, 10)),

                // Focus Ring Colors
                new Label()
                    .SetText("Focus Ring Farben (fokussiere mit Tab):")
                    .SetTextSize(14)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(0, 15, 0, 5)),
                new HStack(
                    new Button()
                        .SetText("Standard")
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("Rot")
                        .SetFocusRingColor(SKColors.Red)
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("Gruen")
                        .SetFocusRingColor(SKColors.LimeGreen)
                        .SetFocusRingWidth(3)
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("Breit Gelb")
                        .SetFocusRingColor(SKColors.Yellow)
                        .SetFocusRingWidth(4)
                        .SetFocusRingOffset(4)
                        .SetCommand(vm.ButtonCommand)
                        .SetPadding(new Margin(15, 8))
                ).SetMargin(new Margin(0, 0, 0, 20)),

                // Section 2: Interactive Controls
                new Label()
                    .SetText("2. Interactive Controls mit Accessibility")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(0, 10, 0, 10)),

                // Checkbox
                new HStack(
                    new Checkbox()
                        .BindIsChecked(nameof(vm.IsChecked), () => vm.IsChecked, v => vm.IsChecked = v)
                        .SetAccessibilityLabel("Test Checkbox"),
                    new Label()
                        .SetText("Checkbox mit Accessibility Label")
                        .SetTextColor(SKColors.White)
                        .SetMargin(new Margin(10, 0, 0, 0))
                ).SetMargin(new Margin(0, 0, 0, 10)),

                // Toggle
                new HStack(
                    new Toggle()
                        .SetAccessibilityLabel("Feature aktivieren")
                        .SetAccessibilityHint("Schaltet das Feature ein oder aus"),
                    new Label()
                        .SetText("Toggle mit Hint")
                        .SetTextColor(SKColors.White)
                        .SetMargin(new Margin(10, 0, 0, 0))
                ).SetMargin(new Margin(0, 0, 0, 10)),

                // Slider
                new Label()
                    .SetText("Slider (0-100):")
                    .SetTextColor(SKColors.White),
                new Slider()
                    .SetMinimum(0)
                    .SetMaximum(100)
                    .BindValue(nameof(vm.SliderValue), () => (float)vm.SliderValue, v => vm.SliderValue = v)
                    .SetAccessibilityLabel("Lautstaerke")
                    .SetDesiredWidth(300)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                // RadioButtons
                new Label()
                    .SetText("RadioButtons:")
                    .SetTextColor(SKColors.White),
                new HStack(
                    new RadioButton()
                        .SetText("Option A")
                        .SetGroup("options")
                        .SetValue("A")
                        .SetIsSelected(true),
                    new RadioButton()
                        .SetText("Option B")
                        .SetGroup("options")
                        .SetValue("B"),
                    new RadioButton()
                        .SetText("Option C")
                        .SetGroup("options")
                        .SetValue("C")
                ).SetMargin(new Margin(0, 0, 0, 20)),

                // Section 3: Form Controls
                new Label()
                    .SetText("3. Form Controls")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(0, 10, 0, 10)),

                // Entry
                new Label()
                    .SetText("Text Eingabe:")
                    .SetTextColor(SKColors.White),
                new Entry()
                    .SetPlaceholder("Gib hier Text ein...")
                    .BindText(nameof(vm.EntryText), () => vm.EntryText, v => vm.EntryText = v)
                    .SetAccessibilityLabel("Texteingabe Feld")
                    .SetAccessibilityHint("Gib beliebigen Text ein")
                    .SetDesiredWidth(300)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                // DatePicker
                new Label()
                    .SetText("Datum:")
                    .SetTextColor(SKColors.White),
                new DatePicker()
                    .SetAccessibilityLabel("Geburtsdatum")
                    .SetAccessibilityHint("Waehle dein Geburtsdatum")
                    .SetMargin(new Margin(0, 0, 0, 10)),

                // TimePicker
                new Label()
                    .SetText("Zeit:")
                    .SetTextColor(SKColors.White),
                new TimePicker()
                    .SetAccessibilityLabel("Termin Uhrzeit")
                    .SetMargin(new Margin(0, 0, 0, 10)),

                // ComboBox
                new Label()
                    .SetText("ComboBox:")
                    .SetTextColor(SKColors.White),
                new ComboBox<string>()
                    .SetItemsSource(vm.ComboItems)
                    .SetAccessibilityLabel("Land auswaehlen")
                    .SetDesiredWidth(200)
                    .SetMargin(new Margin(0, 0, 0, 20)),

                // Section 4: High Contrast
                new Label()
                    .SetText("4. High Contrast (Win: Alt+Shift+PrintScreen)")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(0, 10, 0, 10)),
                new HStack(
                    new Button()
                        .SetText("X")
                        .SetAccessibilityLabel("Schliessen")
                        .SetHighContrastBackground(SKColors.Yellow)
                        .SetHighContrastForeground(SKColors.Black)
                        .SetPadding(new Margin(15, 8)),
                    new Button()
                        .SetText("OK")
                        .SetHighContrastBackground(SKColors.White)
                        .SetHighContrastForeground(SKColors.Black)
                        .SetPadding(new Margin(15, 8)),
                    new Checkbox()
                        .SetHighContrastBackground(SKColors.White)
                        .SetAccessibilityLabel("High Contrast Checkbox")
                ).SetMargin(new Margin(0, 0, 0, 10)),

                new Label()
                    .SetText("Hinweis: High Contrast muss in PlusUiConfiguration aktiviert sein")
                    .SetTextSize(11)
                    .SetTextColor(SKColors.Gray)

            ).SetMargin(new Margin(20))
        );
    }
}
