using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ShadowDemo;

public class ShadowDemoPage(ShadowDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            // Header with back button
            new HStack(
                new Button()
                    .SetText("← Back")
                    .SetTextSize(18)
                    .SetCommand(vm.NavCommand)
                    .SetTextColor(Colors.White)
                    .SetPadding(new Margin(10, 5)),
                new Label()
                    .SetText("Shadow Demo")
                    .SetTextSize(24)
                    .SetTextColor(Colors.White)
                    .SetMargin(new Margin(20, 0, 0, 0))
            ).SetMargin(new Margin(0, 0, 0, 20)),

            new ScrollView(
                new VStack(
                    // Interactive Shadow Controls Section
                    CreateControlsSection(),

                    new Solid().SetDesiredHeight(30).IgnoreStyling(),

                    // Examples Section
                    CreateExamplesSection(),

                    new Solid().SetDesiredHeight(30).IgnoreStyling(),

                    // Material Design Elevation Section
                    CreateMaterialSection(),

                    new Solid().SetDesiredHeight(30).IgnoreStyling(),

                    // Dynamic Hover Shadow Section
                    CreateDynamicSection(),

                    new Solid().SetDesiredHeight(50).IgnoreStyling()
                ).SetMargin(new Margin(20))
            )
        );
    }

    private UiElement CreateControlsSection()
    {
        return new VStack(
            new Label()
                .SetText("Interactive Shadow Controls")
                .SetTextSize(20)
                .SetTextColor(Colors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            // Shadow Blur Control
            new HStack(
                new Label()
                    .SetText("Blur:")
                    .SetTextSize(16)
                    .SetTextColor(Colors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowBlur), () => $"{vm.ShadowBlur:F1}")
                    .SetTextSize(16)
                    .SetTextColor(Colors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Shadow Offset Y Control
            new HStack(
                new Label()
                    .SetText("Offset Y:")
                    .SetTextSize(16)
                    .SetTextColor(Colors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowOffsetY), () => $"{vm.ShadowOffsetY:F1}")
                    .SetTextSize(16)
                    .SetTextColor(Colors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Shadow Alpha Control
            new HStack(
                new Label()
                    .SetText("Alpha:")
                    .SetTextSize(16)
                    .SetTextColor(Colors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowAlpha), () => $"{vm.ShadowAlpha}")
                    .SetTextSize(16)
                    .SetTextColor(Colors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Shadow Spread Control
            new HStack(
                new Label()
                    .SetText("Spread:")
                    .SetTextSize(16)
                    .SetTextColor(Colors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowSpread), () => $"{vm.ShadowSpread:F1}")
                    .SetTextSize(16)
                    .SetTextColor(Colors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Corner Radius Control
            new HStack(
                new Label()
                    .SetText("Corner Radius:")
                    .SetTextSize(16)
                    .SetTextColor(Colors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.CornerRadius), () => $"{vm.CornerRadius:F1}")
                    .SetTextSize(16)
                    .SetTextColor(Colors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 15)),

            // Live Preview with current settings - wrapped in container for spacing
            new VStack(
                new Border()
                    .SetBackground(new SolidColorBackground(Colors.White))
                    .BindCornerRadius(nameof(vm.CornerRadius), () => vm.CornerRadius)
                    .BindShadowColor(nameof(vm.ShadowAlpha), () => Colors.Black.WithAlpha(vm.ShadowAlpha))
                    .BindShadowOffset(nameof(vm.ShadowOffsetY), () => new Point(vm.ShadowOffsetX, vm.ShadowOffsetY))
                    .BindShadowBlur(nameof(vm.ShadowBlur), () => vm.ShadowBlur)
                    .BindShadowSpread(nameof(vm.ShadowSpread), () => vm.ShadowSpread)
                    .AddChild(
                        new Label()
                            .SetText("Live Preview")
                            .SetTextSize(18)
                            .SetTextColor(Colors.Black)
                            .SetMargin(new Margin(20))
                            .SetHorizontalAlignment(HorizontalAlignment.Center)
                    )
            ).SetMargin(new Margin(30))
        );
    }

    private UiElement CreateExamplesSection()
    {
        return new VStack(
            new Label()
                .SetText("Shadow Examples")
                .SetTextSize(20)
                .SetTextColor(Colors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            new HStack(
                // Card with subtle shadow - wrapped for spacing
                new VStack(
                    new Border()
                        .SetBackground(new SolidColorBackground(Colors.White))
                        .SetCornerRadius(8)
                        .SetShadowColor(Colors.Black.WithAlpha(80))
                        .SetShadowOffset(new Point(0, 2))
                        .SetShadowBlur(8)
                        .SetShadowSpread(0)
                        .AddChild(
                            new VStack(
                                new Label()
                                    .SetText("Card Shadow")
                                    .SetTextSize(16)
                                    .SetTextColor(Colors.Black),
                                new Label()
                                    .SetText("Subtle elevation")
                                    .SetTextSize(12)
                                    .SetTextColor(Colors.Gray)
                            ).SetMargin(new Margin(15))
                        )
                ).SetMargin(new Margin(15)),

                // Button with stronger shadow - wrapped for spacing
                new VStack(
                    new Border()
                        .SetBackground(new SolidColorBackground(new Color(66, 133, 244)))
                        .SetCornerRadius(4)
                        .SetShadowColor(Colors.Black.WithAlpha(100))
                        .SetShadowOffset(new Point(0, 4))
                        .SetShadowBlur(12)
                        .SetShadowSpread(0)
                        .AddChild(
                            new Label()
                                .SetText("Button")
                                .SetTextSize(16)
                                .SetTextColor(Colors.White)
                                .SetMargin(new Margin(20, 10))
                        )
                ).SetMargin(new Margin(15))
            ).SetMargin(new Margin(0, 0, 0, 10)),

            new HStack(
                // Rounded card - wrapped for spacing
                new VStack(
                    new Border()
                        .SetBackground(new SolidColorBackground(Colors.White))
                        .SetCornerRadius(16)
                        .SetShadowColor(Colors.Black.WithAlpha(70))
                        .SetShadowOffset(new Point(0, 3))
                        .SetShadowBlur(10)
                        .AddChild(
                            new Label()
                                .SetText("Rounded")
                                .SetTextSize(14)
                                .SetTextColor(Colors.Black)
                                .SetMargin(new Margin(15))
                        )
                ).SetMargin(new Margin(15)),

                // No corner radius - wrapped for spacing
                new VStack(
                    new Border()
                        .SetBackground(new SolidColorBackground(Colors.White))
                        .SetCornerRadius(0)
                        .SetShadowColor(Colors.Black.WithAlpha(90))
                        .SetShadowOffset(new Point(0, 2))
                        .SetShadowBlur(6)
                        .AddChild(
                            new Label()
                                .SetText("Sharp")
                                .SetTextSize(14)
                                .SetTextColor(Colors.Black)
                                .SetMargin(new Margin(15))
                        )
                ).SetMargin(new Margin(15))
            )
        );
    }

    private UiElement CreateMaterialSection()
    {
        return new VStack(
            new Label()
                .SetText("Material Design Elevation")
                .SetTextSize(20)
                .SetTextColor(Colors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            new HStack(
                new Button()
                    .SetText("−")
                    .SetTextSize(20)
                    .SetCommand(vm.DecreaseElevationCommand)
                    .SetPadding(new Margin(15, 5))
                    .SetTextColor(Colors.White)
                    .SetBackground(new SolidColorBackground(new Color(244, 67, 54))),
                new Label()
                    .BindText(nameof(vm.Elevation), () => $"Elevation: {vm.Elevation}")
                    .SetTextSize(16)
                    .SetTextColor(Colors.White)
                    .SetMargin(new Margin(10, 0)),
                new Button()
                    .SetText("+")
                    .SetTextSize(20)
                    .SetCommand(vm.IncreaseElevationCommand)
                    .SetPadding(new Margin(15, 5))
                    .SetTextColor(Colors.White)
                    .SetBackground(new SolidColorBackground(new Color(76, 175, 80)))
            ).SetMargin(new Margin(0, 0, 0, 15)),

            // Material elevation card with binding - wrapped for spacing
            new VStack(
                new Border()
                    .SetBackground(new SolidColorBackground(Colors.White))
                    .SetCornerRadius(4)
                    .BindShadowColor(nameof(vm.Elevation), () =>
                        vm.Elevation > 0 ? Colors.Black.WithAlpha((byte)(vm.Elevation * 25)) : Colors.Transparent)
                    .BindShadowOffset(nameof(vm.Elevation), () => new Point(0, vm.Elevation * 2f))
                    .BindShadowBlur(nameof(vm.Elevation), () => vm.Elevation * 3f)
                    .BindShadowSpread(nameof(vm.Elevation), () => vm.Elevation * 0.5f)
                    .AddChild(
                        new VStack(
                            new Label()
                                .SetText("Material Card")
                                .SetTextSize(18)
                                .SetTextColor(Colors.Black),
                            new Label()
                                .SetText("Shadow changes with elevation level")
                                .SetTextSize(12)
                                .SetTextColor(Colors.Gray)
                                .SetMargin(new Margin(0, 5, 0, 0))
                        ).SetMargin(new Margin(20))
                    )
            ).SetMargin(new Margin(30))
        );
    }

    private UiElement CreateDynamicSection()
    {
        return new VStack(
            new Label()
                .SetText("Dynamic Shadow on Hover")
                .SetTextSize(20)
                .SetTextColor(Colors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            new Button()
                .SetText("Toggle Hover State")
                .SetTextSize(16)
                .SetCommand(vm.ToggleHoverCommand)
                .SetPadding(new Margin(15, 8))
                .SetTextColor(Colors.White)
                .SetBackground(new SolidColorBackground(new Color(156, 39, 176)))
                .SetMargin(new Margin(0, 0, 0, 15)),

            // Card with dynamic shadow - wrapped for spacing
            new VStack(
                new Border()
                    .SetBackground(new SolidColorBackground(Colors.White))
                    .SetCornerRadius(8)
                    .BindShadowColor(nameof(vm.IsHovered), () =>
                        vm.IsHovered ? Colors.Black.WithAlpha(120) : Colors.Black.WithAlpha(70))
                    .BindShadowBlur(nameof(vm.IsHovered), () => vm.IsHovered ? 20f : 8f)
                    .BindShadowOffset(nameof(vm.IsHovered), () =>
                        vm.IsHovered ? new Point(0, 10) : new Point(0, 4))
                    .AddChild(
                        new VStack(
                            new Label()
                                .BindText(nameof(vm.IsHovered), () =>
                                    vm.IsHovered ? "Hovered! 🎯" : "Not Hovered")
                                .SetTextSize(18)
                                .SetTextColor(Colors.Black)
                                .SetHorizontalAlignment(HorizontalAlignment.Center),
                            new Label()
                                .SetText("Shadow animates on hover")
                                .SetTextSize(12)
                                .SetTextColor(Colors.Gray)
                                .SetMargin(new Margin(0, 5, 0, 0))
                                .SetHorizontalAlignment(HorizontalAlignment.Center)
                        ).SetMargin(new Margin(20))
                    )
            ).SetMargin(new Margin(30))
        );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element
            => element.SetBackground(new SolidColorBackground(Colors.White)));
    }
}
