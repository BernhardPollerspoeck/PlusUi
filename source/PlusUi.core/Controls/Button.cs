using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Models;
using SkiaSharp;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// A clickable button control that can display text and execute commands.
/// </summary>
/// <example>
/// <code>
/// // Simple button with text
/// new Button()
///     .SetText("Click Me")
///     .SetCommand(myCommand);
///
/// // Button with styling
/// new Button()
///     .SetText("Submit")
///     .SetPadding(new Margin(12, 8))
///     .SetBackground(new SolidColorBackground(Colors.Blue))
///     .SetFontSize(16);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Button : UiTextElement, IInputControl, IHoverableControl, IFocusable
{
    private IImageLoaderService? _imageLoaderService;
    private bool _isHovered;

    #region IHoverableControl
    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            if (_isHovered != value)
            {
                _isHovered = value;
                InvalidateMeasure();
            }
        }
    }
    #endregion

    #region HoverBackground
    internal IBackground? HoverBackground { get; set; }
    public Button SetHoverBackground(IBackground background)
    {
        HoverBackground = background;
        return this;
    }
    public Button BindHoverBackground(string propertyName, Func<IBackground> propertyGetter)
    {
        RegisterBinding(propertyName, () => HoverBackground = propertyGetter());
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
    }
    public Button SetPadding(Margin padding)
    {
        Padding = padding;
        return this;
    }
    public Button BindPadding(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => Padding = propertyGetter());
        return this;
    }
    #endregion

    #region command
    internal ICommand? Command { get; set; }
    public Button SetCommand(ICommand command)
    {
        Command = command;
        return this;
    }
    public Button BindCommand(string propertyName, Func<ICommand?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Command = propertyGetter());
        return this;
    }

    internal object? CommandParameter { get; set; }
    public Button SetCommandParameter(object parameter)
    {
        CommandParameter = parameter;
        return this;
    }
    public Button BindCommandParameter(string propertyName, Func<object> propertyGetter)
    {
        RegisterBinding(propertyName, () => CommandParameter = propertyGetter());
        return this;
    }

    internal Action? OnClick { get; set; }
    public Button SetOnClick(Action onClick)
    {
        OnClick = onClick;
        return this;
    }
    public Button BindOnClick(string propertyName, Func<Action?> propertyGetter)
    {
        RegisterBinding(propertyName, () => OnClick = propertyGetter());
        return this;
    }
    #endregion

    #region Icon
    internal string? Icon
    {
        get => field;
        set
        {
            field = value;
            // For button icons, we support static images and SVGs
            _imageLoaderService ??= ServiceProviderService.ServiceProvider?.GetRequiredService<IImageLoaderService>();
            var (staticImage, _, svgImage) = _imageLoaderService?.LoadImage(value, OnIconLoadedFromWeb, null, OnSvgIconLoadedFromWeb) ?? (default, default, default);
            _iconImage = staticImage;
            _iconSvgImage = svgImage;
            InvalidateMeasure();
        }
    }
    public Button SetIcon(string icon)
    {
        Icon = icon;
        return this;
    }
    public Button BindIcon(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Icon = propertyGetter());
        return this;
    }
    #endregion

    #region IconPosition
    internal IconPosition IconPosition
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = IconPosition.Leading;
    public Button SetIconPosition(IconPosition iconPosition)
    {
        IconPosition = iconPosition;
        return this;
    }
    public Button BindIconPosition(string propertyName, Func<IconPosition> propertyGetter)
    {
        RegisterBinding(propertyName, () => IconPosition = propertyGetter());
        return this;
    }
    #endregion

    #region Icon rendering cache
    private SKImage? _iconImage;
    private SvgImageInfo? _iconSvgImage;
    private SKImage? _renderedIconSvg;
    private float _lastIconSvgSize;

    private void OnIconLoadedFromWeb(SKImage? image)
    {
        if (image != null)
        {
            _iconImage = image;
            _iconSvgImage = null;
            InvalidateMeasure();
        }
    }

    private void OnSvgIconLoadedFromWeb(SvgImageInfo? svgImage)
    {
        if (svgImage != null)
        {
            _iconSvgImage = svgImage;
            _iconImage = null;
            _renderedIconSvg = null;
            InvalidateMeasure();
        }
    }
    #endregion

    #region IconTintColor
    internal Color? IconTintColor { get; set; }

    public Button SetIconTintColor(Color tintColor)
    {
        IconTintColor = tintColor;
        _renderedIconSvg = null;
        InvalidateMeasure();
        return this;
    }

    public Button BindIconTintColor(string propertyName, Func<Color?> propertyGetter)
    {
        RegisterBinding(propertyName, () =>
        {
            IconTintColor = propertyGetter();
            _renderedIconSvg = null;
            InvalidateMeasure();
        });
        return this;
    }
    #endregion

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override bool InterceptsClicks => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Button;

    public Button()
    {
        HorizontalTextAlignment = HorizontalTextAlignment.Center;
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Text;
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
    void IFocusable.OnBlur() => OnBlur();
    #endregion

    #region IInputControl
    public void InvokeCommand()
    {
        OnClick?.Invoke();
        if (Command?.CanExecute(CommandParameter) ?? false)
        {
            Command.Execute(CommandParameter);
        }
    }
    #endregion


    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Render hover background if hovered
        if (_isHovered && HoverBackground is not null)
        {
            var hoverRect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);
            HoverBackground.Render(canvas, hoverRect, CornerRadius);
        }

        var textRect = new SKRect(
            Position.X + VisualOffset.X + Padding.Left,
            Position.Y + VisualOffset.Y + Padding.Top,
            Position.X + VisualOffset.X + ElementSize.Width - Padding.Right,
            Position.Y + VisualOffset.Y + ElementSize.Height - Padding.Bottom);

        // Calculate icon sizes and spacing
        var hasText = !string.IsNullOrEmpty(Text);
        var hasIcon = _iconImage != null || _iconSvgImage != null;
        var hasLeadingIcon = hasIcon && IconPosition.HasFlag(IconPosition.Leading);
        var hasTrailingIcon = hasIcon && IconPosition.HasFlag(IconPosition.Trailing);
        var iconSize = TextSize; // Icon size matches text size
        var iconSpacing = hasText ? 8f : 0f; // Spacing between icon and text

        // Render SVG icon at correct size if needed
        SKImage? iconToRender = _iconImage;
        if (_iconSvgImage != null)
        {
            if (_renderedIconSvg == null || Math.Abs(_lastIconSvgSize - iconSize) > 0.1f)
            {
                _renderedIconSvg?.Dispose();
                _renderedIconSvg = _iconSvgImage.RenderToImage(iconSize, iconSize, IconTintColor);
                _lastIconSvgSize = iconSize;
            }
            iconToRender = _renderedIconSvg;
        }

        // Calculate total content width
        var textWidth = hasText ? Font.MeasureText(Text!) : 0f;
        var leadingIconWidth = hasLeadingIcon ? iconSize + iconSpacing : 0f;
        var trailingIconWidth = hasTrailingIcon ? iconSize + iconSpacing : 0f;
        var totalContentWidth = leadingIconWidth + textWidth + trailingIconWidth;

        // Calculate starting X position to center content
        var startX = textRect.Left + (textRect.Width - totalContentWidth) / 2;
        var currentX = startX;
        var centerY = textRect.MidY;

        // Render leading icon
        if (hasLeadingIcon && iconToRender != null)
        {
            var iconRect = new SKRect(
                currentX,
                centerY - iconSize / 2,
                currentX + iconSize,
                centerY + iconSize / 2);
            var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
            canvas.DrawImage(iconToRender, iconRect, samplingOptions);
            currentX += iconSize + iconSpacing;
        }

        // Render text
        if (hasText)
        {
            var textX = currentX + textWidth / 2;
            var textY = centerY + (TextSize / 2);
            canvas.DrawText(
                Text!,
                textX,
                textY,
                SKTextAlign.Center,
                Font,
                Paint);
            currentX += textWidth + iconSpacing;
        }

        // Render trailing icon
        if (hasTrailingIcon && iconToRender != null)
        {
            var iconRect = new SKRect(
                currentX,
                centerY - iconSize / 2,
                currentX + iconSize,
                centerY + iconSize / 2);
            var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
            canvas.DrawImage(iconToRender, iconRect, samplingOptions);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var hasText = !string.IsNullOrEmpty(Text);
        var hasIcon = _iconImage != null || _iconSvgImage != null;
        var hasLeadingIcon = hasIcon && IconPosition.HasFlag(IconPosition.Leading);
        var hasTrailingIcon = hasIcon && IconPosition.HasFlag(IconPosition.Trailing);

        var textWidth = hasText ? Font.MeasureText(Text!) : 0f;
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        // Icon size matches text size
        var iconSize = TextSize;
        var iconSpacing = hasText ? 8f : 0f; // Spacing between icon and text

        // Calculate total width
        var leadingIconWidth = hasLeadingIcon ? iconSize + iconSpacing : 0f;
        var trailingIconWidth = hasTrailingIcon ? iconSize + iconSpacing : 0f;
        var totalWidth = leadingIconWidth + textWidth + trailingIconWidth;

        // Height is determined by the larger of text or icon
        var contentHeight = Math.Max(textHeight, iconSize);

        return new Size(
            Math.Min(totalWidth + Padding.Left + Padding.Right, availableSize.Width),
            Math.Min(contentHeight + Padding.Top + Padding.Bottom, availableSize.Height));
    }
}
