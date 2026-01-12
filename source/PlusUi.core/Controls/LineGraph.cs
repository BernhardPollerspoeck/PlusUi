using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A simple line graph control for visualizing time-series data.
/// </summary>
[GenerateShadowMethods]
public partial class LineGraph : UiElement
{
    protected internal override bool IsFocusable => false;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

    public LineGraph()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
    }

    #region DataPoints
    internal IReadOnlyList<float> DataPoints
    {
        get => field;
        set => field = value;
    } = [];

    public LineGraph SetDataPoints(IReadOnlyList<float> points)
    {
        DataPoints = points;
        return this;
    }

    public LineGraph BindDataPoints(Expression<Func<IReadOnlyList<float>>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DataPoints = getter());
        return this;
    }

    public LineGraph BindDataPoints(Expression<Func<List<float>>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DataPoints = getter());
        return this;
    }
    #endregion

    #region LineColor
    internal Color LineColor
    {
        get => field;
        set => field = value;
    } = new Color(100, 200, 255);

    public LineGraph SetLineColor(Color color)
    {
        LineColor = color;
        return this;
    }

    public LineGraph BindLineColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => LineColor = getter());
        return this;
    }
    #endregion

    #region FillColor
    internal Color? FillColor
    {
        get => field;
        set => field = value;
    }

    public LineGraph SetFillColor(Color? color)
    {
        FillColor = color;
        return this;
    }

    public LineGraph BindFillColor(Expression<Func<Color?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FillColor = getter());
        return this;
    }
    #endregion

    #region MinValue
    internal float? MinValue
    {
        get => field;
        set => field = value;
    }

    public LineGraph SetMinValue(float? value)
    {
        MinValue = value;
        return this;
    }
    #endregion

    #region MaxValue
    internal float? MaxValue
    {
        get => field;
        set => field = value;
    }

    public LineGraph SetMaxValue(float? value)
    {
        MaxValue = value;
        return this;
    }
    #endregion

    #region LineThickness
    internal float LineThickness
    {
        get => field;
        set => field = value;
    } = 2f;

    public LineGraph SetLineThickness(float thickness)
    {
        LineThickness = thickness;
        return this;
    }
    #endregion

    #region GridColor
    internal Color? GridColor
    {
        get => field;
        set => field = value;
    }

    public LineGraph SetGridColor(Color? color)
    {
        GridColor = color;
        return this;
    }
    #endregion

    #region ShowAxisLabels
    internal bool ShowAxisLabels
    {
        get => field;
        set => field = value;
    } = true;

    public LineGraph SetShowAxisLabels(bool show)
    {
        ShowAxisLabels = show;
        return this;
    }
    #endregion

    #region LabelColor
    internal Color LabelColor
    {
        get => field;
        set => field = value;
    } = new Color(150, 150, 150);

    public LineGraph SetLabelColor(Color color)
    {
        LabelColor = color;
        return this;
    }
    #endregion

    #region ValueFormat
    internal string ValueFormat
    {
        get => field;
        set => field = value;
    } = "F1";

    public LineGraph SetValueFormat(string format)
    {
        ValueFormat = format;
        return this;
    }
    #endregion

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Return minimal desired size - let parent/Stretch determine actual size
        var height = DesiredSize?.Height ?? 60f;
        var width = DesiredSize?.Width ?? 100f;
        return new Size(width, height);
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible || DataPoints.Count < 2)
        {
            return;
        }

        // Calculate content bounds - ElementSize already excludes margins
        var fullRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        // Calculate min/max
        var dataMin = MinValue ?? DataPoints.Min();
        var dataMax = MaxValue ?? DataPoints.Max();

        // Avoid division by zero
        if (Math.Abs(dataMax - dataMin) < 0.0001f)
        {
            dataMax = dataMin + 1f;
        }

        // Reserve space for Y-axis labels on the left
        var labelWidth = ShowAxisLabels ? 36f : 0f;
        var graphRect = new SKRect(
            fullRect.Left + labelWidth,
            fullRect.Top,
            fullRect.Right,
            fullRect.Bottom);

        // Draw Y-axis labels
        if (ShowAxisLabels)
        {
            using var labelFont = new SKFont { Size = 10f };
            using var labelPaint = new SKPaint
            {
                Color = LabelColor,
                IsAntialias = true
            };

            var maxLabel = dataMax.ToString(ValueFormat);
            var minLabel = dataMin.ToString(ValueFormat);

            // Draw max value at top
            canvas.DrawText(maxLabel, fullRect.Left + labelWidth - 4, fullRect.Top + 10, SKTextAlign.Right, labelFont, labelPaint);

            // Draw min value at bottom
            canvas.DrawText(minLabel, fullRect.Left + labelWidth - 4, fullRect.Bottom - 2, SKTextAlign.Right, labelFont, labelPaint);
        }

        // Clip to graph bounds to prevent drawing outside
        canvas.Save();
        canvas.ClipRect(graphRect);

        // Draw grid lines if GridColor is set
        if (GridColor.HasValue)
        {
            using var gridPaint = new SKPaint
            {
                Color = GridColor.Value,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f,
                IsAntialias = true
            };

            // Draw 3 horizontal grid lines (25%, 50%, 75%)
            for (int i = 1; i <= 3; i++)
            {
                var y = graphRect.Top + (graphRect.Height * i / 4f);
                canvas.DrawLine(graphRect.Left, y, graphRect.Right, y, gridPaint);
            }
        }

        // Build path for line
        var path = new SKPath();
        var fillPath = new SKPath();

        var pointCount = DataPoints.Count;
        var xStep = graphRect.Width / (pointCount - 1);

        for (int i = 0; i < pointCount; i++)
        {
            var normalizedValue = (DataPoints[i] - dataMin) / (dataMax - dataMin);
            var x = graphRect.Left + (i * xStep);
            var y = graphRect.Bottom - (normalizedValue * graphRect.Height);

            if (i == 0)
            {
                path.MoveTo(x, y);
                fillPath.MoveTo(x, graphRect.Bottom);
                fillPath.LineTo(x, y);
            }
            else
            {
                path.LineTo(x, y);
                fillPath.LineTo(x, y);
            }
        }

        // Close fill path
        fillPath.LineTo(graphRect.Right, graphRect.Bottom);
        fillPath.Close();

        // Draw fill if FillColor is set
        if (FillColor.HasValue)
        {
            using var fillPaint = new SKPaint
            {
                Color = FillColor.Value,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawPath(fillPath, fillPaint);
        }

        // Draw line
        using var linePaint = new SKPaint
        {
            Color = LineColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = LineThickness,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        canvas.DrawPath(path, linePaint);

        // Restore canvas state (remove clipping)
        canvas.Restore();
    }
}
