using SkiaSharp;
using System.Collections.Concurrent;
using System.Windows.Input;

namespace PlusUi.core;

public class Button : UiTextElement<Button>, IInputControl
{
    protected override bool SkipBackground => true;
    private static readonly ConcurrentDictionary<string, SKImage?> _imageCache = new();

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
            _iconImage = CreateIconImage();
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
    private SKImage? CreateIconImage()
    {
        if (string.IsNullOrEmpty(Icon))
        {
            return null;
        }

        if (_imageCache.TryGetValue(Icon, out var cachedImage))
        {
            return cachedImage;
        }

        // First try the entry assembly
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            var image = TryLoadImageFromAssembly(assembly, Icon);
            if (image != null)
            {
                _imageCache[Icon] = image;
                return image;
            }
        }

        // Try all loaded assemblies in the current AppDomain if not found
        foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Skip system assemblies to improve performance
            if (loadedAssembly.IsDynamic)
            {
                continue;
            }

            var image = TryLoadImageFromAssembly(loadedAssembly, Icon);
            if (image != null)
            {
                _imageCache[Icon] = image;
                return image;
            }
        }

        // Return null if resource not found instead of throwing
        return null;
    }

    private static SKImage? TryLoadImageFromAssembly(System.Reflection.Assembly assembly, string resourceName)
    {
        var resourceNames = assembly.GetManifestResourceNames();
        var fullResourceName = resourceNames.FirstOrDefault(name =>
            name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (fullResourceName == null)
        {
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
            return null;

        using var codec = SKCodec.Create(stream);
        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
        using var bitmap = new SKBitmap(info);
        codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        return SKImage.FromBitmap(bitmap);
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

        if (BackgroundPaint is not null)
        {
            var rect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);
            if (CornerRadius > 0)
            {
                canvas.DrawRoundRect(rect, CornerRadius, CornerRadius, BackgroundPaint);
            }
            else
            {
                canvas.DrawRect(rect, BackgroundPaint);
            }
        }

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
