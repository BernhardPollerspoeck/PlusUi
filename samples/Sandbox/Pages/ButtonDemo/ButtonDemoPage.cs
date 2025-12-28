using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ButtonDemo;

public class ButtonDemoPage(ButtonDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Page Title
                new Label()
                    .SetText("Button Icon Demo")
                    .SetTextSize(32)
                    .SetTextColor(Colors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                // Section: Text Only Buttons
                CreateSection("Text Only Buttons",
                    new VStack(
                        new Button()
                            .SetText("Simple Button")
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Larger Button")
                            .SetTextSize(18)
                            .SetPadding(new Margin(20, 12))
                            .SetBackground(new SolidColorBackground(Colors.MediumSeaGreen))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5))
                    )
                ),

                // Section: Icon Only Buttons
                CreateSection("Icon Only Buttons",
                    new HStack(
                        new Button()
                            .SetIcon("plusui.png")
                            .SetPadding(new Margin(12))
                            .SetBackground(new SolidColorBackground(Colors.OrangeRed))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(5)),
                        new Button()
                            .SetIcon("plusui.png")
                            .SetTextSize(24)
                            .SetPadding(new Margin(15))
                            .SetBackground(new SolidColorBackground(Colors.Purple))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(5)),
                        new Button()
                            .SetIcon("plusui.png")
                            .SetTextSize(32)
                            .SetPadding(new Margin(18))
                            .SetBackground(new SolidColorBackground(Colors.DarkSlateBlue))
                            .SetCornerRadius(50)
                            .SetMargin(new Margin(5))
                    ).SetHorizontalAlignment(HorizontalAlignment.Center)
                ),

                // Section: Icon Leading (Default)
                CreateSection("Icon Leading (Default Position)",
                    new VStack(
                        new Button()
                            .SetText("Save")
                            .SetIcon("plusui.png")
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.ForestGreen))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Upload File")
                            .SetIcon("plusui.png")
                            .SetTextSize(16)
                            .SetPadding(new Margin(18, 12))
                            .SetBackground(new SolidColorBackground(Colors.RoyalBlue))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Download")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Leading)  // Explicitly set
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.DarkOrange))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5))
                    )
                ),

                // Section: Icon Trailing
                CreateSection("Icon Trailing",
                    new VStack(
                        new Button()
                            .SetText("Next")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Continue")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetTextSize(16)
                            .SetPadding(new Margin(18, 12))
                            .SetBackground(new SolidColorBackground(Colors.MediumPurple))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Submit")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.Crimson))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5))
                    )
                ),

                // Section: Icons on Both Sides
                CreateSection("Icons on Both Sides",
                    new VStack(
                        new Button()
                            .SetText("Important Action")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Leading | IconPosition.Trailing)
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.DarkGoldenrod))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Premium Feature")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Leading | IconPosition.Trailing)
                            .SetTextSize(18)
                            .SetPadding(new Margin(20, 12))
                            .SetBackground(new SolidColorBackground(Colors.DarkMagenta))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5))
                    )
                ),

                // Section: With Web Icons
                CreateSection("With Web Icons (Async Loading)",
                    new VStack(
                        new Button()
                            .SetText("Random Image")
                            .SetIcon("https://picsum.photos/50")
                            .SetIconPosition(IconPosition.Leading)
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.Teal))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Another Random")
                            .SetIcon("https://picsum.photos/60")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetPadding(new Margin(15, 10))
                            .SetBackground(new SolidColorBackground(Colors.SlateBlue))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5))
                    )
                ),

                // Section: Mixed Styles
                CreateSection("Mixed Styles & Sizes",
                    new Grid()
                        .AddColumn(Column.Star)
                        .AddColumn(Column.Star)
                        .AddColumn(Column.Star)
                        .AddRow(Row.Auto)
                        .AddRow(Row.Auto)
                        .AddChild(
                            new Button()
                                .SetText("Small")
                                .SetIcon("plusui.png")
                                .SetTextSize(12)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(Colors.CornflowerBlue))
                                .SetCornerRadius(6)
                                .SetMargin(new Margin(5)))
                        .AddChild(
                            new Button()
                                .SetText("Medium")
                                .SetIcon("plusui.png")
                                .SetTextSize(14)
                                .SetPadding(new Margin(12, 8))
                                .SetBackground(new SolidColorBackground(Colors.MediumSlateBlue))
                                .SetCornerRadius(8)
                                .SetMargin(new Margin(5)),
                            1, 0)
                        .AddChild(
                            new Button()
                                .SetText("Large")
                                .SetIcon("plusui.png")
                                .SetTextSize(16)
                                .SetPadding(new Margin(14, 10))
                                .SetBackground(new SolidColorBackground(Colors.DarkSlateBlue))
                                .SetCornerRadius(10)
                                .SetMargin(new Margin(5)),
                            2, 0)
                        .AddChild(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(12)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(Colors.IndianRed))
                                .SetCornerRadius(20)
                                .SetMargin(new Margin(5)),
                            0, 1)
                        .AddChild(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(16)
                                .SetPadding(new Margin(12))
                                .SetBackground(new SolidColorBackground(Colors.Tomato))
                                .SetCornerRadius(25)
                                .SetMargin(new Margin(5)),
                            1, 1)
                        .AddChild(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(16))
                                .SetBackground(new SolidColorBackground(Colors.OrangeRed))
                                .SetCornerRadius(30)
                                .SetMargin(new Margin(5)),
                            2, 1)
                )
            )
        );
    }

    private UiElement CreateSection(string title, UiElement content)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(20)
                .SetTextColor(Colors.LightGray)
                .SetMargin(new Margin(0, 15, 0, 10)),
            new Border()
                .AddChild(content)
                .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
