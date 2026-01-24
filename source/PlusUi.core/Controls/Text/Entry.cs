using PlusUi.core.Attributes;
using PlusUi.core.Services;
using PlusUi.core.UiPropGen;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A text input control with full cursor navigation, text selection, and clipboard support.
/// Supports both single-line and multi-line modes.
/// </summary>
[GenerateShadowMethods]
[UiPropGenPadding]
[UiPropGenPlaceholder]
[UiPropGenPlaceholderColor]
public partial class Entry : UiTextElement, ITextInputControl, IFocusable, IScrollableControl
{
    private readonly IClipboardService? _clipboard;
    private readonly Scrollbar _scrollbar;

    private bool _isSelected;
    private DateTime _selectionTime;
    private int _cursorPosition;
    private int _selectionStart = -1;
    private int _selectionEnd = -1;
    private float _scrollOffset;
    private float _verticalScrollOffset;
    private float _totalContentHeight;

    public Entry()
    {
        _clipboard = ServiceProviderService.ServiceProvider?.GetService<IClipboardService>();

        _scrollbar = new Scrollbar()
            .SetOrientation(ScrollbarOrientation.Vertical)
            .SetWidth(8)
            .SetAutoHide(true)
            .SetOnValueChanged(OnScrollbarValueChanged);
        _scrollbar.Parent = this;

        SetBackground(new SolidColorBackground(PlusUiDefaults.BackgroundInput));
        SetCornerRadius(PlusUiDefaults.CornerRadius);
        SetDesiredHeight(40);
        SetDesiredWidth(200);
        SetHighContrastBackground(PlusUiDefaults.HcInputBackground);
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
        PlaceholderColor = PlusUiDefaults.TextPlaceholder;
        Padding = new Margin(12, 8);
        FocusBorderColor = PlusUiDefaults.AccentPrimary;
        FocusBorderThickness = 2;
    }

    private void OnScrollbarValueChanged(float scrollOffset)
    {
        _verticalScrollOffset = scrollOffset;
    }

    protected internal override bool IsFocusable => true;
    public override bool InterceptsClicks => true;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.TextInput;

    public override string? GetComputedAccessibilityLabel()
        => AccessibilityLabel ?? Placeholder ?? "Text input";

    public override string? GetComputedAccessibilityValue()
        => AccessibilityValue ?? (IsPassword ? new string('*', Text?.Length ?? 0) : Text);

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
    void IFocusable.OnBlur() => OnBlur();

    #endregion

    #region Properties

    #region IsPassword

    internal bool IsPassword { get; set; }

    public Entry SetIsPassword(bool isPassword)
    {
        IsPassword = isPassword;
        return this;
    }

    public Entry BindIsPassword(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsPassword = getter());
        return this;
    }

    #endregion

    #region PasswordChar

    internal char PasswordChar { get; set; } = '•';

    public Entry SetPasswordChar(char passwordChar)
    {
        PasswordChar = passwordChar;
        return this;
    }

    public Entry BindPasswordChar(Expression<Func<char>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => PasswordChar = getter());
        return this;
    }

    #endregion

    #region MaxLength

    internal int? MaxLength { get; set; }

    public Entry SetMaxLength(int maxLength)
    {
        MaxLength = maxLength;
        return this;
    }

    public Entry BindMaxLength(Expression<Func<int>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MaxLength = getter());
        return this;
    }

    #endregion

    #region Keyboard

    internal KeyboardType Keyboard { get; set; } = KeyboardType.Default;

    public Entry SetKeyboard(KeyboardType keyboard)
    {
        Keyboard = keyboard;
        return this;
    }

    public Entry BindKeyboard(Expression<Func<KeyboardType>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Keyboard = getter());
        return this;
    }

    #endregion

    #region ReturnKey

    internal ReturnKeyType ReturnKey { get; set; } = ReturnKeyType.Default;

    public Entry SetReturnKey(ReturnKeyType returnKey)
    {
        ReturnKey = returnKey;
        return this;
    }

    public Entry BindReturnKey(Expression<Func<ReturnKeyType>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ReturnKey = getter());
        return this;
    }

    #endregion

    #region FocusBorderColor

    internal Color FocusBorderColor
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    }

    public Entry SetFocusBorderColor(Color color)
    {
        FocusBorderColor = color;
        return this;
    }

    public Entry BindFocusBorderColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusBorderColor = getter());
        return this;
    }

    #endregion

    #region FocusBorderThickness

    internal float FocusBorderThickness
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    }

    public Entry SetFocusBorderThickness(float thickness)
    {
        FocusBorderThickness = thickness;
        return this;
    }

    public Entry BindFocusBorderThickness(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusBorderThickness = getter());
        return this;
    }

    #endregion

    #region SelectionColor

    internal Color SelectionColor { get; set; } = new Color(100, 149, 237, 100);

    public Entry SetSelectionColor(Color color)
    {
        SelectionColor = color;
        return this;
    }

    public Entry BindSelectionColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectionColor = getter());
        return this;
    }

    #endregion

    #region IsMultiLine

    internal bool IsMultiLine
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            if (value)
            {
                // Set default MinLines and MaxLines for multi-line mode (5 lines)
                if (MinLines == 1) MinLines = 5;
                if (!MaxLines.HasValue) MaxLines = 5;

                // Clear DesiredSize.Height so multi-line can calculate height from MinLines/MaxLines
                DesiredSize = DesiredSize.HasValue
                    ? new Size(DesiredSize.Value.Width, -1)
                    : null;
            }
            InvalidateMeasure();
        }
    }

    public Entry SetIsMultiLine(bool isMultiLine)
    {
        IsMultiLine = isMultiLine;
        return this;
    }

    public Entry BindIsMultiLine(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsMultiLine = getter());
        return this;
    }

    #endregion

    #region MinLines

    internal int MinLines
    {
        get => field;
        set
        {
            if (field == value) return;
            field = Math.Max(1, value);
            InvalidateMeasure();
        }
    } = 1;

    public Entry SetMinLines(int minLines)
    {
        MinLines = minLines;
        return this;
    }

    public Entry BindMinLines(Expression<Func<int>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MinLines = getter());
        return this;
    }

    #endregion

    #region MaxLines

    internal new int? MaxLines
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value.HasValue ? Math.Max(1, value.Value) : null;
            InvalidateMeasure();
        }
    }

    public new Entry SetMaxLines(int? maxLines)
    {
        MaxLines = maxLines;
        return this;
    }

    public new Entry BindMaxLines(Expression<Func<int?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MaxLines = getter());
        return this;
    }

    #endregion

    #region TextWrapping

    internal new TextWrapping TextWrapping
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    } = TextWrapping.NoWrap;

    public new Entry SetTextWrapping(TextWrapping wrapping)
    {
        TextWrapping = wrapping;
        return this;
    }

    public new Entry BindTextWrapping(Expression<Func<TextWrapping>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextWrapping = getter());
        return this;
    }

    #endregion

    #endregion

    #region Text Changed Callback

    private Action<string?>? _onTextChanged;

    public Entry SetOnTextChanged(Action<string?> callback)
    {
        _onTextChanged = callback;
        return this;
    }

    #endregion

    #region Text Binding

    public Entry BindText(Expression<Func<string?>> propertyExpression, Action<string> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            Text = getter();
            _cursorPosition = Math.Min(_cursorPosition, Text?.Length ?? 0);
            ClearSelection();
        });
        foreach (var segment in path)
        {
            RegisterSetter<string>(segment, propertySetter);
        }
        RegisterSetter<string>(nameof(Text), propertySetter);
        return this;
    }

    public Entry BindText<T>(
        Expression<Func<T>> propertyExpression,
        Action<T> propertySetter,
        Func<T, string?>? toControl = null,
        Func<string, T>? toSource = null)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            Text = toControl != null ? toControl(getter()) : getter()?.ToString();
            _cursorPosition = Math.Min(_cursorPosition, Text?.Length ?? 0);
            ClearSelection();
        });
        Action<string> wrappedSetter = controlValue => propertySetter(toSource != null ? toSource(controlValue) : (T)(object)controlValue);
        foreach (var segment in path)
        {
            RegisterSetter<string>(segment, wrappedSetter);
        }
        RegisterSetter<string>(nameof(Text), wrappedSetter);
        return this;
    }

    #endregion

    #region Selection

    private bool HasSelection => _selectionStart >= 0 && _selectionEnd >= 0 && _selectionStart != _selectionEnd;

    private int SelectionMin => Math.Min(_selectionStart, _selectionEnd);
    private int SelectionMax => Math.Max(_selectionStart, _selectionEnd);

    private void ClearSelection()
    {
        _selectionStart = -1;
        _selectionEnd = -1;
    }

    private void BeginSelection()
    {
        if (_selectionStart < 0)
        {
            _selectionStart = _cursorPosition;
            _selectionEnd = _cursorPosition;
        }
    }

    private void ExtendSelection()
    {
        _selectionEnd = _cursorPosition;
    }

    private string? GetSelectedText()
    {
        if (!HasSelection || string.IsNullOrEmpty(Text)) return null;
        return Text.Substring(SelectionMin, SelectionMax - SelectionMin);
    }

    private void DeleteSelection()
    {
        if (!HasSelection || string.IsNullOrEmpty(Text)) return;

        var min = SelectionMin;
        var max = SelectionMax;
        Text = Text[..min] + Text[max..];
        _cursorPosition = min;
        ClearSelection();
        NotifyTextChanged();
    }

    private void SelectAll()
    {
        if (string.IsNullOrEmpty(Text)) return;
        _selectionStart = 0;
        _selectionEnd = Text.Length;
        _cursorPosition = Text.Length;
    }

    #endregion

    #region Clipboard

    private void Copy()
    {
        var selectedText = GetSelectedText();
        if (!string.IsNullOrEmpty(selectedText))
        {
            _clipboard?.SetText(selectedText);
        }
    }

    private void Cut()
    {
        Copy();
        DeleteSelection();
    }

    private void Paste()
    {
        var clipboardText = _clipboard?.GetText();
        if (string.IsNullOrEmpty(clipboardText)) return;

        if (HasSelection)
        {
            DeleteSelection();
        }

        var text = Text ?? string.Empty;

        if (MaxLength.HasValue)
        {
            var available = MaxLength.Value - text.Length;
            if (available <= 0) return;
            clipboardText = clipboardText[..Math.Min(clipboardText.Length, available)];
        }

        Text = text[.._cursorPosition] + clipboardText + text[_cursorPosition..];
        _cursorPosition += clipboardText.Length;
        NotifyTextChanged();
    }

    #endregion

    #region Input Handling

    public void SetSelectionStatus(bool isSelected)
    {
        if (!_isSelected && isSelected)
        {
            _selectionTime = TimeProvider.System.GetLocalNow().LocalDateTime;
        }
        _isSelected = isSelected;

        if (!isSelected)
        {
            ClearSelection();
        }
    }

    public void HandleInput(PlusKey key, bool shift, bool ctrl)
    {
        var text = Text ?? string.Empty;

        switch (key)
        {
            case PlusKey.Backspace:
                if (HasSelection)
                {
                    DeleteSelection();
                }
                else if (_cursorPosition > 0)
                {
                    Text = text[..(_cursorPosition - 1)] + text[_cursorPosition..];
                    _cursorPosition--;
                    NotifyTextChanged();
                }
                break;

            case PlusKey.Delete:
                if (HasSelection)
                {
                    DeleteSelection();
                }
                else if (_cursorPosition < text.Length)
                {
                    Text = text[.._cursorPosition] + text[(_cursorPosition + 1)..];
                    NotifyTextChanged();
                }
                break;

            case PlusKey.Enter:
                if (IsMultiLine)
                {
                    if (HasSelection)
                    {
                        DeleteSelection();
                        text = Text ?? string.Empty;
                    }
                    Text = text[.._cursorPosition] + '\n' + text[_cursorPosition..];
                    _cursorPosition++;
                    NotifyTextChanged();
                }
                break;

            case PlusKey.ArrowLeft:
                if (shift)
                    BeginSelection();

                if (ctrl)
                    _cursorPosition = FindPreviousWordBoundary(text, _cursorPosition);
                else if (_cursorPosition > 0)
                    _cursorPosition--;

                if (shift)
                    ExtendSelection();
                else
                    ClearSelection();
                break;

            case PlusKey.ArrowRight:
                if (shift)
                    BeginSelection();

                if (ctrl)
                    _cursorPosition = FindNextWordBoundary(text, _cursorPosition);
                else if (_cursorPosition < text.Length)
                    _cursorPosition++;

                if (shift)
                    ExtendSelection();
                else
                    ClearSelection();
                break;

            case PlusKey.ArrowUp:
                if (IsMultiLine)
                {
                    if (shift)
                        BeginSelection();

                    var (line, column) = GetLineAndColumn(_cursorPosition);
                    if (line > 0)
                        _cursorPosition = GetCursorPositionFromLineColumn(line - 1, column);

                    if (shift)
                        ExtendSelection();
                    else
                        ClearSelection();
                }
                break;

            case PlusKey.ArrowDown:
                if (IsMultiLine)
                {
                    if (shift)
                        BeginSelection();

                    var (line, column) = GetLineAndColumn(_cursorPosition);
                    var lines = GetLines();
                    if (line < lines.Length - 1)
                        _cursorPosition = GetCursorPositionFromLineColumn(line + 1, column);

                    if (shift)
                        ExtendSelection();
                    else
                        ClearSelection();
                }
                break;

            case PlusKey.Home:
                if (shift)
                    BeginSelection();

                if (IsMultiLine && !ctrl)
                {
                    var (line, _) = GetLineAndColumn(_cursorPosition);
                    _cursorPosition = GetLineStartPosition(line);
                }
                else
                {
                    _cursorPosition = 0;
                }

                if (shift)
                    ExtendSelection();
                else
                    ClearSelection();
                break;

            case PlusKey.End:
                if (shift)
                    BeginSelection();

                if (IsMultiLine && !ctrl)
                {
                    var (line, _) = GetLineAndColumn(_cursorPosition);
                    _cursorPosition = GetLineEndPosition(line);
                }
                else
                {
                    _cursorPosition = text.Length;
                }

                if (shift)
                    ExtendSelection();
                else
                    ClearSelection();
                break;

            case PlusKey.A:
                if (ctrl)
                {
                    SelectAll();
                }
                break;

            case PlusKey.C:
                if (ctrl)
                {
                    Copy();
                }
                break;

            case PlusKey.X:
                if (ctrl)
                {
                    Cut();
                }
                break;

            case PlusKey.V:
                if (ctrl)
                {
                    Paste();
                }
                break;
        }

        EnsureCursorVisible();
        ResetCursorBlink();
    }

    public void HandleInput(char chr)
    {
        if (char.IsControl(chr)) return;

        if (HasSelection)
        {
            DeleteSelection();
        }

        var text = Text ?? string.Empty;

        if (MaxLength.HasValue && text.Length >= MaxLength.Value)
        {
            return;
        }

        Text = text[.._cursorPosition] + chr + text[_cursorPosition..];
        _cursorPosition++;
        NotifyTextChanged();
        EnsureCursorVisible();
        ResetCursorBlink();
    }

    public void HandleClick(float localX, float localY)
    {
        var text = Text ?? string.Empty;

        if (IsMultiLine)
        {
            Font.GetFontMetrics(out var fontMetrics);
            var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;
            var lineSpacing = lineHeight * 0.2f;
            var totalLineHeight = lineHeight + lineSpacing;

            var lines = GetLines();
            var clickedLine = (int)((localY - Padding.Top + _verticalScrollOffset) / totalLineHeight);
            clickedLine = Math.Clamp(clickedLine, 0, lines.Length - 1);

            var lineText = IsPassword ? new string(PasswordChar, lines[clickedLine].Length) : lines[clickedLine];
            var columnInLine = GetCharacterIndexAtX(lineText, localX - Padding.Left);

            _cursorPosition = GetCursorPositionFromLineColumn(clickedLine, columnInLine);
        }
        else
        {
            var displayText = IsPassword ? new string(PasswordChar, text.Length) : text;
            _cursorPosition = GetCharacterIndexAtX(displayText, localX - Padding.Left + _scrollOffset);
        }

        ClearSelection();
        ResetCursorBlink();
    }

    private int GetCharacterIndexAtX(string text, float x)
    {
        if (string.IsNullOrEmpty(text) || x <= 0) return 0;

        for (var i = 1; i <= text.Length; i++)
        {
            var width = Font.MeasureText(text[..i]);
            if (width >= x)
            {
                var prevWidth = i > 1 ? Font.MeasureText(text[..(i - 1)]) : 0;
                return (x - prevWidth) < (width - x) ? i - 1 : i;
            }
        }

        return text.Length;
    }

    private static int FindPreviousWordBoundary(string text, int position)
    {
        if (position <= 0) return 0;

        var i = position - 1;
        while (i > 0 && char.IsWhiteSpace(text[i])) i--;
        while (i > 0 && !char.IsWhiteSpace(text[i - 1])) i--;
        return i;
    }

    private static int FindNextWordBoundary(string text, int position)
    {
        if (position >= text.Length) return text.Length;

        var i = position;
        while (i < text.Length && !char.IsWhiteSpace(text[i])) i++;
        while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
        return i;
    }

    private string[] GetLines()
    {
        var text = Text ?? string.Empty;
        if (string.IsNullOrEmpty(text)) return [string.Empty];
        return text.Split('\n');
    }

    private (int line, int column) GetLineAndColumn(int cursorPosition)
    {
        var text = Text ?? string.Empty;
        if (string.IsNullOrEmpty(text)) return (0, 0);

        var line = 0;
        var column = 0;
        for (var i = 0; i < cursorPosition && i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                line++;
                column = 0;
            }
            else
            {
                column++;
            }
        }
        return (line, column);
    }

    private int GetCursorPositionFromLineColumn(int line, int column)
    {
        var lines = GetLines();
        if (line < 0) line = 0;
        if (line >= lines.Length) line = lines.Length - 1;

        var position = 0;
        for (var i = 0; i < line; i++)
        {
            position += lines[i].Length + 1;
        }

        var maxColumn = lines[line].Length;
        column = Math.Min(column, maxColumn);
        position += column;

        return Math.Min(position, (Text?.Length ?? 0));
    }

    private int GetLineStartPosition(int line)
    {
        var lines = GetLines();
        if (line <= 0) return 0;

        var position = 0;
        for (var i = 0; i < line && i < lines.Length; i++)
        {
            position += lines[i].Length + 1;
        }
        return position;
    }

    private int GetLineEndPosition(int line)
    {
        var lines = GetLines();
        if (line < 0) return 0;
        if (line >= lines.Length) line = lines.Length - 1;

        var position = 0;
        for (var i = 0; i <= line && i < lines.Length; i++)
        {
            position += lines[i].Length;
            if (i < line) position++;
        }
        return position;
    }

    private void NotifyTextChanged()
    {
        _onTextChanged?.Invoke(Text);
        if (_setter.TryGetValue(nameof(Text), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(Text ?? string.Empty);
            }
        }
    }

    private void ResetCursorBlink()
    {
        _selectionTime = TimeProvider.System.GetLocalNow().LocalDateTime;
    }

    private void EnsureCursorVisible()
    {
        if (IsMultiLine)
        {
            EnsureCursorVisibleMultiLine();
        }
        else
        {
            EnsureCursorVisibleSingleLine();
        }
    }

    private void EnsureCursorVisibleSingleLine()
    {
        var text = Text ?? string.Empty;
        var displayText = IsPassword ? new string(PasswordChar, text.Length) : text;
        var textBeforeCursor = displayText[..Math.Min(_cursorPosition, displayText.Length)];
        var cursorX = Font.MeasureText(textBeforeCursor);

        var visibleWidth = ElementSize.Width - Padding.Left - Padding.Right;

        if (cursorX - _scrollOffset > visibleWidth - 10)
        {
            _scrollOffset = cursorX - visibleWidth + 20;
        }
        else if (cursorX - _scrollOffset < 10)
        {
            _scrollOffset = Math.Max(0, cursorX - 20);
        }
    }

    private void EnsureCursorVisibleMultiLine()
    {
        Font.GetFontMetrics(out var fontMetrics);
        var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;
        var lineSpacing = lineHeight * 0.2f;
        var totalLineHeight = lineHeight + lineSpacing;

        var (cursorLine, _) = GetLineAndColumn(_cursorPosition);
        var cursorY = cursorLine * totalLineHeight;

        var visibleHeight = ElementSize.Height - Padding.Top - Padding.Bottom;

        if (cursorY - _verticalScrollOffset > visibleHeight - lineHeight)
        {
            _verticalScrollOffset = cursorY - visibleHeight + lineHeight + lineSpacing;
        }
        else if (cursorY - _verticalScrollOffset < 0)
        {
            _verticalScrollOffset = Math.Max(0, cursorY);
        }
    }

    #endregion

    #region IScrollableControl

    public bool IsScrolling { get; set; }

    public void HandleScroll(float deltaX, float deltaY)
    {
        if (!IsMultiLine) return;

        var maxOffset = Math.Max(0, _totalContentHeight - (ElementSize.Height - Padding.Top - Padding.Bottom));
        _verticalScrollOffset = Math.Clamp(_verticalScrollOffset + deltaY, 0, maxOffset);
        UpdateScrollbar();
    }

    private void UpdateScrollbar()
    {
        if (!IsMultiLine) return;

        var viewportHeight = ElementSize.Height - Padding.Top - Padding.Bottom;
        var maxOffset = Math.Max(0, _totalContentHeight - viewportHeight);
        _scrollbar.UpdateScrollState(_verticalScrollOffset, maxOffset, viewportHeight, _totalContentHeight);
    }

    private bool NeedsScrollbar()
    {
        if (!IsMultiLine) return false;
        var viewportHeight = ElementSize.Height - Padding.Top - Padding.Bottom;
        return _totalContentHeight > viewportHeight;
    }

    #endregion

    #region Measure & Render

    protected override Margin? GetDebugPadding() => Padding;

    private float GetLineHeight()
    {
        Font.GetFontMetrics(out var fontMetrics);
        return fontMetrics.Descent - fontMetrics.Ascent + fontMetrics.Leading;
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        Font.GetFontMetrics(out var fontMetrics);
        var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;
        var lineSpacing = lineHeight * 0.2f;

        float height;
        if (IsMultiLine)
        {
            var lines = GetLines();
            var actualLineCount = lines.Length;
            var displayLineCount = Math.Max(actualLineCount, MinLines);
            if (MaxLines.HasValue)
            {
                displayLineCount = Math.Min(displayLineCount, MaxLines.Value);
            }

            _totalContentHeight = actualLineCount * lineHeight + (actualLineCount - 1) * lineSpacing;
            height = displayLineCount * lineHeight + (displayLineCount - 1) * lineSpacing + Padding.Top + Padding.Bottom;

            _scrollbar.Measure(new Size(_scrollbar.Width, height - Padding.Top - Padding.Bottom), true);
        }
        else
        {
            _totalContentHeight = lineHeight;
            height = DesiredSize?.Height ?? (lineHeight + Padding.Top + Padding.Bottom);
        }

        var width = DesiredSize?.Width ?? 200;

        return new Size(
            Math.Min(width, availableSize.Width),
            Math.Min(height, availableSize.Height));
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible) return;

        var rect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        canvas.Save();
        canvas.ClipRect(rect);

        if (IsMultiLine)
        {
            RenderMultiLine(canvas, rect);
            RenderScrollbar(canvas, rect);
        }
        else
        {
            RenderSingleLine(canvas, rect);
        }

        canvas.Restore();

        if (_isSelected)
        {
            RenderFocusBorder(canvas, rect);
        }
    }

    private void RenderScrollbar(SKCanvas canvas, SKRect rect)
    {
        if (!NeedsScrollbar()) return;

        UpdateScrollbar();

        var scrollbarX = rect.Right - _scrollbar.Width - 2;
        var scrollbarY = rect.Top + Padding.Top;
        var scrollbarHeight = rect.Height - Padding.Top - Padding.Bottom;

        _scrollbar.Arrange(new Rect(scrollbarX, scrollbarY, _scrollbar.Width, scrollbarHeight));

        var originalOffset = _scrollbar.VisualOffset;
        _scrollbar.SetVisualOffset(new Point(VisualOffset.X, VisualOffset.Y));
        _scrollbar.Render(canvas);
        _scrollbar.SetVisualOffset(originalOffset);
    }

    private void RenderSingleLine(SKCanvas canvas, SKRect rect)
    {
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;
        var baselineY = Position.Y + VisualOffset.Y + Padding.Top + textHeight + fontMetrics.Ascent / 4;

        var text = Text ?? string.Empty;
        var hasText = !string.IsNullOrEmpty(text);
        var displayText = hasText
            ? (IsPassword ? new string(PasswordChar, text.Length) : text)
            : (Placeholder ?? string.Empty);

        var showingPlaceholder = !hasText && !string.IsNullOrEmpty(Placeholder);
        var textX = Position.X + VisualOffset.X + Padding.Left - _scrollOffset;

        if (_isSelected && HasSelection && hasText)
        {
            RenderSelection(canvas, displayText, textX, baselineY, fontMetrics);
        }

        if (showingPlaceholder)
        {
            using var placeholderPaint = new SKPaint
            {
                Color = PlaceholderColor,
                IsAntialias = true
            };
            canvas.DrawText(displayText, textX, baselineY, Font, placeholderPaint);
        }
        else if (hasText)
        {
            canvas.DrawText(displayText, textX, baselineY, Font, Paint);
        }

        if (_isSelected)
        {
            RenderCursor(canvas, displayText, textX, baselineY, textHeight);
        }
    }

    private void RenderMultiLine(SKCanvas canvas, SKRect rect)
    {
        Font.GetFontMetrics(out var fontMetrics);
        var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;
        var lineSpacing = lineHeight * 0.2f;
        var totalLineHeight = lineHeight + lineSpacing;

        var text = Text ?? string.Empty;
        var hasText = !string.IsNullOrEmpty(text);
        var lines = hasText ? text.Split('\n') : [Placeholder ?? string.Empty];
        var showingPlaceholder = !hasText && !string.IsNullOrEmpty(Placeholder);

        var textX = Position.X + VisualOffset.X + Padding.Left;
        var startY = Position.Y + VisualOffset.Y + Padding.Top + lineHeight + fontMetrics.Ascent / 4 - _verticalScrollOffset;

        var (cursorLine, cursorColumn) = GetLineAndColumn(_cursorPosition);

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var baselineY = startY + lineIndex * totalLineHeight;

            if (baselineY + fontMetrics.Descent < rect.Top || baselineY + fontMetrics.Ascent > rect.Bottom)
                continue;

            var lineText = IsPassword && hasText ? new string(PasswordChar, lines[lineIndex].Length) : lines[lineIndex];

            if (_isSelected && HasSelection && hasText)
            {
                RenderLineSelection(canvas, lineIndex, lines, textX, baselineY, fontMetrics);
            }

            if (showingPlaceholder && lineIndex == 0)
            {
                using var placeholderPaint = new SKPaint
                {
                    Color = PlaceholderColor,
                    IsAntialias = true
                };
                canvas.DrawText(lineText, textX, baselineY, Font, placeholderPaint);
            }
            else if (hasText)
            {
                canvas.DrawText(lineText, textX, baselineY, Font, Paint);
            }

            if (_isSelected && lineIndex == cursorLine)
            {
                RenderCursorAtLine(canvas, lineText, cursorColumn, textX, baselineY, lineHeight);
            }
        }
    }

    private void RenderLineSelection(SKCanvas canvas, int lineIndex, string[] lines, float textX, float baselineY, SKFontMetrics fontMetrics)
    {
        var lineStartPos = 0;
        for (var i = 0; i < lineIndex; i++)
        {
            lineStartPos += lines[i].Length + 1;
        }
        var lineEndPos = lineStartPos + lines[lineIndex].Length;

        var selMin = SelectionMin;
        var selMax = SelectionMax;

        if (selMax <= lineStartPos || selMin >= lineEndPos)
            return;

        var lineSelStart = Math.Max(0, selMin - lineStartPos);
        var lineSelEnd = Math.Min(lines[lineIndex].Length, selMax - lineStartPos);

        var lineText = IsPassword ? new string(PasswordChar, lines[lineIndex].Length) : lines[lineIndex];
        var startX = textX + (lineSelStart > 0 ? Font.MeasureText(lineText[..lineSelStart]) : 0);
        var endX = textX + Font.MeasureText(lineText[..lineSelEnd]);

        var selRect = new SKRect(
            startX,
            baselineY + fontMetrics.Ascent,
            endX,
            baselineY + fontMetrics.Descent);

        using var selPaint = new SKPaint
        {
            Color = SelectionColor,
            IsAntialias = true
        };
        canvas.DrawRect(selRect, selPaint);
    }

    private void RenderCursorAtLine(SKCanvas canvas, string lineText, int column, float textX, float baselineY, float lineHeight)
    {
        var elapsed = (DateTime.Now - _selectionTime).TotalMilliseconds;
        if ((elapsed % 1000) >= 500) return;

        var cursorCol = Math.Min(column, lineText.Length);
        var textBeforeCursor = cursorCol > 0 ? lineText[..cursorCol] : string.Empty;
        var cursorX = textX + Font.MeasureText(textBeforeCursor);

        using var cursorPaint = new SKPaint
        {
            Color = Paint.Color,
            StrokeWidth = 2,
            IsAntialias = true
        };

        var cursorTop = baselineY - lineHeight * 0.8f;
        var cursorBottom = baselineY + lineHeight * 0.2f;
        canvas.DrawLine(cursorX, cursorTop, cursorX, cursorBottom, cursorPaint);
    }

    private void RenderSelection(SKCanvas canvas, string displayText, float textX, float baselineY, SKFontMetrics fontMetrics)
    {
        var selMin = Math.Min(SelectionMin, displayText.Length);
        var selMax = Math.Min(SelectionMax, displayText.Length);

        var startX = textX + (selMin > 0 ? Font.MeasureText(displayText[..selMin]) : 0);
        var endX = textX + Font.MeasureText(displayText[..selMax]);

        var selRect = new SKRect(
            startX,
            baselineY + fontMetrics.Ascent,
            endX,
            baselineY + fontMetrics.Descent);

        using var selPaint = new SKPaint
        {
            Color = SelectionColor,
            IsAntialias = true
        };
        canvas.DrawRect(selRect, selPaint);
    }

    private void RenderCursor(SKCanvas canvas, string displayText, float textX, float baselineY, float textHeight)
    {
        var elapsed = (DateTime.Now - _selectionTime).TotalMilliseconds;
        if ((elapsed % 1000) >= 500) return;

        var cursorPos = Math.Min(_cursorPosition, displayText.Length);
        var textBeforeCursor = cursorPos > 0 ? displayText[..cursorPos] : string.Empty;
        var cursorX = textX + Font.MeasureText(textBeforeCursor);

        using var cursorPaint = new SKPaint
        {
            Color = Paint.Color,
            StrokeWidth = 2,
            IsAntialias = true
        };

        var cursorTop = baselineY - textHeight * 0.8f;
        var cursorBottom = baselineY + textHeight * 0.2f;
        canvas.DrawLine(cursorX, cursorTop, cursorX, cursorBottom, cursorPaint);
    }

    private void RenderFocusBorder(SKCanvas canvas, SKRect rect)
    {
        if (FocusBorderThickness <= 0) return;

        using var borderPaint = new SKPaint
        {
            Color = FocusBorderColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = FocusBorderThickness,
            IsAntialias = true
        };

        var borderRect = new SKRect(
            rect.Left + FocusBorderThickness / 2,
            rect.Top + FocusBorderThickness / 2,
            rect.Right - FocusBorderThickness / 2,
            rect.Bottom - FocusBorderThickness / 2);

        var cornerRadius = CornerRadius;
        if (cornerRadius > 0)
        {
            canvas.DrawRoundRect(borderRect, cornerRadius, cornerRadius, borderPaint);
        }
        else
        {
            canvas.DrawRect(borderRect, borderPaint);
        }
    }

    public override UiElement? HitTest(Point point)
    {
        if (!IsVisible) return null;

        if (!(point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
              point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height))
        {
            return null;
        }

        if (IsMultiLine && NeedsScrollbar())
        {
            var scrollbarHit = _scrollbar.HitTest(point);
            if (scrollbarHit != null) return scrollbarHit;
        }

        return this;
    }

    #endregion
}
