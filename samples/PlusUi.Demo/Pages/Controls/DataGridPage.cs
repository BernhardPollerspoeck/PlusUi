using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class Product
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public bool InStock { get; set; }
}

public class DataGridPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "DataGrid";

    protected override string Description =>
        "A tabular data grid with typed columns (text, checkbox, button, template) bound to a collection.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Typed columns",
            new DataGrid<Product>()
                .SetItemsSource(new List<Product>
                {
                    new() { Name = "Laptop", Quantity = 5, InStock = true },
                    new() { Name = "Mouse", Quantity = 20, InStock = false },
                    new() { Name = "Keyboard", Quantity = 12, InStock = true },
                    new() { Name = "Monitor", Quantity = 3, InStock = true },
                })
                .SetRowHeight(32)
                .AddColumn(new DataGridTextColumn<Product>()
                    .SetHeader("Name")
                    .SetBinding(p => p.Name)
                    .SetWidth(DataGridColumnWidth.Star(2)))
                .AddColumn(new DataGridTextColumn<Product>()
                    .SetHeader("Qty")
                    .SetBinding(p => p.Quantity.ToString())
                    .SetWidth(DataGridColumnWidth.Absolute(80)))
                .AddColumn(new DataGridCheckboxColumn<Product>()
                    .SetHeader("In stock")
                    .SetBinding(p => p.InStock, (p, v) => p.InStock = v)
                    .SetWidth(DataGridColumnWidth.Absolute(100)))
                .SetDesiredHeight(220)),
    ];
}
