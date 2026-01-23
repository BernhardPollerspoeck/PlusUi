using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.UiPropGen;
using PlusUi.core.Services;
using SkiaSharp;
using System.Globalization;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A date picker control with calendar popup for date selection.
/// Supports date range restrictions, custom display formats, and two-way binding.
/// </summary>
/// <example>
/// <code>
/// new DatePicker()
///     .SetPlaceholder("Select date...")
///     .SetDisplayFormat("dd.MM.yyyy")
///     .SetMinDate(new DateOnly(1900, 1, 1))
///     .SetMaxDate(DateOnly.FromDateTime(DateTime.Today))
///     .BindSelectedDate(nameof(vm.BirthDate), () => vm.BirthDate, v => vm.BirthDate = v);
/// </code>
/// </example>
[GenerateShadowMethods]
[UiPropGenPadding]
[UiPropGenPlaceholder]
[UiPropGenPlaceholderColor]
public partial class DatePicker : UiElement, IInputControl, ITextInputControl, IHoverableControl, IFocusable, IKeyboardInputHandler
{
    private const float ArrowSize = 8f;
    private IOverlayService? _overlayService;
    private DatePickerCalendarOverlay? _calendarOverlay;
    private IPlatformService? _platformService;
    private bool _isSelected;
    private DateTime _selectionTime;
    private string _inputBuffer = string.Empty;

    #region SelectedDate
    private Action<DateOnly?>? _onSelectedDateChanged;

    internal DateOnly? SelectedDate
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            _inputBuffer = string.Empty;
            InvalidateMeasure();
        }
    }

    public DatePicker SetSelectedDate(DateOnly? date)
    {
        SelectedDate = date;
        return this;
    }

    /// <summary>
    /// Sets a callback that is invoked when SelectedDate changes.
    /// </summary>
    public DatePicker SetOnSelectedDateChanged(Action<DateOnly?> callback)
    {
        _onSelectedDateChanged = callback;
        return this;
    }

    public DatePicker BindSelectedDate(Expression<Func<DateOnly?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedDate = getter());
        return this;
    }

    public DatePicker BindSelectedDate(Expression<Func<DateOnly?>> propertyExpression, Action<DateOnly?> setter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedDate = getter());
        foreach (var segment in path)
        {
            RegisterSetter<DateOnly?>(segment, setter);
        }
        RegisterSetter<DateOnly?>(nameof(SelectedDate), setter);
        return this;
    }

    #endregion

    #region MinDate
    internal DateOnly? MinDate { get; set; }

    public DatePicker SetMinDate(DateOnly? minDate)
    {
        MinDate = minDate;
        return this;
    }

    public DatePicker BindMinDate(Expression<Func<DateOnly?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MinDate = getter());
        return this;
    }
    #endregion

    #region MaxDate
    internal DateOnly? MaxDate { get; set; }

    public DatePicker SetMaxDate(DateOnly? maxDate)
    {
        MaxDate = maxDate;
        return this;
    }

    public DatePicker BindMaxDate(Expression<Func<DateOnly?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MaxDate = getter());
        return this;
    }
    #endregion

    #region DisplayFormat
    internal string DisplayFormat { get; set; } = "dd.MM.yyyy";

    public DatePicker SetDisplayFormat(string format)
    {
        DisplayFormat = format;
        InvalidateMeasure();
        return this;
    }

    public DatePicker BindDisplayFormat(Expression<Func<string>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DisplayFormat = getter());
        return this;
    }
    #endregion

    #region TextColor
    internal SKColor TextColor
    {
        get => field;
        set
        {
            field = value;
            UpdatePaint();
        }
    } = PlusUiDefaults.TextPrimary;

    public DatePicker SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }

    public DatePicker BindTextColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextColor = getter());
        return this;
    }
    #endregion

    #region TextSize
    internal float TextSize
    {
        get => field;
        set
        {
            field = value;
            UpdatePaint();
            InvalidateMeasure();
        }
    } = PlusUiDefaults.FontSize;

    public DatePicker SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public DatePicker BindTextSize(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextSize = getter());
        return this;
    }
    #endregion

    #region FontFamily
    internal string? FontFamily
    {
        get => field;
        set
        {
            field = value;
            UpdatePaint();
            InvalidateMeasure();
        }
    }

    public DatePicker SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        return this;
    }

    public DatePicker BindFontFamily(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FontFamily = getter());
        return this;
    }
    #endregion

    #region WeekStart
    internal DayOfWeekStart WeekStart { get; set; } = DayOfWeekStart.Monday;

    public DatePicker SetWeekStart(DayOfWeekStart weekStart)
    {
        WeekStart = weekStart;
        return this;
    }

    public DatePicker BindWeekStart(Expression<Func<DayOfWeekStart>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => WeekStart = getter());
        return this;
    }
    #endregion

    #region ShowTodayButton
    internal bool ShowTodayButton { get; set; } = true;

    public DatePicker SetShowTodayButton(bool show)
    {
        ShowTodayButton = show;
        return this;
    }

    public DatePicker BindShowTodayButton(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ShowTodayButton = getter());
        return this;
    }
    #endregion

    #region CalendarBackground
    internal SKColor CalendarBackground { get; set; } = PlusUiDefaults.BackgroundPrimary;

    public DatePicker SetCalendarBackground(SKColor color)
    {
        CalendarBackground = color;
        return this;
    }

    public DatePicker BindCalendarBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => CalendarBackground = getter());
        return this;
    }
    #endregion

    #region HoverBackground
    internal SKColor HoverBackground { get; set; } = PlusUiDefaults.BackgroundHover;

    public DatePicker SetHoverBackground(SKColor color)
    {
        HoverBackground = color;
        return this;
    }

    public DatePicker BindHoverBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HoverBackground = getter());
        return this;
    }
    #endregion

    #region SelectedBackground
    internal SKColor SelectedBackground { get; set; } = PlusUiDefaults.BackgroundSelected;

    public DatePicker SetSelectedBackground(SKColor color)
    {
        SelectedBackground = color;
        return this;
    }

    public DatePicker BindSelectedBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedBackground = getter());
        return this;
    }
    #endregion

    #region TodayBorderColor
    internal SKColor TodayBorderColor { get; set; } = PlusUiDefaults.AccentPrimary;

    public DatePicker SetTodayBorderColor(SKColor color)
    {
        TodayBorderColor = color;
        return this;
    }

    public DatePicker BindTodayBorderColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TodayBorderColor = getter());
        return this;
    }
    #endregion

    #region IsOpen
    internal bool IsOpen
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;

            if (value)
            {
                RegisterCalendarOverlay();
            }
            else
            {
                UnregisterCalendarOverlay();
            }

            InvalidateMeasure();
        }
    }

    public DatePicker SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
        return this;
    }

    public DatePicker BindIsOpen(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsOpen = getter());
        return this;
    }

    private void RegisterCalendarOverlay()
    {
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null) return;

        _calendarOverlay = new DatePickerCalendarOverlay(this);
        _overlayService.RegisterOverlay(_calendarOverlay);
    }

    private void UnregisterCalendarOverlay()
    {
        if (_overlayService != null && _calendarOverlay != null)
        {
            _overlayService.UnregisterOverlay(_calendarOverlay);
            _calendarOverlay = null;
        }
    }
    #endregion

    #region IsHovered (IHoverableControl)
    public bool IsHovered { get; set; }
    #endregion

    private SKFont _font;
    private SKPaint _paint;

    // Internal accessors for DatePickerCalendarOverlay
    internal SKFont Font => _font;
    internal SKPaint Paint => _paint;

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.DatePicker;

    public DatePicker()
    {
        SetDesiredSize(new Size(200, 40));
        SetHighContrastBackground(PlusUiDefaults.HcInputBackground);
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
        PlaceholderColor = PlusUiDefaults.TextPlaceholder;
        UpdatePaint();
    }

    [MemberNotNull(nameof(_font), nameof(_paint))]
    private void UpdatePaint()
    {
        // Release old paint if exists (for property changes)
        if (_paint is not null && _font is not null)
        {
            PaintRegistry.Release(_paint, _font);
        }

        // Get or create paint from registry
        var typeface = string.IsNullOrEmpty(FontFamily)
            ? SKTypeface.FromFamilyName(null)
            : SKTypeface.FromFamilyName(FontFamily);

        (_paint, _font) = PaintRegistry.GetOrCreate(
            color: TextColor,
            size: TextSize,
            typeface: typeface
        );
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Placeholder ?? "Date picker";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        if (!string.IsNullOrEmpty(AccessibilityValue))
        {
            return AccessibilityValue;
        }
        return SelectedDate?.ToString(DisplayFormat, System.Globalization.CultureInfo.CurrentCulture);
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsOpen)
        {
            traits |= AccessibilityTrait.Expanded;
        }
        traits |= AccessibilityTrait.HasPopup;
        return traits;
    }

    #region IFocusable
    bool IFocusable.IsFocusable => IsFocusable;
    int? IFocusable.TabIndex => TabIndex;
    bool IFocusable.TabStop => TabStop;
    bool IFocusable.IsFocused
    {
        get => IsFocused;
        set => IsFocused = value;
    }
    void IFocusable.OnFocus() => OnFocus();
    void IFocusable.OnBlur()
    {
        OnBlur();
        // Close picker when losing focus
        IsOpen = false;
    }
    #endregion

    #region IKeyboardInputHandler
    /// <inheritdoc />
    public bool HandleKeyboardInput(PlusKey key)
    {
        if (!IsOpen)
        {
            // When closed, Enter/Space opens the picker
            if (key == PlusKey.Enter || key == PlusKey.Space)
            {
                IsOpen = true;
                return true;
            }
            return false;
        }

        // When open, forward to calendar overlay
        if (_calendarOverlay == null) return false;

        switch (key)
        {
            case PlusKey.Escape:
                IsOpen = false;
                return true;
            case PlusKey.ArrowLeft:
                NavigateDay(-1);
                return true;
            case PlusKey.ArrowRight:
                NavigateDay(1);
                return true;
            case PlusKey.ArrowUp:
                NavigateDay(-7);
                return true;
            case PlusKey.ArrowDown:
                NavigateDay(7);
                return true;
            case PlusKey.Enter:
            case PlusKey.Space:
                SelectHoveredDay();
                return true;
            default:
                return false;
        }
    }

    private void NavigateDay(int offset)
    {
        if (_calendarOverlay == null) return;

        // Initialize hover to selected date or first of month
        if (_calendarOverlay._hoveredDayIndex < 0)
        {
            var firstDay = new DateOnly(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month, 1);
            var startDayOfWeek = WeekStart == DayOfWeekStart.Monday
                ? ((int)firstDay.DayOfWeek + 6) % 7
                : (int)firstDay.DayOfWeek;

            if (SelectedDate.HasValue && SelectedDate.Value.Year == _calendarOverlay.DisplayedMonth.Year
                && SelectedDate.Value.Month == _calendarOverlay.DisplayedMonth.Month)
            {
                _calendarOverlay._hoveredDayIndex = startDayOfWeek + SelectedDate.Value.Day - 1;
            }
            else
            {
                _calendarOverlay._hoveredDayIndex = startDayOfWeek;
            }
            return;
        }

        var newIndex = _calendarOverlay._hoveredDayIndex + offset;
        var daysInMonth = DateTime.DaysInMonth(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month);
        var firstDayOfMonth = new DateOnly(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month, 1);
        var startOffset = WeekStart == DayOfWeekStart.Monday
            ? ((int)firstDayOfMonth.DayOfWeek + 6) % 7
            : (int)firstDayOfMonth.DayOfWeek;

        var minIndex = startOffset;
        var maxIndex = startOffset + daysInMonth - 1;

        if (newIndex < minIndex)
        {
            // Go to previous month
            _calendarOverlay.DisplayedMonth = _calendarOverlay.DisplayedMonth.AddMonths(-1);
            var newDaysInMonth = DateTime.DaysInMonth(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month);
            var newFirstDay = new DateOnly(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month, 1);
            var newStartOffset = WeekStart == DayOfWeekStart.Monday
                ? ((int)newFirstDay.DayOfWeek + 6) % 7
                : (int)newFirstDay.DayOfWeek;
            _calendarOverlay._hoveredDayIndex = newStartOffset + newDaysInMonth - 1;
        }
        else if (newIndex > maxIndex)
        {
            // Go to next month
            _calendarOverlay.DisplayedMonth = _calendarOverlay.DisplayedMonth.AddMonths(1);
            var newFirstDay = new DateOnly(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month, 1);
            var newStartOffset = WeekStart == DayOfWeekStart.Monday
                ? ((int)newFirstDay.DayOfWeek + 6) % 7
                : (int)newFirstDay.DayOfWeek;
            _calendarOverlay._hoveredDayIndex = newStartOffset;
        }
        else
        {
            _calendarOverlay._hoveredDayIndex = newIndex;
        }
    }

    private void SelectHoveredDay()
    {
        if (_calendarOverlay == null || _calendarOverlay._hoveredDayIndex < 0) return;

        var firstDayOfMonth = new DateOnly(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month, 1);
        var startDayOfWeek = WeekStart == DayOfWeekStart.Monday
            ? ((int)firstDayOfMonth.DayOfWeek + 6) % 7
            : (int)firstDayOfMonth.DayOfWeek;

        var day = _calendarOverlay._hoveredDayIndex - startDayOfWeek + 1;
        var daysInMonth = DateTime.DaysInMonth(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month);

        if (day >= 1 && day <= daysInMonth)
        {
            var selectedDate = new DateOnly(_calendarOverlay.DisplayedMonth.Year, _calendarOverlay.DisplayedMonth.Month, day);
            if (IsDateInRange(selectedDate))
            {
                SetSelectedDate(selectedDate);
                InvokeSetters();
                IsOpen = false;
            }
        }
    }
    #endregion

    #region IInputControl
    public void InvokeCommand()
    {
        IsOpen = !IsOpen;
    }
    #endregion

    #region ITextInputControl
    public void SetSelectionStatus(bool isSelected)
    {
        if (!_isSelected && isSelected)
        {
            _selectionTime = TimeProvider.System.GetLocalNow().LocalDateTime;
        }
        _isSelected = isSelected;
    }

    public void HandleInput(PlusKey key)
    {
        if (key == PlusKey.Backspace)
        {
            if (_inputBuffer.Length > 0)
            {
                _inputBuffer = _inputBuffer[..^1];
            }
        }
        else if (key == PlusKey.Enter)
        {
            TryParseAndSetDate();
            _inputBuffer = string.Empty;
        }
    }

    public void HandleInput(char chr)
    {
        // Accept digits and date separators
        if (!char.IsDigit(chr) && chr != '.' && chr != '/' && chr != '-')
            return;

        _inputBuffer += chr;

        // Try to parse when buffer matches common date lengths
        if (_inputBuffer.Length >= 8)
        {
            TryParseAndSetDate();
        }
    }

    private void TryParseAndSetDate()
    {
        // Try various common formats
        string[] formats = { "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "ddMMyyyy" };

        foreach (var format in formats)
        {
            if (DateOnly.TryParseExact(_inputBuffer, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                if (IsDateInRange(date))
                {
                    SelectedDate = date;
                    InvokeSetters();
                    _inputBuffer = string.Empty;
                    return;
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Checks if a date is within the min/max range.
    /// </summary>
    internal bool IsDateInRange(DateOnly date)
    {
        if (MinDate.HasValue && date < MinDate.Value)
            return false;
        if (MaxDate.HasValue && date > MaxDate.Value)
            return false;
        return true;
    }

    /// <summary>
    /// Invokes the setters for two-way binding after selection changes.
    /// </summary>
    internal void InvokeSetters()
    {
        _onSelectedDateChanged?.Invoke(SelectedDate);
        if (_setter.TryGetValue(nameof(SelectedDate), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(SelectedDate);
            }
        }
    }

    protected override Margin? GetDebugPadding() => Padding;

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible) return;

        RenderDatePickerButton(canvas);
    }

    private void RenderDatePickerButton(SKCanvas canvas)
    {
        var rect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        // Determine display text
        string displayText;
        bool showingPlaceholder;

        if (!string.IsNullOrEmpty(_inputBuffer) && _isSelected)
        {
            displayText = _inputBuffer;
            showingPlaceholder = false;
        }
        else if (SelectedDate.HasValue)
        {
            displayText = SelectedDate.Value.ToString(DisplayFormat, CultureInfo.CurrentCulture);
            showingPlaceholder = false;
        }
        else
        {
            displayText = Placeholder ?? string.Empty;
            showingPlaceholder = true;
        }

        // Render text
        if (!string.IsNullOrEmpty(displayText))
        {
            var originalColor = _paint.Color;
            if (showingPlaceholder)
            {
                _paint.Color = PlaceholderColor;
            }

            _font.GetFontMetrics(out var fontMetrics);
            var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

            canvas.DrawText(
                displayText,
                rect.Left + Padding.Left,
                rect.Top + Padding.Top + textHeight,
                SKTextAlign.Left,
                _font,
                _paint);

            if (showingPlaceholder)
            {
                _paint.Color = originalColor;
            }
        }

        // Draw cursor if selected
        if (_isSelected && !IsOpen)
        {
            RenderCursor(canvas, rect);
        }

        // Draw calendar icon/arrow
        RenderCalendarIcon(canvas, rect);
    }

    private void RenderCursor(SKCanvas canvas, SKRect rect)
    {
        var elapsedMilliseconds = (TimeProvider.System.GetLocalNow().LocalDateTime - _selectionTime).TotalMilliseconds;
        if ((elapsedMilliseconds % 1000) < 500)
        {
            var displayText = !string.IsNullOrEmpty(_inputBuffer) ? _inputBuffer :
                (SelectedDate?.ToString(DisplayFormat, CultureInfo.CurrentCulture) ?? string.Empty);

            var textWidth = _font.MeasureText(displayText);
            _font.GetFontMetrics(out var fontMetrics);
            var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

            using var cursorPaint = new SKPaint
            {
                Color = TextColor,
                IsAntialias = true,
                StrokeWidth = 1
            };

            var cursorX = rect.Left + Padding.Left + textWidth + 2;
            var cursorTop = rect.Top + Padding.Top;
            var cursorBottom = cursorTop + textHeight;

            canvas.DrawLine(cursorX, cursorTop, cursorX, cursorBottom, cursorPaint);
        }
    }

    private void RenderCalendarIcon(SKCanvas canvas, SKRect rect)
    {
        var iconCenterX = rect.Right - Padding.Right - ArrowSize - 4;
        var iconCenterY = rect.Top + rect.Height / 2;

        using var iconPaint = new SKPaint
        {
            Color = TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f
        };

        // Draw a simple calendar icon (rectangle with top bar)
        var iconSize = ArrowSize * 1.5f;
        var iconRect = new SKRect(
            iconCenterX - iconSize / 2,
            iconCenterY - iconSize / 2,
            iconCenterX + iconSize / 2,
            iconCenterY + iconSize / 2);

        canvas.DrawRect(iconRect, iconPaint);

        // Top bar
        canvas.DrawLine(iconRect.Left, iconRect.Top + iconSize * 0.25f,
                       iconRect.Right, iconRect.Top + iconSize * 0.25f, iconPaint);
    }

    /// <summary>
    /// Gets the calendar popup position.
    /// </summary>
    internal SKRect GetCalendarRect(float calendarWidth, float calendarHeight)
    {
        var buttonBottom = Position.Y + VisualOffset.Y + ElementSize.Height;
        var buttonTop = Position.Y + VisualOffset.Y;
        var buttonLeft = Position.X + VisualOffset.X;

        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowHeight = _platformService?.WindowSize.Height ?? 800f;
        var windowWidth = _platformService?.WindowSize.Width ?? 1200f;

        // Check if calendar fits below
        var spaceBelow = windowHeight - buttonBottom;
        var opensUpward = spaceBelow < calendarHeight && buttonTop > spaceBelow;

        // Horizontal position - ensure it stays within window
        var left = buttonLeft;
        if (left + calendarWidth > windowWidth - 4)
        {
            left = windowWidth - calendarWidth - 4;
        }
        if (left < 4)
        {
            left = 4;
        }

        if (opensUpward)
        {
            return new SKRect(left, buttonTop - calendarHeight - 4, left + calendarWidth, buttonTop - 4);
        }
        else
        {
            return new SKRect(left, buttonBottom + 4, left + calendarWidth, buttonBottom + calendarHeight + 4);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        var width = DesiredSize?.Width ?? 200f;
        var height = textHeight + Padding.Top + Padding.Bottom;

        return new Size(
            Math.Min(width, availableSize.Width),
            Math.Min(height, availableSize.Height));
    }

    public override UiElement? HitTest(Point point)
    {
        if (point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
            point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height)
        {
            return this;
        }
        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnregisterCalendarOverlay();

            // Release paint from registry
            if (_paint is not null && _font is not null)
            {
                PaintRegistry.Release(_paint, _font);
            }
        }
        base.Dispose(disposing);
    }
}
