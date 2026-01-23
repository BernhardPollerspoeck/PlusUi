using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Binding;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.Focus;
using SkiaSharp;
using System.ComponentModel;
using System.Linq.Expressions;
using PlusUi.core.UiPropGen;

namespace PlusUi.core;

/// <summary>
/// Base class for all UI elements in PlusUi.
/// Provides layout, rendering, data binding, styling, accessibility, and input handling.
/// </summary>
[GenerateGenericWrapper]
[UiPropGenDebug]
[UiPropGenIsVisible]
[UiPropGenVisualOffset]
[UiPropGenCornerRadius]
[UiPropGenShadowSpread]
[UiPropGenTabIndex]
[UiPropGenTabStop]
[UiPropGenFocusRingColor]
[UiPropGenFocusRingWidth]
[UiPropGenFocusRingOffset]
[UiPropGenFocusedBorderColor]
[UiPropGenAccessibilityLabel]
[UiPropGenAccessibilityHint]
[UiPropGenAccessibilityValue]
[UiPropGenIsAccessibilityElement]
[UiPropGenHighContrastForeground]
[UiPropGenMargin]
[UiPropGenHorizontalAlignment]
[UiPropGenVerticalAlignment]
[UiPropGenShadowColor]
[UiPropGenShadowOffset]
[UiPropGenShadowBlur]
[UiPropGenOpacity]
[UiPropGenBackground]
[UiPropGenFocusedBackground]
[UiPropGenHighContrastBackground]
[UiPropGenDesiredSize]
public abstract partial class UiElement : IDisposable
{
    #region Fields
    private readonly Dictionary<string, List<Action>> _bindings = [];
    private readonly List<PathBindingTracker> _pathBindings = [];
    protected readonly Dictionary<string, List<Action<object?>>> _setter = [];
    protected bool _ignoreStyling;
    #endregion

    #region Services
    /// <summary>
    /// Paint registry service for managing shared paint/font resources.
    /// </summary>
    protected IPaintRegistryService PaintRegistry { get; }

    /// <summary>
    /// Expression path service for binding path extraction.
    /// </summary>
    private protected IExpressionPathService ExpressionPathService { get; }

    /// <summary>
    /// Configuration for the PlusUi application.
    /// </summary>
    protected PlusUiConfiguration? Configuration { get; }

    /// <summary>
    /// Accessibility settings service for High Contrast detection and touch target sizes.
    /// </summary>
    protected IAccessibilitySettingsService? AccessibilitySettings { get; }

    /// <summary>
    /// Focus manager for handling focus changes.
    /// </summary>
    private IFocusManager? FocusManager { get; }
    #endregion

    #region Constructor
    protected UiElement()
    {
        // Services
        PaintRegistry = ServiceProviderService.ServiceProvider?.GetService<IPaintRegistryService>()
            ?? throw new InvalidOperationException("PaintRegistry service not available. Ensure the application is properly initialized.");
        ExpressionPathService = ServiceProviderService.ServiceProvider?.GetService<IExpressionPathService>()
            ?? new Binding.ExpressionPathService();
        Configuration = ServiceProviderService.ServiceProvider?.GetService<PlusUiConfiguration>();
        AccessibilitySettings = ServiceProviderService.ServiceProvider?.GetService<IAccessibilitySettingsService>();
        FocusManager = ServiceProviderService.ServiceProvider?.GetService<IFocusManager>();

        // Layout defaults
        IsVisible = PlusUiDefaults.IsVisible;
        Opacity = PlusUiDefaults.Opacity;
        HorizontalAlignment = PlusUiDefaults.HorizontalAlignment;
        VerticalAlignment = PlusUiDefaults.VerticalAlignment;
        CornerRadius = PlusUiDefaults.CornerRadiusNone;

        // Shadow defaults
        ShadowColor = PlusUiDefaults.ShadowColorNone;
        ShadowBlur = PlusUiDefaults.ShadowBlur;
        ShadowSpread = PlusUiDefaults.ShadowSpread;

        // Focus defaults
        TabStop = PlusUiDefaults.TabStop;
        FocusRingColor = PlusUiDefaults.AccentPrimary;
        FocusRingWidth = PlusUiDefaults.FocusRingWidth;
        FocusRingOffset = PlusUiDefaults.FocusRingOffset;

        // Accessibility defaults
        AccessibilityTraits = PlusUiDefaults.AccessibilityTraits;
        IsAccessibilityElement = PlusUiDefaults.IsAccessibilityElement;
    }
    #endregion

    protected virtual bool NeedsMeasure { get; set; } = true;
    protected internal virtual bool NeedsArrange { get; set; } = true;


    protected virtual bool SkipBackgroundRendering => false;

    public UiElement IgnoreStyling()
    {
        _ignoreStyling = true;
        return this;
    }

    #region Tooltip
    /// <summary>
    /// The tooltip attachment for this element, if any.
    /// </summary>
    internal TooltipAttachment? Tooltip { get; set; }
    #endregion

    #region ContextMenu
    /// <summary>
    /// The context menu for this element, if any.
    /// Opens when the user right-clicks (or long-presses on touch).
    /// </summary>
    internal ContextMenu? ContextMenu { get; set; }
    #endregion

    #region Focus
    /// <summary>
    /// Gets whether this element can receive keyboard focus.
    /// Override in derived classes to define whether the control should be focusable.
    /// </summary>
    protected internal abstract bool IsFocusable { get; }

    /// <summary>
    /// Gets or sets whether this element currently has keyboard focus.
    /// </summary>
    public bool IsFocused { get; internal set; }

    /// <summary>
    /// Programmatically sets focus to this element.
    /// </summary>
    /// <returns>True if focus was successfully set, false otherwise.</returns>
    public bool Focus()
    {
        if (!IsFocusable || !IsVisible)
        {
            return false;
        }

        if (FocusManager == null)
        {
            return false;
        }

        if (this is IFocusable focusable)
        {
            FocusManager.SetFocus(focusable);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called when this element receives focus.
    /// Override in subclasses to handle focus events.
    /// </summary>
    protected internal virtual void OnFocus() { }

    /// <summary>
    /// Called when this element loses focus.
    /// Override in subclasses to handle blur events.
    /// </summary>
    protected internal virtual void OnBlur() { }

    /// <summary>
    /// Renders the focus ring around this element if it has focus.
    /// </summary>
    protected virtual void RenderFocusRing(SKCanvas canvas)
    {
        if (!IsFocused || !IsFocusable)
        {
            return;
        }

        using var paint = new SKPaint
        {
            Color = FocusRingColor,
            IsStroke = true,
            StrokeWidth = FocusRingWidth,
            IsAntialias = true
        };

        var rect = new SKRect(
            Position.X + VisualOffset.X - FocusRingOffset,
            Position.Y + VisualOffset.Y - FocusRingOffset,
            Position.X + VisualOffset.X + ElementSize.Width + FocusRingOffset,
            Position.Y + VisualOffset.Y + ElementSize.Height + FocusRingOffset);

        if (CornerRadius > 0)
        {
            canvas.DrawRoundRect(rect, CornerRadius + FocusRingOffset, CornerRadius + FocusRingOffset, paint);
        }
        else
        {
            canvas.DrawRect(rect, paint);
        }
    }
    #endregion

    #region Accessibility
    /// <summary>
    /// Gets the semantic accessibility role of this element.
    /// Override in derived classes to define the semantic role of the control.
    /// </summary>
    public abstract AccessibilityRole AccessibilityRole { get; }

    /// <summary>
    /// Gets or sets the accessibility traits that describe the state of this element.
    /// </summary>
    public AccessibilityTrait AccessibilityTraits { get; protected internal set; }

    /// <summary>
    /// Sets the accessibility traits.
    /// </summary>
    public UiElement SetAccessibilityTraits(AccessibilityTrait traits)
    {
        AccessibilityTraits = traits;
        return this;
    }

    /// <summary>
    /// Adds accessibility traits to the existing traits.
    /// </summary>
    public UiElement AddAccessibilityTraits(AccessibilityTrait traits)
    {
        AccessibilityTraits |= traits;
        return this;
    }

    /// <summary>
    /// Removes accessibility traits from the existing traits.
    /// </summary>
    public UiElement RemoveAccessibilityTraits(AccessibilityTrait traits)
    {
        AccessibilityTraits &= ~traits;
        return this;
    }

    /// <summary>
    /// Binds the accessibility traits.
    /// </summary>
    public UiElement BindAccessibilityTraits(Expression<Func<AccessibilityTrait>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AccessibilityTraits = getter());
        return this;
    }

    /// <summary>
    /// Gets the computed accessibility label for this element.
    /// If no explicit label is set, derived classes can override to generate one automatically.
    /// </summary>
    public virtual string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel;
    }

    /// <summary>
    /// Gets the computed accessibility value for this element.
    /// If no explicit value is set, derived classes can override to generate one automatically.
    /// </summary>
    public virtual string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue;
    }

    /// <summary>
    /// Gets the computed accessibility traits for this element.
    /// Derived classes can override to add dynamic traits based on element state.
    /// </summary>
    public virtual AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = AccessibilityTraits;

        // Add dynamic traits based on element state
        if (!IsVisible)
        {
            traits |= AccessibilityTrait.Hidden;
        }
        if (IsFocusable)
        {
            traits |= AccessibilityTrait.Focusable;
        }
        if (IsFocused)
        {
            traits |= AccessibilityTrait.Focused;
        }

        return traits;
    }

    /// <summary>
    /// Gets the effective background considering focus state and high contrast mode.
    /// Priority: High contrast > Focused > Normal
    /// </summary>
    protected IBackground? GetEffectiveBackground()
    {
        // High contrast takes priority (only if enabled in config)
        if (Configuration?.EnableHighContrastSupport == true && HighContrastBackground != null)
        {
            // ForceHighContrast bypasses system detection
            if (Configuration.ForceHighContrast)
            {
                return HighContrastBackground;
            }

            if (AccessibilitySettings?.IsHighContrastEnabled == true)
            {
                return HighContrastBackground;
            }
        }

        // Then focused state
        if (IsFocused && FocusedBackground != null)
        {
            return FocusedBackground;
        }

        return Background;
    }
    #endregion

    #region MinimumTouchTarget
    private bool? _enforceMinimumTouchTarget;

    /// <summary>
    /// Gets or sets whether this element should enforce minimum touch target size.
    /// When true, the element ensures it meets accessibility guidelines (44x44 pts).
    /// If not explicitly set, uses the global EnforceMinimumTouchTargets configuration.
    /// </summary>
    internal bool EnforceMinimumTouchTarget
    {
        get
        {
            // If explicitly set on this control, use that value
            if (_enforceMinimumTouchTarget.HasValue)
            {
                return _enforceMinimumTouchTarget.Value;
            }

            // Otherwise, use global configuration
            return Configuration?.EnforceMinimumTouchTargets ?? false;
        }
        set => _enforceMinimumTouchTarget = value;
    }

    /// <summary>
    /// Sets whether to enforce minimum touch target size for accessibility.
    /// This overrides the global EnforceMinimumTouchTargets configuration for this control.
    /// </summary>
    public UiElement SetEnforceMinimumTouchTarget(bool enforce)
    {
        _enforceMinimumTouchTarget = enforce;
        return this;
    }

    /// <summary>
    /// Binds whether to enforce minimum touch target size for accessibility.
    /// </summary>
    public UiElement BindEnforceMinimumTouchTarget(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => _enforceMinimumTouchTarget = getter());
        return this;
    }

    /// <summary>
    /// Gets the minimum touch target size from accessibility settings.
    /// Returns 44 by default (Apple/Google recommendation).
    /// </summary>
    protected float GetMinimumTouchTargetSize()
    {
        return AccessibilitySettings?.MinimumTouchTargetSize ?? 44f;
    }
    #endregion

    [EditorBrowsable(EditorBrowsableState.Never)]
    public UiElement? Parent { get; set; }


    [EditorBrowsable(EditorBrowsableState.Never)]
    public Size ElementSize { get; protected set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Point Position { get; protected set; }

    public virtual INotifyPropertyChanged? Context
    {
        get => field;
        internal set
        {
            if (field == value) return;

            // Unsubscribe from old context
            if (field != null)
            {
                field.PropertyChanged -= OnContextPropertyChanged;
            }

            field = value;

            // Subscribe to new context
            if (field != null)
            {
                field.PropertyChanged += OnContextPropertyChanged;
            }

            // Update path bindings with new context
            foreach (var tracker in _pathBindings)
            {
                tracker.SetContext(field);
            }

            // Update all bindings with new context
            UpdateBindings();
        }
    }

    private void OnContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update bindings for the changed property (or all if null)
        if (!string.IsNullOrEmpty(e.PropertyName))
        {
            UpdateBindings(e.PropertyName);
        }
        else
        {
            UpdateBindings();
        }

        // Trigger re-measure and render
        InvalidateMeasure();
    }

    #region Measuring
    public Size Measure(Size availableSize, bool dontStretch = false)
    {
        if (NeedsMeasure || dontStretch)
        {
            // Don't stretch to infinite/MaxValue sizes
            var canStretchWidth = !dontStretch && HorizontalAlignment == HorizontalAlignment.Stretch && availableSize.Width < float.MaxValue;
            var canStretchHeight = !dontStretch && VerticalAlignment == VerticalAlignment.Stretch && availableSize.Height < float.MaxValue;

            var constrainedAvailable = new Size(
                DesiredSize?.Width >= 0 ? Math.Min(DesiredSize.Value.Width, availableSize.Width) : availableSize.Width,
                DesiredSize?.Height >= 0 ? Math.Min(DesiredSize.Value.Height, availableSize.Height) : availableSize.Height
            );

            var measuredSize = MeasureInternal(constrainedAvailable, dontStretch);

            // Calculate max size accounting for margin (element can't be larger than available space minus margin)
            var maxWidth = Math.Max(0, availableSize.Width - Margin.Horizontal);
            var maxHeight = Math.Max(0, availableSize.Height - Margin.Vertical);

            // For width: Use DesiredSize if set, or stretch to max width if alignment is Stretch, otherwise use measured width
            var desiredWidth = DesiredSize?.Width >= 0
                ? Math.Min(DesiredSize.Value.Width, availableSize.Width)
                : canStretchWidth
                    ? maxWidth
                    : Math.Min(measuredSize.Width, maxWidth);

            // For height: Use DesiredSize if set, or stretch to max height if alignment is Stretch, otherwise use measured height
            var desiredHeight = DesiredSize?.Height >= 0
                ? Math.Min(DesiredSize.Value.Height, availableSize.Height)
                : canStretchHeight
                    ? maxHeight
                    : Math.Min(measuredSize.Height, maxHeight);

            // Enforce minimum touch target size for accessibility
            if (EnforceMinimumTouchTarget)
            {
                var minSize = GetMinimumTouchTargetSize();
                desiredWidth = Math.Max(desiredWidth, minSize);
                desiredHeight = Math.Max(desiredHeight, minSize);
            }

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
    public virtual void InvalidateMeasure()
    {
        NeedsMeasure = true;
        if (Parent is { NeedsMeasure: false })
        {
            Parent?.InvalidateMeasure();
        }
        InvalidateArrange(); // Size changes require position recalculation
    }

    public void InvalidateArrange()
    {
        NeedsArrange = true;
        if (Parent is { NeedsArrange: false })
        {
            Parent?.InvalidateArrange();
        }
    }

    internal void ForceInvalidateMeasureToRoot()
    {
        NeedsMeasure = true;
        NeedsArrange = true;
        Parent?.ForceInvalidateMeasureToRoot();
    }
    #endregion

    #region Arranging
    public Point Arrange(Rect bounds)
    {
        // Stretch alignment: expand ElementSize to fill available space (only if no explicit DesiredSize)
        if (HorizontalAlignment == HorizontalAlignment.Stretch && DesiredSize?.Width is null or <= 0)
        {
            var stretchWidth = Math.Max(0, bounds.Width - Margin.Horizontal);
            if (stretchWidth > ElementSize.Width)
            {
                ElementSize = new Size(stretchWidth, ElementSize.Height);
            }
        }
        if (VerticalAlignment == VerticalAlignment.Stretch && DesiredSize?.Height is null or <= 0)
        {
            var stretchHeight = Math.Max(0, bounds.Height - Margin.Vertical);
            if (stretchHeight > ElementSize.Height)
            {
                ElementSize = new Size(ElementSize.Width, stretchHeight);
            }
        }

        if (NeedsArrange)
        {
            Position = ArrangeInternal(bounds);
            NeedsArrange = false;
        }
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

    protected void RegisterPathBinding(string[] pathSegments, Action updateAction)
    {
        var tracker = new PathBindingTracker(pathSegments, updateAction);
        tracker.SetContext(Context);
        _pathBindings.Add(tracker);

        // Also register with the old binding system for manual UpdateBindings() calls
        // This ensures backward compatibility with tests and scenarios not using INotifyPropertyChanged
        foreach (var segment in pathSegments)
        {
            RegisterBinding(segment, updateAction);
        }
    }
    protected void RegisterSetter<TValue>(string propertyName, Action<TValue> setter)
    {
        if (!_setter.TryGetValue(propertyName, out var setterActions))
        {
            setterActions = [];
            _setter.Add(propertyName, setterActions);
        }
        setterActions.Add(value => setter((TValue)value!));
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

    protected virtual Margin? GetDebugPadding() => null;

    public virtual void Render(SKCanvas canvas)
    {
        if (Debug)
        {
            var elementRect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);

            if (Margin.Horizontal > 0 || Margin.Vertical > 0)
            {
                var marginPaint = new SKPaint
                {
                    Color = new SKColor(255, 0, 255, 180),
                    IsStroke = true,
                    StrokeWidth = 2
                };
                var marginRect = new SKRect(
                    Position.X + VisualOffset.X - Margin.Left,
                    Position.Y + VisualOffset.Y - Margin.Top,
                    Position.X + VisualOffset.X + ElementSize.Width + Margin.Right,
                    Position.Y + VisualOffset.Y + ElementSize.Height + Margin.Bottom);
                canvas.DrawRect(marginRect, marginPaint);
            }

            var elementBoundsPaint = new SKPaint
            {
                Color = new SKColor(255, 0, 0, 180),
                IsStroke = true,
                StrokeWidth = 1
            };
            canvas.DrawRect(elementRect, elementBoundsPaint);
        }


        if (IsVisible)
        {
            var useOpacityLayer = Opacity < 1f;
            if (useOpacityLayer)
            {
                canvas.SaveLayer(new SKPaint { Color = SKColors.White.WithAlpha((byte)(Opacity * 255)) });
            }

            RenderShadow(canvas);

            var effectiveBackground = GetEffectiveBackground();
            if (!SkipBackgroundRendering && effectiveBackground is not null)
            {
                var rect = new SKRect(
                    Position.X + VisualOffset.X,
                    Position.Y + VisualOffset.Y,
                    Position.X + VisualOffset.X + ElementSize.Width,
                    Position.Y + VisualOffset.Y + ElementSize.Height);

                effectiveBackground.Render(canvas, rect, CornerRadius);
            }

            // Render focus ring if this element has focus
            RenderFocusRing(canvas);

            if (useOpacityLayer)
            {
                canvas.Restore();
            }
        }

        if (Debug)
        {
            var padding = GetDebugPadding();
            if (padding != null && (padding.Value.Horizontal > 0 || padding.Value.Vertical > 0))
            {
                var paddingPaint = new SKPaint
                {
                    Color = new SKColor(0, 255, 255, 200),
                    IsStroke = true,
                    StrokeWidth = 2
                };

                var contentRect = new SKRect(
                    Position.X + VisualOffset.X + padding.Value.Left,
                    Position.Y + VisualOffset.Y + padding.Value.Top,
                    Position.X + VisualOffset.X + ElementSize.Width - padding.Value.Right,
                    Position.Y + VisualOffset.Y + ElementSize.Height - padding.Value.Bottom);
                canvas.DrawRect(contentRect, paddingPaint);
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

    /// <summary>
    /// Indicates whether this element intercepts and handles click events.
    /// When false, clicks pass through to the parent (e.g., for row selection in lists).
    /// Default: false (opt-in for interactive controls like Button, Entry).
    /// </summary>
    public virtual bool InterceptsClicks => false;
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

                // Dispose path binding trackers
                foreach (var tracker in _pathBindings)
                {
                    tracker.Dispose();
                }
                _pathBindings.Clear();
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

