using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using SkiaSharp;
using System.Globalization;

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
public partial class DatePicker : UiElement, IInputControl, ITextInputControl, IHoverableControl
{
    private const float ArrowSize = 8f;
    private IOverlayService? _overlayService;
    private DatePickerCalendarOverlay? _calendarOverlay;
    private IPlatformService? _platformService;
    private bool _isSelected;
    private DateTime _selectionTime;
    private string _inputBuffer = string.Empty;

    #region SelectedDate
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

    public DatePicker BindSelectedDate(string propertyName, Func<DateOnly?> propertyGetter)
    {
        RegisterBinding(propertyName, () => SelectedDate = propertyGetter());
        return this;
    }

    public DatePicker BindSelectedDate(string propertyName, Func<DateOnly?> propertyGetter, Action<DateOnly?> propertySetter)
    {
        RegisterBinding(propertyName, () => SelectedDate = propertyGetter());
        RegisterSetter(nameof(SelectedDate), propertySetter);
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

    public DatePicker BindMinDate(string propertyName, Func<DateOnly?> propertyGetter)
    {
        RegisterBinding(propertyName, () => MinDate = propertyGetter());
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

    public DatePicker BindMaxDate(string propertyName, Func<DateOnly?> propertyGetter)
    {
        RegisterBinding(propertyName, () => MaxDate = propertyGetter());
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

    public DatePicker BindDisplayFormat(string propertyName, Func<string> propertyGetter)
    {
        RegisterBinding(propertyName, () => DisplayFormat = propertyGetter());
        return this;
    }
    #endregion

    #region Placeholder
    internal string? Placeholder { get; set; }

    public DatePicker SetPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    public DatePicker BindPlaceholder(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Placeholder = propertyGetter());
        return this;
    }
    #endregion

    #region PlaceholderColor
    internal SKColor PlaceholderColor { get; set; } = new SKColor(180, 180, 180);

    public DatePicker SetPlaceholderColor(SKColor color)
    {
        PlaceholderColor = color;
        return this;
    }

    public DatePicker BindPlaceholderColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => PlaceholderColor = propertyGetter());
        return this;
    }
    #endregion

    #region TextColor
    internal SKColor TextColor { get; set; } = SKColors.White;

    public DatePicker SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }

    public DatePicker BindTextColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextColor = propertyGetter());
        return this;
    }
    #endregion

    #region TextSize
    internal float TextSize { get; set; } = 14f;

    public DatePicker SetTextSize(float size)
    {
        TextSize = size;
        InvalidateMeasure();
        return this;
    }

    public DatePicker BindTextSize(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextSize = propertyGetter());
        return this;
    }
    #endregion

    #region FontFamily
    internal string? FontFamily { get; set; }

    public DatePicker SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        InvalidateMeasure();
        return this;
    }

    public DatePicker BindFontFamily(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => FontFamily = propertyGetter());
        return this;
    }
    #endregion

    #region Padding
    internal Margin Padding
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = new Margin(12, 8);

    public DatePicker SetPadding(Margin padding)
    {
        Padding = padding;
        return this;
    }

    public DatePicker BindPadding(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => Padding = propertyGetter());
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

    public DatePicker BindWeekStart(string propertyName, Func<DayOfWeekStart> propertyGetter)
    {
        RegisterBinding(propertyName, () => WeekStart = propertyGetter());
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

    public DatePicker BindShowTodayButton(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => ShowTodayButton = propertyGetter());
        return this;
    }
    #endregion

    #region CalendarBackground
    internal SKColor CalendarBackground { get; set; } = new SKColor(40, 40, 40);

    public DatePicker SetCalendarBackground(SKColor color)
    {
        CalendarBackground = color;
        return this;
    }

    public DatePicker BindCalendarBackground(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => CalendarBackground = propertyGetter());
        return this;
    }
    #endregion

    #region HoverBackground
    internal SKColor HoverBackground { get; set; } = new SKColor(60, 60, 60);

    public DatePicker SetHoverBackground(SKColor color)
    {
        HoverBackground = color;
        return this;
    }

    public DatePicker BindHoverBackground(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => HoverBackground = propertyGetter());
        return this;
    }
    #endregion

    #region SelectedBackground
    internal SKColor SelectedBackground { get; set; } = new SKColor(0, 120, 215);

    public DatePicker SetSelectedBackground(SKColor color)
    {
        SelectedBackground = color;
        return this;
    }

    public DatePicker BindSelectedBackground(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => SelectedBackground = propertyGetter());
        return this;
    }
    #endregion

    #region TodayBorderColor
    internal SKColor TodayBorderColor { get; set; } = new SKColor(0, 120, 215);

    public DatePicker SetTodayBorderColor(SKColor color)
    {
        TodayBorderColor = color;
        return this;
    }

    public DatePicker BindTodayBorderColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => TodayBorderColor = propertyGetter());
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

    public DatePicker BindIsOpen(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsOpen = propertyGetter());
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

    private SKFont? _font;
    internal SKFont Font
    {
        get
        {
            if (_font == null)
            {
                var typeface = string.IsNullOrEmpty(FontFamily)
                    ? SKTypeface.FromFamilyName(null)
                    : SKTypeface.FromFamilyName(FontFamily);
                _font = new SKFont(typeface, TextSize);
            }
            return _font;
        }
    }

    private SKPaint? _paint;
    internal SKPaint Paint
    {
        get
        {
            if (_paint == null)
            {
                _paint = new SKPaint
                {
                    Color = TextColor,
                    IsAntialias = true
                };
            }
            else
            {
                _paint.Color = TextColor;
            }
            return _paint;
        }
    }

    public DatePicker()
    {
        SetDesiredSize(new Size(200, 40));
    }

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
        if (_setter.TryGetValue(nameof(SelectedDate), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(SelectedDate);
            }
        }
    }

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
            var originalColor = Paint.Color;
            if (showingPlaceholder)
            {
                Paint.Color = PlaceholderColor;
            }

            Font.GetFontMetrics(out var fontMetrics);
            var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

            canvas.DrawText(
                displayText,
                rect.Left + Padding.Left,
                rect.Top + Padding.Top + textHeight,
                SKTextAlign.Left,
                Font,
                Paint);

            if (showingPlaceholder)
            {
                Paint.Color = originalColor;
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

            var textWidth = Font.MeasureText(displayText);
            Font.GetFontMetrics(out var fontMetrics);
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
        Font.GetFontMetrics(out var fontMetrics);
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
            _font?.Dispose();
            _paint?.Dispose();
        }
        base.Dispose(disposing);
    }
}
