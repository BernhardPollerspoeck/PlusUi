using SkiaSharp;
using System.Windows.Input;

namespace PlusUi.core;

public class Button : UiTextElement<Button>, IInputControl
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

    #region command
    public ICommand? Command { get; set; }
    public Button SetCommand(ICommand command)
    {
        Command = command;
        return this;
    }

    public object? CommandParameter { get; set; }
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
        var textWidth = Font.MeasureText(Text);

        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;
        if (BackgroundPaint is not null)
        {
            canvas.DrawRect(
                Position.X,
                Position.Y,
                ElementSize.Width,
                ElementSize.Height,
                BackgroundPaint);
        }

        canvas.DrawText(
            Text,
            Position.X + (ElementSize.Width / 2),
            Position.Y + textHeight,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal(Size availableSize)
    {
        var textWidth = Font.MeasureText(Text);
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(textWidth + Padding.Left + Padding.Right, availableSize.Width),
            Math.Min(textHeight + Padding.Top + Padding.Bottom, availableSize.Height));
    }
}
