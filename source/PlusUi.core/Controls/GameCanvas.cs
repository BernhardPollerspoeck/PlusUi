using System;
using System.Diagnostics;
using System.Linq.Expressions;
using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// The per-frame draw call arguments handed to a <see cref="GameCanvas"/> renderer.
/// The canvas is already translated so that (0,0) is the canvas' top-left corner and clipped to
/// <see cref="Size"/> — draw in local coordinates from 0..Width / 0..Height.
/// </summary>
public readonly struct GameCanvasDrawContext(
    SKCanvas canvas,
    Size size,
    TimeSpan deltaTime,
    TimeSpan totalTime,
    long frameCount)
{
    /// <summary>The Skia canvas to draw into (origin at the canvas' top-left, clipped to <see cref="Size"/>).</summary>
    public SKCanvas Canvas { get; } = canvas;

    /// <summary>The current size of the drawable area.</summary>
    public Size Size { get; } = size;

    /// <summary>Time elapsed since the previous frame (zero on the first frame).</summary>
    public TimeSpan DeltaTime { get; } = deltaTime;

    /// <summary>Time elapsed since the first frame was drawn.</summary>
    public TimeSpan TotalTime { get; } = totalTime;

    /// <summary>Zero-based index of this frame.</summary>
    public long FrameCount { get; } = frameCount;
}

/// <summary>
/// A control that hands the developer a raw Skia canvas once per frame for imperative,
/// immediate-mode rendering — the opposite of the declarative control tree. Its
/// <see cref="Render(SKCanvas)"/> is invoked every frame by the render loop, so animations and
/// games can redraw continuously without invalidation.
/// </summary>
/// <remarks>
/// Two ways to supply the per-frame logic:
/// <list type="bullet">
/// <item>Fluent callback: <c>new GameCanvas().SetOnDraw(ctx =&gt; ...)</c>.</item>
/// <item>Subclass and override <see cref="Draw(GameCanvasDrawContext)"/>.</item>
/// </list>
/// Input is intentionally NOT handled here — subscribe to <see cref="IGlobalInputService"/> to
/// read pointer/keyboard globally, keeping canvas and input decoupled.
/// </remarks>
/// <example>
/// <code>
/// new GameCanvas()
///     .SetOnDraw(ctx =>
///     {
///         using var paint = new SKPaint { Color = SKColors.Lime, IsAntialias = true };
///         ctx.Canvas.DrawCircle(ctx.Size.Width / 2, ctx.Size.Height / 2, 20, paint);
///     });
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class GameCanvas : UiElement
{
    private readonly Stopwatch _clock = new();
    private TimeSpan _lastFrameTime;
    private long _frameCount;

    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

    public GameCanvas()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
    }

    #region OnDraw
    internal Action<GameCanvasDrawContext>? OnDraw
    {
        get => field;
        set => field = value;
    }

    /// <summary>
    /// Sets the per-frame draw callback. Called once per rendered frame with a
    /// <see cref="GameCanvasDrawContext"/>. Ignored when <see cref="Draw"/> is overridden to not
    /// call the base implementation.
    /// </summary>
    public GameCanvas SetOnDraw(Action<GameCanvasDrawContext> onDraw)
    {
        OnDraw = onDraw;
        return this;
    }

    /// <summary>
    /// Binds the per-frame draw callback to a view-model property that returns the delegate.
    /// </summary>
    public GameCanvas BindOnDraw(Expression<Func<Action<GameCanvasDrawContext>?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => OnDraw = getter());
        return this;
    }
    #endregion

    /// <summary>
    /// Per-frame render hook. The default implementation invokes the <see cref="SetOnDraw"/>
    /// callback; override in a subclass for an object-oriented game/render loop.
    /// </summary>
    protected virtual void Draw(GameCanvasDrawContext context)
    {
        OnDraw?.Invoke(context);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Fill the available space by default; honour an explicit DesiredSize per axis.
        var width = DesiredSize?.Width is > 0 ? DesiredSize.Value.Width : availableSize.Width;
        var height = DesiredSize?.Height is > 0 ? DesiredSize.Value.Height : availableSize.Height;
        return new Size(width, height);
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Advance the frame clock. It starts lazily on the first rendered frame so TotalTime is
        // measured from first paint (not construction); that first frame reports zero delta/total.
        TimeSpan now;
        TimeSpan delta;
        if (!_clock.IsRunning)
        {
            _clock.Start();
            now = TimeSpan.Zero;
            delta = TimeSpan.Zero;
        }
        else
        {
            now = _clock.Elapsed;
            delta = now - _lastFrameTime;
        }
        _lastFrameTime = now;

        var left = Position.X + VisualOffset.X;
        var top = Position.Y + VisualOffset.Y;

        canvas.Save();
        canvas.ClipRect(new SKRect(left, top, left + ElementSize.Width, top + ElementSize.Height));
        canvas.Translate(left, top);

        Draw(new GameCanvasDrawContext(canvas, ElementSize, delta, now, _frameCount++));

        canvas.Restore();
    }
}
