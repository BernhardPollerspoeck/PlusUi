using PlusUi.core.Enumerations;
using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.CoreElements;

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

    #region BackgroundColor
    public SKColor BackgroundColor
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
    public Margin Margin
    {
        get => field;
        set
        {
            field = value;
            UpdateBindings(nameof(Margin));
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
    public HorizontalAlignment HorizontalAlignment
    {
        get => field;
        set
        {
            field = value;
            UpdateBindings(nameof(HorizontalAlignment));
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
    public VerticalAlignment VerticalAlignment
    {
        get => field;
        set
        {
            field = value;
            UpdateBindings(nameof(VerticalAlignment));
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

    #region size
    public Size? DesiredSize
    {
        get => field;
        set
        {
            field = value;
            UpdateBindings(nameof(DesiredSize));
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
        DesiredSize = new Size(width, DesiredSize?.Height ?? 0);
        return this;
    }
    public UiElement SetDesiredHeight(float height)
    {
        DesiredSize = new Size(DesiredSize?.Width ?? 0, height);
        return this;
    }
    public UiElement BindDesiredWidth(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => DesiredSize = new Size(propertyGetter(), DesiredSize?.Height ?? 0));
        return this;
    }
    public UiElement BindDesiredHeight(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => DesiredSize = new Size(DesiredSize?.Width ?? 0, propertyGetter()));
        return this;
    }
    #endregion

    public Size ElementSize { get; private set; }
    public UiElement? Parent { get; set; }
    public Point Position { get; set; }

    #region Measuring
    public Size Measure(Size availableSize)
    {
        if (_needsMeasure)
        {
            ElementSize = DesiredSize.HasValue
                ? new Size(
                    Math.Min(DesiredSize.Value.Width, availableSize.Width),
                    Math.Min(DesiredSize.Value.Height, availableSize.Height))
                : MeasureInternal(availableSize);
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
            HorizontalAlignment.Center => bounds.CenterX - ElementSize.Width / 2 + Margin.Left - Margin.Right,
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.X + Margin.Left,
        };

        var y = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.CenterY - ElementSize.Height / 2 + Margin.Top - Margin.Bottom,
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
            canvas.DrawRect(
                Position.X,
                Position.Y,
                ElementSize.Width,
                ElementSize.Height,
                BackgroundPaint);
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
}

