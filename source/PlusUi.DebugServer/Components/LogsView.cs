using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Pages;

namespace PlusUi.DebugServer.Components;

public class LogsView : UserControl
{
    private readonly MainViewModel _viewModel;

    public LogsView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new Grid()
            .AddRow(Row.Auto)  // Filter bar
            .AddRow(Row.Star)  // Logs list
            .AddColumn(Column.Star)

            // Filter Bar
            .AddChild(
                row: 0,
                column: 0,
                child: CreateFilterBar())

            // Logs List
            .AddChild(
                row: 1,
                column: 0,
                child: CreateLogsList());
    }

    private UiElement CreateFilterBar()
    {
        return new Border()
            .SetBackground(new Color(35, 35, 35))
            .SetMargin(new Margin(8))
            .AddChild(new Grid()
                .AddColumn(Column.Star)
                .AddColumn(Column.Auto)
                .AddRow(Row.Auto)
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetMargin(new(4))
                .AddChild(
                    new HStack(
                        new Label()
                            .SetText("Log Level:")
                            .SetTextColor(new Color(200, 200, 200))
                            .SetTextSize(16)
                            .SetVerticalAlignment(VerticalAlignment.Center)
                            .SetMargin(new Margin(0, 4, 20, 4)),
                        CreateLogLevelButton(LogLevel.Trace),
                        CreateLogLevelButton(LogLevel.Debug),
                        CreateLogLevelButton(LogLevel.Information),
                        CreateLogLevelButton(LogLevel.Warning),
                        CreateLogLevelButton(LogLevel.Error),
                        CreateLogLevelButton(LogLevel.Critical))
                    .SetMargin(new Margin(16))
                    )

                .AddChild(
                    column: 1,
                    child: new Button()
                        .SetText("Clear Logs")
                        .SetBackground(new Color(60, 60, 60))
                        .SetTextSize(15)
                        .SetPadding(new Margin(20, 12))
                        .SetMargin(new(0,0,10,0))
                        .SetCornerRadius(4)
                        .SetVerticalAlignment(VerticalAlignment.Center)
                        .SetOnClick(() =>
                        {
                            _viewModel.SelectedApp?.ClearLogs();
                        })));
    }

    private UiElement CreateLogLevelButton(LogLevel level)
    {
        var button = new Button()
            .SetText(level.ToString())
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetCornerRadius(4)
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetOnClick(() =>
            {
                _viewModel.LogLevelFilter = level;
            });

        button.BindBackground(
            nameof(_viewModel.LogLevelFilter),
            () => _viewModel.LogLevelFilter == level
                ? new SolidColorBackground(GetLogLevelColor(level))
                : new SolidColorBackground(new Color(50, 50, 50)));

        return button;
    }

    private UiElement CreateLogsList()
    {
        var list = new ItemsList<LogMessageDto>()
            .SetOrientation(Orientation.Vertical)
            .SetItemTemplate((log, index) => CreateLogItem(log));

        list.BindItemsSource(
            nameof(_viewModel.FilteredLogs),
            () => _viewModel.FilteredLogs);

        return list;
    }

    private UiElement CreateLogItem(LogMessageDto? log)
    {
        // Handle null items gracefully
        if (log == null)
        {
            return new Border()
                .SetBackground(new Color(40, 40, 40))
                .SetMargin(new Margin(0, 0, 0, 1))
                .AddChild(new Label()
                    .SetText("[Invalid log entry]")
                    .SetTextColor(new Color(150, 150, 150))
                    .SetTextSize(12)
                    .SetMargin(new Margin(12, 8)));
        }

        var mainText = log.Message;
        if (!string.IsNullOrEmpty(log.Category))
        {
            mainText = $"[{log.Category}] {log.Message}";
        }
        if (!string.IsNullOrEmpty(log.Exception))
        {
            mainText = $"{mainText}\n{log.Exception}";
        }

        return new Border()
            .SetBackground(new Color(40, 40, 40))
            .SetMargin(new Margin(0, 0, 0, 1))
            .AddChild(new HStack()
                .SetVerticalAlignment(VerticalAlignment.Center)
                .AddChild(new Label()
                    .SetText(log.Timestamp.ToString("HH:mm:ss.fff"))
                    .SetTextColor(new Color(150, 150, 150))
                    .SetTextSize(11)
                    .SetDesiredWidth(90)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(12, 8, 8, 8)))
                .AddChild(new Label()
                    .SetText(log.Level.ToString())
                    .SetTextColor(GetLogLevelColor(log.Level))
                    .SetTextSize(11)
                    .SetDesiredWidth(80)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(0, 8, 12, 8)))
                .AddChild(new Label()
                    .SetText(mainText)
                    .SetTextColor(new Color(220, 220, 220))
                    .SetTextSize(12)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(0, 8, 12, 8))));
    }

    private static Color GetLogLevelColor(LogLevel level) => level switch
    {
        LogLevel.Trace => new Color(150, 150, 150),
        LogLevel.Debug => new Color(100, 180, 255),
        LogLevel.Information => new Color(100, 200, 100),
        LogLevel.Warning => new Color(255, 200, 80),
        LogLevel.Error => new Color(255, 100, 100),
        LogLevel.Critical => new Color(255, 50, 50),
        _ => new Color(128, 128, 128)
    };
}
