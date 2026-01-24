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
/// A time picker control with hour/minute selector popup.
/// Supports 12/24-hour formats, minute increments, and time range restrictions.
/// </summary>
/// <example>
/// <code>
/// new TimePicker()
///     .SetSelectedTime(new TimeOnly(9, 0))
///     .SetMinuteIncrement(15)
///     .Set24HourFormat(true)
///     .BindSelectedTime(nameof(vm.StartTime), () => vm.StartTime, v => vm.StartTime = v);
/// </code>
/// </example>
[GenerateShadowMethods]
[UiPropGenPadding]
[UiPropGenPlaceholder]
[UiPropGenPlaceholderColor]
public partial class TimePicker : UiElement, IInputControl, ITextInputControl, IHoverableControl, IFocusable, IKeyboardInputHandler
{
    private const float ArrowSize = 8f;
    private IOverlayService? _overlayService;
    private TimePickerSelectorOverlay? _selectorOverlay;
    private IPlatformService? _platformService;
    private bool _isSelected;
    private DateTime _selectionTime;
    private string _inputBuffer = string.Empty;

    #region SelectedTime
    private Action<TimeOnly?>? _onSelectedTimeChanged;

    internal TimeOnly? SelectedTime
    {
        get => field;
        set
        {
            if (field == value) return;
            // Round to nearest increment
            if (value.HasValue && MinuteIncrement > 1)
            {
                var minutes = value.Value.Minute;
                var roundedMinutes = (int)Math.Round((double)minutes / MinuteIncrement) * MinuteIncrement;
                if (roundedMinutes >= 60) roundedMinutes = 60 - MinuteIncrement;
                value = new TimeOnly(value.Value.Hour, roundedMinutes);
            }
            field = value;
            _inputBuffer = string.Empty;
            InvalidateMeasure();
        }
    }

    public TimePicker SetSelectedTime(TimeOnly? time)
    {
        SelectedTime = time;
        return this;
    }

    /// <summary>
    /// Sets a callback that is invoked when SelectedTime changes.
    /// </summary>
    public TimePicker SetOnSelectedTimeChanged(Action<TimeOnly?> callback)
    {
        _onSelectedTimeChanged = callback;
        return this;
    }

    public TimePicker BindSelectedTime(Expression<Func<TimeOnly?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedTime = getter());
        return this;
    }

    public TimePicker BindSelectedTime(Expression<Func<TimeOnly?>> propertyExpression, Action<TimeOnly?> setter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedTime = getter());
        foreach (var segment in path)
        {
            RegisterSetter<TimeOnly?>(segment, setter);
        }
        RegisterSetter<TimeOnly?>(nameof(SelectedTime), setter);
        return this;
    }

    #endregion

    #region MinTime
    internal TimeOnly? MinTime { get; set; }

    public TimePicker SetMinTime(TimeOnly? minTime)
    {
        MinTime = minTime;
        return this;
    }

    public TimePicker BindMinTime(Expression<Func<TimeOnly?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MinTime = getter());
        return this;
    }
    #endregion

    #region MaxTime
    internal TimeOnly? MaxTime { get; set; }

    public TimePicker SetMaxTime(TimeOnly? maxTime)
    {
        MaxTime = maxTime;
        return this;
    }

    public TimePicker BindMaxTime(Expression<Func<TimeOnly?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MaxTime = getter());
        return this;
    }
    #endregion

    #region DisplayFormat
    internal string DisplayFormat { get; set; } = "HH:mm";

    public TimePicker SetDisplayFormat(string format)
    {
        DisplayFormat = format;
        InvalidateMeasure();
        return this;
    }

    public TimePicker BindDisplayFormat(Expression<Func<string>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DisplayFormat = getter());
        return this;
    }
    #endregion

    #region MinuteIncrement
    internal int MinuteIncrement
    {
        get => field;
        set
        {
            // Only allow valid increments
            if (value != 1 && value != 5 && value != 10 && value != 15 && value != 30)
            {
                value = 1;
            }
            field = value;
        }
    } = 1;

    public TimePicker SetMinuteIncrement(int increment)
    {
        MinuteIncrement = increment;
        return this;
    }

    public TimePicker BindMinuteIncrement(Expression<Func<int>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MinuteIncrement = getter());
        return this;
    }
    #endregion

    #region Is24HourFormat
    internal bool Is24HourFormat { get; set; } = true;

    public TimePicker Set24HourFormat(bool is24Hour)
    {
        Is24HourFormat = is24Hour;
        if (!is24Hour && DisplayFormat == "HH:mm")
        {
            DisplayFormat = "hh:mm tt";
        }
        else if (is24Hour && DisplayFormat == "hh:mm tt")
        {
            DisplayFormat = "HH:mm";
        }
        return this;
    }

    public TimePicker Bind24HourFormat(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Is24HourFormat = getter());
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

    public TimePicker SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }

    public TimePicker BindTextColor(Expression<Func<SKColor>> propertyExpression)
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

    public TimePicker SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public TimePicker BindTextSize(Expression<Func<float>> propertyExpression)
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

    public TimePicker SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        return this;
    }

    public TimePicker BindFontFamily(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FontFamily = getter());
        return this;
    }
    #endregion

    #region SelectorBackground
    internal SKColor SelectorBackground { get; set; } = PlusUiDefaults.BackgroundPrimary;

    public TimePicker SetSelectorBackground(SKColor color)
    {
        SelectorBackground = color;
        return this;
    }

    public TimePicker BindSelectorBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectorBackground = getter());
        return this;
    }
    #endregion

    #region HoverBackground
    internal SKColor HoverBackground { get; set; } = PlusUiDefaults.BackgroundHover;

    public TimePicker SetHoverBackground(SKColor color)
    {
        HoverBackground = color;
        return this;
    }

    public TimePicker BindHoverBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HoverBackground = getter());
        return this;
    }
    #endregion

    #region SelectedBackground
    internal SKColor SelectedBackground { get; set; } = PlusUiDefaults.BackgroundSelected;

    public TimePicker SetSelectedBackground(SKColor color)
    {
        SelectedBackground = color;
        return this;
    }

    public TimePicker BindSelectedBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedBackground = getter());
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
                RegisterSelectorOverlay();
            }
            else
            {
                UnregisterSelectorOverlay();
            }

            InvalidateMeasure();
        }
    }

    public TimePicker SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
        return this;
    }

    public TimePicker BindIsOpen(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsOpen = getter());
        return this;
    }

    private void RegisterSelectorOverlay()
    {
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null) return;

        _selectorOverlay = new TimePickerSelectorOverlay(this);
        _overlayService.RegisterOverlay(_selectorOverlay);
    }

    private void UnregisterSelectorOverlay()
    {
        if (_overlayService != null && _selectorOverlay != null)
        {
            _overlayService.UnregisterOverlay(_selectorOverlay);
            _selectorOverlay = null;
        }
    }
    #endregion

    #region IsHovered (IHoverableControl)
    public bool IsHovered { get; set; }
    #endregion

    private SKFont _font;
    private SKPaint _paint;

    // Internal accessors for TimePickerSelectorOverlay
    internal SKFont Font => _font;
    internal SKPaint Paint => _paint;

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.TimePicker;

    public TimePicker()
    {
        SetDesiredSize(new Size(150, 40));
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
        return AccessibilityLabel ?? Placeholder ?? "Time picker";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        if (!string.IsNullOrEmpty(AccessibilityValue))
        {
            return AccessibilityValue;
        }
        return SelectedTime?.ToString(DisplayFormat, System.Globalization.CultureInfo.CurrentCulture);
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

        // When open, navigate time
        var currentTime = SelectedTime ?? new TimeOnly(12, 0);

        switch (key)
        {
            case PlusKey.Escape:
                IsOpen = false;
                return true;
            case PlusKey.ArrowUp:
                // Increment hour
                SetSelectedTime(currentTime.AddHours(1));
                InvokeTimeSetters();
                _selectorOverlay?.ScrollToSelection();
                return true;
            case PlusKey.ArrowDown:
                // Decrement hour
                SetSelectedTime(currentTime.AddHours(-1));
                InvokeTimeSetters();
                _selectorOverlay?.ScrollToSelection();
                return true;
            case PlusKey.ArrowRight:
                // Increment minutes
                SetSelectedTime(currentTime.AddMinutes(MinuteIncrement));
                InvokeTimeSetters();
                _selectorOverlay?.ScrollToSelection();
                return true;
            case PlusKey.ArrowLeft:
                // Decrement minutes
                SetSelectedTime(currentTime.AddMinutes(-MinuteIncrement));
                InvokeTimeSetters();
                _selectorOverlay?.ScrollToSelection();
                return true;
            case PlusKey.Enter:
            case PlusKey.Space:
                IsOpen = false;
                return true;
            default:
                return false;
        }
    }

    private void InvokeTimeSetters()
    {
        if (_setter.TryGetValue(nameof(SelectedTime), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(SelectedTime);
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

    public void HandleInput(PlusKey key, bool shift, bool ctrl)
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
            TryParseAndSetTime();
            _inputBuffer = string.Empty;
        }
    }

    public void HandleInput(char chr)
    {
        if (!char.IsDigit(chr) && chr != ':')
            return;

        _inputBuffer += chr;

        if (_inputBuffer.Length >= 4 && !_inputBuffer.Contains(':'))
        {
            _inputBuffer = _inputBuffer.Insert(2, ":");
        }

        if (_inputBuffer.Length >= 5)
        {
            TryParseAndSetTime();
        }
    }

    public void HandleClick(float localX, float localY)
    {
    }

    private void TryParseAndSetTime()
    {
        // Try various common formats
        string[] formats = { "HH:mm", "H:mm", "hh:mm tt", "h:mm tt", "HHmm" };

        foreach (var format in formats)
        {
            if (TimeOnly.TryParseExact(_inputBuffer, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            {
                if (IsTimeInRange(time))
                {
                    SelectedTime = time;
                    InvokeSetters();
                    _inputBuffer = string.Empty;
                    return;
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Checks if a time is within the min/max range.
    /// </summary>
    internal bool IsTimeInRange(TimeOnly time)
    {
        if (MinTime.HasValue && time < MinTime.Value)
            return false;
        if (MaxTime.HasValue && time > MaxTime.Value)
            return false;
        return true;
    }

    /// <summary>
    /// Invokes the setters for two-way binding after selection changes.
    /// </summary>
    internal void InvokeSetters()
    {
        _onSelectedTimeChanged?.Invoke(SelectedTime);
        if (_setter.TryGetValue(nameof(SelectedTime), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(SelectedTime);
            }
        }
    }

    /// <summary>
    /// Gets available hours based on format.
    /// </summary>
    internal IEnumerable<int> GetAvailableHours()
    {
        if (Is24HourFormat)
        {
            for (int h = 0; h < 24; h++)
                yield return h;
        }
        else
        {
            for (int h = 1; h <= 12; h++)
                yield return h;
        }
    }

    /// <summary>
    /// Gets available minutes based on increment.
    /// </summary>
    internal IEnumerable<int> GetAvailableMinutes()
    {
        for (int m = 0; m < 60; m += MinuteIncrement)
            yield return m;
    }

    protected override Margin? GetDebugPadding() => Padding;

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible) return;

        RenderTimePickerButton(canvas);
    }

    private void RenderTimePickerButton(SKCanvas canvas)
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
        else if (SelectedTime.HasValue)
        {
            displayText = SelectedTime.Value.ToString(DisplayFormat, CultureInfo.CurrentCulture);
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

        // Draw clock icon
        RenderClockIcon(canvas, rect);
    }

    private void RenderCursor(SKCanvas canvas, SKRect rect)
    {
        var elapsedMilliseconds = (TimeProvider.System.GetLocalNow().LocalDateTime - _selectionTime).TotalMilliseconds;
        if ((elapsedMilliseconds % 1000) < 500)
        {
            var displayText = !string.IsNullOrEmpty(_inputBuffer) ? _inputBuffer :
                (SelectedTime?.ToString(DisplayFormat, CultureInfo.CurrentCulture) ?? string.Empty);

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

    private void RenderClockIcon(SKCanvas canvas, SKRect rect)
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

        // Draw a simple clock icon (circle with hands)
        var iconRadius = ArrowSize * 0.8f;
        canvas.DrawCircle(iconCenterX, iconCenterY, iconRadius, iconPaint);

        // Hour hand
        canvas.DrawLine(iconCenterX, iconCenterY, iconCenterX, iconCenterY - iconRadius * 0.5f, iconPaint);
        // Minute hand
        canvas.DrawLine(iconCenterX, iconCenterY, iconCenterX + iconRadius * 0.4f, iconCenterY, iconPaint);
    }

    /// <summary>
    /// Gets the selector popup position.
    /// </summary>
    internal SKRect GetSelectorRect(float selectorWidth, float selectorHeight)
    {
        var buttonBottom = Position.Y + VisualOffset.Y + ElementSize.Height;
        var buttonTop = Position.Y + VisualOffset.Y;
        var buttonLeft = Position.X + VisualOffset.X;

        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowHeight = _platformService?.WindowSize.Height ?? 800f;
        var windowWidth = _platformService?.WindowSize.Width ?? 1200f;

        // Check if selector fits below
        var spaceBelow = windowHeight - buttonBottom;
        var opensUpward = spaceBelow < selectorHeight && buttonTop > spaceBelow;

        // Horizontal position - ensure it stays within window
        var left = buttonLeft;
        if (left + selectorWidth > windowWidth - 4)
        {
            left = windowWidth - selectorWidth - 4;
        }
        if (left < 4)
        {
            left = 4;
        }

        if (opensUpward)
        {
            return new SKRect(left, buttonTop - selectorHeight - 4, left + selectorWidth, buttonTop - 4);
        }
        else
        {
            return new SKRect(left, buttonBottom + 4, left + selectorWidth, buttonBottom + selectorHeight + 4);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        var width = DesiredSize?.Width ?? 150f;
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
            UnregisterSelectorOverlay();

            // Release paint from registry
            if (_paint is not null && _font is not null)
            {
                PaintRegistry.Release(_paint, _font);
            }
        }
        base.Dispose(disposing);
    }
}
