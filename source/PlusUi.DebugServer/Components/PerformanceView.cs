using PlusUi.core;
using PlusUi.DebugServer.Pages;

namespace PlusUi.DebugServer.Components;

internal class PerformanceView : UserControl
{
    private readonly MainViewModel _viewModel;

    public PerformanceView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new Grid()
            .SetMargin(new Margin(16))
            .AddRow(Row.Auto) // Stats Cards
            .AddRow(Row.Auto) // Frame Breakdown + On-Demand Status
            .AddRow(Row.Star) // Graphs (fill remaining space)
            .AddColumn(Column.Star)

            // Row 0: Stats Cards
            .AddChild(row: 0, column: 0, child: new HStack()
                .SetSpacing(12)
                .SetMargin(new Margin(0, 0, 0, 16))
                .AddChild(CreateStatCard("FPS", () => _viewModel.FpsDisplay, new Color(100, 200, 100)))
                .AddChild(CreateStatCard("Utilization", () => _viewModel.UtilizationDisplay, new Color(100, 150, 255)))
                .AddChild(CreateStatCard("Memory", () => _viewModel.MemoryDisplay, new Color(255, 180, 100)))
                .AddChild(CreateStatCard("Frame Time", () => _viewModel.FrameTimeDisplay, new Color(200, 100, 200))))

            // Row 1: Frame Breakdown + On-Demand Status
            .AddChild(row: 1, column: 0, child: new HStack()
                .SetSpacing(24)
                .SetMargin(new Margin(0, 0, 0, 16))
                .AddChild(CreateFrameBreakdownSection())
                .AddChild(new VStack()
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .AddChild(new HStack()
                        .SetSpacing(8)
                        .AddChild(new Label()
                            .SetText("On-Demand:")
                            .SetTextSize(12)
                            .SetTextColor(new Color(150, 150, 150)))
                        .AddChild(new Label()
                            .BindText(() => _viewModel.DidRender ? "Rendered" : "Skipped")
                            .SetTextSize(12)
                            .BindTextColor(() => _viewModel.DidRender ? new Color(100, 200, 100) : new Color(150, 150, 150))))))

            // Row 2: Graphs (2x2 Grid that fills remaining space)
            .AddChild(row: 2, column: 0, child: CreateGraphsGrid());
    }

    private Grid CreateGraphsGrid()
    {
        return new Grid()
            .AddRow(Row.Star)
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)
            .AddChild(row: 0, column: 0, child: CreateGraphCard("Frame Time (ms)", () => _viewModel.FrameTimeHistory, new Color(200, 100, 200)))
            .AddChild(row: 0, column: 1, child: CreateGraphCard("FPS", () => _viewModel.FpsHistory, new Color(100, 200, 100)))
            .AddChild(row: 1, column: 0, child: CreateGraphCard("Memory (MB)", () => _viewModel.MemoryHistory, new Color(255, 180, 100)))
            .AddChild(row: 1, column: 1, child: CreateRenderActivityGraph());
    }

    private Border CreateFrameBreakdownSection()
    {
        var border = new Border()
            .SetBackground(new Color(45, 45, 45))
            .SetCornerRadius(8)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .AddChild(new VStack()
                .SetMargin(new Margin(16))
                .SetSpacing(12)
                .AddChild(new Label()
                    .SetText("Frame Breakdown")
                    .SetTextSize(14)
                    .SetTextColor(Colors.White))
                .AddChild(CreateBreakdownRow("Measure", () => $"{_viewModel.MeasureTimeMs:F4} ms"))
                .AddChild(CreateBreakdownRow("Arrange", () => $"{_viewModel.ArrangeTimeMs:F4} ms"))
                .AddChild(CreateBreakdownRow("Render", () => $"{_viewModel.RenderTimeMs:F4} ms")));
        return border;
    }

    private Border CreateStatCard(string title, System.Linq.Expressions.Expression<Func<string?>> valueBinding, Color accentColor)
    {
        var border = new Border()
            .SetBackground(new Color(45, 45, 45))
            .SetCornerRadius(8)
            .AddChild(new VStack()
                .SetMargin(new Margin(16, 12))
                .SetSpacing(4)
                .AddChild(new Label()
                    .SetText(title)
                    .SetTextSize(11)
                    .SetTextColor(new Color(150, 150, 150)))
                .AddChild(new Label()
                    .BindText(valueBinding)
                    .SetTextSize(20)
                    .SetTextColor(accentColor)));
        border.SetDesiredWidth(120);
        return border;
    }

    private HStack CreateBreakdownRow(string label, System.Linq.Expressions.Expression<Func<string?>> valueBinding)
    {
        return new HStack()
            .SetSpacing(16)
            .AddChild(new Label()
                .SetText(label)
                .SetTextSize(12)
                .SetTextColor(new Color(180, 180, 180)))
            .AddChild(new Label()
                .BindText(valueBinding)
                .SetTextSize(12)
                .SetTextColor(new Color(100, 200, 255)));
    }

    private Border CreateGraphCard(string title, System.Linq.Expressions.Expression<Func<List<float>>> dataBinding, Color lineColor)
    {
        return new Border()
            .SetBackground(new Color(45, 45, 45))
            .SetCornerRadius(8)
            .SetMargin(new Margin(4))
            .AddChild(new Grid()
                .SetMargin(new Margin(12))
                .AddRow(Row.Auto) // Title
                .AddRow(Row.Star) // Graph fills remaining
                .AddColumn(Column.Star)
                .AddChild(row: 0, column: 0, child: new Label()
                    .SetText(title)
                    .SetTextSize(11)
                    .SetTextColor(new Color(150, 150, 150))
                    .SetMargin(new Margin(0, 0, 0, 8)))
                .AddChild(row: 1, column: 0, child: new LineGraph()
                    .BindDataPoints(dataBinding)
                    .SetLineColor(lineColor)
                    .SetFillColor(new Color(lineColor.R, lineColor.G, lineColor.B, 50))
                    .SetGridColor(new Color(60, 60, 60))
                    .SetLineThickness(1.5f)));
    }

    private Border CreateRenderActivityGraph()
    {
        return new Border()
            .SetBackground(new Color(45, 45, 45))
            .SetCornerRadius(8)
            .SetMargin(new Margin(4))
            .AddChild(new Grid()
                .SetMargin(new Margin(12))
                .AddRow(Row.Auto) // Title
                .AddRow(Row.Star) // Graph fills remaining
                .AddColumn(Column.Star)
                .AddChild(row: 0, column: 0, child: new Label()
                    .SetText("Render Activity")
                    .SetTextSize(11)
                    .SetTextColor(new Color(150, 150, 150))
                    .SetMargin(new Margin(0, 0, 0, 8)))
                .AddChild(row: 1, column: 0, child: new LineGraph()
                    .BindDataPoints(() => _viewModel.RenderActivityHistory)
                    .SetLineColor(new Color(100, 200, 100))
                    .SetMinValue(0f)
                    .SetMaxValue(1f)
                    .SetGridColor(new Color(60, 60, 60))
                    .SetLineThickness(2f)));
    }
}
