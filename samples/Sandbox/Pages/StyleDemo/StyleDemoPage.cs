using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.StyleDemo;

public class StyleDemoPage(StyleDemoPageViewModel vm, IThemeService themeService) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with Theme Toggle
                new VStack(
                    new Label()
                        .SetText("PlusUiStyle Demo")
                        .SetTextSize(32)
                        .SetFontWeight(FontWeight.Bold)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetMargin(new Margin(0, 20, 0, 10)),

                    new Label()
                        .SetText("Modern Light & Dark Theme System")
                        .SetTextSize(16)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetMargin(new Margin(0, 0, 0, 20)),

                    // Theme Switcher
                    new Border(
                        new HStack(
                            new Label()
                                .SetText("Theme:")
                                .SetFontWeight(FontWeight.SemiBold)
                                .SetVerticalAlignment(VerticalAlignment.Center),

                            new Button()
                                .SetText("Light")
                                .OnClick(() => themeService.SetTheme(Theme.Light))
                                .SetMargin(new Margin(8, 0, 0, 0)),

                            new Button()
                                .SetText("Dark")
                                .OnClick(() => themeService.SetTheme(Theme.Dark))
                                .SetMargin(new Margin(4, 0, 0, 0))
                        ).SetHorizontalAlignment(HorizontalAlignment.Center)
                    ).SetMargin(new Margin(0, 0, 0, 30))
                        .SetHorizontalAlignment(HorizontalAlignment.Center),

                    new Separator()
                        .SetMargin(new Margin(0, 0, 0, 30))
                ),

                // Text Elements Section
                CreateSection("Text Elements",
                    new VStack(
                        new Label()
                            .SetText("This is a standard Label with default styling"),

                        new Label()
                            .SetText("Bold Label")
                            .SetFontWeight(FontWeight.Bold),

                        new Link()
                            .SetText("This is a clickable Link")
                    )
                ),

                // Input Controls Section
                CreateSection("Input Controls",
                    new VStack(
                        new Label()
                            .SetText("Entry (Text Input):")
                            .SetFontWeight(FontWeight.SemiBold)
                            .SetMargin(new Margin(0, 0, 0, 8)),

                        new Entry()
                            .SetPlaceholder("Enter your text here...")
                            .BindText(nameof(vm.EntryText), () => vm.EntryText, value => vm.EntryText = value),

                        new Entry()
                            .SetPlaceholder("Password field")
                            .SetIsPassword(true)
                            .SetMargin(new Margin(0, 8, 0, 0))
                    )
                ),

                // Buttons Section
                CreateSection("Buttons",
                    new HStack(
                        new Button()
                            .SetText("Primary Button"),

                        new Button()
                            .SetText("Secondary")
                            .SetMargin(new Margin(8, 0, 0, 0)),

                        new Button()
                            .SetText("Action")
                            .SetMargin(new Margin(8, 0, 0, 0))
                    ).SetHorizontalAlignment(HorizontalAlignment.Center)
                ),

                // Selection Controls Section
                CreateSection("Selection Controls",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Checkbox:")
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetDesiredWidth(120),

                            new Checkbox()
                                .BindIsChecked(nameof(vm.IsChecked), () => vm.IsChecked, value => vm.IsChecked = value),

                            new Label()
                                .BindText(nameof(vm.IsChecked), () => vm.IsChecked ? "Checked" : "Unchecked")
                                .SetMargin(new Margin(12, 0, 0, 0))
                        ).SetMargin(new Margin(0, 8)),

                        new HStack(
                            new Label()
                                .SetText("Toggle:")
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetDesiredWidth(120),

                            new Toggle()
                                .BindIsOn(nameof(vm.IsToggled), () => vm.IsToggled, value => vm.IsToggled = value),

                            new Label()
                                .BindText(nameof(vm.IsToggled), () => vm.IsToggled ? "ON" : "OFF")
                                .SetMargin(new Margin(12, 0, 0, 0))
                        ).SetMargin(new Margin(0, 8)),

                        new HStack(
                            new Label()
                                .SetText("Slider:")
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetDesiredWidth(120),

                            new Slider()
                                .BindValue(nameof(vm.SliderValue), () => vm.SliderValue, value => vm.SliderValue = value),

                            new Label()
                                .BindText(nameof(vm.SliderValue), () => $"{vm.SliderValue:F2}")
                                .SetMargin(new Margin(12, 0, 0, 0))
                                .SetDesiredWidth(50)
                        ).SetMargin(new Margin(0, 8))
                    )
                ),

                // Progress & Loading Section
                CreateSection("Progress & Loading",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("ProgressBar:")
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetDesiredWidth(120),

                            new ProgressBar()
                                .BindProgress(nameof(vm.ProgressValue), () => vm.ProgressValue)
                        ).SetMargin(new Margin(0, 8)),

                        new HStack(
                            new Label()
                                .SetText("ActivityIndicator:")
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetDesiredWidth(120),

                            new ActivityIndicator()
                                .SetIsRunning(true)
                        ).SetMargin(new Margin(0, 8))
                    )
                ),

                // Visual Elements Section
                CreateSection("Visual Elements",
                    new VStack(
                        new Label()
                            .SetText("Border Container:")
                            .SetFontWeight(FontWeight.SemiBold)
                            .SetMargin(new Margin(0, 0, 0, 8)),

                        new Border(
                            new Label()
                                .SetText("Content inside a styled Border")
                                .SetMargin(new Margin(0))
                        ),

                        new Label()
                            .SetText("Separator:")
                            .SetFontWeight(FontWeight.SemiBold)
                            .SetMargin(new Margin(0, 16, 0, 8)),

                        new Separator(),

                        new Label()
                            .SetText("Solid (colored rectangle):")
                            .SetFontWeight(FontWeight.SemiBold)
                            .SetMargin(new Margin(0, 16, 0, 8)),

                        new HStack(
                            new Solid(),
                            new Solid().SetMargin(new Margin(8, 0, 0, 0)),
                            new Solid().SetMargin(new Margin(8, 0, 0, 0))
                        ).SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
                ),

                // Footer
                new Label()
                    .SetText("All controls above use PlusUiStyle for consistent theming")
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetTextSize(12)
                    .SetMargin(new Margin(0, 40, 0, 20))
            ).SetMargin(new Margin(20))
        ).SetBackground(new SolidColorBackground(new SKColor(15, 23, 42))); // Dark background
    }

    private static Border CreateSection(string title, UiElement content)
    {
        return new Border(
            new VStack(
                new Label()
                    .SetText(title)
                    .SetTextSize(20)
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetMargin(new Margin(0, 0, 0, 16)),

                content
            )
        ).SetMargin(new Margin(0, 0, 0, 24));
    }
}
