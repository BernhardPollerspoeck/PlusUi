using SkiaSharp;
using System.Globalization;

namespace PlusUi.core;

/// <summary>
/// Calendar overlay for DatePicker with month/year navigation.
/// </summary>
internal partial class DatePickerCalendarOverlay(DatePicker datePicker) : UiElement, IInputControl, IDismissableOverlay
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Container;

    // Layout constants
    private const float HeaderHeight = 40f;
    private const float DayHeaderHeight = 28f;
    private static readonly float DayCellSize = PlusUiDefaults.ItemHeight;
    private const float TodayButtonHeight = 36f;
    private const float CalendarPadding = 8f;
    private const int DaysInWeek = 7;
    private const int MaxWeeksShown = 6;

    internal static readonly float CalendarWidth = DayCellSize * DaysInWeek + CalendarPadding * 2;

    // Current view state
    internal DateOnly DisplayedMonth { get; set; } = new DateOnly(
            datePicker.SelectedDate?.Year ?? DateTime.Today.Year,
            datePicker.SelectedDate?.Month ?? DateTime.Today.Month,
            1);

    // Hit testing results
    private int _hitDayIndex = -1;
    private bool _hitPrevMonth;
    private bool _hitNextMonth;
    private bool _hitTodayButton;
    private bool _hitOnDatePicker;

    // Hover tracking
    internal int _hoveredDayIndex = -1;

    public override void Render(SKCanvas canvas)
    {
        if (!datePicker.IsOpen) return;

        var calendarHeight = CalculateCalendarHeight();
        var calendarRect = datePicker.GetCalendarRect(CalendarWidth, calendarHeight);

        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = datePicker.CalendarBackground,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(calendarRect, datePicker.CornerRadius, datePicker.CornerRadius, bgPaint);

        // Draw border
        using var borderPaint = new SKPaint
        {
            Color = datePicker.TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        canvas.DrawRoundRect(calendarRect, datePicker.CornerRadius, datePicker.CornerRadius, borderPaint);

        // Render calendar content
        RenderHeader(canvas, calendarRect);
        RenderDayHeaders(canvas, calendarRect);
        RenderDays(canvas, calendarRect);

        if (datePicker.ShowTodayButton)
        {
            RenderTodayButton(canvas, calendarRect);
        }
    }

    private float CalculateCalendarHeight()
    {
        var height = HeaderHeight + DayHeaderHeight + (MaxWeeksShown * DayCellSize) + CalendarPadding * 2;
        if (datePicker.ShowTodayButton)
        {
            height += TodayButtonHeight;
        }
        return height;
    }

    private void RenderHeader(SKCanvas canvas, SKRect calendarRect)
    {
        var headerY = calendarRect.Top + CalendarPadding;
        var headerCenterY = headerY + HeaderHeight / 2;

        // Month/Year text
        var monthYearText = DisplayedMonth.ToString("MMMM yyyy", CultureInfo.CurrentCulture);

        datePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        using var textPaint = new SKPaint
        {
            Color = datePicker.TextColor,
            IsAntialias = true
        };

        canvas.DrawText(
            monthYearText,
            calendarRect.Left + CalendarWidth / 2,
            headerCenterY + textHeight / 2 - fontMetrics.Descent,
            SKTextAlign.Center,
            datePicker.Font,
            textPaint);

        // Navigation arrows
        RenderNavigationArrow(canvas, calendarRect.Left + CalendarPadding + 16, headerCenterY, true);
        RenderNavigationArrow(canvas, calendarRect.Right - CalendarPadding - 16, headerCenterY, false);
    }

    private void RenderNavigationArrow(SKCanvas canvas, float centerX, float centerY, bool isLeft)
    {
        using var arrowPaint = new SKPaint
        {
            Color = datePicker.TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var arrowSize = 8f;
        var arrowPath = new SKPath();

        if (isLeft)
        {
            arrowPath.MoveTo(centerX + arrowSize / 2, centerY - arrowSize / 2);
            arrowPath.LineTo(centerX - arrowSize / 2, centerY);
            arrowPath.LineTo(centerX + arrowSize / 2, centerY + arrowSize / 2);
        }
        else
        {
            arrowPath.MoveTo(centerX - arrowSize / 2, centerY - arrowSize / 2);
            arrowPath.LineTo(centerX + arrowSize / 2, centerY);
            arrowPath.LineTo(centerX - arrowSize / 2, centerY + arrowSize / 2);
        }
        arrowPath.Close();

        canvas.DrawPath(arrowPath, arrowPaint);
    }

    private void RenderDayHeaders(SKCanvas canvas, SKRect calendarRect)
    {
        var dayHeaderY = calendarRect.Top + CalendarPadding + HeaderHeight;
        var dayNames = GetDayNames();

        using var headerPaint = new SKPaint
        {
            Color = PlusUiDefaults.TextPlaceholder,
            IsAntialias = true
        };

        datePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        for (int i = 0; i < DaysInWeek; i++)
        {
            var cellX = calendarRect.Left + CalendarPadding + (i * DayCellSize);
            var cellCenterX = cellX + DayCellSize / 2;
            var cellCenterY = dayHeaderY + DayHeaderHeight / 2;

            canvas.DrawText(
                dayNames[i],
                cellCenterX,
                cellCenterY + textHeight / 2 - fontMetrics.Descent,
                SKTextAlign.Center,
                datePicker.Font,
                headerPaint);
        }
    }

    private string[] GetDayNames()
    {
        var culture = CultureInfo.CurrentCulture;
        var dayNames = new string[7];
        var startDay = datePicker.WeekStart == DayOfWeekStart.Monday ? DayOfWeek.Monday : DayOfWeek.Sunday;

        for (int i = 0; i < 7; i++)
        {
            var day = (DayOfWeek)(((int)startDay + i) % 7);
            dayNames[i] = culture.DateTimeFormat.AbbreviatedDayNames[(int)day].Substring(0, 2);
        }

        return dayNames;
    }

    private void RenderDays(SKCanvas canvas, SKRect calendarRect)
    {
        var daysStartY = calendarRect.Top + CalendarPadding + HeaderHeight + DayHeaderHeight;
        var firstDayOfMonth = new DateOnly(DisplayedMonth.Year, DisplayedMonth.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(DisplayedMonth.Year, DisplayedMonth.Month);

        // Calculate starting position based on day of week
        var startDayOfWeek = datePicker.WeekStart == DayOfWeekStart.Monday
            ? ((int)firstDayOfMonth.DayOfWeek + 6) % 7
            : (int)firstDayOfMonth.DayOfWeek;

        var today = DateOnly.FromDateTime(DateTime.Today);

        using var dayPaint = new SKPaint
        {
            Color = datePicker.TextColor,
            IsAntialias = true
        };

        using var disabledPaint = new SKPaint
        {
            Color = PlusUiDefaults.BorderColor,
            IsAntialias = true
        };

        datePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var currentDate = new DateOnly(DisplayedMonth.Year, DisplayedMonth.Month, day);
            var dayIndex = startDayOfWeek + day - 1;
            var row = dayIndex / DaysInWeek;
            var col = dayIndex % DaysInWeek;

            var cellX = calendarRect.Left + CalendarPadding + (col * DayCellSize);
            var cellY = daysStartY + (row * DayCellSize);
            var cellRect = new SKRect(cellX, cellY, cellX + DayCellSize, cellY + DayCellSize);

            var isSelected = datePicker.SelectedDate == currentDate;
            var isToday = currentDate == today;
            var isHovered = dayIndex == _hoveredDayIndex;
            var isSelectable = datePicker.IsDateInRange(currentDate);

            // Background for selected/hovered day
            if (isSelected)
            {
                using var selectedPaint = new SKPaint
                {
                    Color = datePicker.SelectedBackground,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawRoundRect(cellRect, 4, 4, selectedPaint);
            }
            else if (isHovered && isSelectable)
            {
                using var hoverPaint = new SKPaint
                {
                    Color = datePicker.HoverBackground,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawRoundRect(cellRect, 4, 4, hoverPaint);
            }

            // Today border
            if (isToday && !isSelected)
            {
                using var todayPaint = new SKPaint
                {
                    Color = datePicker.TodayBorderColor,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1.5f
                };
                canvas.DrawRoundRect(cellRect.Left + 2, cellRect.Top + 2,
                    cellRect.Width - 4, cellRect.Height - 4, 4, 4, todayPaint);
            }

            // Day number
            var paint = isSelectable ? dayPaint : disabledPaint;
            canvas.DrawText(
                day.ToString(),
                cellX + DayCellSize / 2,
                cellY + DayCellSize / 2 + textHeight / 2 - fontMetrics.Descent,
                SKTextAlign.Center,
                datePicker.Font,
                paint);
        }
    }

    private void RenderTodayButton(SKCanvas canvas, SKRect calendarRect)
    {
        var buttonY = calendarRect.Bottom - TodayButtonHeight - CalendarPadding;
        var buttonRect = new SKRect(
            calendarRect.Left + CalendarPadding,
            buttonY,
            calendarRect.Right - CalendarPadding,
            buttonY + TodayButtonHeight - 4);

        using var buttonPaint = new SKPaint
        {
            Color = datePicker.HoverBackground,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(buttonRect, 4, 4, buttonPaint);

        using var textPaint = new SKPaint
        {
            Color = datePicker.TextColor,
            IsAntialias = true
        };

        datePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        canvas.DrawText(
            "Today",
            buttonRect.MidX,
            buttonRect.MidY + textHeight / 2 - fontMetrics.Descent,
            SKTextAlign.Center,
            datePicker.Font,
            textPaint);
    }

    public override UiElement? HitTest(Point point)
    {
        if (!datePicker.IsOpen) return null;

        ResetHitState();

        var calendarHeight = CalculateCalendarHeight();
        var calendarRect = datePicker.GetCalendarRect(CalendarWidth, calendarHeight);

        // Check if inside calendar
        if (point.X >= calendarRect.Left && point.X <= calendarRect.Right &&
            point.Y >= calendarRect.Top && point.Y <= calendarRect.Bottom)
        {
            // Check header navigation arrows
            var headerY = calendarRect.Top + CalendarPadding;
            if (point.Y >= headerY && point.Y <= headerY + HeaderHeight)
            {
                var arrowLeftX = calendarRect.Left + CalendarPadding + 16;
                var arrowRightX = calendarRect.Right - CalendarPadding - 16;

                if (Math.Abs(point.X - arrowLeftX) < 16)
                {
                    _hitPrevMonth = true;
                    return this;
                }
                if (Math.Abs(point.X - arrowRightX) < 16)
                {
                    _hitNextMonth = true;
                    return this;
                }
            }

            // Check Today button
            if (datePicker.ShowTodayButton)
            {
                var buttonY = calendarRect.Bottom - TodayButtonHeight - CalendarPadding;
                if (point.Y >= buttonY && point.Y <= buttonY + TodayButtonHeight)
                {
                    _hitTodayButton = true;
                    return this;
                }
            }

            // Check day cells
            var daysStartY = calendarRect.Top + CalendarPadding + HeaderHeight + DayHeaderHeight;
            if (point.Y >= daysStartY)
            {
                var relativeX = point.X - calendarRect.Left - CalendarPadding;
                var relativeY = point.Y - daysStartY;

                var col = (int)(relativeX / DayCellSize);
                var row = (int)(relativeY / DayCellSize);

                if (col >= 0 && col < DaysInWeek && row >= 0 && row < MaxWeeksShown)
                {
                    var dayIndex = row * DaysInWeek + col;
                    var day = GetDayFromIndex(dayIndex);

                    if (day.HasValue)
                    {
                        _hitDayIndex = dayIndex;
                        _hoveredDayIndex = dayIndex;
                        return this;
                    }
                }
            }

            // Inside calendar but not on any interactive element
            return this;
        }

        // Clear hover
        _hoveredDayIndex = -1;

        // Check if on DatePicker itself
        if (point.X >= datePicker.Position.X && point.X <= datePicker.Position.X + datePicker.ElementSize.Width &&
            point.Y >= datePicker.Position.Y && point.Y <= datePicker.Position.Y + datePicker.ElementSize.Height)
        {
            _hitOnDatePicker = true;
            return this;
        }

        return null;
    }

    private void ResetHitState()
    {
        _hitDayIndex = -1;
        _hitPrevMonth = false;
        _hitNextMonth = false;
        _hitTodayButton = false;
        _hitOnDatePicker = false;
    }

    private int? GetDayFromIndex(int dayIndex)
    {
        var firstDayOfMonth = new DateOnly(DisplayedMonth.Year, DisplayedMonth.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(DisplayedMonth.Year, DisplayedMonth.Month);

        var startDayOfWeek = datePicker.WeekStart == DayOfWeekStart.Monday
            ? ((int)firstDayOfMonth.DayOfWeek + 6) % 7
            : (int)firstDayOfMonth.DayOfWeek;

        var day = dayIndex - startDayOfWeek + 1;
        if (day >= 1 && day <= daysInMonth)
        {
            return day;
        }
        return null;
    }

    public void InvokeCommand()
    {
        if (_hitOnDatePicker)
        {
            Dismiss();
            return;
        }

        if (_hitPrevMonth)
        {
            DisplayedMonth = DisplayedMonth.AddMonths(-1);
            return;
        }

        if (_hitNextMonth)
        {
            DisplayedMonth = DisplayedMonth.AddMonths(1);
            return;
        }

        if (_hitTodayButton)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (datePicker.IsDateInRange(today))
            {
                datePicker.SetSelectedDate(today);
                datePicker.InvokeSetters();
            }
            Dismiss();
            return;
        }

        if (_hitDayIndex >= 0)
        {
            var day = GetDayFromIndex(_hitDayIndex);
            if (day.HasValue)
            {
                var selectedDate = new DateOnly(DisplayedMonth.Year, DisplayedMonth.Month, day.Value);
                if (datePicker.IsDateInRange(selectedDate))
                {
                    datePicker.SetSelectedDate(selectedDate);
                    datePicker.InvokeSetters();
                    Dismiss();
                }
            }
        }
    }

    public void Dismiss()
    {
        datePicker.SetIsOpen(false);
    }
}
