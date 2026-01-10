using PlusUi.core;
using SkiaSharp;
using System.Linq.Expressions;

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
                    .SetTextColor(Colors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                // Separator Section
                CreateSection("Separator - Visual Dividers",
                    new VStack(
                        new Label()
                            .SetText("Horizontal Separator (default)")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 5)),
                        new Separator()
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Color & Thickness")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new Separator()
                            .SetColor(new Color(0, 122, 255))
                            .SetThickness(3)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Vertical Separator in HStack")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new HStack(
                            new Label()
                                .SetText("Left")
                                .SetTextColor(Colors.White)
                                .SetMargin(new Margin(10, 0)),
                            new Separator()
                                .SetOrientation(Orientation.Vertical)
                                .SetColor(Colors.Gray)
                                .SetThickness(2)
                                .SetDesiredHeight(30),
                            new Label()
                                .SetText("Middle")
                                .SetTextColor(Colors.White)
                                .SetMargin(new Margin(10, 0)),
                            new Separator()
                                .SetOrientation(Orientation.Vertical)
                                .SetColor(Colors.Gray)
                                .SetThickness(2)
                                .SetDesiredHeight(30),
                            new Label()
                                .SetText("Right")
                                .SetTextColor(Colors.White)
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
                                .SetTextColor(Colors.LightGray)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Toggle()
                                .BindIsOn(() => vm.IsToggled, value => vm.IsToggled = value)
                                .SetMargin(new Margin(15, 0, 0, 0)),
                            new Label()
                                .BindText(() => vm.IsToggled ? "ON" : "OFF")
                                .SetTextColor(Colors.White)
                                .SetMargin(new Margin(15, 0, 0, 0))
                        ).SetMargin(new Margin(0, 10)),

                        new HStack(
                            new Label()
                                .SetText("Custom Colors:")
                                .SetTextColor(Colors.LightGray)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Toggle()
                                .SetIsOn(true)
                                .SetOnColor(Colors.Purple)
                                .SetOffColor(new Color(60, 60, 60))
                                .SetThumbColor(Colors.Gold)
                                .SetMargin(new Margin(15, 0, 0, 0))
                        ).SetMargin(new Margin(0, 10)),

                        new HStack(
                            new Label()
                                .SetText("Multiple Toggles:")
                                .SetTextColor(Colors.LightGray)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Toggle()
                                .SetIsOn(true)
                                .SetOnColor(new Color(255, 59, 48))
                                .SetMargin(new Margin(10, 0, 0, 0)),
                            new Toggle()
                                .SetIsOn(false)
                                .SetOnColor(new Color(255, 149, 0))
                                .SetMargin(new Margin(10, 0, 0, 0)),
                            new Toggle()
                                .SetIsOn(true)
                                .SetOnColor(new Color(52, 199, 89))
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ).SetMargin(new Margin(0, 10))
                    )
                ),

                // ProgressBar Section
                CreateSection("ProgressBar - Determinate Progress",
                    new VStack(
                        new Label()
                            .SetText("Default Progress (65%)")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .BindProgress(() => vm.Progress)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Colors")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new ProgressBar()
                            .SetProgress(0.35f)
                            .SetProgressColor(new Color(255, 59, 48))
                            .SetTrackColor(new Color(50, 50, 50))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Different Progress Values")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 10, 0, 5)),
                        new ProgressBar()
                            .SetProgress(0.2f)
                            .SetProgressColor(new Color(255, 149, 0))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .SetProgress(0.5f)
                            .SetProgressColor(new Color(255, 204, 0))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .SetProgress(0.8f)
                            .SetProgressColor(new Color(52, 199, 89))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 5)),
                        new ProgressBar()
                            .SetProgress(1.0f)
                            .SetProgressColor(new Color(48, 209, 88))
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
                                    .SetTextColor(Colors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .BindIsRunning(() => vm.IsLoading)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Custom Color")
                                    .SetTextColor(Colors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new Color(255, 59, 48))
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Fast Speed")
                                    .SetTextColor(Colors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new Color(52, 199, 89))
                                    .SetSpeed(2.0f)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Slow Speed")
                                    .SetTextColor(Colors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new Color(255, 149, 0))
                                    .SetSpeed(0.5f)
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Large Size")
                                    .SetTextColor(Colors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new Color(175, 82, 222))
                                    .SetDesiredSize(new(60, 60))
                                    .SetMargin(new Margin(0, 10))
                            ).SetMargin(new Margin(10)),

                            new VStack(
                                new Label()
                                    .SetText("Thick Stroke")
                                    .SetTextColor(Colors.LightGray)
                                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                                new ActivityIndicator()
                                    .SetColor(new Color(175, 82, 222))
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
                                .SetTextColor(Colors.LightGray),
                            new Label()
                                .BindText(() => $"{vm.SliderValue:F1}")
                                .SetTextColor(Colors.White)
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ),
                        new Slider()
                            .BindValue(() => vm.SliderValue, value => vm.SliderValue = value)
                            .SetMinimum(0)
                            .SetMaximum(100)
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Range (0-10)")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Slider()
                            .SetValue(5)
                            .SetMinimum(0)
                            .SetMaximum(10)
                            .SetMinimumTrackColor(new Color(255, 59, 48))
                            .SetDesiredWidth(400)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("Custom Colors")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Slider()
                            .SetValue(75)
                            .SetMinimumTrackColor(new Color(52, 199, 89))
                            .SetMaximumTrackColor(new Color(60, 60, 60))
                            .SetThumbColor(new Color(255, 204, 0))
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
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 5)),
                        new Entry()
                            .BindText(() => vm.EntryText, text => vm.EntryText = text)
                            .SetPlaceholder("Enter your name...")
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10)),

                        new Entry()
                            .SetText(string.Empty)
                            .SetPlaceholder("Email address")
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10)),

                        new Label()
                            .SetText("MaxLength Property (10 chars max)")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Entry()
                            .BindText(() => vm.MaxLengthText, text => vm.MaxLengthText = text)
                            .SetMaxLength(10)
                            .SetPlaceholder("Max 10 characters")
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                            .SetCornerRadius(8)
                            .SetMargin(new Margin(0, 10)),
                        new Label()
                            .BindText(() => $"Characters: {(vm.MaxLengthText != null ? vm.MaxLengthText.Length : 0)}/10")
                            .SetTextColor(Colors.Gray)
                            .SetTextSize(12)
                            .SetMargin(new Margin(0, 5)),

                        new Label()
                            .SetText("Combined: Placeholder + MaxLength + Password")
                            .SetTextColor(Colors.LightGray)
                            .SetMargin(new Margin(0, 20, 0, 5)),
                        new Entry()
                            .SetText(string.Empty)
                            .SetPlaceholder("Password (max 20 chars)")
                            .SetIsPassword(true)
                            .SetMaxLength(20)
                            .SetPadding(new Margin(10, 8))
                            .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
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
