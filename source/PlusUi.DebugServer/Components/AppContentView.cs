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
            .AddRow(Row.Auto)  // Status Bar
            .AddRow(Row.Star)  // Feature Tabs
            .AddColumn(Column.Star)

            // Status Bar Placeholder
            .AddChild(
                row: 0,
                column: 0,
                child: new Border()
                    .SetBackground(new Color(50, 50, 50))
                    .AddChild(new Label()
                        .SetText("FPS: 60 (100%) │ Memory: 45MB │ Render: 16ms │ Modified: No")
                        .SetTextColor(new Color(220, 220, 220))
                        .SetTextSize(12)
                        .SetMargin(new Margin(12, 8))))

            // Feature Tabs
            .AddChild(
                row: 1,
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
                            .SetContent(CreatePlaceholder("Performance Content")),
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
