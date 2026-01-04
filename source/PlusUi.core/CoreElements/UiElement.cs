using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
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
    protected internal virtual bool NeedsArrange { get; set; } = true;

    #region Debug
    protected bool Debug { get; private set; }
    public UiElement SetDebug(bool debug = true)
    {
        Debug = debug;
        return this;
    }
    #endregion

    #region IsVisible
    public bool IsVisible { get; internal set; } = true;

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

    #region Opacity
    internal float Opacity { get; set; } = 1f;
    public UiElement SetOpacity(float opacity)
    {
        Opacity = Math.Clamp(opacity, 0f, 1f);
        return this;
    }
    public UiElement BindOpacity(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Opacity = Math.Clamp(propertyGetter(), 0f, 1f));
        return this;
    }
    #endregion

    #region Background
    /// <summary>
    /// The background of the element (gradient, solid color, or custom).
    /// </summary>
    internal IBackground? Background { get; set; }
    protected virtual bool SkipBackgroundRendering => false;
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
    public UiElement SetBackground(Color color)
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
    public UiElement BindBackground(string propertyName, Func<Color> propertyGetter)
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
    internal Color BackgroundColor
    {
        get => (Background as SolidColorBackground)?.Color ?? Colors.Transparent;
        set
        {
            Background = new SolidColorBackground(value);
        }
    }

    /// <summary>
    /// [Obsolete] Use SetBackground() instead.
    /// </summary>
    [Obsolete("Use SetBackground() instead")]
    public UiElement SetBackgroundColor(Color color)
    {
        return SetBackground(new SolidColorBackground(color));
    }

    /// <summary>
    /// [Obsolete] Use BindBackground() instead.
    /// </summary>
    [Obsolete("Use BindBackground() instead")]
    public UiElement BindBackgroundColor(string propertyName, Func<Color> propertyGetter)
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
            if (field.Equals(value)) return;
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
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    } = HorizontalAlignment.Undefined;
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
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    } = VerticalAlignment.Undefined;
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
    internal Color ShadowColor
    {
        get => field;
        set
        {
            field = value;
            InvalidateShadowCache();
        }
    } = Colors.Transparent;
    public UiElement SetShadowColor(Color color)
    {
        ShadowColor = color;
        return this;
    }
    public UiElement BindShadowColor(string propertyName, Func<Color> propertyGetter)
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
            if (field.HasValue && field.Equals(value)) return;
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
    /// Gets or sets the tab index for focus order.
    /// Null means automatic order (declaration order).
    /// Negative values exclude the element from tab navigation.
    /// </summary>
    internal int? TabIndex { get; set; }

    /// <summary>
    /// Gets or sets whether this element should be included in tab navigation.
    /// When false, the element is skipped during Tab/Shift+Tab navigation.
    /// Default is true for focusable elements.
    /// </summary>
    internal bool TabStop { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this element currently has keyboard focus.
    /// </summary>
    public bool IsFocused { get; internal set; }

    /// <summary>
    /// Gets or sets the color of the focus ring.
    /// </summary>
    internal Color FocusRingColor { get; set; } = new Color(0, 122, 255); // iOS blue

    /// <summary>
    /// Gets or sets the width of the focus ring stroke.
    /// </summary>
    internal float FocusRingWidth { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the offset of the focus ring from the element bounds.
    /// </summary>
    internal float FocusRingOffset { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the background color when the element has focus.
    /// When null, no focused background is applied.
    /// </summary>
    internal IBackground? FocusedBackground { get; set; }

    /// <summary>
    /// Gets or sets the border color when the element has focus.
    /// When null, no focused border color is applied.
    /// </summary>
    internal Color? FocusedBorderColor { get; set; }

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

        var focusManager = ServiceProviderService.ServiceProvider?.GetService<Services.Focus.IFocusManager>();
        if (focusManager == null)
        {
            return false;
        }

        if (this is IFocusable focusable)
        {
            focusManager.SetFocus(focusable);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the tab index for focus order.
    /// </summary>
    public UiElement SetTabIndex(int? tabIndex)
    {
        TabIndex = tabIndex;
        return this;
    }

    /// <summary>
    /// Binds the TabIndex property.
    /// </summary>
    public UiElement BindTabIndex(string propertyName, Func<int?> propertyGetter)
    {
        RegisterBinding(propertyName, () => TabIndex = propertyGetter());
        return this;
    }

    /// <summary>
    /// Sets whether this element should be included in tab navigation.
    /// </summary>
    public UiElement SetTabStop(bool tabStop)
    {
        TabStop = tabStop;
        return this;
    }

    /// <summary>
    /// Binds the TabStop property.
    /// </summary>
    public UiElement BindTabStop(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => TabStop = propertyGetter());
        return this;
    }

    /// <summary>
    /// Sets the focus ring color.
    /// </summary>
    public UiElement SetFocusRingColor(Color color)
    {
        FocusRingColor = color;
        return this;
    }

    /// <summary>
    /// Sets the focus ring width.
    /// </summary>
    public UiElement SetFocusRingWidth(float width)
    {
        FocusRingWidth = width;
        return this;
    }

    /// <summary>
    /// Sets the focus ring offset from element bounds.
    /// </summary>
    public UiElement SetFocusRingOffset(float offset)
    {
        FocusRingOffset = offset;
        return this;
    }

    /// <summary>
    /// Sets the background to use when the element has focus.
    /// </summary>
    public UiElement SetFocusedBackground(IBackground? background)
    {
        FocusedBackground = background;
        return this;
    }

    /// <summary>
    /// Sets a solid color background to use when the element has focus.
    /// </summary>
    public UiElement SetFocusedBackground(Color color)
    {
        FocusedBackground = new SolidColorBackground(color);
        return this;
    }

    /// <summary>
    /// Sets the border color to use when the element has focus.
    /// </summary>
    public UiElement SetFocusedBorderColor(Color? color)
    {
        FocusedBorderColor = color;
        return this;
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
    /// Gets or sets the accessibility label that describes this element to assistive technologies.
    /// This is the primary text read by screen readers.
    /// </summary>
    public string? AccessibilityLabel { get; protected internal set; }

    /// <summary>
    /// Gets or sets additional accessibility hint text that provides more context about the element.
    /// This is typically read after the label and value.
    /// </summary>
    public string? AccessibilityHint { get; protected internal set; }

    /// <summary>
    /// Gets or sets the current accessibility value of the element (e.g., slider position, text content).
    /// </summary>
    public string? AccessibilityValue { get; protected internal set; }

    /// <summary>
    /// Gets the semantic accessibility role of this element.
    /// Override in derived classes to define the semantic role of the control.
    /// </summary>
    public abstract AccessibilityRole AccessibilityRole { get; }

    /// <summary>
    /// Gets or sets the accessibility traits that describe the state of this element.
    /// </summary>
    public AccessibilityTrait AccessibilityTraits { get; protected internal set; } = AccessibilityTrait.None;

    /// <summary>
    /// Gets or sets whether this element should be exposed to assistive technologies.
    /// When false, the element is hidden from accessibility.
    /// </summary>
    public bool IsAccessibilityElement { get; protected internal set; } = true;

    /// <summary>
    /// Sets the accessibility label.
    /// </summary>
    public UiElement SetAccessibilityLabel(string? label)
    {
        AccessibilityLabel = label;
        return this;
    }

    /// <summary>
    /// Sets the accessibility hint.
    /// </summary>
    public UiElement SetAccessibilityHint(string? hint)
    {
        AccessibilityHint = hint;
        return this;
    }

    /// <summary>
    /// Sets the accessibility value.
    /// </summary>
    public UiElement SetAccessibilityValue(string? value)
    {
        AccessibilityValue = value;
        return this;
    }

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
    /// Sets whether this element should be exposed to assistive technologies.
    /// </summary>
    public UiElement SetIsAccessibilityElement(bool isAccessible)
    {
        IsAccessibilityElement = isAccessible;
        return this;
    }

    /// <summary>
    /// Binds the accessibility label.
    /// </summary>
    public UiElement BindAccessibilityLabel(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => AccessibilityLabel = propertyGetter());
        return this;
    }

    /// <summary>
    /// Binds the accessibility hint.
    /// </summary>
    public UiElement BindAccessibilityHint(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => AccessibilityHint = propertyGetter());
        return this;
    }

    /// <summary>
    /// Binds the accessibility value.
    /// </summary>
    public UiElement BindAccessibilityValue(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => AccessibilityValue = propertyGetter());
        return this;
    }

    /// <summary>
    /// Binds the accessibility traits.
    /// </summary>
    public UiElement BindAccessibilityTraits(string propertyName, Func<AccessibilityTrait> propertyGetter)
    {
        RegisterBinding(propertyName, () => AccessibilityTraits = propertyGetter());
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
    /// Gets or sets the background to use when high contrast mode is enabled.
    /// When set and high contrast is active, this replaces the normal Background.
    /// </summary>
    internal IBackground? HighContrastBackground { get; set; }

    /// <summary>
    /// Gets or sets the foreground/text color to use when high contrast mode is enabled.
    /// </summary>
    internal Color? HighContrastForeground { get; set; }

    /// <summary>
    /// Sets the high contrast background.
    /// </summary>
    public UiElement SetHighContrastBackground(IBackground? background)
    {
        HighContrastBackground = background;
        return this;
    }

    /// <summary>
    /// Sets the high contrast background as a solid color.
    /// </summary>
    public UiElement SetHighContrastBackground(Color color)
    {
        HighContrastBackground = new SolidColorBackground(color);
        return this;
    }

    /// <summary>
    /// Sets the high contrast foreground color.
    /// </summary>
    public UiElement SetHighContrastForeground(Color color)
    {
        HighContrastForeground = color;
        return this;
    }

    /// <summary>
    /// Gets the effective background considering focus state and high contrast mode.
    /// Priority: High contrast > Focused > Normal
    /// </summary>
    protected IBackground? GetEffectiveBackground()
    {
        var config = ServiceProviderService.ServiceProvider?.GetService<PlusUiConfiguration>();

        // High contrast takes priority (only if enabled in config)
        if (config?.EnableHighContrastSupport == true && HighContrastBackground != null)
        {
            // ForceHighContrast bypasses system detection
            if (config.ForceHighContrast)
            {
                return HighContrastBackground;
            }

            var settings = ServiceProviderService.ServiceProvider?.GetService<Services.Accessibility.IAccessibilitySettingsService>();
            if (settings?.IsHighContrastEnabled == true)
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
            var config = ServiceProviderService.ServiceProvider?.GetService<PlusUiConfiguration>();
            return config?.EnforceMinimumTouchTargets ?? false;
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
    /// Gets the minimum touch target size from accessibility settings.
    /// Returns 44 by default (Apple/Google recommendation).
    /// </summary>
    protected float GetMinimumTouchTargetSize()
    {
        var settings = ServiceProviderService.ServiceProvider?.GetService<Services.Accessibility.IAccessibilitySettingsService>();
        return settings?.MinimumTouchTargetSize ?? 44f;
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
            // Constrain available size by DesiredSize before measuring children
            // This ensures children know the actual available space for wrapping
            var constrainedAvailable = new Size(
                DesiredSize?.Width >= 0 ? Math.Min(DesiredSize.Value.Width, availableSize.Width) : availableSize.Width,
                DesiredSize?.Height >= 0 ? Math.Min(DesiredSize.Value.Height, availableSize.Height) : availableSize.Height
            );

            var measuredSize = MeasureInternal(constrainedAvailable, dontStretch);

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

    #region Services
    /// <summary>
    /// Helper property for accessing PaintRegistry service without local caching.
    /// Returns null during shutdown when ServiceProvider is disposed.
    /// </summary>
    protected IPaintRegistryService? PaintRegistry
    {
        get
        {
            try
            {
                return ServiceProviderService.ServiceProvider?.GetService<IPaintRegistryService>();
            }
            catch (ObjectDisposedException)
            {
                // ServiceProvider already disposed during shutdown - return null gracefully
                return null;
            }
        }
    }

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

