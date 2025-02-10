using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.UiElements;

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

    public UiElement? Parent { get; set; }
    public Size Size { get; private set; }

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

    public Size Measure(Size availableSize)
    {
        if (_needsMeasure)
        {
            Size = MeasureInternal(availableSize) + Margin;
            _needsMeasure = false;
        }
        return Size;
    }
    protected void InvalidateMeasure()
    {
        _needsMeasure = true;
        Parent?.InvalidateMeasure();
    }
    protected abstract Size MeasureInternal(Size availableSize);

    public virtual void Render(SKCanvas canvas, SKPoint location)
    {
        if (BackgroundColor != SKColors.Transparent)
        {
            canvas.DrawRect(
                location.X,
                location.Y,
                Size.Width,
                Size.Height,
                BackgroundPaint);
        }
    }
}
