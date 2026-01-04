using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class ActivityIndicator : UiElement
{
    private TimeProvider? _timeProvider;
    private DateTimeOffset _startTime;

    protected internal override bool IsFocusable => false;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Spinner;

    public ActivityIndicator()
    {
        SetDesiredSize(new(40, 40));
    }

    public override void BuildContent()
    {
        base.BuildContent();
        _timeProvider = ServiceProviderService.ServiceProvider?.GetService<TimeProvider>() ?? TimeProvider.System;
        _startTime = _timeProvider.GetUtcNow();
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? "Loading";
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsRunning)
        {
            traits |= AccessibilityTrait.Busy;
        }
        return traits;
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
                if (value && _timeProvider != null)
                {
                    _startTime = _timeProvider.GetUtcNow();
                }

                // Notify InvalidationTracker that rendering state changed
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
    internal Color Color
    {
        get => field;
        set
        {
            field = value;
        }
    } = new Color(0, 122, 255); // iOS blue

    public ActivityIndicator SetColor(Color color)
    {
        Color = color;
        return this;
    }

    public ActivityIndicator BindColor(string propertyName, Func<Color> propertyGetter)
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

    #region StrokeThickness
    internal float StrokeThickness { get; set; } = 3.0f;
    public ActivityIndicator SetStrokeThickness(float thickness)
    {
        StrokeThickness = thickness;
        return this;
    }
    public ActivityIndicator BindStrokeThickness(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => StrokeThickness = propertyGetter());
        return this;
    }

    #endregion

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible || !IsRunning || _timeProvider == null)
        {
            return;
        }

        var centerX = Position.X + VisualOffset.X + (ElementSize.Width / 2);
        var centerY = Position.Y + VisualOffset.Y + (ElementSize.Height / 2);
        var radius = Math.Min(ElementSize.Width, ElementSize.Height) / 2 - 2;

        // Calculate rotation based on elapsed time (using TimeProvider for testability)
        var elapsedSeconds = (_timeProvider.GetUtcNow() - _startTime).TotalSeconds;
        var rotationDegrees = (float)((elapsedSeconds * 360 * Speed) % 360);

        using var paint = new SKPaint
        {
            Color = Color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = StrokeThickness,
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
