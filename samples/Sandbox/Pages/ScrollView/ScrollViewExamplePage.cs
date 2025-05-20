using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ScrollView;

internal class ScrollViewExamplePage(ScrollViewExamplePageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("ScrollView Example")
                .SetTextSize(24)
                .SetMargin(new Margin(10)),
                
            new HStack(
                new Label()
                    .SetText("Horizontal Scrolling:")
                    .SetTextSize(16)
                    .SetMargin(new Margin(10)),
                    
                new Checkbox()
                    .BindIsChecked(nameof(vm.IsHorizontalScrollingEnabled), 
                        () => vm.IsHorizontalScrollingEnabled, 
                        isChecked => vm.IsHorizontalScrollingEnabled = isChecked)
                    .SetMargin(new Margin(10))
            ),
            
            new HStack(
                new Label()
                    .SetText("Vertical Scrolling:")
                    .SetTextSize(16)
                    .SetMargin(new Margin(10)),
                    
                new Checkbox()
                    .BindIsChecked(nameof(vm.IsVerticalScrollingEnabled), 
                        () => vm.IsVerticalScrollingEnabled, 
                        isChecked => vm.IsVerticalScrollingEnabled = isChecked)
                    .SetMargin(new Margin(10))
            ),
            
            // ScrollView with a VStack of content
            new ScrollView(
                new VStack(
                    // A grid to demonstrate horizontal scrolling
                    new Grid()
                        .AddColumn(200)
                        .AddColumn(200)
                        .AddColumn(200)
                        .AddColumn(200)
                        .AddColumn(200)
                        .AddRow(50)
                        .AddChild(
                            new Solid()
                                .SetBackgroundColor(SKColors.Red)
                                .SetMargin(new Margin(5))
                                .IgnoreStyling())
                        .AddChild(column: 1, child:
                            new Solid()
                                .SetBackgroundColor(SKColors.Green)
                                .SetMargin(new Margin(5))
                                .IgnoreStyling())
                        .AddChild(column: 2, child:
                            new Solid()
                                .SetBackgroundColor(SKColors.Blue)
                                .SetMargin(new Margin(5))
                                .IgnoreStyling())
                        .AddChild(column: 3, child:
                            new Solid()
                                .SetBackgroundColor(SKColors.Yellow)
                                .SetMargin(new Margin(5))
                                .IgnoreStyling())
                        .AddChild(column: 4, child:
                            new Solid()
                                .SetBackgroundColor(SKColors.Purple)
                                .SetMargin(new Margin(5))
                                .IgnoreStyling()),
                                
                    // Multiple labels to demonstrate vertical scrolling
                    new Label()
                        .BindText(nameof(vm.LongText), () => vm.LongText)
                        .SetTextSize(18)
                        .SetMargin(new Margin(10)),
                    
                    new Label()
                        .BindText(nameof(vm.LongText), () => vm.LongText)
                        .SetTextSize(18)
                        .SetMargin(new Margin(10)),
                        
                    new Label()
                        .BindText(nameof(vm.LongText), () => vm.LongText)
                        .SetTextSize(18)
                        .SetMargin(new Margin(10))
                )
            )
            .BindCanScrollHorizontally(nameof(vm.IsHorizontalScrollingEnabled), () => vm.IsHorizontalScrollingEnabled)
            .BindCanScrollVertically(nameof(vm.IsVerticalScrollingEnabled), () => vm.IsVerticalScrollingEnabled)
            .SetBackgroundColor(new SKColor(220, 220, 220))
            .SetCornerRadius(10)
            .SetMargin(new Margin(10))
            .SetDesiredHeight(300),
            
            new Button()
                .SetText("Back")
                .SetTextSize(18)
                .SetCommand(vm.NavCommand)
                .SetBackgroundColor(SKColors.White)
                .SetTextColor(SKColors.Black)
                .SetMargin(new Margin(10))
        );
    }
}