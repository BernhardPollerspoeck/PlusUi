using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.UiElements;



public class Label : UiTextElement
{
    #region return type conversion
    public new Label SetText(string text)
    {
        base.SetText(text);
        return this;
    }
    public new Label BindText(string propertyName, System.Func<string> propertyGetter)
    {
        base.BindText(propertyName, propertyGetter);
        return this;
    }
    public new Label SetTextSize(float fontSize)
    {
        base.SetTextSize(fontSize);
        return this;
    }
    public new Label BindTextSize(string propertyName, System.Func<float> propertyGetter)
    {
        base.BindTextSize(propertyName, propertyGetter);
        return this;
    }
    public new Label SetTextColor(SKColor color)
    {
        base.SetTextColor(color);
        return this;
    }
    public new Label BindTextColor(string propertyName, Func<SKColor> propertyGetter)
    {
        base.BindTextColor(propertyName, propertyGetter);
        return this;
    }
    public new Label SetHorizontalAlignment(SKTextAlign alignment)
    {
        base.SetHorizontalAlignment(alignment);
        return this;
    }
    public new Label BindHorizontalAlignment(string propertyName, Func<SKTextAlign> propertyGetter)
    {
        base.BindHorizontalAlignment(propertyName, propertyGetter);
        return this;
    }

    public new Label SetBackgroundColor(SKColor color)
    {
        base.SetBackgroundColor(color);
        return this;
    }
    public new Label BindBackgroundColor(string propertyName, Func<SKColor> propertyGetter)
    {
        base.BindBackgroundColor(propertyName, propertyGetter);
        return this;
    }
    public new Label SetMargin(Margin margin)
    {
        base.SetMargin(margin);
        return this;
    }
    public new Label BindMargin(string propertyName, Func<Margin> propertyGetter)
    {
        base.BindMargin(propertyName, propertyGetter);
        return this;
    }
    #endregion

    #region UiElement
    public override void Render(SKCanvas canvas, SKPoint location)
    {
        base.Render(canvas, location);
        canvas.DrawText(
            Text,
            Margin.Left + location.X,
            Margin.Top + location.Y + TextSize,
            HorizontalAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal()
    {
        return new Size(Font.MeasureText(Text), TextSize);
    }
    #endregion
}
