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
                    .SetTextColor(SKColors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                // Section: Text Only Buttons
                CreateSection("Text Only Buttons",
                    new VStack(
                        new Button()
                            .SetText("Simple Button")
                            .SetPadding(new Margin(15, 10))
                            .SetBackgroundColor(SKColors.DodgerBlue)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Larger Button")
                            .SetTextSize(18)
                            .SetPadding(new Margin(20, 12))
                            .SetBackgroundColor(SKColors.MediumSeaGreen)
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
                            .SetBackgroundColor(SKColors.OrangeRed)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(5)),
                        new Button()
                            .SetIcon("plusui.png")
                            .SetTextSize(24)
                            .SetPadding(new Margin(15))
                            .SetBackgroundColor(SKColors.Purple)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(5)),
                        new Button()
                            .SetIcon("plusui.png")
                            .SetTextSize(32)
                            .SetPadding(new Margin(18))
                            .SetBackgroundColor(SKColors.DarkSlateBlue)
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
                            .SetBackgroundColor(SKColors.ForestGreen)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Upload File")
                            .SetIcon("plusui.png")
                            .SetTextSize(16)
                            .SetPadding(new Margin(18, 12))
                            .SetBackgroundColor(SKColors.RoyalBlue)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Download")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Leading)  // Explicitly set
                            .SetPadding(new Margin(15, 10))
                            .SetBackgroundColor(SKColors.DarkOrange)
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
                            .SetBackgroundColor(SKColors.DodgerBlue)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Continue")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetTextSize(16)
                            .SetPadding(new Margin(18, 12))
                            .SetBackgroundColor(SKColors.MediumPurple)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Submit")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetPadding(new Margin(15, 10))
                            .SetBackgroundColor(SKColors.Crimson)
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
                            .SetBackgroundColor(SKColors.DarkGoldenrod)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Premium Feature")
                            .SetIcon("plusui.png")
                            .SetIconPosition(IconPosition.Leading | IconPosition.Trailing)
                            .SetTextSize(18)
                            .SetPadding(new Margin(20, 12))
                            .SetBackgroundColor(SKColors.DarkMagenta)
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
                            .SetBackgroundColor(SKColors.Teal)
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 5)),
                        new Button()
                            .SetText("Another Random")
                            .SetIcon("https://picsum.photos/60")
                            .SetIconPosition(IconPosition.Trailing)
                            .SetPadding(new Margin(15, 10))
                            .SetBackgroundColor(SKColors.SlateBlue)
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
                        .AddChild(0, 0,
                            new Button()
                                .SetText("Small")
                                .SetIcon("plusui.png")
                                .SetTextSize(12)
                                .SetPadding(new Margin(10, 6))
                                .SetBackgroundColor(SKColors.CornflowerBlue)
                                .SetCornerRadius(6)
                                .SetMargin(new Margin(5)))
                        .AddChild(1, 0,
                            new Button()
                                .SetText("Medium")
                                .SetIcon("plusui.png")
                                .SetTextSize(14)
                                .SetPadding(new Margin(12, 8))
                                .SetBackgroundColor(SKColors.MediumSlateBlue)
                                .SetCornerRadius(8)
                                .SetMargin(new Margin(5)))
                        .AddChild(2, 0,
                            new Button()
                                .SetText("Large")
                                .SetIcon("plusui.png")
                                .SetTextSize(16)
                                .SetPadding(new Margin(14, 10))
                                .SetBackgroundColor(SKColors.DarkSlateBlue)
                                .SetCornerRadius(10)
                                .SetMargin(new Margin(5)))
                        .AddChild(0, 1,
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(12)
                                .SetPadding(new Margin(8))
                                .SetBackgroundColor(SKColors.IndianRed)
                                .SetCornerRadius(20)
                                .SetMargin(new Margin(5)))
                        .AddChild(1, 1,
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(16)
                                .SetPadding(new Margin(12))
                                .SetBackgroundColor(SKColors.Tomato)
                                .SetCornerRadius(25)
                                .SetMargin(new Margin(5)))
                        .AddChild(2, 1,
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(16))
                                .SetBackgroundColor(SKColors.OrangeRed)
                                .SetCornerRadius(30)
                                .SetMargin(new Margin(5)))
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
                .SetTextColor(SKColors.LightGray)
                .SetMargin(new Margin(0, 15, 0, 10)),
            new Border(content)
                .SetBackgroundColor(new SKColor(40, 40, 40))
                .SetCornerRadius(12)
                .SetPadding(new Margin(20))
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
