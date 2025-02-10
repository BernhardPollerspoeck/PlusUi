using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.UiElements;

public abstract class UiElement
{
    private readonly Dictionary<string, List<Action>> _bindings = [];

    #region Background Color
    protected SKColor BackgroundColor
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

    #region margin
    protected Margin Margin
    {
        get => field;
        set
        {
            field = value;
            Measure();
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
        //TODO: called too often => stack overflow 
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

    public Size Measure()
    {
        Size = MeasureInternal() + Margin;
        return Size;
    }
    protected abstract Size MeasureInternal();

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
