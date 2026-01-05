using PlusUi.core;
using PlusUi.DebugServer.Pages;

namespace PlusUi.DebugServer.Components;

public class WaitingOverlay : UserControl
{
    private readonly MainViewModel _viewModel;

    public WaitingOverlay(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new Grid()
            .SetBackground(new Color(25, 25, 25))
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddChild(
                row: 0,
                column: 0,
                child: new VStack()
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                    .AddChild(new Label()
                        .SetText("⏳ Waiting for connections...")
                        .SetTextColor(new Color(150, 150, 150))
                        .SetTextSize(18)
                        .SetMargin(new Margin(0, 0, 0, 16)))
                    .AddChild(new Label()
                        .SetText("Start a PlusUi app with debug bridge enabled")
                        .SetTextColor(new Color(120, 120, 120))
                        .SetTextSize(13)
                        .SetMargin(new Margin(0, 0, 0, 8)))
                    .AddChild(new Label()
                        .SetText("The app will automatically connect to this debug server")
                        .SetTextColor(new Color(100, 100, 100))
                        .SetTextSize(12)))
            .BindIsVisible(nameof(_viewModel.HasConnectedApps),
                () => !_viewModel.HasConnectedApps);
    }
}
