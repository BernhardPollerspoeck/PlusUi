using PlusUi.core;
using PlusUi.DebugServer.Pages;
using System.ComponentModel;

namespace PlusUi.DebugServer.Components;

/// <summary>
/// Displays debug server status with refresh button.
/// </summary>
public class DebugStatusBar : UserControl
{
    private readonly MainViewModel _viewModel;

    public DebugStatusBar(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new HStack()
            .SetMargin(new Margin(8))
            .SetBackground(new Color(45, 45, 45))
            .AddChild(new Label()
                .BindText(nameof(_viewModel.StatusText), () => _viewModel.StatusText)
                .SetTextSize(14)
                .SetTextColor(Colors.White)
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetMargin(new Margin(8, 0)))
            .AddChild(new Button()
                .SetText("Refresh Tree")
                .SetCommand(_viewModel.RefreshTreeCommand)
                .SetBackground(new Color(60, 60, 60))
                .SetTextColor(Colors.White)
                .SetPadding(new Margin(12, 6)));
    }
}
