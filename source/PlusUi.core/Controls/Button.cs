using PlusUi.core.CoreElements;
using PlusUi.core.Enumerations;
using PlusUi.core.Interfaces;
using PlusUi.core.Structures;
using SkiaSharp;
using System.Windows.Input;

namespace PlusUi.core.Controls;

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
        TextAlignment = TextAlignment.Center;
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
            Padding.Left + Position.X + (ElementSize.Width / 2),
            Padding.Top + Position.Y + TextSize,
            (SKTextAlign)TextAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal(Size availableSize)
    {
        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(Font.MeasureText(Text) + Padding.Left + Padding.Right, availableSize.Width),
            Math.Min(TextSize + Padding.Top + Padding.Bottom, availableSize.Height));
    }
}
