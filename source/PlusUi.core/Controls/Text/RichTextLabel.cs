using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A control that displays multiple styled text segments (runs) inline.
/// Supports mixed colors, font weights, styles, sizes, and families within a single control.
/// </summary>
[GenerateShadowMethods]
public partial class RichTextLabel : UiElement
{
    #region Types

    private readonly record struct RunStyleKey(
        Color Color,
        float Size,
        FontWeight Weight,
        FontStyle Style,
        string? FontFamily);

    private class RunPaintEntry
    {
        public required SKPaint Paint { get; init; }
        public required SKFont Font { get; init; }
        public int RefCount;
    }

    private class RunFragment
    {
        public required TextRun Run { get; init; }
        public required string Text { get; init; }
        public required SKPaint Paint { get; init; }
        public required SKFont Font { get; init; }
        public required float Width { get; init; }
        public required float Ascent { get; init; }
        public required float Descent { get; init; }
    }

    private class WrappedLine
    {
        public List<RunFragment> Fragments { get; } = [];
        public float Width { get; set; }
        public float MaxAscent { get; set; }
        public float MaxDescent { get; set; }
        public float Height => MaxDescent - MaxAscent;
    }

    #endregion

    #region Services

    private readonly IFontRegistryService? _fontRegistry;

    #endregion

    #region Fields

    private readonly List<TextRun> _runs = [];
    private readonly Dictionary<RunStyleKey, RunPaintEntry> _paintCache = [];
    private List<WrappedLine>? _cachedLines;
    private float _cachedMaxWidth;

    #endregion

    #region Constructor

    public RichTextLabel()
    {
        _fontRegistry = ServiceProviderService.ServiceProvider?.GetService<IFontRegistryService>();

        TextSize = PlusUiDefaults.FontSize;
        TextColor = PlusUiDefaults.TextPrimary;
        FontWeight = PlusUiDefaults.FontWeight;
        FontStyle = PlusUiDefaults.FontStyle;
        HorizontalTextAlignment = PlusUiDefaults.HorizontalTextAlignment;
        TextWrapping = PlusUiDefaults.TextWrapping;
    }

    #endregion

    #region Abstract Implementation

    protected internal override bool IsFocusable => false;

    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Label;

    public override string? GetComputedAccessibilityLabel()
    {
        if (!string.IsNullOrEmpty(AccessibilityLabel))
        {
            return AccessibilityLabel;
        }
        return string.Concat(_runs.Select(r => r.Text));
    }

    #endregion

    #region Default Text Properties

    internal float TextSize
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public RichTextLabel BindTextSize(Expression<Func<float>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => TextSize = getter());
        return this;
    }

    internal Color TextColor
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetTextColor(Color color)
    {
        TextColor = color;
        return this;
    }

    public RichTextLabel BindTextColor(Expression<Func<Color>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => TextColor = getter());
        return this;
    }

    internal FontWeight FontWeight
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetFontWeight(FontWeight weight)
    {
        FontWeight = weight;
        return this;
    }

    public RichTextLabel BindFontWeight(Expression<Func<FontWeight>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => FontWeight = getter());
        return this;
    }

    internal FontStyle FontStyle
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetFontStyle(FontStyle style)
    {
        FontStyle = style;
        return this;
    }

    public RichTextLabel BindFontStyle(Expression<Func<FontStyle>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => FontStyle = getter());
        return this;
    }

    internal string? FontFamily
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetFontFamily(string? family)
    {
        FontFamily = family;
        return this;
    }

    public RichTextLabel BindFontFamily(Expression<Func<string?>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => FontFamily = getter());
        return this;
    }

    #endregion

    #region Layout Properties

    internal TextWrapping TextWrapping
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetTextWrapping(TextWrapping wrapping)
    {
        TextWrapping = wrapping;
        return this;
    }

    public RichTextLabel BindTextWrapping(Expression<Func<TextWrapping>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => TextWrapping = getter());
        return this;
    }

    internal HorizontalTextAlignment HorizontalTextAlignment
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    }

    public RichTextLabel SetHorizontalTextAlignment(HorizontalTextAlignment alignment)
    {
        HorizontalTextAlignment = alignment;
        return this;
    }

    public RichTextLabel BindHorizontalTextAlignment(Expression<Func<HorizontalTextAlignment>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => HorizontalTextAlignment = getter());
        return this;
    }

    internal int? MaxLines
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateLayout();
        }
    }

    public RichTextLabel SetMaxLines(int? maxLines)
    {
        MaxLines = maxLines;
        return this;
    }

    public RichTextLabel BindMaxLines(Expression<Func<int?>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => MaxLines = getter());
        return this;
    }

    #endregion

    #region Run Management

    public RichTextLabel AddRun(TextRun run)
    {
        run.SetParent(this);
        _runs.Add(run);
        RegisterRunBindings(run);
        InvalidateLayout();
        return this;
    }

    public RichTextLabel ClearRuns()
    {
        foreach (var run in _runs)
        {
            run.SetParent(null);
        }
        _runs.Clear();
        InvalidateLayout();
        return this;
    }

    public RichTextLabel SetRuns(IEnumerable<TextRun> runs)
    {
        ClearRuns();
        foreach (var run in runs)
        {
            AddRun(run);
        }
        return this;
    }

    public RichTextLabel BindRuns(Expression<Func<IEnumerable<TextRun>>> expr)
    {
        var path = ExpressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        RegisterPathBinding(path, () => SetRuns(getter()));
        return this;
    }

    private void RegisterRunBindings(TextRun run)
    {
        foreach (var (path, update) in run.GetBindings())
        {
            RegisterPathBinding(path, update);
        }
    }

    internal void InvalidateFromRun()
    {
        InvalidateLayout();
    }

    #endregion

    #region Paint Cache

    private (SKPaint paint, SKFont font) GetOrCreatePaint(RunStyleKey key)
    {
        if (_paintCache.TryGetValue(key, out var entry))
        {
            entry.RefCount++;
            return (entry.Paint, entry.Font);
        }

        SKTypeface? typeface = null;
        try
        {
            typeface = _fontRegistry?.GetTypeface(key.FontFamily, key.Weight, key.Style);
        }
        catch
        {
            // Use default
        }

        var (paint, font) = PaintRegistry.GetOrCreate(
            color: key.Color,
            size: key.Size,
            typeface: typeface);

        _paintCache[key] = new RunPaintEntry
        {
            Paint = paint,
            Font = font,
            RefCount = 1
        };

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

    private RunStyleKey GetStyleKeyForRun(TextRun run)
    {
        return new RunStyleKey(
            run.Color ?? TextColor,
            run.FontSize ?? TextSize,
            run.FontWeight ?? FontWeight,
            run.FontStyle ?? FontStyle,
            run.FontFamily ?? FontFamily);
    }

    #endregion

    #region Layout

    private void InvalidateLayout()
    {
        _cachedLines = null;
        ClearPaintCache();
        InvalidateMeasure();
    }

    private static string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        text = text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

        var sb = new System.Text.StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (ch >= ' ' && ch != '\x7F')
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }

    private List<WrappedLine> BuildLines(float maxWidth)
    {
        if (_cachedLines != null && Math.Abs(_cachedMaxWidth - maxWidth) < 0.01f)
        {
            return _cachedLines;
        }

        ClearPaintCache();
        var lines = new List<WrappedLine>();

        if (_runs.Count == 0)
        {
            _cachedLines = lines;
            _cachedMaxWidth = maxWidth;
            return lines;
        }

        var currentLine = new WrappedLine();
        var currentX = 0f;

        foreach (var run in _runs)
        {
            var key = GetStyleKeyForRun(run);
            var (paint, font) = GetOrCreatePaint(key);
            font.GetFontMetrics(out var metrics);
            var ascent = metrics.Ascent;
            var descent = metrics.Descent;

            var text = CleanText(run.Text ?? string.Empty);
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            if (TextWrapping == TextWrapping.NoWrap)
            {
                var width = font.MeasureText(text);
                currentLine.Fragments.Add(new RunFragment
                {
                    Run = run,
                    Text = text,
                    Paint = paint,
                    Font = font,
                    Width = width,
                    Ascent = ascent,
                    Descent = descent
                });
                currentX += width;
                currentLine.Width = currentX;
                currentLine.MaxAscent = Math.Min(currentLine.MaxAscent, ascent);
                currentLine.MaxDescent = Math.Max(currentLine.MaxDescent, descent);
            }
            else
            {
                var remaining = text;

                while (remaining.Length > 0)
                {
                    var availableWidth = maxWidth - currentX;
                    var (fitted, rest) = FitText(remaining, availableWidth, font, TextWrapping);

                    if (fitted.Length == 0 && currentLine.Fragments.Count > 0)
                    {
                        lines.Add(currentLine);
                        currentLine = new WrappedLine();
                        currentX = 0;
                        continue;
                    }

                    if (fitted.Length == 0)
                    {
                        fitted = remaining[..1];
                        rest = remaining.Length > 1 ? remaining[1..] : string.Empty;
                    }

                    var fragmentWidth = font.MeasureText(fitted);
                    currentLine.Fragments.Add(new RunFragment
                    {
                        Run = run,
                        Text = fitted,
                        Paint = paint,
                        Font = font,
                        Width = fragmentWidth,
                        Ascent = ascent,
                        Descent = descent
                    });
                    currentX += fragmentWidth;
                    currentLine.Width = currentX;
                    currentLine.MaxAscent = Math.Min(currentLine.MaxAscent, ascent);
                    currentLine.MaxDescent = Math.Max(currentLine.MaxDescent, descent);

                    if (!string.IsNullOrEmpty(rest))
                    {
                        lines.Add(currentLine);
                        currentLine = new WrappedLine();
                        currentX = 0;
                    }

                    remaining = rest;
                }
            }
        }

        if (currentLine.Fragments.Count > 0)
        {
            lines.Add(currentLine);
        }

        if (MaxLines.HasValue && lines.Count > MaxLines.Value)
        {
            lines = lines.Take(MaxLines.Value).ToList();
        }

        _cachedLines = lines;
        _cachedMaxWidth = maxWidth;
        return lines;
    }

    private static (string fitted, string rest) FitText(string text, float maxWidth, SKFont font, TextWrapping wrapping)
    {
        if (maxWidth <= 0)
        {
            return (string.Empty, text);
        }

        var fullWidth = font.MeasureText(text);
        if (fullWidth <= maxWidth)
        {
            return (text, string.Empty);
        }

        if (wrapping == TextWrapping.WordWrap)
        {
            var lastSpace = -1;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    var measured = font.MeasureText(text[..(i + 1)]);
                    if (measured > maxWidth)
                    {
                        break;
                    }
                    lastSpace = i;
                }
            }

            if (lastSpace > 0)
            {
                return (text[..lastSpace], text[(lastSpace + 1)..].TrimStart());
            }

            for (var i = 1; i < text.Length; i++)
            {
                if (font.MeasureText(text[..i]) > maxWidth)
                {
                    return (text[..(i - 1)], text[(i - 1)..]);
                }
            }
        }
        else
        {
            for (var i = 1; i < text.Length; i++)
            {
                if (font.MeasureText(text[..i]) > maxWidth)
                {
                    return (text[..(i - 1)], text[(i - 1)..]);
                }
            }
        }

        return (text, string.Empty);
    }

    #endregion

    #region Measure

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (_runs.Count == 0)
        {
            return new Size(0, 0);
        }

        var effectiveWidth = availableSize.Width;
        if (DesiredSize?.Width >= 0)
        {
            effectiveWidth = Math.Min(DesiredSize.Value.Width, availableSize.Width);
        }

        var lines = BuildLines(effectiveWidth);

        if (lines.Count == 0)
        {
            return new Size(0, 0);
        }

        var maxWidth = lines.Max(l => l.Width);
        var totalHeight = lines.Sum(l => l.Height);

        return new Size(
            Math.Min(maxWidth, availableSize.Width),
            Math.Min(totalHeight, availableSize.Height));
    }

    #endregion

    #region Render

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        if (!IsVisible || _runs.Count == 0)
        {
            return;
        }

        var clipRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);
        canvas.Save();
        canvas.ClipRect(clipRect);

        var lines = BuildLines(ElementSize.Width);

        var y = Position.Y + VisualOffset.Y;
        foreach (var line in lines)
        {
            var baseline = y - line.MaxAscent;

            var x = HorizontalTextAlignment switch
            {
                HorizontalTextAlignment.Center => Position.X + VisualOffset.X + (ElementSize.Width - line.Width) / 2,
                HorizontalTextAlignment.Right => Position.X + VisualOffset.X + ElementSize.Width - line.Width,
                _ => Position.X + VisualOffset.X
            };

            foreach (var fragment in line.Fragments)
            {
                canvas.DrawText(fragment.Text, x, baseline, fragment.Font, fragment.Paint);
                x += fragment.Width;
            }

            y += line.Height;
        }

        canvas.Restore();
    }

    #endregion

    #region Bindings

    protected override void UpdateBindingsInternal()
    {
        foreach (var run in _runs)
        {
            run.UpdateBindings();
        }
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var run in _runs)
        {
            run.UpdateBindings(propertyName);
        }
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearPaintCache();
            foreach (var run in _runs)
            {
                run.SetParent(null);
            }
            _runs.Clear();
        }
        base.Dispose(disposing);
    }

    #endregion
}
