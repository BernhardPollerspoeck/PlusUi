using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.TextRendering;

public class TextRenderPage(TextRenderPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Grid()
            .AddRow(Row.Auto)
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)

            .AddChild(new Button()
                .SetText("Back")
                .SetTextSize(20)
                .SetCommand(vm.NavCommand)
                .SetTextColor(SKColors.Black))

            .AddChild(row: 1, child: new VStack(
                new Label()
                    .SetText("Hello World")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.Black)
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Label()
                    .SetText("Small Text with Margin")
                    .SetTextSize(12)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(20))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Label()
                    .SetText("Large Bold Text")
                    .SetTextSize(32)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(5, 15))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Label()
                    .SetText("Text with different margins")
                    .SetTextSize(18)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(30, 5, 10, 5))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Label()
                    .SetText("Right-side label")
                    .SetTextSize(24)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(10))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Label()
                    .SetText("Small text with big margin")
                    .SetTextSize(10)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(40))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Label()
                    .SetText("Different horizontal margins")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(8, 25))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling()
                )
            )
            .AddChild(row: 1, column: 1, child: new VStack(
                new Button()
                    .SetText("Hello World")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.Black)
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Small Text with Margin")
                    .SetTextSize(12)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(20))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Large Bold Text")
                    .SetTextSize(32)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(5, 15))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Text with different margins")
                    .SetTextSize(18)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(30, 5, 10, 5))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Right-side button")
                    .SetTextSize(24)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(10))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Small text with big margin")
                    .SetTextSize(10)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(10))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Different horizontal margins")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.Black)
                    .SetMargin(new Margin(8, 25))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Button with Padding 10")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.Black)
                    .SetPadding(new Margin(10))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Button with Padding 20")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.Black)
                    .SetPadding(new Margin(20))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling(),

                new Button()
                    .SetText("Button with Padding 30")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.Black)
                    .SetPadding(new Margin(30))
                    .SetBackgroundColor(SKColors.White)
                    .IgnoreStyling()
                )
            );
    }
}
