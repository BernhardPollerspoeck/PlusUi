using System;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class DatePickerPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "DatePicker";

    protected override string Description =>
        "Picks a date from a pop-up calendar. Supports min/max range and a custom display format.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Note("DatePicker sets no default background or border, so it renders as floating text plus a hand-drawn calendar icon (known issue)."),
            new DatePicker()
                .SetSelectedDate(DateOnly.FromDateTime(DateTime.Today))),

        Section("Custom format & range",
            new DatePicker()
                .SetDisplayFormat("yyyy-MM-dd")
                .SetMinDate(new DateOnly(2020, 1, 1))
                .SetMaxDate(new DateOnly(2030, 12, 31))
                .SetSelectedDate(new DateOnly(2026, 6, 10))),
    ];
}
