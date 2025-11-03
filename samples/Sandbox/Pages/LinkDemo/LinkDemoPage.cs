using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.LinkDemo;

public class LinkDemoPage(LinkDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with back button and page title
                new HStack(
                    new Button()
                        .SetText("← Back")
                        .SetTextSize(18)
                        .SetCommand(vm.NavCommand)
                        .SetTextColor(SKColors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("Link Control Demo")
                        .SetTextSize(32)
                        .SetTextColor(SKColors.White)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetMargin(new Margin(20, 0, 0, 30))
                ).SetMargin(new Margin(0, 0, 0, 20)),

                // Section: Basic Links
                CreateSection("Basic Links",
                    new VStack(
                        new Link()
                            .SetText("Visit PlusUi Repository on GitHub")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.DeepSkyBlue)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 10)),

                        new Link()
                            .SetText("Visit QSP.app")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.LightSeaGreen)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 10))
                    )
                ),

                // Section: Different Styles
                CreateSection("Different Styles & Sizes",
                    new VStack(
                        new Link()
                            .SetText("Small Link (12px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.CornflowerBlue)
                            .SetTextSize(12)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Medium Link (16px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.DodgerBlue)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Large Link (20px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.RoyalBlue)
                            .SetTextSize(20)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Extra Large Link (24px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.SteelBlue)
                            .SetTextSize(24)
                            .SetMargin(new Margin(0, 8))
                    )
                ),

                // Section: Custom Colors
                CreateSection("Custom Colors",
                    new VStack(
                        new Link()
                            .SetText("Blue Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.Blue)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Purple Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.Purple)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Orange Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.Orange)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Green Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.LimeGreen)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8))
                    )
                ),

                // Section: Underline Thickness
                CreateSection("Underline Thickness",
                    new VStack(
                        new Link()
                            .SetText("Thin Underline (0.5px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.Cyan)
                            .SetUnderlineThickness(0.5f)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Normal Underline (1px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.DeepSkyBlue)
                            .SetUnderlineThickness(1f)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Thick Underline (2px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.DodgerBlue)
                            .SetUnderlineThickness(2f)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Very Thick Underline (3px)")
                            .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                            .SetTextColor(SKColors.RoyalBlue)
                            .SetUnderlineThickness(3f)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8))
                    )
                ),

                // Section: Alignment
                CreateSection("Text Alignment",
                    new VStack(
                        new Link()
                            .SetText("Left Aligned Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.Aqua)
                            .SetHorizontalTextAlignment(HorizontalTextAlignment.Left)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Center Aligned Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.Turquoise)
                            .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8)),

                        new Link()
                            .SetText("Right Aligned Link")
                            .SetUrl("https://qsp.app")
                            .SetTextColor(SKColors.MediumTurquoise)
                            .SetHorizontalTextAlignment(HorizontalTextAlignment.Right)
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8))
                    )
                ),

                // Section: Combined with Text
                CreateSection("Combined with Regular Text",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Check out our project on ")
                                .SetTextColor(SKColors.White)
                                .SetTextSize(16),
                            new Link()
                                .SetText("GitHub")
                                .SetUrl("https://github.com/BernhardPollerspoeck/PlusUi")
                                .SetTextColor(SKColors.DeepSkyBlue)
                                .SetTextSize(16)
                        ).SetMargin(new Margin(0, 10)),

                        new HStack(
                            new Label()
                                .SetText("Learn more at ")
                                .SetTextColor(SKColors.White)
                                .SetTextSize(16),
                            new Link()
                                .SetText("qsp.app")
                                .SetUrl("https://qsp.app")
                                .SetTextColor(SKColors.LightSeaGreen)
                                .SetTextSize(16),
                            new Label()
                                .SetText(" for details")
                                .SetTextColor(SKColors.White)
                                .SetTextSize(16)
                        ).SetMargin(new Margin(0, 10))
                    )
                ),

                // Info Section
                new VStack(
                    new Label()
                        .SetText("ℹ️ Click any link to open it in your default browser")
                        .SetTextSize(14)
                        .SetTextColor(SKColors.LightGray)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                        .SetMargin(new Margin(0, 30, 0, 10))
                ).SetMargin(new Margin(20, 0))
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
            new Border()
                .AddChild(content)
                .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
