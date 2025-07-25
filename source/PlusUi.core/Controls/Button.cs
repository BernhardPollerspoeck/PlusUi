using SkiaSharp;
using System.Windows.Input;

namespace PlusUi.core;

public class Button : UiTextElement<Button>, IInputControl
{
    protected override bool SkipBackground => true;

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

        var textX = textRect.MidX;
        var textY = textRect.MidY + (TextSize / 2);

        canvas.DrawText(
            Text ?? string.Empty,
            textX,
            textY,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var textWidth = Font.MeasureText(Text ?? string.Empty);
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(textWidth + Padding.Left + Padding.Right, availableSize.Width),
            Math.Min(textHeight + Padding.Top + Padding.Bottom, availableSize.Height));
    }
}
