using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Pages;

namespace PlusUi.DebugServer.Components;

internal class AppContentView : UserControl
{
    private readonly MainViewModel _viewModel;

    public AppContentView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new Grid()
            .SetBackground(new Color(30, 30, 30))
            .AddRow(Row.Star)
            .AddColumn(Column.Star)

            // Feature Tabs
            .AddChild(
                row: 0,
                column: 0,
                child: new TabControl()
                    .SetTabs(new[]
                    {
                        new TabItem()
                            .SetHeader("Inspector")
                            .SetContent(CreateInspectorContent()),
                        new TabItem()
                            .SetHeader("Logs")
                            .SetContent(new LogsView(_viewModel)),
                        new TabItem()
                            .SetHeader("Performance")
                            .SetContent(new PerformanceView(_viewModel)),
                        new TabItem()
                            .SetHeader("Changes")
                            .SetContent(CreatePlaceholder("Changes Content")),
                        new TabItem()
                            .SetHeader("Screenshot")
                            .SetContent(CreatePlaceholder("Screenshot Content"))
                    })
                    .SetHeaderBackgroundColor(new Color(35, 35, 35))
                    .SetActiveTabBackgroundColor(new Color(45, 45, 45))
                    .SetHeaderTextSize(13));
    }

    private UiElement CreateInspectorContent()
    {
        return new Grid()
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)

            // Left: Element Tree
            .AddChild(
                row: 0,
                column: 0,
                child: new Border()
                    .SetBackground(new Color(40, 40, 40))
                    .SetCornerRadius(4)
                    .SetMargin(new Margin(8))
                    .AddChild(new ElementTreeView(_viewModel)))

            // Right: Property Grid
            .AddChild(
                row: 0,
                column: 1,
                child: new Border()
                    .SetBackground(new Color(40, 40, 40))
                    .SetCornerRadius(4)
                    .SetMargin(new Margin(8))
                    .AddChild(new PropertyGridView(_viewModel)));
    }

    private UiElement CreatePlaceholder(string text)
    {
        return new VStack()
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .AddChild(new Label()
                .SetText(text)
                .SetTextColor(new Color(120, 120, 120))
                .SetTextSize(16));
    }

}
