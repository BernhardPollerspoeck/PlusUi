using SkiaSharp;

namespace PlusUi.core;

public class Label : UiTextElement<Label>
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
    public Label SetPadding(Margin padding)
    {
        Padding = padding;
        return this;
    }
    public Label BindPadding(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => Padding = propertyGetter());
        return this;
    }
    #endregion
    
    #region UiElement
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        canvas.DrawText(
            Text ?? string.Empty,
            Position.X + Padding.Left,
            Position.Y + TextSize + Padding.Top,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);
    }
    
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var textWidth = Font.MeasureText(Text ?? string.Empty);
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        return new Size(
            Math.Min(textWidth + Padding.Left + Padding.Right, availableSize.Width),
            Math.Min(textHeight + Padding.Top + Padding.Bottom, availableSize.Height));
    }
    #endregion
}
