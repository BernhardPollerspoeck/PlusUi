using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.RenderLoopDemo;

public class RenderLoopDemoPage(RenderLoopDemoViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                new Button()
                    .SetText("← Back")
                    .SetPadding(new Margin(10, 5))
                    .SetMargin(new Margin(0, 0, 0, 20))
                    .SetCommand(vm.BackCommand),

                new Label()
                    .SetText("Render Loop Demo")
                    .SetTextSize(32)
                    .SetFontWeight(FontWeight.Bold)
                    .SetMargin(new Margin(0, 0, 0, 20)),

                new Label()
                    .SetText("This page demonstrates the smart rendering system")
                    .SetTextSize(16)
                    .SetMargin(new Margin(0, 0, 0, 20)),

                // PropertyChanged Section
                new VStack(
                    new Label()
                        .SetText("1. PropertyChanged Bindings")
                        .SetTextSize(24)
                        .SetFontWeight(FontWeight.SemiBold)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new Label()
                        .SetText("Countdown updates via INotifyPropertyChanged:")
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new HStack(
                        new Label()
                            .SetText("Time remaining:"),
                        new Label()
                            .BindText(() => vm.RemainingSeconds.ToString())
                            .SetFontWeight(FontWeight.Bold)
                            .SetMargin(new Margin(5, 0, 5, 0)),
                        new Label()
                            .SetText("seconds")
                    ),

                    new HStack(
                        new Button()
                            .SetText("Start Countdown")
                            .SetCommand(vm.StartCountdownCommand)
                            .SetMargin(new Margin(0, 0, 10, 0)),
                        new Button()
                            .SetText("Stop Countdown")
                            .SetCommand(vm.StopCountdownCommand)
                    ).SetMargin(new Margin(0, 10, 0, 0))
                )
                
                .SetBackground(new SolidColorBackground(new Color(240, 240, 255)))
                .SetCornerRadius(8)
                .SetMargin(new Margin(0, 0, 0, 20)),

                // ActivityIndicator Section
                new VStack(
                    new Label()
                        .SetText("2. ActivityIndicator (IInvalidator)")
                        .SetTextSize(24)
                        .SetFontWeight(FontWeight.SemiBold)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new Label()
                        .SetText("Spinner uses continuous rendering when running:")
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new HStack(
                        new ActivityIndicator()
                            .BindIsRunning(() => vm.IsActivityRunning)
                            .SetDesiredSize(new(40, 40))
                            .SetMargin(new Margin(0, 0, 20, 0)),
                        new VStack(
                            new Label()
                                .BindText(() => vm.ActivityStatusText)
                                .SetFontWeight(FontWeight.SemiBold),
                            new Label()
                                .SetText("(Continuous rendering only when running)")
                                .SetTextSize(12)
                                .SetTextColor(new Color(100, 100, 100))
                        )
                    ),

                    new HStack(
                        new Button()
                            .SetText("Start Spinner")
                            .SetCommand(vm.StartActivityCommand)
                            .SetMargin(new Margin(0, 0, 10, 0)),
                        new Button()
                            .SetText("Stop Spinner")
                            .SetCommand(vm.StopActivityCommand)
                    ).SetMargin(new Margin(0, 10, 0, 0))
                )
                
                .SetBackground(new SolidColorBackground(new Color(255, 240, 240)))
                .SetCornerRadius(8)
                .SetMargin(new Margin(0, 0, 0, 20)),

                // Combined Test
                new VStack(
                    new Label()
                        .SetText("3. Combined Test")
                        .SetTextSize(24)
                        .SetFontWeight(FontWeight.SemiBold)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new Label()
                        .SetText("Run both animations simultaneously:")
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new Button()
                        .SetText("Start Everything")
                        .SetCommand(vm.StartAllCommand)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new Button()
                        .SetText("Stop Everything")
                        .SetCommand(vm.StopAllCommand)
                )
                
                .SetBackground(new SolidColorBackground(new Color(240, 255, 240)))
                .SetCornerRadius(8)
                .SetMargin(new Margin(0, 0, 0, 20)),

                // Info Section
                new VStack(
                    new Label()
                        .SetText("How it works:")
                        .SetTextSize(20)
                        .SetFontWeight(FontWeight.SemiBold)
                        .SetMargin(new Margin(0, 0, 0, 10)),

                    new Label()
                        .SetText("• PropertyChanged: Renders only when ViewModel fires PropertyChanged")
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 0, 0, 5)),

                    new Label()
                        .SetText("• ActivityIndicator: Uses IInvalidator for continuous 60 FPS rendering")
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 0, 0, 5)),

                    new Label()
                        .SetText("• Smart System: Only renders when needed, stops when idle")
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 0, 0, 5)),

                    new Label()
                        .SetText("• Battery Efficient: VSync and early-exit optimize CPU usage")
                        .SetTextSize(14)
                )
                
                .SetBackground(new SolidColorBackground(new Color(250, 250, 250)))
                .SetCornerRadius(8)
            )
        );
    }
}
