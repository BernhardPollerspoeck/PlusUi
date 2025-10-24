using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ItemsListDemo;

internal class ItemsListDemoPage(ItemsListDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("ItemsList Example")
                .SetTextSize(24)
                .SetMargin(new Margin(10)),

            new Label()
                .SetText("Vertical List with Virtualization:")
                .SetTextSize(16)
                .SetMargin(new Margin(10)),

            // Vertical ItemsList
            new ItemsList<ItemsListDemoPageViewModel.ItemModel>()
                .BindItemsSource(nameof(vm.Items), () => vm.Items)
                .SetItemTemplate((item, index) =>
                    new HStack(
                        new Solid(50, index == 2 ? 100 : 50)
                            .SetBackgroundColor(item.Color)
                            .SetMargin(new Margin(5)),
                        new VStack(
                            new Label()
                                .SetText($"#{index + 1}: {item.Title}")
                                .SetTextSize(16)
                                .SetTextColor(SKColors.White),
                            new Label()
                                .SetText(item.Description)
                                .SetTextSize(12)
                                .SetTextColor(SKColors.LightGray)
                        )
                        .SetMargin(new Margin(5))
                    )
                    .SetBackgroundColor(new SKColor(40, 40, 40))
                    .SetMargin(new Margin(5, 2))
                    .SetCornerRadius(5)
                )
                .SetOrientation(Orientation.Vertical)
                .SetScrollFactor(1.5f) // Faster scrolling
                .SetBackgroundColor(new SKColor(20, 20, 20))
                .SetCornerRadius(10)
                .SetMargin(new Margin(10))
                .SetDesiredHeight(300),

            new Label()
                .SetText("Horizontal List:")
                .SetTextSize(16)
                .SetMargin(new Margin(10)),

            // Horizontal ItemsList
            new ItemsList<ItemsListDemoPageViewModel.ItemModel>()
                .BindItemsSource(nameof(vm.HorizontalItems), () => vm.HorizontalItems)
                .SetItemTemplate((item, index) =>
                    new VStack(
                        new Solid(index == 2 ? 130 : 80, 80)
                            .SetBackgroundColor(item.Color)
                            .SetMargin(new Margin(5)),
                        new Label()
                            .SetText($"#{index + 1}: {item.Title}")
                            .SetTextSize(12)
                            .SetTextColor(SKColors.White)
                    )
                    .SetBackgroundColor(new SKColor(40, 40, 40))
                    .SetMargin(new Margin(2, 5))
                    .SetCornerRadius(5)
                )
                .SetOrientation(Orientation.Horizontal)
                .SetBackgroundColor(new SKColor(20, 20, 20))
                .SetCornerRadius(10)
                .SetMargin(new Margin(10))
                .SetDesiredHeight(120),

            new HStack(
                new Button()
                    .SetText("Add Item")
                    .SetTextSize(14)
                    .SetCommand(vm.AddItemCommand)
                    .SetPadding(new(10, 5))
                    .SetTextColor(SKColors.Black)
                    .SetBackgroundColor(SKColors.Green)
                    .SetMargin(new Margin(5)),

                new Button()
                    .SetText("Remove Item")
                    .SetTextSize(14)
                    .SetCommand(vm.RemoveItemCommand)
                    .SetPadding(new(10, 5))
                    .SetTextColor(SKColors.Black)
                    .SetBackgroundColor(SKColors.Red)
                    .SetMargin(new Margin(5))
            )
            .SetMargin(new Margin(10)),

            new Button()
                .SetText("Back")
                .SetTextSize(18)
                .SetCommand(vm.NavCommand)
                .SetPadding(new(10, 5))
                .SetTextColor(SKColors.Black)
                .SetBackgroundColor(SKColors.White)
                .SetMargin(new Margin(10))
        );
    }
}
