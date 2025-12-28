using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.TextWrapDemo;

public class TextWrapDemoPage(TextWrapDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Page Title
                new Label()
                    .SetText("Text Wrapping & Truncation Demo")
                    .SetTextSize(28)
                    .SetTextColor(Colors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                // Section: Text Wrapping Options
                CreateSection("Text Wrapping Options",
                    new VStack(
                        CreateDemoRow("NoWrap (Default)", 
                            new Label()
                                .SetText(vm.SampleText)
                                .SetTextWrapping(TextWrapping.NoWrap)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(40, 40, 60)))
                                .SetTextColor(Colors.White)),

                    CreateDemoRow("Wrap",
                        new Label()
                            .SetText(vm.SampleText)
                            .SetTextWrapping(TextWrapping.Wrap)
                            .SetDesiredWidth(300)
                            .SetBackground(new SolidColorBackground(new Color(40, 40, 60)))
                            .SetTextColor(Colors.White)),

                    CreateDemoRow("WordWrap",
                        new Label()
                            .SetText(vm.SampleText)
                            .SetTextWrapping(TextWrapping.WordWrap)
                            .SetDesiredWidth(300)
                            .SetBackground(new SolidColorBackground(new Color(40, 40, 60)))
                            .SetTextColor(Colors.White))
                    )
                ),

                // Section: Text Truncation Options
                CreateSection("Text Truncation Options",
                    new VStack(
                        CreateDemoRow("None (Default)",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextTruncation(TextTruncation.None)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 40)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("Start",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextTruncation(TextTruncation.Start)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 40)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("Middle",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextTruncation(TextTruncation.Middle)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 40)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("End",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextTruncation(TextTruncation.End)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 40)))
                                .SetTextColor(Colors.White))
                    )
                ),

                // Section: MaxLines
                CreateSection("MaxLines Property",
                    new VStack(
                        CreateDemoRow("MaxLines = 1",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextWrapping(TextWrapping.WordWrap)
                                .SetMaxLines(1)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(40, 60, 40)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("MaxLines = 2",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextWrapping(TextWrapping.WordWrap)
                                .SetMaxLines(2)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(40, 60, 40)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("MaxLines = 3",
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextWrapping(TextWrapping.WordWrap)
                                .SetMaxLines(3)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(40, 60, 40)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("Short Text + MaxLines = 5",
                            new Label()
                                .SetText(vm.ShortText)
                                .SetTextWrapping(TextWrapping.WordWrap)
                                .SetMaxLines(5)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(40, 60, 40)))
                                .SetTextColor(Colors.White))
                    )
                ),

                // Section: Combined Features
                CreateSection("Combined: Wrapping + MaxLines + Truncation",
                    new VStack(
                        CreateDemoRow("WordWrap + MaxLines(2) + End", 
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextWrapping(TextWrapping.WordWrap)
                                .SetMaxLines(2)
                                .SetTextTruncation(TextTruncation.End)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 60)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("WordWrap + MaxLines(3) + Middle", 
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextWrapping(TextWrapping.WordWrap)
                                .SetMaxLines(3)
                                .SetTextTruncation(TextTruncation.Middle)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 60)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("Wrap + MaxLines(1) + Start", 
                            new Label()
                                .SetText(vm.LongText)
                                .SetTextWrapping(TextWrapping.Wrap)
                                .SetMaxLines(1)
                                .SetTextTruncation(TextTruncation.Start)
                                .SetDesiredWidth(300)
                                .SetBackground(new SolidColorBackground(new Color(60, 40, 60)))
                                .SetTextColor(Colors.White))
                    )
                ),

                // Section: Different Text Alignments with Truncation
                CreateSection("Text Alignment + Truncation",
                    new VStack(
                        CreateDemoRow("Left + End Truncation",
                            new Label()
                                .SetText(vm.SampleText)
                                .SetHorizontalTextAlignment(HorizontalTextAlignment.Left)
                                .SetTextTruncation(TextTruncation.End)
                                .SetDesiredWidth(250)
                                .SetBackground(new SolidColorBackground(new Color(50, 50, 70)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("Center + Middle Truncation",
                            new Label()
                                .SetText(vm.SampleText)
                                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                                .SetTextTruncation(TextTruncation.Middle)
                                .SetDesiredWidth(250)
                                .SetBackground(new SolidColorBackground(new Color(50, 50, 70)))
                                .SetTextColor(Colors.White)),

                        CreateDemoRow("Right + Start Truncation",
                            new Label()
                                .SetText(vm.SampleText)
                                .SetHorizontalTextAlignment(HorizontalTextAlignment.Right)
                                .SetTextTruncation(TextTruncation.Start)
                                .SetDesiredWidth(250)
                                .SetBackground(new SolidColorBackground(new Color(50, 50, 70)))
                                .SetTextColor(Colors.White))
                    )
                ),

                // Back Button
                new Solid().SetDesiredHeight(20).IgnoreStyling(),
                new Button()
                    .SetText("Back to Main Page")
                    .SetPadding(new Margin(20, 10))
                    .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
                    .SetCornerRadius(8)
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                    .SetCommand(vm.NavCommand),
                new Solid().SetDesiredHeight(20).IgnoreStyling()
            )
        );
    }

    private UiElement CreateSection(string title, UiElement content)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(22)
                .SetTextColor(Colors.LightBlue)
                .SetMargin(new Margin(0, 20, 0, 10)),
            new Border()
                .AddChild(content)
                .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }

    private UiElement CreateDemoRow(string label, UiElement demo)
    {
        return new VStack(
            new Label()
                .SetText(label)
                .SetTextSize(14)
                .SetTextColor(Colors.LightGray)
                .SetMargin(new Margin(10, 10, 10, 5)),
            new HStack(
                demo.SetMargin(new Margin(10, 5, 10, 10))
            )
        );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element 
            => element.SetBackground(new SolidColorBackground(new Color(20, 20, 20))));
    }
}
