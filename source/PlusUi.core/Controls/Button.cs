using PlusUi.core.CoreElements;
using PlusUi.core.Enumerations;
using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.Controls;

public class Button : UiTextElement<Button>
{
    #region Padding
    public Margin Padding
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

    public Button()
    {
        TextSize = 25;
        TextAlignment = TextAlignment.Center;
    }

    public override void Render(SKCanvas canvas)
    {
        var textWidth = Font.MeasureText(Text);


        using var paint = new SKPaint
        {
            Color = SKColors.Blue,
            IsAntialias = true,
        };
        canvas.DrawRect(
            Position.X + Padding.Left,
            Position.Y + Padding.Top,
            Margin.Left + textWidth + Margin.Right,
            Margin.Top + TextSize + Margin.Bottom,
            paint);

        canvas.DrawText(
            Text,
            Padding.Left + Margin.Left + Position.X + (textWidth / 2),
            Padding.Left + Margin.Left + Position.Y + TextSize,
            (SKTextAlign)TextAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal(Size availableSize)
    {
     //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(Font.MeasureText(Text), availableSize.Width),
            Math.Min(TextSize, availableSize.Height));
    }
}
