using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.NewControlsDemo;

public class NewControlsDemoPage(NewControlsDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Page Title
                new Label()
                    .SetText("New Controls Demo")
                    .SetTextSize(32)
                    .SetTextColor(SKColors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                // Separator Section
                CreateSection("Separator - Visual Dividers",
                    new VStack(
                        new Label()
                            .SetText("Horizontal Separator (default)")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 5)),
                        new Separator()
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Color & Thickness")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new Separator()
                            .SetColor(new SKColor(0, 122, 255))
                            .SetThickness(3)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Vertical Separator in HStack")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new HStack(
                            new Label()
                                .SetText("Left")
                                .SetTextColor(SKColors.White)
                                .SetMargin(new Margin(10, 0)),
                            new Separator()
                                .SetOrientation(Orientation.Vertical)
                                .SetColor(SKColors.Gray)
                                .SetThickness(2)
                                .SetDesiredHeight(30),
                            new Label()
                                .SetText("Middle")
                                .SetTextColor(SKColors.White)
                                .SetMargin(new Margin(10, 0)),
                            new Separator()
                                .SetOrientation(Orientation.Vertical)
                                .SetColor(SKColors.Gray)
                                .SetThickness(2)
                                .SetDesiredHeight(30),
                            new Label()
                                .SetText("Right")
                                .SetTextColor(SKColors.White)
                                .SetMargin(new Margin(10, 0))
                        ).SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
                ),

                // Toggle Section
                CreateSection("Toggle/Switch - On/Off Control",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Basic Toggle:")
                                .SetTextColor(SKColors.LightGray)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Toggle()
                                .BindIsOn(nameof(vm.IsToggled), () => vm.IsToggled, value => vm.IsToggled = value)
                                .SetMargin(new Margin(15, 0, 0, 0)),
                            new Label()
                                .BindText(nameof(vm.IsToggled), () => vm.IsToggled ? "ON" : "OFF")
                                .SetTextColor(SKColors.White)
                                .SetMargin(new Margin(15, 0, 0, 0))
                        ).SetMargin(new Margin(0, 10)),

                        new HStack(
                            new Label()
                                .SetText("Custom Colors:")
                                .SetTextColor(SKColors.LightGray)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Toggle()
                                .SetIsOn(true)
                                .SetOnColor(SKColors.Purple)
                                .SetOffColor(new SKColor(60, 60, 60))
                                .SetThumbColor(SKColors.Gold)
                                .SetMargin(new Margin(15, 0, 0, 0))
                        ).SetMargin(new Margin(0, 10)),

                        new HStack(
                            new Label()
                                .SetText("Multiple Toggles:")
                                .SetTextColor(SKColors.LightGray)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Toggle()
                                .SetIsOn(true)
                                .SetOnColor(new SKColor(255, 59, 48))
                                .SetMargin(new Margin(10, 0, 0, 0)),
                            new Toggle()
                                .SetIsOn(false)
                                .SetOnColor(new SKColor(255, 149, 0))
                                .SetMargin(new Margin(10, 0, 0, 0)),
                            new Toggle()
                                .SetIsOn(true)
                                .SetOnColor(new SKColor(52, 199, 89))
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ).SetMargin(new Margin(0, 10))
                    )
                ),

                // ProgressBar Section
                CreateSection("ProgressBar - Determinate Progress",
                    new VStack(
                        new Label()
                            .SetText("Default Progress (65%)")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .BindProgress(nameof(vm.Progress), () => vm.Progress)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Colors")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new ProgressBar()
                            .SetProgress(0.35f)
                            .SetProgressColor(new SKColor(255, 59, 48))
                            .SetTrackColor(new SKColor(50, 50, 50))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Different Progress Values")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new ProgressBar()
                            .SetProgress(0.2f)
                            .SetProgressColor(new SKColor(255, 149, 0))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .SetProgress(0.5f)
                            .SetProgressColor(new SKColor(255, 204, 0))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .SetProgress(0.8f)
                            .SetProgressColor(new SKColor(52, 199, 89))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .SetProgress(1.0f)
                            .SetProgressColor(new SKColor(48, 209, 88))
                            .SetDesiredHeight(12)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5))
                    )
                ),

                // ActivityIndicator Section
                CreateSection("ActivityIndicator - Loading Spinner",
                    new VStack(
                        new HStack(
                            new VStack(
                                new Label()
                                    .SetText("Default (Running)")
                                    .SetTextColor(SKColors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .BindIsRunning(nameof(vm.IsLoading), () => vm.IsLoading)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Custom Color")
                                    .SetTextColor(SKColors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new SKColor(255, 59, 48))
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Fast Speed")
                                    .SetTextColor(SKColors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new SKColor(52, 199, 89))
                                    .SetSpeed(2.0f)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Slow Speed")
                                    .SetTextColor(SKColors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new SKColor(255, 149, 0))
                                    .SetSpeed(0.5f)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Large Size")
                                    .SetTextColor(SKColors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new SKColor(175, 82, 222))
                                    .SetDesiredSize(new(60, 60))
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Thick Stroke")
                                    .SetTextColor(SKColors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new SKColor(175, 82, 222))
                                    .SetStrokeThickness(8)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10))
                        ).SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
                ),

                // Slider Section
                CreateSection("Slider - Value Selection",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Value:")
                                .SetTextColor(SKColors.LightGray),
                            new Label()
                                .BindText(nameof(vm.SliderValue), () => $"{vm.SliderValue:F1}")
                                .SetTextColor(SKColors.White)
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ),
                        new Slider()
                            .BindValue(nameof(vm.SliderValue), () => vm.SliderValue, value => vm.SliderValue = value)
                            .SetMinimum(0)
                            .SetMaximum(100)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Range (0-10)")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Slider()
                            .SetValue(5)
                            .SetMinimum(0)
                            .SetMaximum(10)
                            .SetMinimumTrackColor(new SKColor(255, 59, 48))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Colors")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Slider()
                            .SetValue(75)
                            .SetMinimumTrackColor(new SKColor(52, 199, 89))
                            .SetMaximumTrackColor(new SKColor(60, 60, 60))
                            .SetThumbColor(new SKColor(255, 204, 0))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10))
                    )
                ),

                // Entry Improvements Section
                CreateSection("Entry Improvements",
                    new VStack(
                        new Label()
                            .SetText("Placeholder Text")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 5)),
                        new Entry()
                            .BindText(nameof(vm.EntryText), () => vm.EntryText, text => vm.EntryText = text)
                            .SetPlaceholder("Enter your name...")
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10)),

                        new Entry()
                            .SetText(string.Empty)
                            .SetPlaceholder("Email address")
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("MaxLength Property (10 chars max)")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Entry()
                            .BindText(nameof(vm.MaxLengthText), () => vm.MaxLengthText, text => vm.MaxLengthText = text)
                            .SetMaxLength(10)
                            .SetPlaceholder("Max 10 characters")
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10)),
                        new Label()
                            .BindText(nameof(vm.MaxLengthText), () => $"Characters: {vm.MaxLengthText?.Length ?? 0}/10")
                            .SetTextColor(SKColors.Gray)
                            .SetTextSize(12)
                            .SetMargin(new Margin(0, 5)),

                        new Label()
                            .SetText("Combined: Placeholder + MaxLength + Password")
                            .SetTextColor(SKColors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Entry()
                            .SetText(string.Empty)
                            .SetPlaceholder("Password (max 20 chars)")
                            .SetIsPassword(true)
                            .SetMaxLength(20)
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10))
                    )
                ),

                // Spacer at bottom
                new Solid().SetDesiredHeight(50)
            )
        ).SetCanScrollHorizontally(false);
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
