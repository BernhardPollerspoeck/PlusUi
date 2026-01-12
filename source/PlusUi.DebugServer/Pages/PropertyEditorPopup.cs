using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.core.CoreElements;
using PlusUi.core.Services.DebugBridge.Models;
using System.Collections.ObjectModel;

namespace PlusUi.DebugServer.Pages;

/// <summary>
/// Result returned from PropertyEditorPopup containing the edited field values.
/// </summary>
internal class PropertyEditorResult
{
    public List<PropertyFieldResult> Fields { get; set; } = [];
}

internal class PropertyFieldResult
{
    public string ElementId { get; set; } = "";
    public string Path { get; set; } = "";
    public string Value { get; set; } = "";
}

internal partial class PropertyEditorPopupViewModel(IPopupService popupService) : ObservableObject
{
    public ObservableCollection<PropertyFieldViewModel> Fields { get; } = [];

    [RelayCommand]
    private void Save()
    {
        popupService.ClosePopup(success: true);
    }

    [RelayCommand]
    private void Cancel()
    {
        popupService.ClosePopup(success: false);
    }

    /// <summary>
    /// Collects only the modified field values as a result object.
    /// </summary>
    public PropertyEditorResult GetResult()
    {
        return new PropertyEditorResult
        {
            Fields = Fields
                .Where(f => f.IsModified) // Only send changed values!
                .Select(f => new PropertyFieldResult
                {
                    ElementId = f.ElementId,
                    Path = f.Path,
                    Value = f.Value
                }).ToList()
        };
    }

    public void Initialize(PropertyDto property)
    {
        Fields.Clear();
        if (property.Children.Count > 0)
        {
            foreach (var child in property.Children)
            {
                Fields.Add(new PropertyFieldViewModel
                {
                    Name = child.Name,
                    Value = child.Value,
                    OriginalValue = child.Value, // Track original
                    Type = child.Type,
                    Path = child.Path,
                    ElementId = child.ElementId
                });
            }
        }
        else
        {
            // Simple property - just one field
            Fields.Add(new PropertyFieldViewModel
            {
                Name = property.Name,
                Value = property.Value,
                OriginalValue = property.Value, // Track original
                Type = property.Type,
                Path = property.Path,
                ElementId = property.ElementId
            });
        }
    }
}

internal partial class PropertyFieldViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _value = "";

    [ObservableProperty]
    private string _type = "";

    [ObservableProperty]
    private string _path = "";

    [ObservableProperty]
    private string _elementId = "";

    // Track original value to detect changes
    public string OriginalValue { get; set; } = "";

    public bool IsModified => Value != OriginalValue;
}

internal class PropertyEditorPopup(PropertyEditorPopupViewModel vm) : UiPopupElement<PropertyDto, PropertyEditorResult>(vm)
{
    public override void Close(bool success)
    {
        // Collect result from ViewModel before closing
        if (success)
        {
            SetResult(vm.GetResult());
        }
        base.Close(success);
    }

    protected override UiElement Build()
    {
        // Initialize the ViewModel with the property data
        if (Argument != null)
        {
            vm.Initialize(Argument);
        }

        return new VStack(
            // Header
            new VStack(
                new Label()
                    .SetText($"Edit {Argument?.Name ?? "Property"}")
                    .SetTextSize(18)
                    .SetTextColor(Colors.White)
                    .SetFontWeight(FontWeight.SemiBold),
                new Label()
                    .SetText(Argument?.Type ?? "")
                    .SetTextSize(12)
                    .SetTextColor(new Color(150, 150, 150))
                    .SetMargin(new Margin(0, 4, 0, 0))
            ).SetMargin(new Margin(24, 24, 24, 12)),

            // DataGrid for Fields
            new DataGrid<PropertyFieldViewModel>()
                .SetItemsSource(vm.Fields)
                .SetCellPadding(new Margin(4))
                .AddColumn(new DataGridTextColumn<PropertyFieldViewModel>()
                    .SetHeader("Property")
                    .SetBinding(f => f.Name)
                    .SetWidth(DataGridColumnWidth.Absolute(80)))
                .AddColumn(new DataGridEditorColumn<PropertyFieldViewModel>()
                    .SetHeader("Value")
                    .SetBinding(f => f.Value, (f, v) => f.Value = v)
                    .SetWidth(DataGridColumnWidth.Absolute(200)))
                .SetRowHeight(36)
                .SetHeaderHeight(32)
                .SetAlternatingRowStyles(true)
                .SetEvenRowStyle(new SolidColorBackground(new Color(45, 45, 45)), Colors.White)
                .SetOddRowStyle(new SolidColorBackground(new Color(50, 50, 50)), Colors.White)
                .SetBackground(new Color(45, 45, 45))
                .SetCornerRadius(6)
                .SetMargin(new Margin(24, 0, 24, 12))
                .SetDesiredHeight(250),

            // Buttons
            new HStack(
                new Button()
                    .SetText("Cancel")
                    .SetCommand(vm.CancelCommand)
                    .SetTextColor(new Color(200, 200, 200))
                    .SetBackground(new Color(60, 60, 60))
                    .SetHoverBackground(new SolidColorBackground(new Color(70, 70, 70)))
                    .SetPadding(new Margin(20, 10))
                    .SetMargin(new Margin(0, 0, 12, 0))
                    .SetCornerRadius(6),
                new Button()
                    .SetText("Save Changes")
                    .SetCommand(vm.SaveCommand)
                    .SetTextColor(Colors.White)
                    .SetBackground(new Color(0, 122, 255))
                    .SetHoverBackground(new SolidColorBackground(new Color(10, 132, 255)))
                    .SetPadding(new Margin(20, 10))
                    .SetCornerRadius(6)
            )
            .SetMargin(new Margin(24, 12, 24, 24))
            .SetHorizontalAlignment(HorizontalAlignment.Right)
        )
        .SetBackground(new Color(40, 40, 40))
        .SetDesiredWidth(340)
        .SetCornerRadius(12)
        .SetMargin(new Margin(40, 40, 40, 40));
    }
}
