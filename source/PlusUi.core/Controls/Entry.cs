using Silk.NET.Input;
using SkiaSharp;

namespace PlusUi.core;

public class Entry : UiTextElement<Entry>, ITextInputControl
{
    private bool _isSelected;
    private DateTime _selectionTime;

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        canvas.DrawText(
            Text,
            Position.X,
            Position.Y + TextSize,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);

        if (_isSelected)
        {
            var elapsedMilliseconds = (DateTime.Now - _selectionTime).TotalMilliseconds;
            if ((elapsedMilliseconds % 1600) < 800)
            {
                Font.GetFontMetrics(out var fontMetrics);
                var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

                var cursorX = Position.X + Font.MeasureText(Text);
                var cursorYStart = Position.Y + (textHeight * 0.1f);
                var cursorYEnd = Position.Y + textHeight - (textHeight * 0.1f);

                var strokeWidth = Paint.StrokeWidth;
                Paint.StrokeWidth = Math.Max(2, textHeight * 0.04f);
                canvas.DrawLine(cursorX, cursorYStart, cursorX, cursorYEnd, Paint);
                Paint.StrokeWidth = strokeWidth;
            }
        }
    }
    public void SetSelectionStatus(bool isSelected)
    {
        if (!_isSelected && isSelected)
        {
            _selectionTime = TimeProvider.System.GetLocalNow().LocalDateTime;
        }
        _isSelected = isSelected;
    }
    public void HandleInput(Key key)
    {
        if (key == Key.Backspace)
        {
            if (Text?.Length > 0)
            {
                Text = Text[..^1];
            }
        }
    }
    public void HandleInput(char chr)
    {
        Text += chr;
    }

}
