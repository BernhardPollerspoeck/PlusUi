using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Pages;
using System.ComponentModel;

namespace PlusUi.DebugServer.Components;

/// <summary>
/// Displays and edits properties of selected UI element.
/// </summary>
internal class PropertyGridView : UserControl
{
    private readonly MainViewModel _viewModel;

    public PropertyGridView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        var treeView = new TreeView();
        treeView.BindItemsSource(() => _viewModel.SortedProperties);
        treeView.SetChildrenSelector<PropertyDto>(prop => prop.Children);
        treeView.SetItemTemplate((item, depth) =>
        {
            if (item is not PropertyDto prop)
                return new Label()
                    .SetText("")
                    .SetTextColor(Colors.Gray)
                    .SetTextSize(12);

            // For simple writable properties without children: show Entry
            UiElement valueControl;
            if (prop.CanWrite && !prop.HasChildren)
            {
                valueControl = new Entry()
                    .SetText(prop.Value)
                    .SetTextColor(Colors.LightGray)
                    .SetTextSize(12)
                    .SetBackground(new Color(40, 40, 40))
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetMargin(new Margin(0, 2, 16, 2))
                    .SetPadding(new Margin(4, 2))
                    .BindText(() => prop.Value, newValue => _viewModel.UpdatePropertyValue(prop, newValue));
            }
            // For complex properties (with children) or writable properties: show Label + Edit button
            else if (prop.CanWrite || prop.HasChildren)
            {
                valueControl = new HStack()
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetMargin(new Margin(0, 0, 16, 0))
                    .AddChild(new Label()
                        .SetText(prop.Value)
                        .SetTextColor(Colors.LightGray)
                        .SetTextSize(12)
                        .SetVerticalAlignment(VerticalAlignment.Center)
                        .SetHorizontalAlignment(HorizontalAlignment.Left))
                    .AddChild(new Button()
                        .SetIcon("pencil.svg")
                        .SetIconTintColor(Colors.LightBlue)
                        .SetBackground(new Color(50, 50, 50))
                        .SetHoverBackground(new SolidColorBackground(new Color(70, 70, 70)))
                        .SetPadding(new Margin(6, 4))
                        .SetMargin(new Margin(8, 0, 0, 0))
                        .SetCornerRadius(3)
                        .SetCommand(_viewModel.EditPropertyCommand)
                        .SetCommandParameter(prop));
            }
            // For read-only properties: just show Label
            else
            {
                valueControl = new Label()
                    .SetText(prop.Value)
                    .SetTextColor(Colors.LightGray)
                    .SetTextSize(12)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetMargin(new Margin(0, 0, 16, 0));
            }

            return new HStack()
                .SetVerticalAlignment(VerticalAlignment.Center)
                .AddChild(CreatePinButton(prop))
                .AddChild(new Label()
                    .SetText(prop.Name)
                    .SetTextColor(prop.HasChildren ? Colors.LightBlue : Colors.White)
                    .SetTextSize(12)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(8, 0, 16, 0)))
                .AddChild(valueControl)
                .AddChild(new Label()
                    .SetText(prop.Type)
                    .SetTextColor(Colors.Gray)
                    .SetTextSize(11)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(0, 0, 8, 0)));
        });
        treeView.SetItemHeight(28);
        treeView.SetIndentation(24);
        treeView.SetExpanderSize(16);
        treeView.SetShowLines(true);
        treeView.SetLineColor(new Color(60, 60, 60));
        treeView.SetBackground(new Color(30, 30, 30));
        return treeView;
    }

    private Button CreatePinButton(PropertyDto prop)
    {
        return new Button()
            .BindIcon(() => _viewModel.PinnedPropertiesService.IsPinned(_viewModel.CurrentElementType, prop.Path)
                ? "pin.svg"
                : "pin-outline.svg")
            .BindIconTintColor(() => _viewModel.PinnedPropertiesService.IsPinned(_viewModel.CurrentElementType, prop.Path)
                    ? Colors.Yellow
                    : new Color(150, 150, 150))
            .SetBackground(Colors.Transparent)
            .SetHoverBackground(new SolidColorBackground(new Color(50, 50, 50)))
            .SetPadding(new Margin(4, 4))
            .SetMargin(new Margin(4, 0, 0, 0))
            .SetCornerRadius(3)
            .SetCommand(new RelayCommand(() =>
            {
                _viewModel.PinnedPropertiesService.TogglePin(_viewModel.CurrentElementType, prop.Path);
                _viewModel.RefreshProperties();
            }));
    }

    private void InvalidateElement()
    {
        InvalidateMeasure();
    }
}

