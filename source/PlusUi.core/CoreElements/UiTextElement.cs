using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using SkiaSharp;

namespace PlusUi.core;

public abstract class UiTextElement : UiElement
{
    #region Accessibility Settings Subscription
    private IAccessibilitySettingsService? _accessibilitySettings;
    private bool _subscribedToAccessibilityChanges;

    private void EnsureAccessibilitySubscription()
    {
        if (_subscribedToAccessibilityChanges)
        {
            return;
        }

        _accessibilitySettings = ServiceProviderService.ServiceProvider?.GetService<IAccessibilitySettingsService>();
        if (_accessibilitySettings != null)
        {
            _accessibilitySettings.SettingsChanged += OnAccessibilitySettingsChanged;
            _subscribedToAccessibilityChanges = true;
        }
    }

    private void OnAccessibilitySettingsChanged(object? sender, AccessibilitySettingsChangedEventArgs e)
    {
        if (e.SettingName == nameof(IAccessibilitySettingsService.FontScaleFactor) && SupportsSystemFontScaling)
        {
            // Recreate paint/font with new scale factor
            UpdatePaintFromRegistry();
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
        else if (e.SettingName == nameof(IAccessibilitySettingsService.IsHighContrastEnabled))
        {
            // Recreate paint with appropriate color
            UpdatePaintFromRegistry();
        }
    }
    #endregion

    #region SupportsSystemFontScaling
    private bool? _supportsSystemFontScaling;

    /// <summary>
    /// Gets or sets whether this element respects system font scaling preferences.
    /// When true, the text size is multiplied by the system font scale factor.
    /// If not explicitly set, uses the global EnableFontScaling configuration.
    /// </summary>
    internal bool SupportsSystemFontScaling
    {
        get
        {
            // If explicitly set on this control, use that value
            if (_supportsSystemFontScaling.HasValue)
            {
                return _supportsSystemFontScaling.Value;
            }

            // Otherwise, use global configuration
            var config = ServiceProviderService.ServiceProvider?.GetService<PlusUiConfiguration>();
            return config?.EnableFontScaling ?? false;
        }
        set => _supportsSystemFontScaling = value;
    }

    /// <summary>
    /// Sets whether this element respects system font scaling.
    /// This overrides the global EnableFontScaling configuration for this control.
    /// </summary>
    public UiTextElement SetSupportsSystemFontScaling(bool supports)
    {
        _supportsSystemFontScaling = supports;
        UpdatePaintFromRegistry();
        InvalidateTextLayoutCache();
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Text
    internal string? Text
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    }
    public UiTextElement SetText(string text)
    {
        Text = text;
        return this;
    }
    public UiTextElement BindText(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Text = propertyGetter());
        return this;
    }
    #endregion

    #region TextSize
    internal float TextSize
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            UpdatePaintFromRegistry();
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    } = 12;
    public UiTextElement SetTextSize(float fontSize)
    {
        TextSize = fontSize;
        return this;
    }
    public UiTextElement BindTextSize(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextSize = propertyGetter());
        return this;
    }
    #endregion

    #region TextColor
    internal Color TextColor
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            UpdatePaintFromRegistry();
        }
    } = Colors.White;
    public UiTextElement SetTextColor(Color color)
    {
        TextColor = color;
        return this;
    }
    public UiTextElement BindTextColor(string propertyName, Func<Color> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextColor = propertyGetter());
        return this;
    }
    #endregion

    #region FontFamily
    internal string? FontFamily
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            UpdatePaintFromRegistry();
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    }
    public UiTextElement SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        return this;
    }
    public UiTextElement BindFontFamily(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => FontFamily = propertyGetter());
        return this;
    }
    #endregion

    #region FontWeight
    internal FontWeight FontWeight
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            UpdatePaintFromRegistry();
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    } = FontWeight.Regular;
    public UiTextElement SetFontWeight(FontWeight weight)
    {
        FontWeight = weight;
        return this;
    }
    public UiTextElement BindFontWeight(string propertyName, Func<FontWeight> propertyGetter)
    {
        RegisterBinding(propertyName, () => FontWeight = propertyGetter());
        return this;
    }
    #endregion

    #region FontStyle
    internal FontStyle FontStyle
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            UpdatePaintFromRegistry();
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    } = FontStyle.Normal;
    public UiTextElement SetFontStyle(FontStyle style)
    {
        FontStyle = style;
        return this;
    }
    public UiTextElement BindFontStyle(string propertyName, Func<FontStyle> propertyGetter)
    {
        RegisterBinding(propertyName, () => FontStyle = propertyGetter());
        return this;
    }
    #endregion

    #region HorizontalTextAlignment
    internal HorizontalTextAlignment HorizontalTextAlignment
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    } = HorizontalTextAlignment.Left;
    public UiTextElement SetHorizontalTextAlignment(HorizontalTextAlignment alignment)
    {
        HorizontalTextAlignment = alignment;
        return this;
    }
    public UiTextElement BindHorizontalTextAlignment(string propertyName, Func<HorizontalTextAlignment> propertyGetter)
    {
        RegisterBinding(propertyName, () => HorizontalTextAlignment = propertyGetter());
        return this;
    }
    #endregion

    #region TextWrapping
    internal TextWrapping TextWrapping
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    } = TextWrapping.NoWrap;
    public UiTextElement SetTextWrapping(TextWrapping textWrapping)
    {
        TextWrapping = textWrapping;
        return this;
    }
    public UiTextElement BindTextWrapping(string propertyName, Func<TextWrapping> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextWrapping = propertyGetter());
        return this;
    }
    #endregion

    #region MaxLines
    internal int? MaxLines
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    }
    public UiTextElement SetMaxLines(int maxLines)
    {
        MaxLines = maxLines;
        return this;
    }
    public UiTextElement BindMaxLines(string propertyName, Func<int> propertyGetter)
    {
        RegisterBinding(propertyName, () => MaxLines = propertyGetter());
        return this;
    }
    #endregion

    #region TextTruncation
    internal TextTruncation TextTruncation
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateTextLayoutCache();
            InvalidateMeasure();
        }
    } = TextTruncation.None;
    public UiTextElement SetTextTruncation(TextTruncation textTruncation)
    {
        TextTruncation = textTruncation;
        return this;
    }
    public UiTextElement BindTextTruncation(string propertyName, Func<TextTruncation> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextTruncation = propertyGetter());
        return this;
    }
    #endregion

    public UiTextElement()
    {
        // Initialize Paint/Font from registry
        UpdatePaintFromRegistry();
    }

    /// <inheritdoc />
    public override void Render(SKCanvas canvas)
    {
        // Update paint color for high contrast mode (ServiceProvider may not be available at construction time)
        Paint.Color = GetEffectiveTextColor();
        base.Render(canvas);
    }

    #region Text Layout Cache
    private string? _cachedWrapText;
    private float _cachedWrapMaxWidth;
    private List<string>? _cachedWrapResult;

    private string? _cachedTruncText;
    private float _cachedTruncMaxWidth;
    private string? _cachedTruncResult;

    private void InvalidateTextLayoutCache()
    {
        _cachedWrapText = null;
        _cachedWrapResult = null;
        _cachedTruncText = null;
        _cachedTruncResult = null;
    }
    #endregion

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var text = Text ?? string.Empty;
        Font.GetFontMetrics(out var fontMetrics);
        var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;

        // Calculate the effective width constraint for text wrapping
        // This should match what will actually be used for rendering
        var effectiveWidth = availableSize.Width;
        if (DesiredSize?.Width >= 0)
        {
            effectiveWidth = Math.Min(DesiredSize.Value.Width, availableSize.Width);
        }

        if (TextWrapping == TextWrapping.NoWrap)
        {
            // No wrapping - single line
            var textWidth = Font.MeasureText(text);
            return new Size(
                Math.Min(textWidth, availableSize.Width),
                Math.Min(lineHeight, availableSize.Height));
        }
        else
        {
            // Text wrapping enabled - use effective width for wrapping
            var lines = WrapText(text, effectiveWidth);
            
            // Apply MaxLines if set
            if (MaxLines.HasValue && lines.Count > MaxLines.Value)
            {
                lines = lines.Take(MaxLines.Value).ToList();
            }

            var maxWidth = lines.Count > 0 
                ? lines.Max(line => Font.MeasureText(line)) 
                : 0f;
            var totalHeight = lineHeight * lines.Count;

            return new Size(
                Math.Min(maxWidth, availableSize.Width),
                Math.Min(totalHeight, availableSize.Height));
        }
    }

    #region render cache
    protected SKPaint Paint { get; private set; }
    protected SKFont Font { get; private set; }

    /// <summary>
    /// Gets the effective text color considering high contrast mode.
    /// </summary>
    protected SKColor GetEffectiveTextColor()
    {
        var config = ServiceProviderService.ServiceProvider?.GetService<PlusUiConfiguration>();
        if (config?.EnableHighContrastSupport == true && HighContrastForeground.HasValue)
        {
            // ForceHighContrast bypasses system detection
            if (config.ForceHighContrast)
            {
                return HighContrastForeground.Value;
            }

            var settings = ServiceProviderService.ServiceProvider?.GetService<IAccessibilitySettingsService>();
            if (settings?.IsHighContrastEnabled == true)
            {
                return HighContrastForeground.Value;
            }
        }
        return TextColor;
    }

    private void UpdatePaintFromRegistry()
    {
        // Ensure we're subscribed to accessibility changes
        EnsureAccessibilitySubscription();

        // Skip if PaintRegistry not available (during shutdown)
        if (PaintRegistry == null)
            return;

        // Release old paint if exists (for property changes)
        if (Paint != null)
        {
            PaintRegistry.Release(Paint, Font);
        }

        // Get typeface from FontRegistry
        SKTypeface? typeface = null;
        try
        {
            var fontRegistry = ServiceProviderService.ServiceProvider?.GetService<IFontRegistryService>();
            typeface = fontRegistry?.GetTypeface(FontFamily, FontWeight, FontStyle);
        }
        catch
        {
            // Silently continue - will use SKTypeface.Default as last resort
        }

        // Calculate effective size (with accessibility scaling)
        var effectiveSize = TextSize;
        if (SupportsSystemFontScaling)
        {
            var accessibilitySettings = ServiceProviderService.ServiceProvider?.GetService<IAccessibilitySettingsService>();
            effectiveSize *= accessibilitySettings?.FontScaleFactor ?? 1.0f;
        }

        // Get or create from registry (uses inherited PaintRegistry property)
        (Paint, Font) = PaintRegistry.GetOrCreate(
            color: GetEffectiveTextColor(),
            size: effectiveSize,
            typeface: typeface
            // Other params use defaults: isAntialias=true, subpixel=true, etc.
        );
    }
    #endregion

    #region Text wrapping and truncation helpers
    protected List<string> WrapText(string text, float maxWidth)
    {
        // Check cache
        if (_cachedWrapText == text && Math.Abs(_cachedWrapMaxWidth - maxWidth) < 0.01f && _cachedWrapResult != null)
        {
            return _cachedWrapResult;
        }

        var lines = new List<string>();
        if (string.IsNullOrEmpty(text) || maxWidth <= 0)
        {
            lines.Add(text);
            _cachedWrapText = text;
            _cachedWrapMaxWidth = maxWidth;
            _cachedWrapResult = lines;
            return lines;
        }

        if (TextWrapping == TextWrapping.WordWrap)
        {
            // Word wrap - break at word boundaries
            var words = text.Split(' ');
            var currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var testWidth = Font.MeasureText(testLine);

                if (testWidth > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }
        }
        else // TextWrapping.Wrap
        {
            // Character wrap - break at any character
            var currentLine = "";

            foreach (var ch in text)
            {
                var testLine = currentLine + ch;
                var testWidth = Font.MeasureText(testLine);

                if (testWidth > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = ch.ToString();
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }
        }

        var result = lines.Count > 0 ? lines : new List<string> { text };
        _cachedWrapText = text;
        _cachedWrapMaxWidth = maxWidth;
        _cachedWrapResult = result;
        return result;
    }

    protected string ApplyTruncation(string text, float maxWidth)
    {
        // Check cache
        if (_cachedTruncText == text && Math.Abs(_cachedTruncMaxWidth - maxWidth) < 0.01f && _cachedTruncResult != null)
        {
            return _cachedTruncResult;
        }

        if (TextTruncation == TextTruncation.None || string.IsNullOrEmpty(text))
        {
            _cachedTruncText = text;
            _cachedTruncMaxWidth = maxWidth;
            _cachedTruncResult = text;
            return text;
        }

        var textWidth = Font.MeasureText(text);
        if (textWidth <= maxWidth)
        {
            _cachedTruncText = text;
            _cachedTruncMaxWidth = maxWidth;
            _cachedTruncResult = text;
            return text;
        }

        const string ellipsis = " ... ";
        var ellipsisWidth = Font.MeasureText(ellipsis);

        var result = TextTruncation switch
        {
            TextTruncation.Start => TruncateStart(text, maxWidth, ellipsis, ellipsisWidth),
            TextTruncation.Middle => TruncateMiddle(text, maxWidth, ellipsis, ellipsisWidth),
            TextTruncation.End => TruncateEnd(text, maxWidth, ellipsis, ellipsisWidth),
            _ => text,
        };

        _cachedTruncText = text;
        _cachedTruncMaxWidth = maxWidth;
        _cachedTruncResult = result;
        return result;
    }

    private string TruncateEnd(string text, float maxWidth, string ellipsis, float ellipsisWidth)
    {
        var availableWidth = maxWidth - ellipsisWidth;
        if (availableWidth <= 0)
        {
            return ellipsis;
        }

        for (int i = text.Length - 1; i >= 0; i--)
        {
            var substring = text.Substring(0, i);
            if (Font.MeasureText(substring) <= availableWidth)
            {
                return substring + ellipsis;
            }
        }

        return ellipsis;
    }

    private string TruncateStart(string text, float maxWidth, string ellipsis, float ellipsisWidth)
    {
        var availableWidth = maxWidth - ellipsisWidth;
        if (availableWidth <= 0)
        {
            return ellipsis;
        }

        for (int i = 0; i < text.Length; i++)
        {
            var substring = text.Substring(i);
            if (Font.MeasureText(substring) <= availableWidth)
            {
                return ellipsis + substring;
            }
        }

        return ellipsis;
    }

    private string TruncateMiddle(string text, float maxWidth, string ellipsis, float ellipsisWidth)
    {
        var availableWidth = maxWidth - ellipsisWidth;
        if (availableWidth <= 0)
        {
            return ellipsis;
        }

        var halfWidth = availableWidth / 2;
        var startChars = 0;
        var endChars = 0;

        // Find how many characters fit at the start
        for (int i = 1; i <= text.Length; i++)
        {
            if (Font.MeasureText(text.Substring(0, i)) <= halfWidth)
            {
                startChars = i;
            }
            else
            {
                break;
            }
        }

        // Find how many characters fit at the end
        for (int i = 1; i <= text.Length; i++)
        {
            if (Font.MeasureText(text.Substring(text.Length - i)) <= halfWidth)
            {
                endChars = i;
            }
            else
            {
                break;
            }
        }

        if (startChars + endChars >= text.Length)
        {
            return text;
        }

        return text.Substring(0, startChars) + ellipsis + text.Substring(text.Length - endChars);
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from accessibility changes
            if (_subscribedToAccessibilityChanges && _accessibilitySettings != null)
            {
                _accessibilitySettings.SettingsChanged -= OnAccessibilitySettingsChanged;
                _subscribedToAccessibilityChanges = false;
                _accessibilitySettings = null;
            }

            // Release paint from registry (safe even if ClearAll already called or during shutdown)
            if (Paint != null)
            {
                PaintRegistry?.Release(Paint, Font);
            }
        }
        base.Dispose(disposing);
    }
}