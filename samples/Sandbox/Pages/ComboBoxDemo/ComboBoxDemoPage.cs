using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ComboBoxDemo;

public class ComboBoxDemoPage(ComboBoxDemoPageViewModel vm) : UiPageElement(vm)
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
                        .SetTextColor(SKColors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("ComboBox Demo")
                        .SetTextSize(24)
                        .SetTextColor(SKColors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 10)),

                // Status display for selections
                new Border()
                    .AddChild(
                        new VStack(
                            new Label()
                                .SetText("Current Selections:")
                                .SetTextSize(14)
                                .SetTextColor(SKColors.Gray),
                            new HStack(
                                new Label()
                                    .SetText("Fruit: ")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Gray),
                                new Label()
                                    .BindText(nameof(vm.SelectedFruit), () => vm.SelectedFruit ?? "(none)")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.LimeGreen)
                            ),
                            new HStack(
                                new Label()
                                    .SetText("Color: ")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Gray),
                                new Label()
                                    .BindText(nameof(vm.SelectedColor), () => vm.SelectedColor ?? "(none)")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Cyan)
                            ),
                            new HStack(
                                new Label()
                                    .SetText("Person: ")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Gray),
                                new Label()
                                    .BindText(nameof(vm.SelectedPerson), () => vm.SelectedPerson?.Name ?? "(none)")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Orange)
                            ),
                            new HStack(
                                new Label()
                                    .SetText("Country Index: ")
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Gray),
                                new Label()
                                    .BindText(nameof(vm.SelectedCountryIndex), () => vm.SelectedCountryIndex.ToString())
                                    .SetTextSize(14)
                                    .SetTextColor(SKColors.Pink)
                            )
                        ).SetMargin(new Margin(16, 8))
                    )
                    .SetBackground(new SolidColorBackground(new SKColor(30, 30, 30)))
                    .SetCornerRadius(8)
                    .SetMargin(new Margin(20, 0, 20, 20)),

                // Section: Basic ComboBox with Placeholder
                CreateSection("Basic ComboBox with Placeholder",
                    new ComboBox<string>()
                        .BindItemsSource(nameof(vm.Fruits), () => vm.Fruits)
                        .SetPlaceholder("Select a fruit...")
                        .BindSelectedItem(nameof(vm.SelectedFruit), () => vm.SelectedFruit, f => vm.SelectedFruit = f)
                        .SetDesiredSize(new Size(250, 40))
                        .SetBackground(new SolidColorBackground(new SKColor(50, 50, 50)))
                        .SetCornerRadius(8)
                ),

                // Section: ComboBox with Custom Styling
                CreateSection("Custom Styled ComboBox",
                    new ComboBox<string>()
                        .BindItemsSource(nameof(vm.Colors), () => vm.Colors)
                        .SetPlaceholder("Pick a color...")
                        .BindSelectedItem(nameof(vm.SelectedColor), () => vm.SelectedColor, c => vm.SelectedColor = c)
                        .SetTextColor(SKColors.Cyan)
                        .SetPlaceholderColor(new SKColor(100, 150, 150))
                        .SetDropdownBackground(new SKColor(20, 40, 40))
                        .SetHoverBackground(new SKColor(40, 80, 80))
                        .SetTextSize(16)
                        .SetDesiredSize(new Size(250, 44))
                        .SetBackground(new SolidColorBackground(new SKColor(30, 60, 60)))
                        .SetCornerRadius(12)
                ),

                // Section: ComboBox with Custom Display Function
                CreateSection("ComboBox with Custom Display (Person objects)",
                    new ComboBox<Person>()
                        .BindItemsSource(nameof(vm.People), () => vm.People)
                        .SetDisplayFunc(p => $"{p.Name} ({p.Age} years)")
                        .SetPlaceholder("Select a person...")
                        .BindSelectedItem(nameof(vm.SelectedPerson), () => vm.SelectedPerson, p => vm.SelectedPerson = p)
                        .SetTextColor(SKColors.Orange)
                        .SetDesiredSize(new Size(280, 40))
                        .SetBackground(new SolidColorBackground(new SKColor(60, 40, 20)))
                        .SetCornerRadius(8)
                ),

                // Section: ComboBox with Index Binding
                CreateSection("ComboBox with Index Binding",
                    new ComboBox<string>()
                        .BindItemsSource(nameof(vm.Countries), () => vm.Countries)
                        .SetPlaceholder("Select a country...")
                        .BindSelectedIndex(nameof(vm.SelectedCountryIndex), () => vm.SelectedCountryIndex, i => vm.SelectedCountryIndex = i)
                        .SetTextColor(SKColors.Pink)
                        .SetDesiredSize(new Size(250, 40))
                        .SetBackground(new SolidColorBackground(new SKColor(60, 30, 50)))
                        .SetCornerRadius(8)
                ),

                // Section: ComboBox with Preset Selection
                CreateSection("ComboBox with Preset Selection",
                    new ComboBox<string>()
                        .SetItemsSource(new[] { "Small", "Medium", "Large", "Extra Large" })
                        .SetSelectedIndex(1)
                        .SetTextSize(18)
                        .SetDesiredSize(new Size(200, 48))
                        .SetBackground(new SolidColorBackground(new SKColor(40, 50, 60)))
                        .SetCornerRadius(6)
                ),

                // Section: Multiple ComboBoxes in a Row
                CreateSection("Multiple ComboBoxes",
                    new HStack(
                        new VStack(
                            new Label()
                                .SetText("Size")
                                .SetTextSize(12)
                                .SetTextColor(SKColors.Gray),
                            new ComboBox<string>()
                                .SetItemsSource(new[] { "S", "M", "L", "XL" })
                                .SetPlaceholder("Size")
                                .SetDesiredSize(new Size(80, 36))
                                .SetBackground(new SolidColorBackground(new SKColor(50, 50, 50)))
                                .SetCornerRadius(4)
                        ).SetMargin(new Margin(0, 0, 16, 0)),
                        new VStack(
                            new Label()
                                .SetText("Color")
                                .SetTextSize(12)
                                .SetTextColor(SKColors.Gray),
                            new ComboBox<string>()
                                .SetItemsSource(new[] { "Red", "Blue", "Green", "Black", "White" })
                                .SetPlaceholder("Color")
                                .SetDesiredSize(new Size(100, 36))
                                .SetBackground(new SolidColorBackground(new SKColor(50, 50, 50)))
                                .SetCornerRadius(4)
                        ).SetMargin(new Margin(0, 0, 16, 0)),
                        new VStack(
                            new Label()
                                .SetText("Quantity")
                                .SetTextSize(12)
                                .SetTextColor(SKColors.Gray),
                            new ComboBox<int>()
                                .SetItemsSource(new[] { 1, 2, 3, 4, 5, 10, 20 })
                                .SetPlaceholder("Qty")
                                .SetDesiredSize(new Size(80, 36))
                                .SetBackground(new SolidColorBackground(new SKColor(50, 50, 50)))
                                .SetCornerRadius(4)
                        )
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
                .SetTextColor(SKColors.LightGray)
                .SetMargin(new Margin(0, 15, 0, 10)),
            new Border()
                .AddChild(
                    new VStack(content)
                        .SetMargin(new Margin(16))
                )
                .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
