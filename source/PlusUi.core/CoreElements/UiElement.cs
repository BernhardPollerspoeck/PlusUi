using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.ComponentModel;

namespace PlusUi.core;

public abstract class UiElement<T> : UiElement where T : UiElement<T>
{
    public new T SetBackgroundColor(SKColor color)
    {
        base.SetBackgroundColor(color);
        return (T)this;
    }
    public new T BindBackgroundColor(string propertyName, Func<SKColor> propertyGetter)
    {
        base.BindBackgroundColor(propertyName, propertyGetter);
        return (T)this;
    }

    public new T SetMargin(Margin margin)
    {
        base.SetMargin(margin);
        return (T)this;
    }
    public new T BindMargin(string propertyName, Func<Margin> propertyGetter)
    {
        base.BindMargin(propertyName, propertyGetter);
        return (T)this;
    }
}
public abstract class UiElement
{
    private bool _needsMeasure = true;
    private readonly Dictionary<string, List<Action>> _bindings = [];
    protected readonly Dictionary<string, List<Action<object>>> _setter = [];

    #region BackgroundColor
    internal SKColor BackgroundColor
    {
        get => field;
        set
        {
            field = value;
            BackgroundPaint = CreateBackgroundPaint();
        }
    } = SKColors.Transparent;
    public UiElement SetBackgroundColor(SKColor color)
    {
        BackgroundColor = color;
        return this;
    }
    public UiElement BindBackgroundColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => BackgroundColor = propertyGetter());
        return this;
    }
    #endregion

    #region Margin
    internal Margin Margin
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public UiElement SetMargin(Margin margin)
    {
        Margin = margin;
        return this;
    }
    public UiElement BindMargin(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => Margin = propertyGetter());
        return this;
    }
    #endregion

    #region HorizontalAlignment
    internal HorizontalAlignment HorizontalAlignment
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = HorizontalAlignment.Left;
    public UiElement SetHorizontalAlignment(HorizontalAlignment alignment)
    {
        HorizontalAlignment = alignment;
        return this;
    }
    public UiElement BindHorizontalAlignment(string propertyName, Func<HorizontalAlignment> propertyGetter)
    {
        RegisterBinding(propertyName, () => HorizontalAlignment = propertyGetter());
        return this;
    }
    #endregion

    #region VerticalAlignment
    internal VerticalAlignment VerticalAlignment
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = VerticalAlignment.Top;
    public UiElement SetVerticalAlignment(VerticalAlignment alignment)
    {
        VerticalAlignment = alignment;
        return this;
    }
    public UiElement BindVerticalAlignment(string propertyName, Func<VerticalAlignment> propertyGetter)
    {
        RegisterBinding(propertyName, () => VerticalAlignment = propertyGetter());
        return this;
    }
    #endregion

    #region CornerRadius
    internal float CornerRadius
    {
        get => field;
        set
        {
            field = value;
        }
    } = 0;
    public UiElement SetCornerRadius(float radius)
    {
        CornerRadius = radius;
        return this;
    }
    public UiElement BindCornerRadius(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => CornerRadius = propertyGetter());
        return this;
    }
    #endregion

    #region size
    internal Size? DesiredSize
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public UiElement SetDesiredSize(Size size)
    {
        DesiredSize = size;
        return this;
    }
    public UiElement BindDesiredSize(string propertyName, Func<Size> propertyGetter)
    {
        RegisterBinding(propertyName, () => DesiredSize = propertyGetter());
        return this;
    }
    public UiElement SetDesiredWidth(float width)
    {
        DesiredSize = new Size(width, DesiredSize?.Height ?? -1);
        return this;
    }
    public UiElement SetDesiredHeight(float height)
    {
        DesiredSize = new Size(DesiredSize?.Width ?? -1, height);
        return this;
    }
    public UiElement BindDesiredWidth(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => DesiredSize = new Size(propertyGetter(), DesiredSize?.Height ?? -10));
        return this;
    }
    public UiElement BindDesiredHeight(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => DesiredSize = new Size(DesiredSize?.Width ?? -1, propertyGetter()));
        return this;
    }
    #endregion

    protected UiElement()
    {
        ApplyStyles();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Size ElementSize { get; protected set; }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public UiElement? Parent { get; set; }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Point Position { get; protected set; }

    #region Measuring
    public Size Measure(Size availableSize)
    {
        if (_needsMeasure)
        {
            var measuredSize = MeasureInternal(availableSize);

            var desiredWidth = DesiredSize?.Width >= 0 ? DesiredSize.Value.Width : measuredSize.Width;
            var desiredHeight = DesiredSize?.Height >= 0 ? DesiredSize.Value.Height : measuredSize.Height;
            ElementSize = new Size(
                Math.Min(desiredWidth, availableSize.Width),
                Math.Min(desiredHeight, availableSize.Height));

            _needsMeasure = false;
        }
        return ElementSize;
    }
    protected virtual Size MeasureInternal(Size availableSize)
    {
        return new Size(
            Math.Min(ElementSize.Width, availableSize.Width),
            Math.Min(ElementSize.Height, availableSize.Height));
    }
    protected void InvalidateMeasure()
    {
        _needsMeasure = true;
        Parent?.InvalidateMeasure();
    }
    #endregion

    #region Arranging
    public Point Arrange(Rect bounds)
    {
        Position = ArrangeInternal(bounds);
        return Position;
    }
    protected virtual Point ArrangeInternal(Rect bounds)
    {
        var x = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => bounds.CenterX - (ElementSize.Width / 2),
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.X + Margin.Left,
        };

        var y = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.CenterY - (ElementSize.Height / 2),
            VerticalAlignment.Bottom => bounds.Bottom - ElementSize.Height - Margin.Bottom,
            _ => bounds.Y + Margin.Top,
        };

        return new Point(x, y);
    }
    #endregion

    #region render cache
    protected SKPaint BackgroundPaint { get; set; } = null!;
    private SKPaint CreateBackgroundPaint()
    {
        return new SKPaint
        {
            Color = BackgroundColor,
            IsAntialias = true,
        };
    }
    #endregion

    public virtual void BuildContent()
    {
    }

    #region bindings
    /// <summary>
    /// This one should get called by the elements binding methods to register value setters
    /// </summary>
    /// <param name="updateAction"></param>
    protected void RegisterBinding(string propertyName, Action updateAction)
    {
        if (!_bindings.TryGetValue(propertyName, out var updateActions))
        {
            updateActions = [];
            _bindings.Add(propertyName, updateActions);
        }

        updateActions.Add(updateAction);

        UpdateBindings(propertyName);
    }
    protected void RegisterSetter<TValue>(string propertyName, Action<TValue> setter)
    {
        if (!_setter.TryGetValue(propertyName, out var setterActions))
        {
            setterActions = [];
            _setter.Add(propertyName, setterActions);
        }
        setterActions.Add(value => setter((TValue)value));
    }
    public void UpdateBindings(string propertyName)
    {
        if (_bindings.TryGetValue(propertyName, out var updateActions))
        {
            foreach (var update in updateActions)
            {
                update();
            }
        }

        UpdateBindingsInternal(propertyName);
    }
    protected virtual void UpdateBindingsInternal(string propertyName) { }
    #endregion

    #region rendering
    public virtual void Render(SKCanvas canvas)
    {
        if (BackgroundColor != SKColors.Transparent)
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
    }
    #endregion

    #region input
    public virtual UiElement? HitTest(Point point)
    {
        return point.X >= Position.X && point.X <= Position.X + ElementSize.Width
            && point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height
            ? this
            : null;
    }
    #endregion

    public virtual void ApplyStyles()
    {
        var style = ServiceProviderService.ServiceProvider?.GetRequiredService<Style>();
        style?.ApplyStyle(this);
    }
}

