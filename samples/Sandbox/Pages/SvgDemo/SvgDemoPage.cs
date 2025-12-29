using PlusUi.core;

namespace Sandbox.Pages.SvgDemo;

public class SvgDemoPage(SvgDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with back button
                new HStack(
                    new Button()
                        .SetText("← Back")
                        .SetTextSize(16)
                        .SetCommand(vm.GoBackCommand)
                        .SetTextColor(Colors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("SVG Support Demo")
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 20)),

                // Section: SVG Images from Web
                CreateSection("SVG Images from Web",
                    new HStack(
                        new Image()
                            .SetImageSource("https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg")
                            .SetDesiredSize(new Size(100, 100))
                            .SetMargin(new Margin(10)),
                        new Image()
                            .SetImageSource("https://upload.wikimedia.org/wikipedia/commons/4/4f/Csharp_Logo.png")
                            .SetDesiredSize(new Size(100, 100))
                            .SetMargin(new Margin(10))
                    ).SetHorizontalAlignment(HorizontalAlignment.Center)
                ),

                // Section: SVG with Tint Colors
                CreateSection("SVG with Tint Colors",
                    new VStack(
                        new Label()
                            .SetText("Same SVG icon with different tint colors:")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new HStack(
                            CreateTintedIcon(Colors.White),
                            CreateTintedIcon(Colors.Red),
                            CreateTintedIcon(Colors.Green),
                            CreateTintedIcon(Colors.Blue),
                            CreateTintedIcon(Colors.Yellow),
                            CreateTintedIcon(Colors.Magenta),
                            CreateTintedIcon(Colors.Cyan)
                        ).SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
                ),

                // Section: Button with SVG Icons
                CreateSection("Buttons with SVG Icons",
                    new VStack(
                        new HStack(
                            new Button()
                                .SetText("Default")
                                .SetIcon("https://www.svgrepo.com/show/532994/plus.svg")
                                .SetPadding(new Margin(15, 10))
                                .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
                                .SetCornerRadius(8)
                                .SetMargin(new Margin(5)),
                            new Button()
                                .SetText("Red Tint")
                                .SetIcon("https://www.svgrepo.com/show/532994/plus.svg")
                                .SetIconTintColor(Colors.Red)
                                .SetPadding(new Margin(15, 10))
                                .SetBackground(new SolidColorBackground(Colors.DarkSlateGray))
                                .SetCornerRadius(8)
                                .SetMargin(new Margin(5)),
                            new Button()
                                .SetText("Yellow Tint")
                                .SetIcon("https://www.svgrepo.com/show/532994/plus.svg")
                                .SetIconTintColor(Colors.Yellow)
                                .SetPadding(new Margin(15, 10))
                                .SetBackground(new SolidColorBackground(Colors.DarkGreen))
                                .SetCornerRadius(8)
                                .SetMargin(new Margin(5))
                        ).SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
                ),

                // Section: Different SVG Sizes
                CreateSection("SVG at Different Sizes",
                    new HStack(
                        new Image()
                            .SetImageSource("https://www.svgrepo.com/show/532994/plus.svg")
                            .SetTintColor(Colors.Gold)
                            .SetDesiredSize(new Size(24, 24))
                            .SetMargin(new Margin(10)),
                        new Image()
                            .SetImageSource("https://www.svgrepo.com/show/532994/plus.svg")
                            .SetTintColor(Colors.Gold)
                            .SetDesiredSize(new Size(48, 48))
                            .SetMargin(new Margin(10)),
                        new Image()
                            .SetImageSource("https://www.svgrepo.com/show/532994/plus.svg")
                            .SetTintColor(Colors.Gold)
                            .SetDesiredSize(new Size(72, 72))
                            .SetMargin(new Margin(10)),
                        new Image()
                            .SetImageSource("https://www.svgrepo.com/show/532994/plus.svg")
                            .SetTintColor(Colors.Gold)
                            .SetDesiredSize(new Size(96, 96))
                            .SetMargin(new Margin(10))
                    ).SetHorizontalAlignment(HorizontalAlignment.Center)
                ),

                new Label()
                    .SetText("SVGs are rendered at native resolution for crisp display at any size!")
                    .SetTextColor(Colors.LightGray)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(20))
            )
        );
    }

    private Image CreateTintedIcon(Color tintColor)
    {
        return new Image()
            .SetImageSource("https://www.svgrepo.com/show/532994/plus.svg")
            .SetTintColor(tintColor)
            .SetDesiredSize(new Size(48, 48))
            .SetMargin(new Margin(8));
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
