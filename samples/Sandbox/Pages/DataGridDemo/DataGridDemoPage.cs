using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.DataGridDemo;

internal class DataGridDemoPage(DataGridDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("DataGrid Demo")
                .SetTextSize(24)
                .SetMargin(new Margin(10)),
                
            // Controls
            new HStack(
                new Label()
                    .SetText("Auto Generate Columns:")
                    .SetTextSize(16)
                    .SetMargin(new Margin(10)),
                    
                new Checkbox()
                    .BindIsChecked(nameof(vm.AutoGenerateColumns), 
                        () => vm.AutoGenerateColumns, 
                        isChecked => vm.AutoGenerateColumns = isChecked)
                    .SetMargin(new Margin(10))
            ),
            
            // Data Grid with auto-generated columns
            new Label()
                .SetText("DataGrid with Auto-Generated Columns:")
                .SetTextSize(18)
                .SetMargin(new Margin(10, 20, 10, 5)),
                
            new DataGrid()
                .BindItemsSource(nameof(vm.People), () => vm.People)
                .BindAutoGenerateColumns(nameof(vm.AutoGenerateColumns), () => vm.AutoGenerateColumns)
                .SetRowSpacing(1)
                .SetColumnSpacing(1)
                .SetBackgroundColor(new SKColor(230, 230, 230))
                .SetMargin(new Margin(10)),
                
            // Data Grid with manual column configuration
            new Label()
                .SetText("DataGrid with Manual Column Configuration:")
                .SetTextSize(18)
                .SetMargin(new Margin(10, 20, 10, 5)),
                
            CreateManuallyConfiguredDataGrid(),
            
            // Back button
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
    
    private DataGrid CreateManuallyConfiguredDataGrid()
    {
        return (DataGrid)(new DataGrid()
            .SetItemsSource(vm.CustomPeople)
            .SetAutoGenerateColumns(false)
            // Add custom columns
            .AddColumn("Id", "ID")
            .AddColumn("Name", "Name", DataGridCellTemplate.Label)
            .AddColumn("Email", "Email Address", (value) => 
                new Label()
                    .SetText(value?.ToString() ?? string.Empty)
                    .SetTextColor(SKColors.Blue)
                    .SetPadding(new Margin(5, 3)))
            .AddColumn("IsActive", "Status", (value) =>
            {
                var isActive = value is bool boolValue && boolValue;
                return new Label()
                    .SetText(isActive ? "Active" : "Inactive")
                    .SetTextColor(isActive ? SKColors.Green : SKColors.Red)
                    .SetPadding(new Margin(5, 3));
            })
            .AddColumn("ImageUrl", "Avatar", DataGridCellTemplate.Image)
            .SetBackgroundColor(new SKColor(230, 230, 230))
            .SetMargin(new Margin(10)));
    }
}