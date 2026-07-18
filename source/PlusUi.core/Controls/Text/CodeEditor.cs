using PlusUi.core.Attributes;
using PlusUi.core.Services;
using PlusUi.core.UiPropGen;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A multi-line text editor for code, with syntax highlighting driven by a highlighter callback.
/// </summary>
/// <remarks>
/// The document is always a plain string. Styling is not stored: whenever the text changes the
/// highlighter is re-run over the whole document and returns <see cref="StyleSpan"/>s addressed by
/// absolute character offsets. This keeps every cursor and selection index valid across edits,
/// because no stored offsets have to be remapped when text is inserted or removed.
///
/// Tabs are normalized to <see cref="TabSize"/> spaces on every input path so that character
/// indices map one-to-one onto rendered columns.
/// </remarks>
[GenerateShadowMethods]
[UiPropGenPadding]
public partial class CodeEditor : UiTextElement, ITextInputControl, IFocusable, IScrollableControl, IKeyboardInputHandler
{
    #region Types

    private readonly record struct RunStyleKey(Color Color, FontWeight Weight, FontStyle Style);

    private sealed class RunPaintEntry
    {
        public required SKPaint Paint { get; init; }
        public required SKFont Font { get; init; }
    }

    /// <summary>A contiguous run of characters on one line that shares a single style.</summary>
    internal readonly record struct LineFragment(int Offset, int Length, StyleSpan? Style);

    /// <summary>One line's cached highlighting, valid while its text and incoming state are unchanged.</summary>
    private sealed class LineCacheEntry
    {
        public string Text = string.Empty;
        public int StateIn;
        public int StateOut;
        public List<StyleSpan> Spans = [];
    }

    #endregion

    #region Fields

    private readonly IClipboardService? _clipboard;
    private readonly Scrollbar _scrollbar;
    private readonly Dictionary<RunStyleKey, RunPaintEntry> _paintCache = [];

    private bool _isSelected;
    private DateTime _caretTime;
    private int _cursorPosition;
    private int _selectionStart = -1;
    private int _selectionEnd = -1;
    private float _scrollOffset;
    private float _verticalScrollOffset;
    private float _totalContentHeight;

    private Func<string, List<StyleSpan>>? _highlighter;
    private string? _highlightedText;
    private List<StyleSpan> _spans = [];

    private LineHighlighter? _lineHighlighter;
    private readonly List<LineCacheEntry> _lineCache = [];

    private string? _cachedLinesText;
    private string[] _cachedLines = [string.Empty];

    private Action<string?>? _onTextChanged;

    #endregion

    public CodeEditor()
    {
        _clipboard = ServiceProviderService.ServiceProvider?.GetService<IClipboardService>();

        _scrollbar = new Scrollbar()
            .SetOrientation(ScrollbarOrientation.Vertical)
            .SetWidth(8)
            .SetAutoHide(true)
            .SetOnValueChanged(offset => _verticalScrollOffset = offset);
        _scrollbar.Parent = this;

        SetBackground(new SolidColorBackground(PlusUiDefaults.BackgroundInput));
        SetCornerRadius(PlusUiDefaults.CornerRadius);
        SetDesiredWidth(480);
        SetDesiredHeight(320);
        SetHighContrastBackground(PlusUiDefaults.HcInputBackground);
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
        Padding = new Margin(8, 6);
        TextWrapping = TextWrapping.NoWrap;
    }

    protected internal override bool IsFocusable => true;
    public override bool InterceptsClicks => true;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.TextInput;

    public override string? GetComputedAccessibilityLabel() => AccessibilityLabel ?? "Code editor";
    public override string? GetComputedAccessibilityValue() => AccessibilityValue ?? Text;

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

    #region TabSize

    internal int TabSize { get; set; } = 4;

    public CodeEditor SetTabSize(int tabSize)
    {
        TabSize = Math.Max(1, tabSize);
        return this;
    }

    public CodeEditor BindTabSize(Expression<Func<int>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TabSize = Math.Max(1, getter()));
        return this;
    }

    #endregion

    #region AutoIndent

    internal bool AutoIndent { get; set; } = true;

    public CodeEditor SetAutoIndent(bool autoIndent)
    {
        AutoIndent = autoIndent;
        return this;
    }

    public CodeEditor BindAutoIndent(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AutoIndent = getter());
        return this;
    }

    #endregion

    #region ShowLineNumbers

    internal bool ShowLineNumbers { get; set; } = true;

    public CodeEditor SetShowLineNumbers(bool showLineNumbers)
    {
        ShowLineNumbers = showLineNumbers;
        InvalidateMeasure();
        return this;
    }

    public CodeEditor BindShowLineNumbers(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            ShowLineNumbers = getter();
            InvalidateMeasure();
        });
        return this;
    }

    #endregion

    #region LineNumberColor

    internal Color LineNumberColor { get; set; } = PlusUiDefaults.TextPlaceholder;

    public CodeEditor SetLineNumberColor(Color color)
    {
        LineNumberColor = color;
        return this;
    }

    public CodeEditor BindLineNumberColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => LineNumberColor = getter());
        return this;
    }

    #endregion

    #region CurrentLineColor

    internal Color CurrentLineColor { get; set; } = new(255, 255, 255, 12);

    public CodeEditor SetCurrentLineColor(Color color)
    {
        CurrentLineColor = color;
        return this;
    }

    public CodeEditor BindCurrentLineColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => CurrentLineColor = getter());
        return this;
    }

    #endregion

    #region SelectionColor

    internal Color SelectionColor { get; set; } = new(100, 149, 237, 100);

    public CodeEditor SetSelectionColor(Color color)
    {
        SelectionColor = color;
        return this;
    }

    public CodeEditor BindSelectionColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectionColor = getter());
        return this;
    }

    #endregion

    #region CaretColor

    internal Color CaretColor { get; set; } = PlusUiDefaults.TextPrimary;

    public CodeEditor SetCaretColor(Color color)
    {
        CaretColor = color;
        return this;
    }

    public CodeEditor BindCaretColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => CaretColor = getter());
        return this;
    }

    #endregion

    #region IsReadOnly

    internal bool IsReadOnly { get; set; }

    public CodeEditor SetIsReadOnly(bool isReadOnly)
    {
        IsReadOnly = isReadOnly;
        return this;
    }

    public CodeEditor BindIsReadOnly(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsReadOnly = getter());
        return this;
    }

    #endregion

    #endregion

    #region Highlighter

    /// <summary>
    /// Sets a whole-document highlighter, invoked whenever the text changes.
    /// Spans use absolute document offsets.
    /// </summary>
    /// <remarks>
    /// Simple, but it re-tokenizes the entire document on every edit. For code, prefer
    /// <see cref="SetLineHighlighter"/>, which only re-runs the lines that actually changed.
    /// </remarks>
    public CodeEditor SetHighlighter(Func<string, List<StyleSpan>> highlighter)
    {
        _highlighter = highlighter;
        _lineHighlighter = null;
        InvalidateHighlight();
        return this;
    }

    public CodeEditor BindHighlighter(Expression<Func<Func<string, List<StyleSpan>>>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            _highlighter = getter();
            _lineHighlighter = null;
            InvalidateHighlight();
        });
        return this;
    }

    /// <summary>
    /// Sets a line-based highlighter. Each line is tokenized with the state the previous line ended
    /// in, and results are cached per line, so an edit re-tokenizes only the affected lines.
    /// </summary>
    public CodeEditor SetLineHighlighter(LineHighlighter highlighter)
    {
        _lineHighlighter = highlighter;
        _highlighter = null;
        InvalidateHighlight();
        return this;
    }

    public CodeEditor BindLineHighlighter(Expression<Func<LineHighlighter>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            _lineHighlighter = getter();
            _highlighter = null;
            InvalidateHighlight();
        });
        return this;
    }

    private void InvalidateHighlight()
    {
        _highlightedText = null;
        _lineCache.Clear();
        ClearPaintCache();
    }

    /// <summary>Spans for the whole document, in absolute offsets. Empty when a line highlighter is used.</summary>
    internal List<StyleSpan> GetSpans()
    {
        var text = Text ?? string.Empty;
        if (_highlighter == null)
        {
            return [];
        }
        if (_highlightedText == text)
        {
            return _spans;
        }

        try
        {
            _spans = _highlighter(text) ?? [];
        }
        catch
        {
            // A faulty highlighter must never take the editor down - fall back to unstyled text.
            _spans = [];
        }
        _highlightedText = text;
        return _spans;
    }

    /// <summary>
    /// Brings the per-line highlight cache up to date. Lines whose text and incoming state are
    /// unchanged keep their cached spans; everything from the first genuine change onward is
    /// re-tokenized until the carried state lines up again.
    /// </summary>
    private void EnsureLineHighlight()
    {
        if (_lineHighlighter == null)
        {
            return;
        }

        var text = Text ?? string.Empty;
        if (_highlightedText == text)
        {
            return;
        }

        var lines = GetLines();
        var state = 0;

        for (var i = 0; i < lines.Length; i++)
        {
            var lineText = lines[i];

            if (i < _lineCache.Count)
            {
                var cached = _lineCache[i];
                if (cached.StateIn == state && string.Equals(cached.Text, lineText, StringComparison.Ordinal))
                {
                    state = cached.StateOut;
                    continue;
                }
            }
            else
            {
                _lineCache.Add(new LineCacheEntry());
            }

            var entry = _lineCache[i];
            entry.Text = lineText;
            entry.StateIn = state;
            entry.Spans.Clear();

            try
            {
                state = _lineHighlighter(lineText, i, state, entry.Spans);
            }
            catch
            {
                // A faulty highlighter must never take the editor down - leave the line unstyled
                // and reset the carried state so one bad line cannot corrupt the rest of the file.
                entry.Spans.Clear();
                state = 0;
            }

            entry.StateOut = state;
        }

        if (_lineCache.Count > lines.Length)
        {
            _lineCache.RemoveRange(lines.Length, _lineCache.Count - lines.Length);
        }

        _highlightedText = text;
    }

    /// <summary>Cached spans for one line, in offsets relative to that line's start.</summary>
    internal IReadOnlyList<StyleSpan> GetLineSpans(int lineIndex)
    {
        EnsureLineHighlight();
        return lineIndex >= 0 && lineIndex < _lineCache.Count ? _lineCache[lineIndex].Spans : [];
    }

    #endregion

    #region Text

    public new CodeEditor SetText(string text)
    {
        Text = ExpandTabs(text, TabSize);
        return this;
    }

    public new CodeEditor BindText(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Text = ExpandTabs(getter() ?? string.Empty, TabSize));
        return this;
    }

    /// <summary>Two-way text binding: the setter is invoked whenever the user edits the document.</summary>
    public CodeEditor BindText(Expression<Func<string?>> propertyExpression, Action<string> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () =>
        {
            Text = ExpandTabs(getter() ?? string.Empty, TabSize);
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

    public CodeEditor SetOnTextChanged(Action<string?> callback)
    {
        _onTextChanged = callback;
        return this;
    }

    internal static string ExpandTabs(string text, int tabSize)
        => string.IsNullOrEmpty(text) || !text.Contains('\t')
            ? text
            : text.Replace("\t", new string(' ', Math.Max(1, tabSize)));

    private void NotifyTextChanged()
    {
        InvalidateMeasure();
        _onTextChanged?.Invoke(Text);
        if (_setter.TryGetValue(nameof(Text), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(Text ?? string.Empty);
            }
        }
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
        }
    }

    private void ExtendSelection() => _selectionEnd = _cursorPosition;

    private string? GetSelectedText()
    {
        if (!HasSelection) return null;
        var text = Text ?? string.Empty;
        return text[SelectionMin..Math.Min(SelectionMax, text.Length)];
    }

    private void DeleteSelection()
    {
        if (!HasSelection) return;
        var text = Text ?? string.Empty;
        var min = SelectionMin;
        var max = Math.Min(SelectionMax, text.Length);
        Text = text[..min] + text[max..];
        _cursorPosition = min;
        ClearSelection();
        NotifyTextChanged();
    }

    private void SelectAll()
    {
        _selectionStart = 0;
        _selectionEnd = (Text ?? string.Empty).Length;
        _cursorPosition = _selectionEnd;
    }

    #endregion

    #region Clipboard

    private void Copy()
    {
        var selected = GetSelectedText();
        if (!string.IsNullOrEmpty(selected))
        {
            _clipboard?.SetText(selected);
        }
    }

    private void Cut()
    {
        if (IsReadOnly) return;
        Copy();
        DeleteSelection();
    }

    private void Paste()
    {
        if (IsReadOnly) return;

        var clipboardText = _clipboard?.GetText();
        if (string.IsNullOrEmpty(clipboardText)) return;

        clipboardText = ExpandTabs(clipboardText.Replace("\r\n", "\n").Replace("\r", "\n"), TabSize);

        if (HasSelection)
        {
            DeleteSelection();
        }

        var text = Text ?? string.Empty;
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
            _caretTime = TimeProvider.System.GetLocalNow().LocalDateTime;
        }
        _isSelected = isSelected;

        if (!isSelected)
        {
            ClearSelection();
        }
    }

    /// <summary>
    /// Claims Tab and Shift+Tab while focused so they indent instead of moving focus.
    /// </summary>
    public bool HandleKeyboardInput(PlusKey key)
    {
        if (IsReadOnly || !_isSelected)
        {
            return false;
        }

        switch (key)
        {
            case PlusKey.Tab:
                IndentSelection();
                return true;
            case PlusKey.ShiftTab:
                OutdentSelection();
                return true;
            default:
                return false;
        }
    }

    public void HandleInput(PlusKey key, bool shift, bool ctrl)
    {
        var text = Text ?? string.Empty;

        switch (key)
        {
            case PlusKey.Backspace:
                if (IsReadOnly) break;
                if (HasSelection)
                {
                    DeleteSelection();
                }
                else if (_cursorPosition > 0)
                {
                    var removed = DeleteIndentBefore(text);
                    Text = text[..(_cursorPosition - removed)] + text[_cursorPosition..];
                    _cursorPosition -= removed;
                    NotifyTextChanged();
                }
                break;

            case PlusKey.Delete:
                if (IsReadOnly) break;
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
                if (IsReadOnly) break;
                if (HasSelection)
                {
                    DeleteSelection();
                    text = Text ?? string.Empty;
                }
                var insert = "\n" + (AutoIndent ? GetCurrentIndent(text) : string.Empty);
                Text = text[.._cursorPosition] + insert + text[_cursorPosition..];
                _cursorPosition += insert.Length;
                NotifyTextChanged();
                break;

            case PlusKey.Tab:
                if (IsReadOnly) break;
                IndentSelection();
                break;

            case PlusKey.ShiftTab:
                if (IsReadOnly) break;
                OutdentSelection();
                break;

            case PlusKey.ArrowLeft:
                if (shift) BeginSelection();
                if (ctrl)
                    _cursorPosition = FindPreviousWordBoundary(text, _cursorPosition);
                else if (_cursorPosition > 0)
                    _cursorPosition--;
                if (shift) ExtendSelection(); else ClearSelection();
                break;

            case PlusKey.ArrowRight:
                if (shift) BeginSelection();
                if (ctrl)
                    _cursorPosition = FindNextWordBoundary(text, _cursorPosition);
                else if (_cursorPosition < text.Length)
                    _cursorPosition++;
                if (shift) ExtendSelection(); else ClearSelection();
                break;

            case PlusKey.ArrowUp:
                {
                    if (shift) BeginSelection();
                    var (line, column) = GetLineAndColumn(_cursorPosition);
                    if (line > 0)
                        _cursorPosition = GetCursorPositionFromLineColumn(line - 1, column);
                    if (shift) ExtendSelection(); else ClearSelection();
                }
                break;

            case PlusKey.ArrowDown:
                {
                    if (shift) BeginSelection();
                    var (line, column) = GetLineAndColumn(_cursorPosition);
                    if (line < GetLines().Length - 1)
                        _cursorPosition = GetCursorPositionFromLineColumn(line + 1, column);
                    if (shift) ExtendSelection(); else ClearSelection();
                }
                break;

            case PlusKey.Home:
                {
                    if (shift) BeginSelection();
                    if (ctrl)
                    {
                        _cursorPosition = 0;
                    }
                    else
                    {
                        var (line, _) = GetLineAndColumn(_cursorPosition);
                        _cursorPosition = GetSmartHomePosition(line);
                    }
                    if (shift) ExtendSelection(); else ClearSelection();
                }
                break;

            case PlusKey.End:
                {
                    if (shift) BeginSelection();
                    if (ctrl)
                    {
                        _cursorPosition = text.Length;
                    }
                    else
                    {
                        var (line, _) = GetLineAndColumn(_cursorPosition);
                        _cursorPosition = GetLineEndPosition(line);
                    }
                    if (shift) ExtendSelection(); else ClearSelection();
                }
                break;

            case PlusKey.A:
                if (ctrl) SelectAll();
                break;

            case PlusKey.C:
                if (ctrl) Copy();
                break;

            case PlusKey.X:
                if (ctrl) Cut();
                break;

            case PlusKey.V:
                if (ctrl) Paste();
                break;
        }

        EnsureCursorVisible();
        ResetCaretBlink();
    }

    public void HandleInput(char chr)
    {
        if (IsReadOnly) return;

        if (chr == '\t')
        {
            IndentSelection();
            EnsureCursorVisible();
            ResetCaretBlink();
            return;
        }

        if (char.IsControl(chr)) return;

        if (HasSelection)
        {
            DeleteSelection();
        }

        var text = Text ?? string.Empty;
        Text = text[.._cursorPosition] + chr + text[_cursorPosition..];
        _cursorPosition++;
        NotifyTextChanged();
        EnsureCursorVisible();
        ResetCaretBlink();
    }

    public void HandleClick(float localX, float localY)
    {
        var lines = GetLines();
        var lineHeight = GetLineHeight();

        var clickedLine = (int)((localY - Padding.Top + _verticalScrollOffset) / lineHeight);
        clickedLine = Math.Clamp(clickedLine, 0, lines.Length - 1);

        var textLeft = Padding.Left + GetGutterWidth();
        var columnInLine = GetCharacterIndexAtX(lines[clickedLine], localX - textLeft + _scrollOffset);

        _cursorPosition = GetCursorPositionFromLineColumn(clickedLine, columnInLine);
        ClearSelection();
        ResetCaretBlink();
    }

    #endregion

    #region Indentation

    /// <summary>
    /// Indents every line touched by the selection, or inserts a single indent step at the caret
    /// when the selection does not span multiple lines.
    /// </summary>
    private void IndentSelection()
    {
        var text = Text ?? string.Empty;
        var indent = new string(' ', TabSize);

        if (HasSelection)
        {
            var (firstLine, lastLine) = GetSelectedLineRange();
            if (firstLine != lastLine)
            {
                ApplyToLines(firstLine, lastLine, line => indent + line, indent.Length);
                return;
            }
            DeleteSelection();
            text = Text ?? string.Empty;
        }

        Text = text[.._cursorPosition] + indent + text[_cursorPosition..];
        _cursorPosition += indent.Length;
        NotifyTextChanged();
    }

    /// <summary>Removes up to one indent step from the start of every line touched by the caret or selection.</summary>
    private void OutdentSelection()
    {
        var (firstLine, lastLine) = HasSelection
            ? GetSelectedLineRange()
            : (GetLineAndColumn(_cursorPosition).line, GetLineAndColumn(_cursorPosition).line);

        ApplyToLines(firstLine, lastLine, line =>
        {
            var toRemove = 0;
            while (toRemove < TabSize && toRemove < line.Length && line[toRemove] == ' ')
            {
                toRemove++;
            }
            return line[toRemove..];
        }, delta: null);
    }

    /// <summary>
    /// Rewrites a range of lines and keeps the caret and selection anchored to the same text.
    /// </summary>
    private void ApplyToLines(int firstLine, int lastLine, Func<string, string> transform, int? delta)
    {
        var lines = GetLines();
        var cursorShift = 0;
        var startShift = 0;
        var endShift = 0;

        var cursorLine = GetLineAndColumn(_cursorPosition).line;
        var selStartLine = _selectionStart >= 0 ? GetLineAndColumn(_selectionStart).line : -1;
        var selEndLine = _selectionEnd >= 0 ? GetLineAndColumn(_selectionEnd).line : -1;

        for (var i = firstLine; i <= lastLine && i < lines.Length; i++)
        {
            var original = lines[i];
            var replaced = transform(original);
            var lineDelta = delta ?? (replaced.Length - original.Length);
            lines[i] = replaced;

            if (i <= cursorLine) cursorShift += lineDelta;
            if (selStartLine >= 0 && i <= selStartLine) startShift += lineDelta;
            if (selEndLine >= 0 && i <= selEndLine) endShift += lineDelta;
        }

        Text = string.Join('\n', lines);

        var length = (Text ?? string.Empty).Length;
        _cursorPosition = Math.Clamp(_cursorPosition + cursorShift, 0, length);
        if (_selectionStart >= 0) _selectionStart = Math.Clamp(_selectionStart + startShift, 0, length);
        if (_selectionEnd >= 0) _selectionEnd = Math.Clamp(_selectionEnd + endShift, 0, length);

        NotifyTextChanged();
    }

    private (int first, int last) GetSelectedLineRange()
    {
        var (first, _) = GetLineAndColumn(SelectionMin);
        var (last, _) = GetLineAndColumn(SelectionMax);
        return (first, last);
    }

    /// <summary>Returns the leading whitespace of the line the caret sits on.</summary>
    private static string GetCurrentIndent(string text, int cursorPosition)
    {
        var lineStart = text.LastIndexOf('\n', Math.Max(0, Math.Min(cursorPosition, text.Length) - 1)) + 1;
        var i = lineStart;
        while (i < text.Length && i < cursorPosition && (text[i] == ' ' || text[i] == '\t'))
        {
            i++;
        }
        return text[lineStart..i];
    }

    private string GetCurrentIndent(string text) => GetCurrentIndent(text, _cursorPosition);

    /// <summary>
    /// Backspace inside leading indentation removes a whole indent step, otherwise a single character.
    /// </summary>
    private int DeleteIndentBefore(string text)
    {
        var lineStart = text.LastIndexOf('\n', Math.Max(0, _cursorPosition - 1)) + 1;
        var before = text[lineStart.._cursorPosition];

        if (before.Length == 0 || before.Any(c => c != ' '))
        {
            return 1;
        }

        var stepRemainder = before.Length % TabSize;
        var step = stepRemainder == 0 ? TabSize : stepRemainder;
        return Math.Min(step, before.Length);
    }

    /// <summary>Home jumps to the first non-whitespace character, and to column 0 when already there.</summary>
    private int GetSmartHomePosition(int line)
    {
        var lineStart = GetLineStartPosition(line);
        var lines = GetLines();
        var lineText = lines[Math.Clamp(line, 0, lines.Length - 1)];

        var indent = 0;
        while (indent < lineText.Length && lineText[indent] == ' ')
        {
            indent++;
        }

        return _cursorPosition == lineStart + indent ? lineStart : lineStart + indent;
    }

    #endregion

    #region Line math

    /// <summary>
    /// The document split into lines. Cached, because this is hit repeatedly per frame and on every
    /// cursor movement - re-splitting a large document each time dominates everything else.
    /// </summary>
    internal string[] GetLines()
    {
        var text = Text ?? string.Empty;
        if (!ReferenceEquals(_cachedLinesText, text) && _cachedLinesText != text)
        {
            _cachedLines = text.Length == 0 ? [string.Empty] : text.Split('\n');
            _cachedLinesText = text;
        }
        return _cachedLines;
    }

    internal (int line, int column) GetLineAndColumn(int cursorPosition)
    {
        var text = Text ?? string.Empty;
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

    internal int GetCursorPositionFromLineColumn(int line, int column)
    {
        var lines = GetLines();
        line = Math.Clamp(line, 0, lines.Length - 1);

        var position = 0;
        for (var i = 0; i < line; i++)
        {
            position += lines[i].Length + 1;
        }

        position += Math.Min(column, lines[line].Length);
        return Math.Min(position, (Text ?? string.Empty).Length);
    }

    private int GetLineStartPosition(int line)
    {
        if (line <= 0) return 0;
        var lines = GetLines();
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
        line = Math.Clamp(line, 0, lines.Length - 1);
        return GetLineStartPosition(line) + lines[line].Length;
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

    #endregion

    #region Span slicing

    /// <summary>
    /// Cuts the styled spans covering a document into the fragments that fall on one line.
    /// Offsets in the result are relative to the start of the line. Gaps between spans become
    /// fragments with a null style, meaning "use the editor's own text style".
    /// Overlapping spans are resolved first-wins by start offset.
    /// </summary>
    internal static List<LineFragment> SliceLine(int lineStart, int lineLength, IReadOnlyList<StyleSpan> spans)
    {
        var result = new List<LineFragment>();
        if (lineLength <= 0)
        {
            return result;
        }

        var lineEnd = lineStart + lineLength;

        var hits = new List<(int Start, int End, StyleSpan Span)>();
        foreach (var span in spans)
        {
            if (span.Length <= 0) continue;
            var start = Math.Max(span.Start, lineStart);
            var end = Math.Min(span.End, lineEnd);
            if (end > start)
            {
                hits.Add((start, end, span));
            }
        }

        hits.Sort((a, b) => a.Start != b.Start ? a.Start.CompareTo(b.Start) : b.End.CompareTo(a.End));

        var cursor = lineStart;
        foreach (var hit in hits)
        {
            if (hit.End <= cursor) continue;

            var start = Math.Max(hit.Start, cursor);
            if (start > cursor)
            {
                result.Add(new LineFragment(cursor - lineStart, start - cursor, null));
            }

            result.Add(new LineFragment(start - lineStart, hit.End - start, hit.Span));
            cursor = hit.End;
        }

        if (cursor < lineEnd)
        {
            result.Add(new LineFragment(cursor - lineStart, lineEnd - cursor, null));
        }

        return result;
    }

    #endregion

    #region Paint cache

    private (SKPaint paint, SKFont font) GetOrCreatePaint(RunStyleKey key)
    {
        if (_paintCache.TryGetValue(key, out var entry))
        {
            return (entry.Paint, entry.Font);
        }

        SKTypeface? typeface = null;
        try
        {
            typeface = FontRegistry?.GetTypeface(FontFamily, key.Weight, key.Style);
        }
        catch
        {
            // Fall back to the default typeface.
        }

        var (paint, font) = PaintRegistry.GetOrCreate(color: key.Color, size: TextSize, typeface: typeface);
        _paintCache[key] = new RunPaintEntry { Paint = paint, Font = font };
        return (paint, font);
    }

    private void ClearPaintCache()
    {
        foreach (var entry in _paintCache.Values)
        {
            PaintRegistry.Release(entry.Paint, entry.Font);
        }
        _paintCache.Clear();
    }

    private RunStyleKey GetStyleKey(StyleSpan? span) => new(
        span?.Color ?? TextColor,
        span?.FontWeight ?? FontWeight,
        span?.FontStyle ?? FontStyle);

    #endregion

    #region Scrolling

    public bool IsScrolling { get; set; }

    /// <summary>Current vertical scroll offset in pixels. Internal for tests.</summary>
    internal float VerticalScrollOffset => _verticalScrollOffset;

    /// <summary>Current horizontal scroll offset in pixels. Internal for tests.</summary>
    internal float HorizontalScrollOffset => _scrollOffset;

    public void HandleScroll(float deltaX, float deltaY)
    {
        // Deltas are added, matching Entry and ScrollView. Subtracting here inverts the wheel.
        var maxScroll = Math.Max(0, _totalContentHeight - (ElementSize.Height - Padding.Top - Padding.Bottom));
        _verticalScrollOffset = Math.Clamp(_verticalScrollOffset + deltaY, 0, maxScroll);
        _scrollOffset = Math.Max(0, _scrollOffset + deltaX);
    }

    private void EnsureCursorVisible()
    {
        var lineHeight = GetLineHeight();
        var (cursorLine, cursorColumn) = GetLineAndColumn(_cursorPosition);

        var cursorY = cursorLine * lineHeight;
        var visibleHeight = ElementSize.Height - Padding.Top - Padding.Bottom;
        if (cursorY - _verticalScrollOffset > visibleHeight - lineHeight)
        {
            _verticalScrollOffset = cursorY - visibleHeight + lineHeight;
        }
        else if (cursorY - _verticalScrollOffset < 0)
        {
            _verticalScrollOffset = Math.Max(0, cursorY);
        }

        var lines = GetLines();
        var lineText = lines[Math.Clamp(cursorLine, 0, lines.Length - 1)];
        var cursorX = Font.MeasureText(lineText[..Math.Min(cursorColumn, lineText.Length)]);
        var visibleWidth = ElementSize.Width - Padding.Left - Padding.Right - GetGutterWidth();
        if (cursorX - _scrollOffset > visibleWidth - 10)
        {
            _scrollOffset = cursorX - visibleWidth + 20;
        }
        else if (cursorX - _scrollOffset < 10)
        {
            _scrollOffset = Math.Max(0, cursorX - 20);
        }
    }

    private void ResetCaretBlink() => _caretTime = TimeProvider.System.GetLocalNow().LocalDateTime;

    private bool NeedsScrollbar()
        => _totalContentHeight > ElementSize.Height - Padding.Top - Padding.Bottom;

    private void UpdateScrollbar()
    {
        var viewportHeight = ElementSize.Height - Padding.Top - Padding.Bottom;
        var maxOffset = Math.Max(0, _totalContentHeight - viewportHeight);
        _scrollbar.UpdateScrollState(_verticalScrollOffset, maxOffset, viewportHeight, _totalContentHeight);
    }

    #endregion

    #region Measure & Render

    protected override Margin? GetDebugPadding() => Padding;

    private float GetLineHeight()
    {
        Font.GetFontMetrics(out var metrics);
        return metrics.Descent - metrics.Ascent + metrics.Leading;
    }

    /// <summary>Width reserved for the line-number gutter, including its separating padding.</summary>
    internal float GetGutterWidth()
    {
        if (!ShowLineNumbers) return 0;

        var digits = Math.Max(2, GetLines().Length.ToString().Length);
        return Font.MeasureText(new string('9', digits)) + 12;
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var lines = GetLines();
        var lineHeight = GetLineHeight();

        _totalContentHeight = lines.Length * lineHeight;

        var height = DesiredSize?.Height ?? (_totalContentHeight + Padding.Top + Padding.Bottom);
        var width = DesiredSize?.Width ?? 480;

        _scrollbar.Measure(new Size(_scrollbar.Width, height - Padding.Top - Padding.Bottom), true);

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

        var lines = GetLines();
        var spans = GetSpans();
        var lineHeight = GetLineHeight();
        Font.GetFontMetrics(out var metrics);

        var gutterWidth = GetGutterWidth();
        var textLeft = rect.Left + Padding.Left + gutterWidth - _scrollOffset;
        var (caretLine, _) = GetLineAndColumn(_cursorPosition);

        var firstVisible = Math.Max(0, (int)(_verticalScrollOffset / lineHeight));
        var visibleCount = (int)((ElementSize.Height - Padding.Top - Padding.Bottom) / lineHeight) + 2;
        var lastVisible = Math.Min(lines.Length - 1, firstVisible + visibleCount);

        var lineStart = GetLineStartPosition(firstVisible);

        for (var i = firstVisible; i <= lastVisible; i++)
        {
            var lineText = lines[i];
            var lineTop = rect.Top + Padding.Top + i * lineHeight - _verticalScrollOffset;
            var baseline = lineTop - metrics.Ascent;

            if (i == caretLine && _isSelected)
            {
                RenderCurrentLine(canvas, rect, lineTop, lineHeight);
            }

            RenderLineSelection(canvas, lineText, lineStart, textLeft, lineTop, lineHeight);

            // A line highlighter yields spans relative to the line, a document highlighter absolute
            // ones - slicing with the matching origin is what makes both feed the same render path.
            if (_lineHighlighter != null)
            {
                RenderLineText(canvas, lineText, 0, GetLineSpans(i), textLeft, baseline);
            }
            else
            {
                RenderLineText(canvas, lineText, lineStart, spans, textLeft, baseline);
            }

            if (ShowLineNumbers)
            {
                RenderLineNumber(canvas, i, rect, baseline, gutterWidth);
            }

            if (i == caretLine && _isSelected && IsCaretVisible())
            {
                RenderCaret(canvas, lineText, textLeft, lineTop, lineHeight);
            }

            lineStart += lineText.Length + 1;
        }

        RenderScrollbar(canvas, rect);
        canvas.Restore();
    }

    private void RenderLineText(
        SKCanvas canvas,
        string lineText,
        int lineStart,
        IReadOnlyList<StyleSpan> spans,
        float textLeft,
        float baseline)
    {
        if (lineText.Length == 0) return;

        if (spans.Count == 0)
        {
            canvas.DrawText(lineText, textLeft, baseline, SKTextAlign.Left, Font, Paint);
            return;
        }

        var x = textLeft;
        foreach (var fragment in SliceLine(lineStart, lineText.Length, spans))
        {
            var fragmentText = lineText.Substring(fragment.Offset, fragment.Length);
            var (paint, font) = GetOrCreatePaint(GetStyleKey(fragment.Style));
            canvas.DrawText(fragmentText, x, baseline, SKTextAlign.Left, font, paint);
            x += font.MeasureText(fragmentText);
        }
    }

    private void RenderLineNumber(SKCanvas canvas, int lineIndex, SKRect rect, float baseline, float gutterWidth)
    {
        var (paint, font) = GetOrCreatePaint(new RunStyleKey(LineNumberColor, FontWeight, FontStyle));
        var label = (lineIndex + 1).ToString();
        var x = rect.Left + Padding.Left + gutterWidth - 12 - font.MeasureText(label);
        canvas.DrawText(label, x, baseline, SKTextAlign.Left, font, paint);
    }

    private void RenderCurrentLine(SKCanvas canvas, SKRect rect, float lineTop, float lineHeight)
    {
        using var paint = new SKPaint { Color = CurrentLineColor, Style = SKPaintStyle.Fill };
        canvas.DrawRect(new SKRect(rect.Left, lineTop, rect.Right, lineTop + lineHeight), paint);
    }

    private void RenderLineSelection(
        SKCanvas canvas,
        string lineText,
        int lineStart,
        float textLeft,
        float lineTop,
        float lineHeight)
    {
        if (!HasSelection) return;

        var lineEnd = lineStart + lineText.Length;
        var from = Math.Max(SelectionMin, lineStart);
        var to = Math.Min(SelectionMax, lineEnd);
        if (to < from) return;

        var startX = textLeft + Font.MeasureText(lineText[..(from - lineStart)]);
        var endX = textLeft + Font.MeasureText(lineText[..(to - lineStart)]);

        // A selection continuing past this line gets a trailing sliver so the newline reads as selected.
        if (SelectionMax > lineEnd)
        {
            endX += Font.MeasureText(" ");
        }

        using var paint = new SKPaint { Color = SelectionColor, Style = SKPaintStyle.Fill };
        canvas.DrawRect(new SKRect(startX, lineTop, endX, lineTop + lineHeight), paint);
    }

    private void RenderCaret(SKCanvas canvas, string lineText, float textLeft, float lineTop, float lineHeight)
    {
        var (_, column) = GetLineAndColumn(_cursorPosition);
        var caretX = textLeft + Font.MeasureText(lineText[..Math.Min(column, lineText.Length)]);

        using var paint = new SKPaint { Color = CaretColor, Style = SKPaintStyle.Fill };
        canvas.DrawRect(new SKRect(caretX, lineTop, caretX + 1.5f, lineTop + lineHeight), paint);
    }

    private bool IsCaretVisible()
    {
        var elapsed = (TimeProvider.System.GetLocalNow().LocalDateTime - _caretTime).TotalMilliseconds;
        return (int)(elapsed / 500) % 2 == 0;
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

    public override UiElement? HitTest(Point point)
    {
        if (!IsVisible || !InterceptsClicks) return null;

        var rect = new Rect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            ElementSize.Width,
            ElementSize.Height);

        return rect.Contains(point) ? this : null;
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearPaintCache();
        }
        base.Dispose(disposing);
    }

    #endregion
}
