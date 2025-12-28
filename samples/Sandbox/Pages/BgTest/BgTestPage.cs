using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.BgTest;
public class BgTestPage(BgTestPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        // Use new gradient background system instead of solid color
        this.SetBackground(new LinearGradient(Colors.LightBlue, Colors.White, 180));

        return new VStack(
            // Gradient background demo boxes
            new Border()
                .SetBackground(new LinearGradient(Colors.Blue, Colors.Purple, 45))
                .SetCornerRadius(10)
                .SetDesiredSize(new Size(200, 100))
                .SetMargin(new(10)),
            
            new Border()
                .SetBackground(new RadialGradient(Colors.White, Colors.Gray))
                .SetCornerRadius(10)
                .SetDesiredSize(new Size(200, 100))
                .SetMargin(new(10)),
            
            new Border()
                .SetBackground(new MultiStopGradient(
                    90,
                    new GradientStop(Colors.Red, 0),
                    new GradientStop(Colors.Yellow, 0.5f),
                    new GradientStop(Colors.Green, 1)))
                .SetCornerRadius(10)
                .SetDesiredSize(new Size(200, 100))
                .SetMargin(new(10)),
            
            new Label()
                .SetText("https://timelane.cloud")
                .SetTextSize(100)
                .SetTextColor(Colors.Black),
            new Button()
                .SetText("Click Me")
                .SetPadding(new(10, 5))
                .SetCommand(vm.NavCommand)
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
