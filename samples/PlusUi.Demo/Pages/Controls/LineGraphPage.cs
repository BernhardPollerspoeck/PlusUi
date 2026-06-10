using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class LineGraphPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "LineGraph";

    protected override string Description =>
        "Plots a series of values as a line chart with optional fill and axis labels.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Demo("6 data points",
                new LineGraph()
                    .SetDataPoints(new float[] { 10, 20, 15, 25, 30, 22 })
                    .SetDesiredHeight(160))),

        Section("With fill & labels",
            new LineGraph()
                .SetDataPoints(new float[] { 5, 12, 8, 18, 14, 24, 20, 30, 26, 34 })
                .SetFillColor(new Color(100, 200, 255, 80))
                .SetShowAxisLabels(true)
                .SetDesiredHeight(160)),
    ];
}
