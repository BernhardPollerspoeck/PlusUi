using PlusUi.core;
using System.Linq.Expressions;

namespace Sandbox.Pages.UniformGridDemo;

public class UniformGridDemoPage(UniformGridDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            // Header with back button
            new HStack(
                new Button()
                    .SetText("← Back")
                    .SetTextSize(16)
                    .SetCommand(vm.GoBackCommand)
                    .SetTextColor(Colors.White)
                    .SetPadding(new Margin(10, 5)),
                new Label()
                    .SetText("UniformGrid Demo - Tic Tac Toe")
                    .SetTextSize(24)
                    .SetTextColor(Colors.White)
                    .SetMargin(new Margin(20, 0, 0, 0))
            ).SetMargin(new Margin(10, 10, 0, 20)),

            // Current player indicator
            new HStack(
                new Label()
                    .SetText("Current: ")
                    .SetTextSize(18)
                    .SetTextColor(Colors.LightGray),
                new Image()
                    .BindImageSource(() => vm.CurrentPlayerImage)
                    .SetDesiredSize(new Size(32, 32))
                    .SetAspect(Aspect.AspectFit)
            ).SetHorizontalAlignment(HorizontalAlignment.Center)
             .SetMargin(new Margin(0, 0, 0, 10)),

            // Status
            new Label()
                .BindText(() => vm.StatusText)
                .SetTextSize(20)
                .SetTextColor(Colors.Yellow)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetMargin(new Margin(0, 0, 0, 20)),

            // Tic Tac Toe Board using UniformGrid
            new Border()
                .AddChild(
                    new UniformGrid()
                        .SetRows(3)
                        .SetColumns(3)
                        .AddChildren(
                            CreateCell(0, () => vm.Cell0Image),
                            CreateCell(1, () => vm.Cell1Image),
                            CreateCell(2, () => vm.Cell2Image),
                            CreateCell(3, () => vm.Cell3Image),
                            CreateCell(4, () => vm.Cell4Image),
                            CreateCell(5, () => vm.Cell5Image),
                            CreateCell(6, () => vm.Cell6Image),
                            CreateCell(7, () => vm.Cell7Image),
                            CreateCell(8, () => vm.Cell8Image)
                        )
                        .SetDesiredSize(new Size(300, 300))
                        .IgnoreStyling()
                )
                .SetBackground(new SolidColorBackground(new Color(60, 60, 60)))
                .SetCornerRadius(12)
                .SetStrokeThickness(0)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetMargin(new Margin(0, 0, 0, 20))
                .IgnoreStyling(),

            // Reset button
            new Button()
                .SetText("New Game")
                .SetTextSize(18)
                .SetPadding(new Margin(30, 15))
                .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
                .SetCornerRadius(8)
                .SetCommand(vm.ResetGameCommand)
                .SetHorizontalAlignment(HorizontalAlignment.Center),

            // Legend
            new HStack(
                new Image()
                    .SetImageSource(UniformGridDemoPageViewModel.PlusUiLogo)
                    .SetDesiredSize(new Size(24, 24))
                    .SetAspect(Aspect.AspectFit),
                new Label()
                    .SetText(" = PlusUi (PNG)    ")
                    .SetTextSize(14)
                    .SetTextColor(Colors.LightGray),
                new Image()
                    .SetImageSource(UniformGridDemoPageViewModel.CSharpLogo)
                    .SetDesiredSize(new Size(24, 24))
                    .SetAspect(Aspect.AspectFit),
                new Label()
                    .SetText(" = C# (Web)")
                    .SetTextSize(14)
                    .SetTextColor(Colors.LightGray)
            ).SetHorizontalAlignment(HorizontalAlignment.Center)
             .SetMargin(new Margin(0, 20, 0, 0)),

            // Info text
            new VStack(
                new Label()
                    .SetText("UniformGrid creates a grid where all cells have equal size.")
                    .SetTextSize(14)
                    .SetTextColor(Colors.Gray)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                new Label()
                    .SetText("Perfect for game boards, calculators, and icon grids!")
                    .SetTextSize(14)
                    .SetTextColor(Colors.Gray)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
            ).SetMargin(new Margin(20, 20, 20, 0))
             .SetHorizontalAlignment(HorizontalAlignment.Center)

        ).SetHorizontalAlignment(HorizontalAlignment.Stretch);
    }

    private UiElement CreateCell(int index, Expression<Func<string?>> getter)
    {
        return new TapGestureDetector(
            new Border()
                .AddChild(
                    new Image()
                        .BindImageSource(getter)
                        .SetAspect(Aspect.AspectFit)
                        .SetDesiredSize(new Size(60, 60))
                        .SetHorizontalAlignment(HorizontalAlignment.Center)
                        .SetVerticalAlignment(VerticalAlignment.Center)
                        .SetMargin(new Margin(0))
                        .IgnoreStyling()
                )
                .SetBackground(new SolidColorBackground(new Color(50, 50, 55)))
                .SetCornerRadius(8)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .IgnoreStyling()
        )
        .SetCommand(vm.CellClickCommand)
        .SetCommandParameter(index)
        .SetHorizontalAlignment(HorizontalAlignment.Stretch)
        .SetVerticalAlignment(VerticalAlignment.Stretch)
        .SetMargin(new Margin(3))
        .IgnoreStyling();
    }
}
