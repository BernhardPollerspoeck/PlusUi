using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using SkiaSharp;
using System.ComponentModel;

namespace PlusUi.core;

[GenerateGenericWrapper]
public abstract class UiElement : IDisposable
{
    private readonly Dictionary<string, List<Action>> _bindings = [];
    protected readonly Dictionary<string, List<Action<object>>> _setter = [];
    protected bool _ignoreStyling;


    protected virtual bool NeedsMeasure { get; set; } = true;

    #region Debug
    protected bool Debug { get; private set; }
    public UiElement SetDebug(bool debug = true)
    {
        Debug = debug;
        return this;
    }
    #endregion

    #region IsVisible
    internal bool IsVisible { get; set; } = true;

    public UiElement SetIsVisible(bool isVisible)
    {
        IsVisible = isVisible;
        return this;
    }

    public UiElement BindIsVisible(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsVisible = propertyGetter());
        return this;
    }
    #endregion

    #region VisualOffset
    internal Point VisualOffset { get; set; } = new Point(0, 0);
    public UiElement SetVisualOffset(Point offset)
    {
        VisualOffset = offset;
        return this;
    }
    public UiElement BindVisualOffset(string propertyName, Func<Point> propertyGetter)
    {
        RegisterBinding(propertyName, () => VisualOffset = propertyGetter());
        return this;
    }
    #endregion

    #region Background
    /// <summary>
    /// The background of the element (gradient, solid color, or custom).
    /// </summary>
    internal IBackground? Background { get; set; }

    public UiElement SetBackground(IBackground? background)
    {
        Background = background;
        return this;
    }

    /// <summary>
    /// Sets a solid color background for the element.
    /// This is a convenience overload that internally creates a SolidColorBackground.
    /// </summary>
    /// <param name="color">The solid color to use for the background</param>
    public UiElement SetBackground(SKColor color)
    {
        Background = new SolidColorBackground(color);
        return this;
    }

    public UiElement BindBackground(string propertyName, Func<IBackground?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Background = propertyGetter());
        return this;
    }

    /// <summary>
    /// Binds a solid color background to a property.
    /// This is a convenience overload that internally creates a SolidColorBackground.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to</param>
    /// <param name="propertyGetter">Function that returns the color from the property</param>
    public UiElement BindBackground(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => Background = new SolidColorBackground(propertyGetter()));
        return this;
    }
    #endregion

    #region BackgroundColor (Deprecated - for backward compatibility)
    /// <summary>
    /// [Obsolete] Use SetBackground() instead. Background color of the element.
    /// </summary>
    [Obsolete("Use SetBackground() instead")]
    internal SKColor BackgroundColor
    {
        get => (Background as SolidColorBackground)?.Color ?? SKColors.Transparent;
        set
        {
            Background = new SolidColorBackground(value);
        }
    }

    /// <summary>
    /// [Obsolete] Use SetBackground() instead.
    /// </summary>
    [Obsolete("Use SetBackground() instead")]
    public UiElement SetBackgroundColor(SKColor color)
    {
        return SetBackground(new SolidColorBackground(color));
    }

    /// <summary>
    /// [Obsolete] Use BindBackground() instead.
    /// </summary>
    [Obsolete("Use BindBackground() instead")]
    public UiElement BindBackgroundColor(string propertyName, Func<SKColor> propertyGetter)
    {
        return BindBackground(propertyName, () => new SolidColorBackground(propertyGetter()));
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
    internal virtual HorizontalAlignment HorizontalAlignment
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
    internal virtual VerticalAlignment VerticalAlignment
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

    #region ShadowColor
    internal SKColor ShadowColor
    {
        get => field;
        set
        {
            field = value;
            InvalidateShadowCache();
        }
    } = SKColors.Transparent;
    public UiElement SetShadowColor(SKColor color)
    {
        ShadowColor = color;
        return this;
    }
    public UiElement BindShadowColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => ShadowColor = propertyGetter());
        return this;
    }
    #endregion

    #region ShadowOffset
    internal Point ShadowOffset
    {
        get => field;
        set
        {
            field = value;
            InvalidateShadowCache();
        }
    } = new Point(0, 0);
    public UiElement SetShadowOffset(Point offset)
    {
        ShadowOffset = offset;
        return this;
    }
    public UiElement BindShadowOffset(string propertyName, Func<Point> propertyGetter)
    {
        RegisterBinding(propertyName, () => ShadowOffset = propertyGetter());
        return this;
    }
    #endregion

    #region ShadowBlur
    internal float ShadowBlur
    {
        get => field;
        set
        {
            field = value;
            InvalidateShadowCache();
        }
    } = 0;
    public UiElement SetShadowBlur(float blur)
    {
        ShadowBlur = blur;
        return this;
    }
    public UiElement BindShadowBlur(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => ShadowBlur = propertyGetter());
        return this;
    }
    #endregion

    #region ShadowSpread
    internal float ShadowSpread { get; set; } = 0;
    public UiElement SetShadowSpread(float spread)
    {
        ShadowSpread = spread;
        return this;
    }
    public UiElement BindShadowSpread(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => ShadowSpread = propertyGetter());
        return this;
    }
    #endregion

    #region size
    internal virtual Size? DesiredSize
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
        RegisterBinding(propertyName, () => DesiredSize = new Size(propertyGetter(), DesiredSize?.Height ?? -1));
        return this;
    }
    public UiElement BindDesiredHeight(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => DesiredSize = new Size(DesiredSize?.Width ?? -1, propertyGetter()));
        return this;
    }
    #endregion

    public UiElement IgnoreStyling()
    {
        _ignoreStyling = true;
        return this;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public UiElement? Parent { get; set; }


    [EditorBrowsable(EditorBrowsableState.Never)]
    public Size ElementSize { get; protected set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Point Position { get; protected set; }

    #region Measuring
    public Size Measure(Size availableSize, bool dontStretch = false)
    {
        if (NeedsMeasure || dontStretch)
        {
            var measuredSize = MeasureInternal(availableSize, dontStretch);

            // For width: Use DesiredSize if set, or stretch to available width if alignment is Stretch, otherwise use measured width
            var desiredWidth = DesiredSize?.Width >= 0
                ? Math.Min(DesiredSize.Value.Width, availableSize.Width)
                : !dontStretch && HorizontalAlignment == HorizontalAlignment.Stretch
                    ? availableSize.Width - Margin.Horizontal
                    : Math.Min(measuredSize.Width, availableSize.Width);

            // For height: Use DesiredSize if set, or stretch to available height if alignment is Stretch, otherwise use measured height
            var desiredHeight = DesiredSize?.Height >= 0
                ? Math.Min(DesiredSize.Value.Height, availableSize.Height)
                : !dontStretch && VerticalAlignment == VerticalAlignment.Stretch
                    ? availableSize.Height - Margin.Vertical
                    : Math.Min(measuredSize.Height, availableSize.Height);

            // Constrain to available size
            ElementSize = new Size(desiredWidth, desiredHeight);

            NeedsMeasure = dontStretch;//if we ignore stretching it is a pure calculation pass. so we need to remeasure again
        }
        return ElementSize;
    }
    public virtual Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        return new Size(
            Math.Min(0, availableSize.Width),
            Math.Min(0, availableSize.Height));
    }
    public void InvalidateMeasure()
    {
        NeedsMeasure = true;
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

    private SKImageFilter? _cachedShadowFilter;
    private SKPaint? _cachedShadowPaint;

    private void InvalidateShadowCache()
    {
        _cachedShadowFilter?.Dispose();
        _cachedShadowFilter = null;
        _cachedShadowPaint?.Dispose();
        _cachedShadowPaint = null;
    }

    private SKImageFilter GetShadowFilter()
    {
        _cachedShadowFilter ??= SKImageFilter.CreateDropShadow(
                ShadowOffset.X,
                ShadowOffset.Y,
                ShadowBlur / 2, // SkiaSharp uses sigma, which is roughly blur/2
                ShadowBlur / 2,
                ShadowColor);
        return _cachedShadowFilter;
    }

    private SKPaint GetShadowPaint()
    {
        _cachedShadowPaint ??= new SKPaint
        {
            IsAntialias = true,
            ImageFilter = GetShadowFilter()
        };
        return _cachedShadowPaint;
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
    public void UpdateBindings()
    {
        foreach (var propertyGroup in _bindings)
        {
            foreach (var update in propertyGroup.Value)
            {
                update();
            }
        }
        UpdateBindingsInternal();
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
    protected virtual void UpdateBindingsInternal() { }
    protected virtual void UpdateBindingsInternal(string propertyName) { }
    #endregion

    #region rendering
    /// <summary>
    /// Renders the shadow for this element if shadow properties are configured.
    /// Only renders when ShadowColor.Alpha > 0 and ShadowBlur > 0.
    /// Default implementation renders shadow on element bounds with CornerRadius support.
    /// Subclasses can override for custom shadow shapes.
    /// </summary>
    protected virtual void RenderShadow(SKCanvas canvas)
    {
        if (ShadowColor.Alpha == 0 || ShadowBlur == 0)
            return;

        var shadowPaint = GetShadowPaint();

        var shadowRect = new SKRect(
            Position.X + VisualOffset.X - ShadowSpread,
            Position.Y + VisualOffset.Y - ShadowSpread,
            Position.X + VisualOffset.X + ElementSize.Width + ShadowSpread,
            Position.Y + VisualOffset.Y + ElementSize.Height + ShadowSpread);

        if (CornerRadius > 0)
        {
            canvas.DrawRoundRect(shadowRect, CornerRadius, CornerRadius, shadowPaint);
        }
        else
        {
            canvas.DrawRect(shadowRect, shadowPaint);
        }
    }

    public virtual void Render(SKCanvas canvas)
    {
        if (Debug)
        {
            var debugPaint = new SKPaint
            {
                Color = SKColors.Red,
                IsStroke = true,
                StrokeWidth = 1
            };
            var rect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);
            canvas.DrawRect(rect, debugPaint);

            if (Margin.Horizontal > 0 || Margin.Vertical > 0)
            {
                var marginRect = new SKRect(
                    Position.X + VisualOffset.X - Margin.Left,
                    Position.Y + VisualOffset.Y - Margin.Top,
                    Position.X + VisualOffset.X + ElementSize.Width + Margin.Right,
                    Position.Y + VisualOffset.Y + ElementSize.Height + Margin.Bottom);
                canvas.DrawRect(marginRect, debugPaint);
            }
        }


        if (IsVisible)
        {
            RenderShadow(canvas);

            if (Background is not null)
            {
                var rect = new SKRect(
                    Position.X + VisualOffset.X,
                    Position.Y + VisualOffset.Y,
                    Position.X + VisualOffset.X + ElementSize.Width,
                    Position.Y + VisualOffset.Y + ElementSize.Height);

                Background.Render(canvas, rect, CornerRadius);
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
        if (_ignoreStyling)
        {
            return;
        }
        var style = ServiceProviderService.ServiceProvider?.GetRequiredService<Style>();
        style?.ApplyStyle(this);
    }

    #region IDisposable
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                InvalidateShadowCache();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}

