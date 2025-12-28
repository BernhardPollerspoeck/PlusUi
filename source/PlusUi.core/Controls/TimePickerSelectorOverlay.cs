using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Time selector overlay with hour and minute columns.
/// </summary>
internal class TimePickerSelectorOverlay : UiElement, IInputControl, IDismissableOverlay, IScrollableControl
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Container;

    private readonly TimePicker _timePicker;

    // Layout constants
    private const float ColumnWidth = 60f;
    private const float AmPmColumnWidth = 50f;
    private const float ItemHeight = 32f;
    private const float MaxVisibleItems = 6;
    private const float SelectorPadding = 8f;
    private const float HeaderHeight = 28f;
    private const float ScrollSpeed = 1f;

    private static readonly float SelectorHeight = HeaderHeight + (ItemHeight * MaxVisibleItems) + SelectorPadding * 2;

    // Hit testing results
    private int _hitHourIndex = -1;
    private int _hitMinuteIndex = -1;
    private int _hitAmPmIndex = -1; // 0 = AM, 1 = PM
    private bool _hitOnTimePicker;

    // Hover tracking
    internal int _hoveredHourIndex = -1;
    internal int _hoveredMinuteIndex = -1;
    internal int _hoveredAmPmIndex = -1;

    // Scroll offsets for columns
    private float _hourScrollOffset;
    private float _minuteScrollOffset;

    // Track which column mouse is over for scrolling
    private int _activeScrollColumn = -1; // 0 = hour, 1 = minute

    // IScrollableControl
    public bool IsScrolling { get; set; }

    public TimePickerSelectorOverlay(TimePicker timePicker)
    {
        _timePicker = timePicker;

        // Initialize scroll to show selected time
        if (timePicker.SelectedTime.HasValue)
        {
            var hours = timePicker.GetAvailableHours().ToList();
            var minutes = timePicker.GetAvailableMinutes().ToList();

            var selectedHour = timePicker.Is24HourFormat
                ? timePicker.SelectedTime.Value.Hour
                : (timePicker.SelectedTime.Value.Hour % 12 == 0 ? 12 : timePicker.SelectedTime.Value.Hour % 12);

            var hourIndex = hours.IndexOf(selectedHour);
            if (hourIndex >= 0)
            {
                _hourScrollOffset = Math.Max(0, (hourIndex - 2) * ItemHeight);
            }

            var minuteIndex = minutes.IndexOf(timePicker.SelectedTime.Value.Minute);
            if (minuteIndex >= 0)
            {
                _minuteScrollOffset = Math.Max(0, (minuteIndex - 2) * ItemHeight);
            }
        }
    }

    private float CalculateSelectorWidth()
    {
        var width = ColumnWidth * 2 + SelectorPadding * 3;
        if (!_timePicker.Is24HourFormat)
        {
            width += AmPmColumnWidth + SelectorPadding;
        }
        return width;
    }

    public override void Render(SKCanvas canvas)
    {
        if (!_timePicker.IsOpen) return;

        var selectorWidth = CalculateSelectorWidth();
        var selectorRect = _timePicker.GetSelectorRect(selectorWidth, SelectorHeight);

        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = _timePicker.SelectorBackground,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(selectorRect, _timePicker.CornerRadius, _timePicker.CornerRadius, bgPaint);

        // Draw border
        using var borderPaint = new SKPaint
        {
            Color = _timePicker.TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        canvas.DrawRoundRect(selectorRect, _timePicker.CornerRadius, _timePicker.CornerRadius, borderPaint);

        // Render columns
        RenderHeaders(canvas, selectorRect);
        RenderHourColumn(canvas, selectorRect);
        RenderMinuteColumn(canvas, selectorRect);

        if (!_timePicker.Is24HourFormat)
        {
            RenderAmPmColumn(canvas, selectorRect);
        }
    }

    private void RenderHeaders(SKCanvas canvas, SKRect selectorRect)
    {
        using var headerPaint = new SKPaint
        {
            Color = new SKColor(180, 180, 180),
            IsAntialias = true
        };

        _timePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;
        var headerY = selectorRect.Top + SelectorPadding + HeaderHeight / 2 + textHeight / 2 - fontMetrics.Descent;

        // Hour header
        var hourX = selectorRect.Left + SelectorPadding + ColumnWidth / 2;
        canvas.DrawText("Hour", hourX, headerY, SKTextAlign.Center, _timePicker.Font, headerPaint);

        // Minute header
        var minuteX = selectorRect.Left + SelectorPadding * 2 + ColumnWidth + ColumnWidth / 2;
        canvas.DrawText("Min", minuteX, headerY, SKTextAlign.Center, _timePicker.Font, headerPaint);

        // AM/PM header
        if (!_timePicker.Is24HourFormat)
        {
            var ampmX = selectorRect.Left + SelectorPadding * 3 + ColumnWidth * 2 + AmPmColumnWidth / 2;
            canvas.DrawText("", ampmX, headerY, SKTextAlign.Center, _timePicker.Font, headerPaint);
        }
    }

    private void RenderHourColumn(SKCanvas canvas, SKRect selectorRect)
    {
        var columnX = selectorRect.Left + SelectorPadding;
        var columnTop = selectorRect.Top + SelectorPadding + HeaderHeight;
        var columnBottom = selectorRect.Bottom - SelectorPadding;

        // Clip to column area
        canvas.Save();
        canvas.ClipRect(new SKRect(columnX, columnTop, columnX + ColumnWidth, columnBottom));

        var hours = _timePicker.GetAvailableHours().ToList();
        var selectedHour = _timePicker.SelectedTime?.Hour;
        if (!_timePicker.Is24HourFormat && selectedHour.HasValue)
        {
            selectedHour = selectedHour.Value % 12;
            if (selectedHour == 0) selectedHour = 12;
        }

        RenderColumn(canvas, columnX, columnTop - _hourScrollOffset, ColumnWidth, hours, selectedHour, _hoveredHourIndex, h => h.ToString("00"));

        canvas.Restore();
    }

    private void RenderMinuteColumn(SKCanvas canvas, SKRect selectorRect)
    {
        var columnX = selectorRect.Left + SelectorPadding * 2 + ColumnWidth;
        var columnTop = selectorRect.Top + SelectorPadding + HeaderHeight;
        var columnBottom = selectorRect.Bottom - SelectorPadding;

        // Clip to column area
        canvas.Save();
        canvas.ClipRect(new SKRect(columnX, columnTop, columnX + ColumnWidth, columnBottom));

        var minutes = _timePicker.GetAvailableMinutes().ToList();
        var selectedMinute = _timePicker.SelectedTime?.Minute;

        RenderColumn(canvas, columnX, columnTop - _minuteScrollOffset, ColumnWidth, minutes, selectedMinute, _hoveredMinuteIndex, m => m.ToString("00"));

        canvas.Restore();
    }

    private void RenderAmPmColumn(SKCanvas canvas, SKRect selectorRect)
    {
        var columnX = selectorRect.Left + SelectorPadding * 3 + ColumnWidth * 2;
        var columnTop = selectorRect.Top + SelectorPadding + HeaderHeight;

        var isAm = _timePicker.SelectedTime?.Hour < 12;
        var isPm = _timePicker.SelectedTime?.Hour >= 12;

        _timePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        // AM item
        var amRect = new SKRect(columnX, columnTop, columnX + AmPmColumnWidth, columnTop + ItemHeight);
        if (isAm && _timePicker.SelectedTime.HasValue)
        {
            using var selectedPaint = new SKPaint
            {
                Color = _timePicker.SelectedBackground,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRoundRect(amRect, 4, 4, selectedPaint);
        }
        else if (_hoveredAmPmIndex == 0)
        {
            using var hoverPaint = new SKPaint
            {
                Color = _timePicker.HoverBackground,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRoundRect(amRect, 4, 4, hoverPaint);
        }

        using var textPaint = new SKPaint
        {
            Color = _timePicker.TextColor,
            IsAntialias = true
        };
        canvas.DrawText("AM", amRect.MidX, amRect.MidY + textHeight / 2 - fontMetrics.Descent,
            SKTextAlign.Center, _timePicker.Font, textPaint);

        // PM item
        var pmRect = new SKRect(columnX, columnTop + ItemHeight, columnX + AmPmColumnWidth, columnTop + ItemHeight * 2);
        if (isPm && _timePicker.SelectedTime.HasValue)
        {
            using var selectedPaint = new SKPaint
            {
                Color = _timePicker.SelectedBackground,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRoundRect(pmRect, 4, 4, selectedPaint);
        }
        else if (_hoveredAmPmIndex == 1)
        {
            using var hoverPaint = new SKPaint
            {
                Color = _timePicker.HoverBackground,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRoundRect(pmRect, 4, 4, hoverPaint);
        }

        canvas.DrawText("PM", pmRect.MidX, pmRect.MidY + textHeight / 2 - fontMetrics.Descent,
            SKTextAlign.Center, _timePicker.Font, textPaint);
    }

    private void RenderColumn(SKCanvas canvas, float columnX, float columnTop, float columnWidth,
        List<int> items, int? selectedValue, int hoveredIndex, Func<int, string> formatter)
    {
        _timePicker.Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        for (int i = 0; i < items.Count; i++)
        {
            var value = items[i];
            var itemY = columnTop + (i * ItemHeight);
            var itemRect = new SKRect(columnX, itemY, columnX + columnWidth, itemY + ItemHeight);

            var isSelected = selectedValue == value;
            var isHovered = i == hoveredIndex;

            // Background for selected/hovered
            if (isSelected)
            {
                using var selectedPaint = new SKPaint
                {
                    Color = _timePicker.SelectedBackground,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawRoundRect(itemRect, 4, 4, selectedPaint);
            }
            else if (isHovered)
            {
                using var hoverPaint = new SKPaint
                {
                    Color = _timePicker.HoverBackground,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawRoundRect(itemRect, 4, 4, hoverPaint);
            }

            // Text
            using var textPaint = new SKPaint
            {
                Color = _timePicker.TextColor,
                IsAntialias = true
            };
            canvas.DrawText(
                formatter(value),
                itemRect.MidX,
                itemRect.MidY + textHeight / 2 - fontMetrics.Descent,
                SKTextAlign.Center,
                _timePicker.Font,
                textPaint);
        }
    }

    public override UiElement? HitTest(Point point)
    {
        if (!_timePicker.IsOpen) return null;

        ResetHitState();

        var selectorWidth = CalculateSelectorWidth();
        var selectorRect = _timePicker.GetSelectorRect(selectorWidth, SelectorHeight);

        // Check if inside selector
        if (point.X >= selectorRect.Left && point.X <= selectorRect.Right &&
            point.Y >= selectorRect.Top && point.Y <= selectorRect.Bottom)
        {
            var columnTop = selectorRect.Top + SelectorPadding + HeaderHeight;

            // Check hour column
            var hourColumnX = selectorRect.Left + SelectorPadding;
            if (point.X >= hourColumnX && point.X <= hourColumnX + ColumnWidth && point.Y >= columnTop)
            {
                _activeScrollColumn = 0;
                var relativeY = point.Y - columnTop + _hourScrollOffset;
                _hitHourIndex = (int)(relativeY / ItemHeight);
                _hoveredHourIndex = _hitHourIndex;
                return this;
            }

            // Check minute column
            var minuteColumnX = selectorRect.Left + SelectorPadding * 2 + ColumnWidth;
            if (point.X >= minuteColumnX && point.X <= minuteColumnX + ColumnWidth && point.Y >= columnTop)
            {
                _activeScrollColumn = 1;
                var relativeY = point.Y - columnTop + _minuteScrollOffset;
                _hitMinuteIndex = (int)(relativeY / ItemHeight);
                _hoveredMinuteIndex = _hitMinuteIndex;
                return this;
            }

            // Check AM/PM column
            if (!_timePicker.Is24HourFormat)
            {
                var ampmColumnX = selectorRect.Left + SelectorPadding * 3 + ColumnWidth * 2;
                if (point.X >= ampmColumnX && point.X <= ampmColumnX + AmPmColumnWidth && point.Y >= columnTop)
                {
                    _activeScrollColumn = -1; // AM/PM doesn't scroll
                    var relativeY = point.Y - columnTop;
                    _hitAmPmIndex = (int)(relativeY / ItemHeight);
                    if (_hitAmPmIndex > 1) _hitAmPmIndex = -1;
                    _hoveredAmPmIndex = _hitAmPmIndex;
                    return this;
                }
            }

            // Inside selector but not on any column (header area)
            _activeScrollColumn = -1;
            return this;
        }

        // Clear hover and scroll column
        _hoveredHourIndex = -1;
        _hoveredMinuteIndex = -1;
        _hoveredAmPmIndex = -1;
        _activeScrollColumn = -1;

        // Check if on TimePicker itself
        if (point.X >= _timePicker.Position.X && point.X <= _timePicker.Position.X + _timePicker.ElementSize.Width &&
            point.Y >= _timePicker.Position.Y && point.Y <= _timePicker.Position.Y + _timePicker.ElementSize.Height)
        {
            _hitOnTimePicker = true;
            return this;
        }

        return null;
    }

    private void ResetHitState()
    {
        _hitHourIndex = -1;
        _hitMinuteIndex = -1;
        _hitAmPmIndex = -1;
        _hitOnTimePicker = false;
    }

    public void InvokeCommand()
    {
        if (_hitOnTimePicker)
        {
            Dismiss();
            return;
        }

        var hours = _timePicker.GetAvailableHours().ToList();
        var minutes = _timePicker.GetAvailableMinutes().ToList();

        // Get current values or defaults
        var currentHour = _timePicker.SelectedTime?.Hour ?? 12;
        var currentMinute = _timePicker.SelectedTime?.Minute ?? 0;
        var isCurrentPm = currentHour >= 12;

        if (_hitHourIndex >= 0 && _hitHourIndex < hours.Count)
        {
            var selectedHour = hours[_hitHourIndex];

            if (!_timePicker.Is24HourFormat)
            {
                // Convert 12-hour to 24-hour
                if (isCurrentPm)
                {
                    selectedHour = selectedHour == 12 ? 12 : selectedHour + 12;
                }
                else
                {
                    selectedHour = selectedHour == 12 ? 0 : selectedHour;
                }
            }

            var newTime = new TimeOnly(selectedHour, currentMinute);
            if (_timePicker.IsTimeInRange(newTime))
            {
                _timePicker.SetSelectedTime(newTime);
                _timePicker.InvokeSetters();
            }
        }

        if (_hitMinuteIndex >= 0 && _hitMinuteIndex < minutes.Count)
        {
            var selectedMinute = minutes[_hitMinuteIndex];
            var newTime = new TimeOnly(currentHour, selectedMinute);
            if (_timePicker.IsTimeInRange(newTime))
            {
                _timePicker.SetSelectedTime(newTime);
                _timePicker.InvokeSetters();
            }
        }

        if (_hitAmPmIndex >= 0)
        {
            var wantsPm = _hitAmPmIndex == 1;
            int newHour;

            if (wantsPm && !isCurrentPm)
            {
                // Switch to PM
                newHour = currentHour + 12;
                if (newHour >= 24) newHour -= 12;
            }
            else if (!wantsPm && isCurrentPm)
            {
                // Switch to AM
                newHour = currentHour - 12;
                if (newHour < 0) newHour += 12;
            }
            else
            {
                newHour = currentHour;
            }

            var newTime = new TimeOnly(newHour, currentMinute);
            if (_timePicker.IsTimeInRange(newTime))
            {
                _timePicker.SetSelectedTime(newTime);
                _timePicker.InvokeSetters();
            }
        }

        // Don't dismiss on selection - allow multiple selections
        // User can click outside to dismiss
    }

    public void Dismiss()
    {
        _timePicker.SetIsOpen(false);
    }

    public void HandleScroll(float deltaX, float deltaY)
    {
        var hours = _timePicker.GetAvailableHours().ToList();
        var minutes = _timePicker.GetAvailableMinutes().ToList();

        // Calculate max scroll for each column
        var visibleHeight = ItemHeight * MaxVisibleItems;
        var maxHourScroll = Math.Max(0, hours.Count * ItemHeight - visibleHeight);
        var maxMinuteScroll = Math.Max(0, minutes.Count * ItemHeight - visibleHeight);

        if (_activeScrollColumn == 0)
        {
            // Scroll hour column
            _hourScrollOffset = Math.Clamp(_hourScrollOffset + deltaY * ScrollSpeed, 0, maxHourScroll);
        }
        else if (_activeScrollColumn == 1)
        {
            // Scroll minute column
            _minuteScrollOffset = Math.Clamp(_minuteScrollOffset + deltaY * ScrollSpeed, 0, maxMinuteScroll);
        }
    }

    /// <summary>
    /// Scrolls to make the selected time visible. Called when time changes via keyboard.
    /// </summary>
    public void ScrollToSelection()
    {
        if (!_timePicker.SelectedTime.HasValue) return;

        var hours = _timePicker.GetAvailableHours().ToList();
        var minutes = _timePicker.GetAvailableMinutes().ToList();

        var selectedHour = _timePicker.Is24HourFormat
            ? _timePicker.SelectedTime.Value.Hour
            : (_timePicker.SelectedTime.Value.Hour % 12 == 0 ? 12 : _timePicker.SelectedTime.Value.Hour % 12);

        var hourIndex = hours.IndexOf(selectedHour);
        if (hourIndex >= 0)
        {
            var visibleHeight = ItemHeight * MaxVisibleItems;
            var maxScroll = Math.Max(0, hours.Count * ItemHeight - visibleHeight);
            // Center the selected item, clamped to valid range
            _hourScrollOffset = Math.Clamp((hourIndex - 2) * ItemHeight, 0, maxScroll);
        }

        var minuteIndex = minutes.IndexOf(_timePicker.SelectedTime.Value.Minute);
        if (minuteIndex >= 0)
        {
            var visibleHeight = ItemHeight * MaxVisibleItems;
            var maxScroll = Math.Max(0, minutes.Count * ItemHeight - visibleHeight);
            _minuteScrollOffset = Math.Clamp((minuteIndex - 2) * ItemHeight, 0, maxScroll);
        }
    }
}
