using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Binding;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.Focus;
using SkiaSharp;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// Base class for all UI elements in PlusUi.
/// Provides layout, rendering, data binding, styling, accessibility, and input handling.
/// </summary>
[GenerateGenericWrapper]
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

    #region Debug
    protected bool Debug { get; private set; }
    public UiElement SetDebug(bool debug = true)
    {
        Debug = debug;
        return this;
    }
    public UiElement BindDebug(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Debug = getter());
        return this;
    }
    #endregion

    #region IsVisible
    public bool IsVisible { get; internal set; }

    public UiElement SetIsVisible(bool isVisible)
    {
        IsVisible = isVisible;
        return this;
    }

    public UiElement BindIsVisible(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsVisible = getter());
        return this;
    }
    #endregion

    #region VisualOffset
    internal Point VisualOffset { get; set; }
    public UiElement SetVisualOffset(Point offset)
    {
        VisualOffset = offset;
        return this;
    }
    public UiElement BindVisualOffset(Expression<Func<Point>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => VisualOffset = getter());
        return this;
    }
    #endregion

    #region Opacity
    internal float Opacity { get; set; }
    public UiElement SetOpacity(float opacity)
    {
        Opacity = Math.Clamp(opacity, 0f, 1f);
        return this;
    }
    public UiElement BindOpacity(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Opacity = Math.Clamp(getter(), 0f, 1f));
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

    public UiElement BindBackground(Expression<Func<IBackground?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Background = getter());
        return this;
    }

    /// <summary>
    /// Binds a solid color background to a property.
    /// This is a convenience overload that internally creates a SolidColorBackground.
    /// </summary>
    /// <param name="propertyExpression">Expression that returns the color from the property</param>
    public UiElement BindBackgroundColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Background = new SolidColorBackground(getter()));
        return this;
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
    public UiElement BindMargin(Expression<Func<Margin>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Margin = getter());
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
    }
    public UiElement SetHorizontalAlignment(HorizontalAlignment alignment)
    {
        HorizontalAlignment = alignment;
        return this;
    }
    public UiElement BindHorizontalAlignment(Expression<Func<HorizontalAlignment>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HorizontalAlignment = getter());
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
    }
    public UiElement SetVerticalAlignment(VerticalAlignment alignment)
    {
        VerticalAlignment = alignment;
        return this;
    }
    public UiElement BindVerticalAlignment(Expression<Func<VerticalAlignment>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => VerticalAlignment = getter());
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
    }
    public UiElement SetCornerRadius(float radius)
    {
        CornerRadius = radius;
        return this;
    }
    public UiElement BindCornerRadius(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => CornerRadius = getter());
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
    }
    public UiElement SetShadowColor(Color color)
    {
        ShadowColor = color;
        return this;
    }
    public UiElement BindShadowColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ShadowColor = getter());
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
    }
    public UiElement SetShadowOffset(Point offset)
    {
        ShadowOffset = offset;
        return this;
    }
    public UiElement BindShadowOffset(Expression<Func<Point>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ShadowOffset = getter());
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
    }
    public UiElement SetShadowBlur(float blur)
    {
        ShadowBlur = blur;
        return this;
    }
    public UiElement BindShadowBlur(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ShadowBlur = getter());
        return this;
    }
    #endregion

    #region ShadowSpread
    internal float ShadowSpread { get; set; }
    public UiElement SetShadowSpread(float spread)
    {
        ShadowSpread = spread;
        return this;
    }
    public UiElement BindShadowSpread(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ShadowSpread = getter());
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
    public UiElement BindDesiredSize(Expression<Func<Size>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DesiredSize = getter());
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
    public UiElement BindDesiredWidth(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DesiredSize = new Size(getter(), DesiredSize?.Height ?? -1));
        return this;
    }
    public UiElement BindDesiredHeight(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DesiredSize = new Size(DesiredSize?.Width ?? -1, getter()));
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
    internal bool TabStop { get; set; }

    /// <summary>
    /// Gets or sets whether this element currently has keyboard focus.
    /// </summary>
    public bool IsFocused { get; internal set; }

    /// <summary>
    /// Gets or sets the color of the focus ring.
    /// </summary>
    internal Color FocusRingColor { get; set; }

    /// <summary>
    /// Gets or sets the width of the focus ring stroke.
    /// </summary>
    internal float FocusRingWidth { get; set; }

    /// <summary>
    /// Gets or sets the offset of the focus ring from the element bounds.
    /// </summary>
    internal float FocusRingOffset { get; set; }

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
    public UiElement BindTabIndex(Expression<Func<int?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TabIndex = getter());
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
    public UiElement BindTabStop(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TabStop = getter());
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
    /// Binds the focus ring color.
    /// </summary>
    public UiElement BindFocusRingColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusRingColor = getter());
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
    /// Binds the focus ring width.
    /// </summary>
    public UiElement BindFocusRingWidth(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusRingWidth = getter());
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
    /// Binds the focus ring offset.
    /// </summary>
    public UiElement BindFocusRingOffset(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusRingOffset = getter());
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
    /// Binds the focused background.
    /// </summary>
    public UiElement BindFocusedBackground(Expression<Func<IBackground?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusedBackground = getter());
        return this;
    }

    /// <summary>
    /// Binds the focused background as a solid color.
    /// </summary>
    public UiElement BindFocusedBackgroundColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusedBackground = new SolidColorBackground(getter()));
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
    /// Binds the focused border color.
    /// </summary>
    public UiElement BindFocusedBorderColor(Expression<Func<Color?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusedBorderColor = getter());
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
    public AccessibilityTrait AccessibilityTraits { get; protected internal set; }

    /// <summary>
    /// Gets or sets whether this element should be exposed to assistive technologies.
    /// When false, the element is hidden from accessibility.
    /// </summary>
    public bool IsAccessibilityElement { get; protected internal set; }

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
    /// Binds whether this element should be exposed to assistive technologies.
    /// </summary>
    public UiElement BindIsAccessibilityElement(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsAccessibilityElement = getter());
        return this;
    }

    /// <summary>
    /// Binds the accessibility label.
    /// </summary>
    public UiElement BindAccessibilityLabel(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AccessibilityLabel = getter());
        return this;
    }

    /// <summary>
    /// Binds the accessibility hint.
    /// </summary>
    public UiElement BindAccessibilityHint(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AccessibilityHint = getter());
        return this;
    }

    /// <summary>
    /// Binds the accessibility value.
    /// </summary>
    public UiElement BindAccessibilityValue(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AccessibilityValue = getter());
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
    /// Binds the high contrast background.
    /// </summary>
    public UiElement BindHighContrastBackground(Expression<Func<IBackground?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HighContrastBackground = getter());
        return this;
    }

    /// <summary>
    /// Binds the high contrast background as a solid color.
    /// </summary>
    public UiElement BindHighContrastBackgroundColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HighContrastBackground = new SolidColorBackground(getter()));
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
    /// Binds the high contrast foreground color.
    /// </summary>
    public UiElement BindHighContrastForeground(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HighContrastForeground = getter());
        return this;
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

