using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class Product
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public int Quantity { get; set; }
    public bool InStock { get; set; }
}

public partial class DataGridPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    private string _lastAction = "No action yet";

    [RelayCommand]
    private void Buy(Product? product) =>
        LastAction = product is null ? "" : $"Bought {product.Name}";
}

public class DataGridPage(DataGridPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "DataGrid";

    protected override string Description =>
        "A tabular data grid with typed columns (text, checkbox, template, button) bound to a collection.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Typed columns",
            new DataGrid<Product>()
                .SetItemsSource(SampleProducts())
                .SetRowHeight(34)
                .AddColumn(new DataGridTextColumn<Product>()
                    .SetHeader("Name")
                    .SetBinding(p => p.Name)
                    .SetWidth(DataGridColumnWidth.Star(2)))
                .AddColumn(new DataGridTextColumn<Product>()
                    .SetHeader("Category")
                    .SetBinding(p => p.Category)
                    .SetWidth(DataGridColumnWidth.Star(1)))
                .AddColumn(new DataGridTextColumn<Product>()
                    .SetHeader("Qty")
                    .SetBinding(p => p.Quantity.ToString())
                    .SetWidth(DataGridColumnWidth.Absolute(60)))
                .AddColumn(new DataGridCheckboxColumn<Product>()
                    .SetHeader("Stock")
                    .SetBinding(p => p.InStock, (p, v) => p.InStock = v)
                    .SetWidth(DataGridColumnWidth.Absolute(70)))
                .AddColumn(new DataGridTemplateColumn<Product>()
                    .SetHeader("Status")
                    .SetCellTemplate((p, _) => new Label()
                        .SetText(p.InStock ? "● Available" : "● Sold out")
                        .SetTextColor(p.InStock ? PlusUiDefaults.AccentSuccess : PlusUiDefaults.AccentError))
                    .SetWidth(DataGridColumnWidth.Star(1)))
                .AddColumn(new DataGridButtonColumn<Product>()
                    .SetHeader("")
                    .SetButtonText("Buy")
                    .SetCommand(vm.BuyCommand, p => p)
                    .SetWidth(DataGridColumnWidth.Absolute(80)))
                .SetDesiredHeight(300)),

        Section("Last action",
            new Label().BindText(() => vm.LastAction)),
    ];

    private static List<Product> SampleProducts() =>
    [
        new() { Name = "Laptop", Category = "Computers", Quantity = 5, InStock = true },
        new() { Name = "Mouse", Category = "Accessories", Quantity = 20, InStock = false },
        new() { Name = "Keyboard", Category = "Accessories", Quantity = 12, InStock = true },
        new() { Name = "Monitor", Category = "Displays", Quantity = 3, InStock = true },
        new() { Name = "Webcam", Category = "Accessories", Quantity = 0, InStock = false },
        new() { Name = "Desk", Category = "Furniture", Quantity = 7, InStock = true },
        new() { Name = "Chair", Category = "Furniture", Quantity = 2, InStock = true },
        new() { Name = "Headset", Category = "Audio", Quantity = 9, InStock = false },
    ];
}
