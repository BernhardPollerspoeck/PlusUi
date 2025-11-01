using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// An activity indicator (spinner) for showing indeterminate progress/loading states.
/// </summary>
[GenerateShadowMethods]
public partial class ActivityIndicator : UiElement
{
    private DateTime _startTime;

    public ActivityIndicator()
    {
        SetDesiredSize(new(40, 40));
        _startTime = DateTime.Now;
    }

    #region IsRunning
    internal bool IsRunning
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                if (value)
                {
                    _startTime = DateTime.Now;
                }
            }
        }
    } = true;

    public ActivityIndicator SetIsRunning(bool isRunning)
    {
        IsRunning = isRunning;
        return this;
    }

    public ActivityIndicator BindIsRunning(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsRunning = propertyGetter());
        return this;
    }
    #endregion

    #region Color
    internal SKColor Color
    {
        get => field;
        set
        {
            field = value;
        }
    } = new SKColor(0, 122, 255); // iOS blue

    public ActivityIndicator SetColor(SKColor color)
    {
        Color = color;
        return this;
    }

    public ActivityIndicator BindColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => Color = propertyGetter());
        return this;
    }
    #endregion

    #region Speed
    internal float Speed
    {
        get => field;
        set
        {
            field = Math.Max(0.1f, value);
        }
    } = 1.0f;

    public ActivityIndicator SetSpeed(float speed)
    {
        Speed = speed;
        return this;
    }

    public ActivityIndicator BindSpeed(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Speed = propertyGetter());
        return this;
    }
    #endregion

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible || !IsRunning)
        {
            return;
        }

        var centerX = Position.X + VisualOffset.X + (ElementSize.Width / 2);
        var centerY = Position.Y + VisualOffset.Y + (ElementSize.Height / 2);
        var radius = Math.Min(ElementSize.Width, ElementSize.Height) / 2 - 2;

        // Calculate rotation based on elapsed time
        var elapsedSeconds = (DateTime.Now - _startTime).TotalSeconds;
        var rotationDegrees = (float)((elapsedSeconds * 360 * Speed) % 360);

        using var paint = new SKPaint
        {
            Color = Color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round
        };

        // Draw spinning arc (270 degrees)
        using var path = new SKPath();
        var rect = new SKRect(
            centerX - radius,
            centerY - radius,
            centerX + radius,
            centerY + radius);

        path.AddArc(rect, rotationDegrees, 270);
        canvas.DrawPath(path, paint);
    }
}
