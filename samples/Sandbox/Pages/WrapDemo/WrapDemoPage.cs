using PlusUi.core;

namespace Sandbox.Pages.WrapDemo;

public class WrapDemoPage(WrapDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Page Title
                new Label()
                    .SetText("HStack / VStack Wrap Demo")
                    .SetTextSize(32)
                    .SetTextColor(Colors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                // Section: HStack Wrap
                CreateSection("HStack with Wrap",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Enable Wrap:")
                                .SetTextColor(Colors.White),
                            new Checkbox()
                                .BindIsChecked(nameof(vm.HStackWrapEnabled), () => vm.HStackWrapEnabled, v => vm.HStackWrapEnabled = v)
                        ),
                        new Label()
                            .SetText("Items wrap to next row when exceeding container width")
                            .SetTextColor(Colors.LightGray)
                            .SetTextSize(12)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new Border()
                            .AddChild(
                                new HStack(
                                    CreateTag("Tag 1", Colors.DodgerBlue),
                                    CreateTag("Tag 2", Colors.MediumSeaGreen),
                                    CreateTag("Long Tag 3", Colors.OrangeRed),
                                    CreateTag("Tag 4", Colors.Purple),
                                    CreateTag("Tag 5", Colors.Teal),
                                    CreateTag("Tag 6", Colors.Crimson),
                                    CreateTag("Tag 7", Colors.DarkOrange),
                                    CreateTag("Tag 8", Colors.SlateBlue)
                                ).BindWrap(nameof(vm.HStackWrapEnabled), () => vm.HStackWrapEnabled)
                                 .SetMargin(new Margin(10))
                            )
                            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                            .SetCornerRadius(8)
                            .SetDesiredWidth(350)
                    )
                ),

                // Section: VStack Wrap
                CreateSection("VStack with Wrap",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Enable Wrap:")
                                .SetTextColor(Colors.White),
                            new Checkbox()
                                .BindIsChecked(nameof(vm.VStackWrapEnabled), () => vm.VStackWrapEnabled, v => vm.VStackWrapEnabled = v)
                        ),
                        new Label()
                            .SetText("Items wrap to next column when exceeding container height")
                            .SetTextColor(Colors.LightGray)
                            .SetTextSize(12)
                            .SetMargin(new Margin(0, 0, 0, 10)),
                        new Border()
                            .AddChild(
                                new VStack(
                                    CreateListItem("Item 1", Colors.DodgerBlue),
                                    CreateListItem("Item 2", Colors.MediumSeaGreen),
                                    CreateListItem("Item 3", Colors.OrangeRed),
                                    CreateListItem("Item 4", Colors.Purple),
                                    CreateListItem("Item 5", Colors.Teal),
                                    CreateListItem("Item 6", Colors.Crimson),
                                    CreateListItem("Item 7", Colors.DarkOrange),
                                    CreateListItem("Item 8", Colors.SlateBlue)
                                ).BindWrap(nameof(vm.VStackWrapEnabled), () => vm.VStackWrapEnabled)
                                 .SetMargin(new Margin(10))
                            )
                            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                            .SetCornerRadius(8)
                            .SetDesiredHeight(200)
                    )
                ),

                // Section: Use Cases
                CreateSection("Practical Use Cases",
                    new VStack(
                        new Label()
                            .SetText("Tag Cloud")
                            .SetTextColor(Colors.Yellow)
                            .SetMargin(new Margin(0, 0, 0, 5)),
                        new Border()
                            .AddChild(
                                new HStack(
                                    CreateTag("C#", Colors.Purple),
                                    CreateTag(".NET", Colors.DodgerBlue),
                                    CreateTag("SkiaSharp", Colors.OrangeRed),
                                    CreateTag("Cross-Platform", Colors.MediumSeaGreen),
                                    CreateTag("UI Framework", Colors.Teal),
                                    CreateTag("MVVM", Colors.Crimson),
                                    CreateTag("Fluent API", Colors.DarkOrange)
                                ).SetWrap(true)
                                 .SetMargin(new Margin(10))
                            )
                            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                            .SetCornerRadius(8)
                            .SetDesiredWidth(300)
                            .SetMargin(new Margin(0, 0, 0, 15)),

                        new Label()
                            .SetText("Button Gallery")
                            .SetTextColor(Colors.Yellow)
                            .SetMargin(new Margin(0, 0, 0, 5)),
                        new Border()
                            .AddChild(
                                new HStack(
                                    CreateActionButton("Save"),
                                    CreateActionButton("Load"),
                                    CreateActionButton("Export"),
                                    CreateActionButton("Import"),
                                    CreateActionButton("Settings"),
                                    CreateActionButton("Help")
                                ).SetWrap(true)
                                 .SetMargin(new Margin(10))
                            )
                            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                            .SetCornerRadius(8)
                            .SetDesiredWidth(350)
                    )
                )
            )
        );
    }

    private static UiElement CreateTag(string text, Color color)
    {
        return new Border()
            .AddChild(
                new Label()
                    .SetText(text)
                    .SetTextColor(Colors.White)
                    .SetTextSize(12)
                    .SetMargin(new Margin(10, 5))
            )
            .SetBackground(new SolidColorBackground(color))
            .SetCornerRadius(12)
            .SetMargin(new Margin(3));
    }

    private static UiElement CreateListItem(string text, Color color)
    {
        return new Border()
            .AddChild(
                new Label()
                    .SetText(text)
                    .SetTextColor(Colors.White)
                    .SetTextSize(12)
                    .SetMargin(new Margin(15, 8))
            )
            .SetBackground(new SolidColorBackground(color))
            .SetCornerRadius(4)
            .SetMargin(new Margin(3));
    }

    private static UiElement CreateActionButton(string text)
    {
        return new Button()
            .SetText(text)
            .SetPadding(new Margin(15, 8))
            .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
            .SetCornerRadius(6)
            .SetMargin(new Margin(3));
    }

    private static UiElement CreateSection(string title, UiElement content)
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
                        .SetMargin(new Margin(15))
                )
                .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
