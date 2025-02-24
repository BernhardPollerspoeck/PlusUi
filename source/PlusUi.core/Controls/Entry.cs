using Silk.NET.Input;
using SkiaSharp;

namespace PlusUi.core;

public class Entry : UiTextElement<Entry>, ITextInputControl
{
    private bool _isSelected;
    private DateTime _selectionTime;

    public Entry BindText(string propertyName, Func<string?> propertyGetter, Action<string> propertySetter)
    {
        base.BindText(propertyName, propertyGetter);
        RegisterSetter(nameof(Text), propertySetter);
        return this;
    }

    public override void Render(SKCanvas canvas)
    {
        var textWidth = Font.MeasureText(Text ?? string.Empty);

        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;
        if (BackgroundPaint is not null)
        {
            var rect = new SKRect(
                Position.X,
                Position.Y,
                Position.X + ElementSize.Width,
                Position.Y + ElementSize.Height);
            if (CornerRadius > 0)
            {
                canvas.DrawRoundRect(rect, CornerRadius, CornerRadius, BackgroundPaint);
            }
            else
            {
                canvas.DrawRect(rect, BackgroundPaint);
            }
        }

        canvas.DrawText(
            Text ?? string.Empty,
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

                var cursorX = Position.X + Font.MeasureText(Text ?? string.Empty);
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
                if (_setter.TryGetValue(nameof(Text), out var textSetter))
                {
                    foreach (var setter in textSetter)
                    {
                        setter(Text);
                    }
                }
            }
        }
    }
    public void HandleInput(char chr)
    {
        Text += chr;
        if (_setter.TryGetValue(nameof(Text), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(Text);
            }
        }
    }

}
