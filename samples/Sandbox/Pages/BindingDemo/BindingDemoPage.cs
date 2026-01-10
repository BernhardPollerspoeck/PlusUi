using PlusUi.core;
using System.Linq.Expressions;

namespace Sandbox.Pages.BindingDemo;

public class BindingDemoPage(BindingDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Back Button
                new Button()
                    .SetText("< Back")
                    .SetPadding(new Margin(15, 8))
                    .SetCommand(vm.GoBackCommand)
                    .SetHorizontalAlignment(HorizontalAlignment.Left)
                    .SetMargin(new Margin(20, 10)),

                // Page Title
                new Label()
                    .SetText("Path-Based Binding Demo")
                    .SetTextSize(28)
                    .SetTextColor(Colors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 10, 0, 20)),

                new Label()
                    .SetText("Each pair of checkboxes is bound to the same property.")
                    .SetTextColor(Colors.LightGray)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(20, 0, 20, 30)),

                // Section 1: Simple Binding (no nesting)
                CreateSection("Simple Binding (0 levels)",
                    "vm.SimpleValue",
                    Colors.Red, Colors.Blue,
                    () => vm.SimpleValue,
                    v => vm.SimpleValue = v
                ),

                // Section 2: One-Level Nested Binding
                CreateSection("1-Level Nested",
                    "vm.Level1.Checked",
                    Colors.Green, Colors.Orange,
                    () => vm.Level1.Checked,
                    v => vm.Level1.Checked = v
                ),

                // Section 3: Two-Level Nested Binding
                CreateSection("2-Level Nested",
                    "vm.Level1.Level2.DeepChecked",
                    Colors.Purple, Colors.Cyan,
                    () => vm.Level1.Level2.DeepChecked,
                    v => vm.Level1.Level2.DeepChecked = v
                ),

                // Section 4: Three-Level Nested Binding
                CreateSection("3-Level Nested",
                    "vm.Level1.Level2.Level3.Value",
                    Colors.Yellow, Colors.Magenta,
                    () => vm.Level1.Level2.Level3.Value,
                    v => vm.Level1.Level2.Level3.Value = v
                ),

                // Section 5: Four-Level Nested Binding
                CreateSection("4-Level Nested",
                    "vm.Level1.Level2.Level3.Level4.Value",
                    Colors.Lime, Colors.Coral,
                    () => vm.Level1.Level2.Level3.Level4.Value,
                    v => vm.Level1.Level2.Level3.Level4.Value = v
                ),

                // Section 6: Five-Level Nested Binding (ULTRA DEEP!)
                CreateSection("5-Level Nested (ULTRA DEEP!)",
                    "vm.Level1.Level2.Level3.Level4.Level5.UltraDeepValue",
                    Colors.Gold, Colors.DeepPink,
                    () => vm.Level1.Level2.Level3.Level4.Level5.UltraDeepValue,
                    v => vm.Level1.Level2.Level3.Level4.Level5.UltraDeepValue = v
                ),

                // Swap Button to test path rebinding
                new VStack(
                    new Label()
                        .SetText("Test Path Rebinding")
                        .SetTextSize(20)
                        .SetTextColor(Colors.LightGray)
                        .SetMargin(new Margin(0, 15, 0, 10)),
                    new Border()
                        .AddChild(
                            new VStack(
                                new Label()
                                    .SetText("Click the button below to replace the entire Level1 object.")
                                    .SetTextColor(Colors.White)
                                    .SetMargin(new Margin(0, 0, 0, 10)),
                                new Label()
                                    .SetText("ALL nested bindings (1-5 levels) should update automatically!")
                                    .SetTextColor(Colors.LightGray)
                                    .SetMargin(new Margin(0, 0, 0, 15)),
                                new Button()
                                    .SetText("Swap Level1 Object (Toggle All Values)")
                                    .SetPadding(new Margin(20, 10))
                                    .SetCommand(vm.SwapLevel1Command)
                                    .SetBackground(new SolidColorBackground(new Color(255, 59, 48)))
                                    .SetCornerRadius(8)
                            )
                        )
                        .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                        .SetCornerRadius(12)
                        .SetMargin(new Margin(0, 0, 0, 10))
                ).SetMargin(new Margin(20, 0)),

                // Spacer at bottom
                new Solid().SetDesiredHeight(50)
            )
        ).SetCanScrollHorizontally(false);
    }

    private static UiElement CreateSection(
        string title,
        string bindingPath,
        Color colorA,
        Color colorB,
        Expression<Func<bool>> bindingExpression,
        Action<bool> setter)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(20)
                .SetTextColor(Colors.LightGray)
                .SetMargin(new Margin(0, 15, 0, 5)),
            new Label()
                .SetText($"Path: {bindingPath}")
                .SetTextSize(12)
                .SetTextColor(Colors.Gray)
                .SetMargin(new Margin(0, 0, 0, 10)),
            new Border()
                .AddChild(
                    new VStack(
                        new HStack(
                            new Checkbox()
                                .BindIsChecked(bindingExpression, setter)
                                .SetColor(colorA),
                            new Label()
                                .SetText($"Checkbox A")
                                .SetTextColor(colorA)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ).SetMargin(new Margin(0, 5)),
                        new HStack(
                            new Checkbox()
                                .BindIsChecked(bindingExpression, setter)
                                .SetColor(colorB),
                            new Label()
                                .SetText($"Checkbox B")
                                .SetTextColor(colorB)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ).SetMargin(new Margin(0, 5)),
                        new Label()
                            .BindText(bindingExpression, v => $"Value: {v}")
                            .SetTextColor(Colors.Yellow)
                            .SetMargin(new Margin(0, 10, 0, 0))
                    )
                )
                .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
