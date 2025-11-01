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
                    .SetTextColor(SKColors.White)
                    .SetPadding(new Margin(10, 5)),
                new Label()
                    .SetText("Shadow Demo")
                    .SetTextSize(24)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(20, 0, 0, 0))
            ).SetMargin(new Margin(0, 0, 0, 20)),

            new ScrollView(
                new VStack(
                    // Interactive Shadow Controls Section
                    CreateControlsSection(),

                    new Solid().SetDesiredHeight(20).IgnoreStyling(),

                    // Examples Section
                    CreateExamplesSection(),

                    new Solid().SetDesiredHeight(20).IgnoreStyling(),

                    // Material Design Elevation Section
                    CreateMaterialSection(),

                    new Solid().SetDesiredHeight(20).IgnoreStyling(),

                    // Dynamic Hover Shadow Section
                    CreateDynamicSection()
                )
            )
        );
    }

    private UiElement CreateControlsSection()
    {
        return new VStack(
            new Label()
                .SetText("Interactive Shadow Controls")
                .SetTextSize(20)
                .SetTextColor(SKColors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            // Shadow Blur Control
            new HStack(
                new Label()
                    .SetText("Blur:")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowBlur), () => $"{vm.ShadowBlur:F1}")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Shadow Offset Y Control
            new HStack(
                new Label()
                    .SetText("Offset Y:")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowOffsetY), () => $"{vm.ShadowOffsetY:F1}")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Shadow Alpha Control
            new HStack(
                new Label()
                    .SetText("Alpha:")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.ShadowAlpha), () => $"{vm.ShadowAlpha}")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 5)),

            // Corner Radius Control
            new HStack(
                new Label()
                    .SetText("Corner Radius:")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White)
                    .SetDesiredWidth(120),
                new Label()
                    .BindText(nameof(vm.CornerRadius), () => $"{vm.CornerRadius:F1}")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.LightGray)
            ).SetMargin(new Margin(0, 0, 0, 10)),

            // Live Preview with current settings
            new Border()
                .SetBackgroundColor(SKColors.White)
                .BindCornerRadius(nameof(vm.CornerRadius), () => vm.CornerRadius)
                .BindShadowColor(nameof(vm.ShadowAlpha), () => SKColors.Black.WithAlpha(vm.ShadowAlpha))
                .BindShadowOffset(nameof(vm.ShadowOffsetY), () => new Point(vm.ShadowOffsetX, vm.ShadowOffsetY))
                .BindShadowBlur(nameof(vm.ShadowBlur), () => vm.ShadowBlur)
                .BindShadowSpread(nameof(vm.ShadowSpread), () => vm.ShadowSpread)
                .SetMargin(new Margin(20))
                .AddChild(
                    new Label()
                        .SetText("Live Preview")
                        .SetTextSize(18)
                        .SetTextColor(SKColors.Black)
                        .SetMargin(new Margin(20))
                        .SetHorizontalAlignment(HorizontalAlignment.Center)
                )
        );
    }

    private UiElement CreateExamplesSection()
    {
        return new VStack(
            new Label()
                .SetText("Shadow Examples")
                .SetTextSize(20)
                .SetTextColor(SKColors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            new HStack(
                // Card with subtle shadow
                new Border()
                    .SetBackgroundColor(SKColors.White)
                    .SetCornerRadius(8)
                    .SetShadowColor(SKColors.Black.WithAlpha(50))
                    .SetShadowOffset(new Point(0, 2))
                    .SetShadowBlur(8)
                    .SetShadowSpread(0)
                    .SetMargin(new Margin(10))
                    .AddChild(
                        new VStack(
                            new Label()
                                .SetText("Card Shadow")
                                .SetTextSize(16)
                                .SetTextColor(SKColors.Black),
                            new Label()
                                .SetText("Subtle elevation")
                                .SetTextSize(12)
                                .SetTextColor(SKColors.Gray)
                        ).SetMargin(new Margin(15))
                    ),

                // Button with stronger shadow
                new Border()
                    .SetBackgroundColor(new SKColor(66, 133, 244))
                    .SetCornerRadius(4)
                    .SetShadowColor(SKColors.Black.WithAlpha(75))
                    .SetShadowOffset(new Point(0, 4))
                    .SetShadowBlur(12)
                    .SetShadowSpread(0)
                    .SetMargin(new Margin(10))
                    .AddChild(
                        new Label()
                            .SetText("Button")
                            .SetTextSize(16)
                            .SetTextColor(SKColors.White)
                            .SetMargin(new Margin(20, 10))
                    )
            ).SetMargin(new Margin(0, 0, 0, 10)),

            new HStack(
                // Rounded card
                new Border()
                    .SetBackgroundColor(SKColors.White)
                    .SetCornerRadius(16)
                    .SetShadowColor(SKColors.Black.WithAlpha(40))
                    .SetShadowOffset(new Point(0, 3))
                    .SetShadowBlur(10)
                    .SetMargin(new Margin(10))
                    .AddChild(
                        new Label()
                            .SetText("Rounded")
                            .SetTextSize(14)
                            .SetTextColor(SKColors.Black)
                            .SetMargin(new Margin(15))
                    ),

                // No corner radius
                new Border()
                    .SetBackgroundColor(SKColors.White)
                    .SetCornerRadius(0)
                    .SetShadowColor(SKColors.Black.WithAlpha(60))
                    .SetShadowOffset(new Point(0, 2))
                    .SetShadowBlur(6)
                    .SetMargin(new Margin(10))
                    .AddChild(
                        new Label()
                            .SetText("Sharp")
                            .SetTextSize(14)
                            .SetTextColor(SKColors.Black)
                            .SetMargin(new Margin(15))
                    )
            )
        );
    }

    private UiElement CreateMaterialSection()
    {
        return new VStack(
            new Label()
                .SetText("Material Design Elevation")
                .SetTextSize(20)
                .SetTextColor(SKColors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            new HStack(
                new Button()
                    .SetText("−")
                    .SetTextSize(20)
                    .SetCommand(vm.DecreaseElevationCommand)
                    .SetPadding(new Margin(15, 5))
                    .SetTextColor(SKColors.White)
                    .SetBackgroundColor(new SKColor(244, 67, 54)),
                new Label()
                    .BindText(nameof(vm.Elevation), () => $"Elevation: {vm.Elevation}")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(10, 0)),
                new Button()
                    .SetText("+")
                    .SetTextSize(20)
                    .SetCommand(vm.IncreaseElevationCommand)
                    .SetPadding(new Margin(15, 5))
                    .SetTextColor(SKColors.White)
                    .SetBackgroundColor(new SKColor(76, 175, 80))
            ).SetMargin(new Margin(0, 0, 0, 10)),

            // Material elevation card with binding
            new Border()
                .SetBackgroundColor(SKColors.White)
                .SetCornerRadius(4)
                .BindShadowColor(nameof(vm.Elevation), () => 
                    vm.Elevation > 0 ? SKColors.Black.WithAlpha((byte)(vm.Elevation * 15)) : SKColors.Transparent)
                .BindShadowOffset(nameof(vm.Elevation), () => new Point(0, vm.Elevation))
                .BindShadowBlur(nameof(vm.Elevation), () => vm.Elevation * 2f)
                .BindShadowSpread(nameof(vm.Elevation), () => vm.Elevation * 0.5f)
                .SetMargin(new Margin(20))
                .AddChild(
                    new VStack(
                        new Label()
                            .SetText("Material Card")
                            .SetTextSize(18)
                            .SetTextColor(SKColors.Black),
                        new Label()
                            .SetText("Shadow changes with elevation level")
                            .SetTextSize(12)
                            .SetTextColor(SKColors.Gray)
                            .SetMargin(new Margin(0, 5, 0, 0))
                    ).SetMargin(new Margin(20))
                )
        );
    }

    private UiElement CreateDynamicSection()
    {
        return new VStack(
            new Label()
                .SetText("Dynamic Shadow on Hover")
                .SetTextSize(20)
                .SetTextColor(SKColors.LightBlue)
                .SetMargin(new Margin(0, 0, 0, 10)),

            new Button()
                .SetText("Toggle Hover State")
                .SetTextSize(16)
                .SetCommand(vm.ToggleHoverCommand)
                .SetPadding(new Margin(15, 8))
                .SetTextColor(SKColors.White)
                .SetBackgroundColor(new SKColor(156, 39, 176))
                .SetMargin(new Margin(0, 0, 0, 10)),

            // Card with dynamic shadow
            new Border()
                .SetBackgroundColor(SKColors.White)
                .SetCornerRadius(8)
                .BindShadowColor(nameof(vm.IsHovered), () => 
                    vm.IsHovered ? SKColors.Black.WithAlpha(100) : SKColors.Black.WithAlpha(50))
                .BindShadowBlur(nameof(vm.IsHovered), () => vm.IsHovered ? 16f : 4f)
                .BindShadowOffset(nameof(vm.IsHovered), () => 
                    vm.IsHovered ? new Point(0, 8) : new Point(0, 2))
                .SetMargin(new Margin(20))
                .AddChild(
                    new VStack(
                        new Label()
                            .BindText(nameof(vm.IsHovered), () => 
                                vm.IsHovered ? "Hovered! 🎯" : "Not Hovered")
                            .SetTextSize(18)
                            .SetTextColor(SKColors.Black)
                            .SetHorizontalAlignment(HorizontalAlignment.Center),
                        new Label()
                            .SetText("Shadow animates on hover")
                            .SetTextSize(12)
                            .SetTextColor(SKColors.Gray)
                            .SetMargin(new Margin(0, 5, 0, 0))
                            .SetHorizontalAlignment(HorizontalAlignment.Center)
                    ).SetMargin(new Margin(20))
                )
        );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element
            => element.SetBackgroundColor(new SKColor(30, 30, 30)));
    }
}
