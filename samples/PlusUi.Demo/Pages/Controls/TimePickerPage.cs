using System;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class TimePickerPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "TimePicker";

    protected override string Description =>
        "Picks a time from a pop-up selector. Supports 12/24-hour format and a minute increment.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Note("Like DatePicker, TimePicker sets no default background, so it renders as floating text plus a clock icon (known issue)."),
            new TimePicker()
                .SetSelectedTime(new TimeOnly(9, 0))),

        Section("24-hour, 15-minute steps",
            new TimePicker()
                .Set24HourFormat(true)
                .SetMinuteIncrement(15)
                .SetSelectedTime(new TimeOnly(14, 30))),
    ];
}
