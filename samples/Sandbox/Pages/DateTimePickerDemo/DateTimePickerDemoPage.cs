using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.DateTimePickerDemo;

public class DateTimePickerDemoPage(DateTimePickerDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with back button
                new HStack(
                    new Button()
                        .SetText("<- Back")
                        .SetTextSize(16)
                        .SetCommand(vm.GoBackCommand)
                        .SetTextColor(Colors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("DatePicker & TimePicker Demo")
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 10)),

                // Status display for selections
                new Border()
                    .AddChild(
                        new VStack(
                            new Label()
                                .SetText("Current Selections:")
                                .SetTextSize(14)
                                .SetTextColor(Colors.Gray),
                            new HStack(
                                new Label()
                                    .SetText("Date: ")
                                    .SetTextSize(14)
                                    .SetTextColor(Colors.Gray),
                                new Label()
                                    .BindText(nameof(vm.FormDate), () => vm.FormDate?.ToString("dd.MM.yyyy") ?? "(none)")
                                    .SetTextSize(14)
                                    .SetTextColor(Colors.LimeGreen)
                            ),
                            new HStack(
                                new Label()
                                    .SetText("Time: ")
                                    .SetTextSize(14)
                                    .SetTextColor(Colors.Gray),
                                new Label()
                                    .BindText(nameof(vm.FormTime), () => vm.FormTime?.ToString("HH:mm") ?? "(none)")
                                    .SetTextSize(14)
                                    .SetTextColor(Colors.Cyan)
                            )
                        ).SetMargin(new Margin(16, 8))
                    )
                    .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                    .SetCornerRadius(8)
                    .SetMargin(new Margin(20, 0, 20, 20)),

                // ============ DatePicker Demos ============
                new Label()
                    .SetText("DatePicker Examples")
                    .SetTextSize(20)
                    .SetTextColor(Colors.Yellow)
                    .SetMargin(new Margin(20, 10, 0, 10)),

                // Basic DatePicker
                CreateSection("Basic DatePicker",
                    new DatePicker()
                        .SetPlaceholder("Select a date...")
                        .BindSelectedDate(nameof(vm.FormDate), () => vm.FormDate, d => vm.FormDate = d)
                        .SetDesiredSize(new Size(220, 40))
                        .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                        .SetCornerRadius(8)
                ),

                // DatePicker with min/max range
                CreateSection("DatePicker with Date Range (2024 only)",
                    new DatePicker()
                        .SetPlaceholder("Pick date in 2024...")
                        .SetMinDate(new DateOnly(2024, 1, 1))
                        .SetMaxDate(new DateOnly(2024, 12, 31))
                        .BindSelectedDate(nameof(vm.SelectedDate), () => vm.SelectedDate, d => vm.SelectedDate = d)
                        .SetDesiredSize(new Size(220, 40))
                        .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                        .SetCornerRadius(8)
                ),

                // DatePicker for birthdate (past dates only)
                CreateSection("Birth Date (Max: Today)",
                    new DatePicker()
                        .SetPlaceholder("Your birth date...")
                        .SetMaxDate(DateOnly.FromDateTime(DateTime.Today))
                        .SetDisplayFormat("dd MMMM yyyy")
                        .SetWeekStart(DayOfWeekStart.Sunday)
                        .BindSelectedDate(nameof(vm.BirthDate), () => vm.BirthDate, d => vm.BirthDate = d)
                        .SetTextColor(Colors.Orange)
                        .SetDesiredSize(new Size(250, 40))
                        .SetBackground(new SolidColorBackground(new Color(60, 40, 20)))
                        .SetCornerRadius(8)
                ),

                // DatePicker for future dates
                CreateSection("Future Appointment (Min: Tomorrow)",
                    new DatePicker()
                        .SetPlaceholder("Select appointment date...")
                        .SetMinDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                        .SetMaxDate(DateOnly.FromDateTime(DateTime.Today.AddMonths(3)))
                        .SetShowTodayButton(false)
                        .BindSelectedDate(nameof(vm.AppointmentDate), () => vm.AppointmentDate, d => vm.AppointmentDate = d)
                        .SetTextColor(Colors.LimeGreen)
                        .SetDesiredSize(new Size(250, 40))
                        .SetBackground(new SolidColorBackground(new Color(30, 50, 30)))
                        .SetCornerRadius(8)
                ),

                // DatePicker with ISO format
                CreateSection("ISO Format (yyyy-MM-dd)",
                    new DatePicker()
                        .SetPlaceholder("Select date...")
                        .SetDisplayFormat("yyyy-MM-dd")
                        .SetDesiredSize(new Size(200, 40))
                        .SetBackground(new SolidColorBackground(new Color(40, 40, 60)))
                        .SetCornerRadius(8)
                ),

                // ============ TimePicker Demos ============
                new Label()
                    .SetText("TimePicker Examples")
                    .SetTextSize(20)
                    .SetTextColor(Colors.Yellow)
                    .SetMargin(new Margin(20, 20, 0, 10)),

                // Basic TimePicker
                CreateSection("Basic TimePicker (24-hour)",
                    new TimePicker()
                        .SetPlaceholder("Select time...")
                        .BindSelectedTime(nameof(vm.FormTime), () => vm.FormTime, t => vm.FormTime = t)
                        .SetDesiredSize(new Size(150, 40))
                        .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                        .SetCornerRadius(8)
                ),

                // TimePicker with 12-hour format
                CreateSection("12-Hour Format (AM/PM)",
                    new TimePicker()
                        .SetPlaceholder("Select time...")
                        .Set24HourFormat(false)
                        .BindSelectedTime(nameof(vm.SelectedTime), () => vm.SelectedTime, t => vm.SelectedTime = t)
                        .SetTextColor(Colors.Cyan)
                        .SetDesiredSize(new Size(180, 40))
                        .SetBackground(new SolidColorBackground(new Color(30, 50, 60)))
                        .SetCornerRadius(8)
                ),

                // TimePicker with 15-minute increments
                CreateSection("15-Minute Increments",
                    new TimePicker()
                        .SetPlaceholder("Meeting time...")
                        .SetMinuteIncrement(15)
                        .BindSelectedTime(nameof(vm.MeetingTime), () => vm.MeetingTime, t => vm.MeetingTime = t)
                        .SetDesiredSize(new Size(150, 40))
                        .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                        .SetCornerRadius(8)
                ),

                // TimePicker with business hours restriction
                CreateSection("Business Hours (9:00 - 17:00)",
                    new TimePicker()
                        .SetPlaceholder("Appointment time...")
                        .SetMinTime(new TimeOnly(9, 0))
                        .SetMaxTime(new TimeOnly(17, 0))
                        .SetMinuteIncrement(30)
                        .BindSelectedTime(nameof(vm.AppointmentTime), () => vm.AppointmentTime, t => vm.AppointmentTime = t)
                        .SetTextColor(Colors.LimeGreen)
                        .SetDesiredSize(new Size(180, 40))
                        .SetBackground(new SolidColorBackground(new Color(30, 50, 30)))
                        .SetCornerRadius(8)
                ),

                // TimePicker with preset time
                CreateSection("Preset Time (Current Time)",
                    new TimePicker()
                        .SetSelectedTime(TimeOnly.FromDateTime(DateTime.Now))
                        .SetMinuteIncrement(5)
                        .SetDesiredSize(new Size(150, 40))
                        .SetBackground(new SolidColorBackground(new Color(50, 40, 50)))
                        .SetCornerRadius(8)
                ),

                // ============ Combined DatePicker + TimePicker ============
                new Label()
                    .SetText("Combined Date & Time")
                    .SetTextSize(20)
                    .SetTextColor(Colors.Yellow)
                    .SetMargin(new Margin(20, 20, 0, 10)),

                CreateSection("Appointment Booking",
                    new HStack(
                        new VStack(
                            new Label()
                                .SetText("Date")
                                .SetTextSize(12)
                                .SetTextColor(Colors.Gray),
                            new DatePicker()
                                .SetPlaceholder("Date...")
                                .SetMinDate(DateOnly.FromDateTime(DateTime.Today))
                                .BindSelectedDate(nameof(vm.AppointmentDate), () => vm.AppointmentDate, d => vm.AppointmentDate = d)
                                .SetDesiredSize(new Size(180, 38))
                                .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                                .SetCornerRadius(6)
                        ).SetMargin(new Margin(0, 0, 16, 0)),
                        new VStack(
                            new Label()
                                .SetText("Time")
                                .SetTextSize(12)
                                .SetTextColor(Colors.Gray),
                            new TimePicker()
                                .SetPlaceholder("Time...")
                                .SetMinuteIncrement(15)
                                .BindSelectedTime(nameof(vm.AppointmentTime), () => vm.AppointmentTime, t => vm.AppointmentTime = t)
                                .SetDesiredSize(new Size(140, 38))
                                .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                                .SetCornerRadius(6)
                        )
                    )
                ),

                // Bottom padding
                new Solid().SetDesiredHeight(50).IgnoreStyling()
            )
        )
        .SetCanScrollHorizontally(false);
    }

    private UiElement CreateSection(string title, UiElement content)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(16)
                .SetTextColor(Colors.LightGray)
                .SetMargin(new Margin(0, 10, 0, 8)),
            new Border()
                .AddChild(
                    new VStack(content)
                        .SetMargin(new Margin(16))
                )
                .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 5))
        ).SetMargin(new Margin(20, 0));
    }
}
