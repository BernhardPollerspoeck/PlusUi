using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.FontDemo;

public class FontDemoPage(FontDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            // Header with back button
            new HStack(
                new Button()
                    .SetText("← Back")
                    .SetTextSize(18)
                    .SetCommand(vm.NavCommand)
                    .SetTextColor(SKColors.White)
                    .SetPadding(new Margin(10, 5)),
                new Label()
                    .SetText("Custom Font Demo")
                    .SetTextSize(24)
                    .SetFontWeight(FontWeight.Bold)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(20, 0, 0, 0))
            ).SetMargin(new Margin(0, 0, 0, 20)),

            // Interactive Controls Section
            new VStack(
                new Label()
                    .SetText("Interactive Font Controls")
                    .SetTextSize(20)
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextColor(SKColors.LightBlue)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                new HStack(
                    new Button()
                        .SetText("Regular")
                        .SetPadding(new Margin(15, 8))
                        .SetCommand(vm.SetRegularWeightCommand)
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.LightGray)),
                    new Button()
                        .SetText("Bold")
                        .SetPadding(new Margin(15, 8))
                        .SetCommand(vm.SetBoldWeightCommand)
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.LightGray))
                        .SetMargin(new Margin(10, 0, 0, 0)),
                    new Button()
                        .SetText("Light")
                        .SetPadding(new Margin(15, 8))
                        .SetCommand(vm.SetLightWeightCommand)
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.LightGray))
                        .SetMargin(new Margin(10, 0, 0, 0)),
                    new Button()
                        .SetText("Toggle Italic")
                        .SetPadding(new Margin(15, 8))
                        .SetCommand(vm.ToggleItalicCommand)
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.LightSalmon))
                        .SetMargin(new Margin(10, 0, 0, 0))
                ).SetMargin(new Margin(0, 0, 0, 20)),

                new Label()
                    .SetText("Dynamic Font Preview:")
                    .BindFontWeight(nameof(vm.SelectedFontWeight), () => vm.SelectedFontWeight)
                    .BindFontStyle(nameof(vm.SelectedFontStyle), () => vm.SelectedFontStyle)
                    .SetTextSize(28)
                    .SetTextColor(SKColors.Yellow)
                    .SetBackground(new SolidColorBackground(new SKColor(50, 50, 50)))
                    .SetMargin(new Margin(10))
            ).SetMargin(new Margin(0, 0, 0, 30)),

            // Font Weight Examples
            new VStack(
                new Label()
                    .SetText("Font Weight Examples")
                    .SetTextSize(20)
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextColor(SKColors.LightGreen)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                new Label()
                    .SetText("Thin (100) - The quick brown fox")
                    .SetFontWeight(FontWeight.Thin)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Light (300) - The quick brown fox")
                    .SetFontWeight(FontWeight.Light)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Regular (400) - The quick brown fox")
                    .SetFontWeight(FontWeight.Regular)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Medium (500) - The quick brown fox")
                    .SetFontWeight(FontWeight.Medium)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("SemiBold (600) - The quick brown fox")
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Bold (700) - The quick brown fox")
                    .SetFontWeight(FontWeight.Bold)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Black (900) - The quick brown fox")
                    .SetFontWeight(FontWeight.Black)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White)
            ).SetMargin(new Margin(0, 0, 0, 30)),

            // Font Style Examples
            new VStack(
                new Label()
                    .SetText("Font Style Examples")
                    .SetTextSize(20)
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextColor(SKColors.LightCoral)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                new Label()
                    .SetText("Normal Style - The quick brown fox jumps over the lazy dog")
                    .SetFontStyle(FontStyle.Normal)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Italic Style - The quick brown fox jumps over the lazy dog")
                    .SetFontStyle(FontStyle.Italic)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Oblique Style - The quick brown fox jumps over the lazy dog")
                    .SetFontStyle(FontStyle.Oblique)
                    .SetTextSize(18)
                    .SetTextColor(SKColors.White)
            ).SetMargin(new Margin(0, 0, 0, 30)),

            // Typography Hierarchy Example
            new VStack(
                new Label()
                    .SetText("Typography Hierarchy")
                    .SetTextSize(20)
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextColor(SKColors.LightGoldenrodYellow)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                new Label()
                    .SetText("Heading 1")
                    .SetFontWeight(FontWeight.Bold)
                    .SetTextSize(32)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Heading 2")
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextSize(28)
                    .SetTextColor(SKColors.LightGray),

                new Label()
                    .SetText("Heading 3")
                    .SetFontWeight(FontWeight.Medium)
                    .SetTextSize(24)
                    .SetTextColor(SKColors.LightGray),

                new Label()
                    .SetText("Body Text - Regular weight for comfortable reading")
                    .SetFontWeight(FontWeight.Regular)
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White),

                new Label()
                    .SetText("Caption - Light weight for secondary information")
                    .SetFontWeight(FontWeight.Light)
                    .SetTextSize(14)
                    .SetTextColor(SKColors.DarkGray)
            ).SetMargin(new Margin(0, 0, 0, 30)),

            // Button Font Examples
            new VStack(
                new Label()
                    .SetText("Buttons with Custom Fonts")
                    .SetTextSize(20)
                    .SetFontWeight(FontWeight.SemiBold)
                    .SetTextColor(SKColors.LightSkyBlue)
                    .SetMargin(new Margin(0, 0, 0, 10)),

                new HStack(
                    new Button()
                        .SetText("Regular Button")
                        .SetFontWeight(FontWeight.Regular)
                        .SetTextSize(16)
                        .SetPadding(new Margin(15, 8))
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.White)),

                    new Button()
                        .SetText("Bold Button")
                        .SetFontWeight(FontWeight.Bold)
                        .SetTextSize(16)
                        .SetPadding(new Margin(15, 8))
                        .SetTextColor(SKColors.White)
                        .SetBackground(new SolidColorBackground(SKColors.DarkBlue))
                        .SetMargin(new Margin(10, 0, 0, 0)),

                    new Button()
                        .SetText("Light Italic")
                        .SetFontWeight(FontWeight.Light)
                        .SetFontStyle(FontStyle.Italic)
                        .SetTextSize(16)
                        .SetPadding(new Margin(15, 8))
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.LightYellow))
                        .SetMargin(new Margin(10, 0, 0, 0))
                )
            ).SetMargin(new Margin(0, 0, 0, 20))
        ).SetMargin(new Margin(20));
    }
}
