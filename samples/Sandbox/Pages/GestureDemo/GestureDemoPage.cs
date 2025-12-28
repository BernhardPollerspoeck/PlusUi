using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.GestureDemo;

public class GestureDemoPage(GestureDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                new HStack(
                    new Button()
                        .SetText("<- Back")
                        .SetTextSize(16)
                        .SetCommand(vm.GoBackCommand)
                        .SetTextColor(Colors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("Gesture & Haptic Demo")
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 10)),

                CreateSection("Last Gesture",
                    new VStack(
                        new Label()
                            .BindText(nameof(vm.LastGesture), () => vm.LastGesture)
                            .SetTextSize(24)
                            .SetTextColor(Colors.LimeGreen),
                        new Button()
                            .SetText("Reset Counters")
                            .SetCommand(vm.ResetCountersCommand)
                            .SetTextColor(Colors.White)
                            .SetPadding(new Margin(12, 8))
                            .SetBackground(new SolidColorBackground(new Color(80, 80, 80)))
                            .SetCornerRadius(4)
                            .SetMargin(new Margin(0, 10, 0, 0))
                    )
                ),

                CreateSection("LongPress Gesture",
                    new VStack(
                        new Label()
                            .SetText("Long press the box below (or right-click on desktop)")
                            .SetTextSize(14)
                            .SetTextColor(Colors.Gray)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new LongPressGestureDetector(
                            new Border()
                                .AddChild(
                                    new VStack(
                                        new Label()
                                            .SetText("Long Press Me!")
                                            .SetTextSize(18)
                                            .SetTextColor(Colors.White),
                                        new Label()
                                            .BindText(nameof(vm.LongPressCount), () => $"Count: {vm.LongPressCount}")
                                            .SetTextSize(14)
                                            .SetTextColor(Colors.LightGray)
                                    ).SetMargin(new Margin(20))
                                )
                                .SetBackground(new SolidColorBackground(new Color(180, 50, 50)))
                                .SetCornerRadius(8)
                        )
                        .BindCommand(nameof(vm.LongPressCommand), () => vm.LongPressCommand)
                    )
                ),

                CreateSection("DoubleTap Gesture",
                    new VStack(
                        new Label()
                            .SetText("Double-tap the box below quickly")
                            .SetTextSize(14)
                            .SetTextColor(Colors.Gray)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new DoubleTapGestureDetector(
                            new Border()
                                .AddChild(
                                    new VStack(
                                        new Label()
                                            .SetText("Double Tap Me!")
                                            .SetTextSize(18)
                                            .SetTextColor(Colors.White),
                                        new Label()
                                            .BindText(nameof(vm.DoubleTapCount), () => $"Count: {vm.DoubleTapCount}")
                                            .SetTextSize(14)
                                            .SetTextColor(Colors.LightGray)
                                    ).SetMargin(new Margin(20))
                                )
                                .SetBackground(new SolidColorBackground(new Color(50, 100, 180)))
                                .SetCornerRadius(8)
                        )
                        .BindCommand(nameof(vm.DoubleTapCommand), () => vm.DoubleTapCommand)
                    )
                ),

                CreateSection("Swipe Gesture",
                    new VStack(
                        new Label()
                            .SetText("Swipe in any direction on the box below")
                            .SetTextSize(14)
                            .SetTextColor(Colors.Gray)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new SwipeGestureDetector(
                            new Border()
                                .AddChild(
                                    new VStack(
                                        new Label()
                                            .SetText("Swipe Me!")
                                            .SetTextSize(18)
                                            .SetTextColor(Colors.White),
                                        new Label()
                                            .BindText(nameof(vm.SwipeDirection), () => $"Direction: {vm.SwipeDirection}")
                                            .SetTextSize(14)
                                            .SetTextColor(Colors.LightGray)
                                    ).SetMargin(new Margin(20))
                                )
                                .SetBackground(new SolidColorBackground(new Color(50, 150, 80)))
                                .SetCornerRadius(8)
                                .SetDesiredSize(new Size(200, 100))
                        )
                        .SetAllowedDirections(SwipeDirection.All)
                        .BindCommand(nameof(vm.SwipeCommand), () => vm.SwipeCommand)
                    )
                ),

                CreateSection("Pinch Gesture",
                    new VStack(
                        new Label()
                            .SetText("Pinch on the box below (Ctrl+scroll on desktop)")
                            .SetTextSize(14)
                            .SetTextColor(Colors.Gray)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new PinchGestureDetector(
                            new Border()
                                .AddChild(
                                    new VStack(
                                        new Label()
                                            .SetText("Pinch Me!")
                                            .SetTextSize(18)
                                            .SetTextColor(Colors.White),
                                        new Label()
                                            .BindText(nameof(vm.PinchScale), () => $"Scale: {vm.PinchScale:F2}")
                                            .SetTextSize(14)
                                            .SetTextColor(Colors.LightGray)
                                    ).SetMargin(new Margin(20))
                                )
                                .SetBackground(new SolidColorBackground(new Color(150, 80, 180)))
                                .SetCornerRadius(8)
                                .SetDesiredSize(new Size(200, 100))
                        )
                        .BindCommand(nameof(vm.PinchCommand), () => vm.PinchCommand)
                    )
                ),

                CreateSection("Haptic Feedback",
                    new VStack(
                        new Label()
                            .SetText(vm.HapticsSupported ? "Haptics supported on this device" : "Haptics not supported on desktop")
                            .SetTextSize(14)
                            .SetTextColor(vm.HapticsSupported ? Colors.LimeGreen : Colors.Orange)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new HStack(
                            new Button()
                                .SetText("Light")
                                .SetCommand(vm.TestHapticLightCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(new Color(60, 60, 60)))
                                .SetCornerRadius(4)
                                .SetMargin(new Margin(0, 0, 5, 0)),
                            new Button()
                                .SetText("Medium")
                                .SetCommand(vm.TestHapticMediumCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(new Color(80, 80, 80)))
                                .SetCornerRadius(4)
                                .SetMargin(new Margin(0, 0, 5, 0)),
                            new Button()
                                .SetText("Heavy")
                                .SetCommand(vm.TestHapticHeavyCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(new Color(100, 100, 100)))
                                .SetCornerRadius(4)
                        ),
                        new HStack(
                            new Button()
                                .SetText("Success")
                                .SetCommand(vm.TestHapticSuccessCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(new Color(50, 150, 50)))
                                .SetCornerRadius(4)
                                .SetMargin(new Margin(0, 0, 5, 0)),
                            new Button()
                                .SetText("Warning")
                                .SetCommand(vm.TestHapticWarningCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(new Color(200, 150, 50)))
                                .SetCornerRadius(4)
                                .SetMargin(new Margin(0, 0, 5, 0)),
                            new Button()
                                .SetText("Error")
                                .SetCommand(vm.TestHapticErrorCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(10, 6))
                                .SetBackground(new SolidColorBackground(new Color(180, 50, 50)))
                                .SetCornerRadius(4)
                        ).SetMargin(new Margin(0, 10, 0, 0))
                    )
                ),

                new Solid().SetDesiredHeight(50).IgnoreStyling()
            )
        )
        .SetCanScrollHorizontally(false);
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
                .AddChild(
                    new VStack(content)
                        .SetMargin(new Margin(16))
                )
                .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
