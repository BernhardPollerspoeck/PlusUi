using PlusUi.core.Attributes;
using SkiaSharp;
using System.Windows.Input;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Button : UiTextElement, IInputControl
{

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
    #endregion

    #region Icon
    internal string? Icon
    {
        get => field;
        set
        {
            field = value;
            // For button icons, we only support static images (no animation)
            var (staticImage, _) = ImageLoaderService.LoadImage(value, OnIconLoadedFromWeb, null);
            _iconImage = staticImage;
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

    private void OnIconLoadedFromWeb(SKImage? image)
    {
        // Update the icon if this is still the active icon source
        if (image != null)
        {
            _iconImage = image;
            InvalidateMeasure();
        }
    }
    #endregion

    public Button()
    {
        HorizontalTextAlignment = HorizontalTextAlignment.Center;
    }

    #region IInputControl
    public void InvokeCommand()
    {
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
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        var textRect = new SKRect(
            Position.X + VisualOffset.X + Padding.Left,
            Position.Y + VisualOffset.Y + Padding.Top,
            Position.X + VisualOffset.X + ElementSize.Width - Padding.Right,
            Position.Y + VisualOffset.Y + ElementSize.Height - Padding.Bottom);

        // Calculate icon sizes and spacing
        var hasText = !string.IsNullOrEmpty(Text);
        var hasLeadingIcon = _iconImage != null && IconPosition.HasFlag(IconPosition.Leading);
        var hasTrailingIcon = _iconImage != null && IconPosition.HasFlag(IconPosition.Trailing);
        var iconSize = TextSize; // Icon size matches text size
        var iconSpacing = hasText ? 8f : 0f; // Spacing between icon and text

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
        if (hasLeadingIcon)
        {
            var iconRect = new SKRect(
                currentX,
                centerY - iconSize / 2,
                currentX + iconSize,
                centerY + iconSize / 2);
            var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
            canvas.DrawImage(_iconImage, iconRect, samplingOptions);
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
        if (hasTrailingIcon)
        {
            var iconRect = new SKRect(
                currentX,
                centerY - iconSize / 2,
                currentX + iconSize,
                centerY + iconSize / 2);
            var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
            canvas.DrawImage(_iconImage, iconRect, samplingOptions);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var hasText = !string.IsNullOrEmpty(Text);
        var hasLeadingIcon = _iconImage != null && IconPosition.HasFlag(IconPosition.Leading);
        var hasTrailingIcon = _iconImage != null && IconPosition.HasFlag(IconPosition.Trailing);
        
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
