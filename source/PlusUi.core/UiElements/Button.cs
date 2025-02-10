using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.UiElements;

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
        HorizontalAlignment = SKTextAlign.Center;
    }

    public override void Render(SKCanvas canvas, SKPoint location)
    {
        var textWidth = Font.MeasureText(Text);


        using var paint = new SKPaint
        {
            Color = SKColors.Blue,
            IsAntialias = true,
        };
        canvas.DrawRect(
            location.X + Padding.Left,
            location.Y + Padding.Top,
            Margin.Left + textWidth + Margin.Right,
            Margin.Top + TextSize + Margin.Bottom,
            paint);

        canvas.DrawText(
            Text,
            Padding.Left + Margin.Left + location.X + (textWidth / 2),
            Padding.Left + Margin.Left + location.Y + TextSize,
            HorizontalAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal(Size availableSize)
    {//TODO: inclue availableSize
        //we need to cut or wrap if the text is too long
        return new Size(Font.MeasureText(Text), TextSize) + Padding;
    }
}

