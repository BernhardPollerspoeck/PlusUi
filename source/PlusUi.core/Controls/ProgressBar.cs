using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A progress bar control for showing determinate progress (0-1 or 0-100).
/// </summary>
[GenerateShadowMethods]
public partial class ProgressBar : UiElement
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.ProgressBar;

    public ProgressBar()
    {
        SetDesiredHeight(PlusUiDefaults.ProgressBarHeight);
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? "Progress";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue ?? $"{(int)(Progress * 100)}%";
    }

    #region Progress
    internal float Progress
    {
        get => field;
        set
        {
            field = Math.Clamp(value, 0f, 1f);
        }
    } = 0f;

    public ProgressBar SetProgress(float progress)
    {
        Progress = progress;
        return this;
    }

    public ProgressBar BindProgress(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Progress = getter());
        return this;
    }
    public ProgressBar BindProgress<T>(Expression<Func<T>> propertyExpression, Func<T, float>? toControl = null)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Progress = toControl != null ? toControl(getter()) : (float)(object)getter()!);
        return this;
    }

    #endregion

    #region ProgressColor
    internal Color ProgressColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = PlusUiDefaults.AccentPrimary;

    public ProgressBar SetProgressColor(Color color)
    {
        ProgressColor = color;
        return this;
    }

    public ProgressBar BindProgressColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ProgressColor = getter());
        return this;
    }
    #endregion

    #region TrackColor
    internal Color TrackColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = PlusUiDefaults.TrackColor;

    public ProgressBar SetTrackColor(Color color)
    {
        TrackColor = color;
        return this;
    }

    public ProgressBar BindTrackColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TrackColor = getter());
        return this;
    }
    #endregion

    /// <inheritdoc />
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Return available width (minus margin) and desired height so the control stretches properly
        var height = DesiredSize?.Height ?? 8f;
        var width = Math.Max(0, availableSize.Width - Margin.Horizontal);
        return new Size(width, height);
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        var trackRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        var cornerRadius = ElementSize.Height / 2;

        // Draw track (background)
        using var trackPaint = new SKPaint
        {
            Color = TrackColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRoundRect(trackRect, cornerRadius, cornerRadius, trackPaint);

        // Draw progress (foreground)
        if (Progress > 0)
        {
            var progressWidth = ElementSize.Width * Progress;
            var progressRect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + progressWidth,
                Position.Y + VisualOffset.Y + ElementSize.Height);

            using var progressPaint = new SKPaint
            {
                Color = ProgressColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawRoundRect(progressRect, cornerRadius, cornerRadius, progressPaint);
        }
    }
}
