using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.RadioButtonDemo;

public class RadioButtonDemoPage(RadioButtonDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with back button
                new HStack(
                    new Button()
                        .SetText("← Back")
                        .SetTextSize(16)
                        .SetCommand(vm.GoBackCommand)
                        .SetTextColor(Colors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("RadioButton Demo")
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 10)),

                // Section: Basic RadioButtons with String Group
                CreateSection("Basic RadioButtons (String Group)",
                    new VStack(
                        new RadioButton()
                            .SetText("Option A")
                            .SetGroup("basicGroup")
                            .SetValue("A")
                            .BindIsSelected(() => vm.IsOptionASelected, v => vm.IsOptionASelected = v),
                        new RadioButton()
                            .SetText("Option B")
                            .SetGroup("basicGroup")
                            .SetValue("B")
                            .BindIsSelected(() => vm.IsOptionBSelected, v => vm.IsOptionBSelected = v)
                            .SetMargin(new Margin(0, 8, 0, 0)),
                        new RadioButton()
                            .SetText("Option C")
                            .SetGroup("basicGroup")
                            .SetValue("C")
                            .BindIsSelected(() => vm.IsOptionCSelected, v => vm.IsOptionCSelected = v)
                            .SetMargin(new Margin(0, 8, 0, 0))
                    )
                ),

                // Section: RadioButtons with Enum Group
                CreateSection("Size Selection (Enum Group)",
                    new HStack(
                        new RadioButton()
                            .SetText("Small")
                            .SetGroup(SizeGroup.Size)
                            .SetValue("S")
                            .BindIsSelected(() => vm.IsSizeSmall, v => vm.IsSizeSmall = v),
                        new RadioButton()
                            .SetText("Medium")
                            .SetGroup(SizeGroup.Size)
                            .SetValue("M")
                            .BindIsSelected(() => vm.IsSizeMedium, v => vm.IsSizeMedium = v)
                            .SetMargin(new Margin(20, 0, 0, 0)),
                        new RadioButton()
                            .SetText("Large")
                            .SetGroup(SizeGroup.Size)
                            .SetValue("L")
                            .BindIsSelected(() => vm.IsSizeLarge, v => vm.IsSizeLarge = v)
                            .SetMargin(new Margin(20, 0, 0, 0))
                    )
                ),

                // Section: Custom Styled RadioButtons
                CreateSection("Payment Method (Custom Styling)",
                    new VStack(
                        new RadioButton()
                            .SetText("Credit Card")
                            .SetGroup(PaymentGroup.Payment)
                            .SetValue("CC")
                            .SetTextColor(Colors.LightGreen)
                            .SetCircleColor(Colors.LightGreen)
                            .SetSelectedColor(Colors.Green)
                            .BindIsSelected(() => vm.IsCreditCard, v => vm.IsCreditCard = v),
                        new RadioButton()
                            .SetText("PayPal")
                            .SetGroup(PaymentGroup.Payment)
                            .SetValue("PP")
                            .SetTextColor(Colors.LightBlue)
                            .SetCircleColor(Colors.LightBlue)
                            .SetSelectedColor(Colors.DodgerBlue)
                            .BindIsSelected(() => vm.IsPayPal, v => vm.IsPayPal = v)
                            .SetMargin(new Margin(0, 12, 0, 0)),
                        new RadioButton()
                            .SetText("Bank Transfer")
                            .SetGroup(PaymentGroup.Payment)
                            .SetValue("BT")
                            .SetTextColor(Colors.Orange)
                            .SetCircleColor(Colors.Orange)
                            .SetSelectedColor(Colors.OrangeRed)
                            .BindIsSelected(() => vm.IsBankTransfer, v => vm.IsBankTransfer = v)
                            .SetMargin(new Margin(0, 12, 0, 0))
                    )
                ),

                // Section: Flexible Layout Demo
                CreateSection("Flexible Layout (Mixed with other controls)",
                    new VStack(
                        new HStack(
                            new RadioButton()
                                .SetText("Yes")
                                .SetGroup("yesNo")
                                .SetValue(true),
                            new Label()
                                .SetText("I agree to the terms and conditions")
                                .SetTextColor(Colors.Gray)
                                .SetTextSize(14)
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ),
                        new HStack(
                            new RadioButton()
                                .SetText("No")
                                .SetGroup("yesNo")
                                .SetValue(false),
                            new Label()
                                .SetText("I do not agree")
                                .SetTextColor(Colors.Gray)
                                .SetTextSize(14)
                                .SetMargin(new Margin(10, 0, 0, 0))
                        ).SetMargin(new Margin(0, 8, 0, 0))
                    )
                ),

                // Section: Different Text Sizes
                CreateSection("Different Text Sizes",
                    new VStack(
                        new RadioButton()
                            .SetText("Small Text (12)")
                            .SetGroup("textSizes")
                            .SetTextSize(12),
                        new RadioButton()
                            .SetText("Medium Text (16)")
                            .SetGroup("textSizes")
                            .SetTextSize(16)
                            .SetMargin(new Margin(0, 8, 0, 0)),
                        new RadioButton()
                            .SetText("Large Text (20)")
                            .SetGroup("textSizes")
                            .SetTextSize(20)
                            .SetMargin(new Margin(0, 8, 0, 0))
                    )
                ),

                // Bottom padding
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
