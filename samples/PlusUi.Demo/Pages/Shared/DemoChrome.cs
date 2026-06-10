using System.Windows.Input;
using PlusUi.core;

namespace PlusUi.Demo.Pages.Shared;

/// <summary>
/// Shared page chrome for demo pages: a sticky top-left back bar that stays fixed
/// while the content below scrolls.
/// </summary>
public static class DemoChrome
{
    /// <summary>
    /// Wraps scrollable content with a fixed back bar at the top. The back bar stays
    /// visible (sticky) because it lives in a separate, non-scrolling grid row.
    /// </summary>
    public static UiElement WithStickyBack(UiElement scrollableContent, ICommand goBackCommand)
    {
        return new Grid()
            .AddRow(Row.Auto)
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddChild(BackBar(goBackCommand), row: 0, column: 0)
            .AddChild(
                new ScrollView(scrollableContent).SetCanScrollHorizontally(false),
                row: 1, column: 0);
    }

    public static UiElement BackBar(ICommand goBackCommand) =>
        new Border()
            .SetBackground(PlusUiDefaults.BackgroundPrimary)
            .SetStrokeThickness(0)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChild(new Button()
                .SetText("← Back")
                .SetHorizontalAlignment(HorizontalAlignment.Left)
                .SetMargin(new Margin(12, 8))
                .SetCommand(goBackCommand));
}
