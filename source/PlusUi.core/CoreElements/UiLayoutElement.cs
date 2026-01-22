using PlusUi.core.Services.DebugBridge;
using PlusUi.core.Services.Focus;
using SkiaSharp;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PlusUi.core;

public abstract partial class UiLayoutElement<T> : UiLayoutElement where T : UiLayoutElement<T>
{
    public new T AddChild(UiElement child)
    {
        ArgumentNullException.ThrowIfNull(child);
        child.Parent = this;
        Children.Add(child);
        return (T)this;
    }
    public new T RemoveChild(UiElement child)
    {
        Children.Remove(child);
        return (T)this;
    }
    public new T ClearChildren()
    {
        Children.Clear();
        return (T)this;
    }

    /// <summary>
    /// Sets the focus scope mode for this container.
    /// </summary>
    public new T SetFocusScope(FocusScopeMode mode)
    {
        FocusScope = mode;
        return (T)this;
    }

    /// <summary>
    /// Sets the accessibility landmark for this container.
    /// </summary>
    public new T SetAccessibilityLandmark(AccessibilityLandmark landmark)
    {
        AccessibilityLandmark = landmark;
        return (T)this;
    }
}

public abstract partial class UiLayoutElement : UiElement, IDebugInspectable
{
    #region Constructor
    protected UiLayoutElement()
    {
        // Focus scope defaults
        FocusScope = FocusScopeMode.None;
        AccessibilityLandmark = AccessibilityLandmark.None;
    }
    #endregion

    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Container;

    #region FocusScope
    /// <summary>
    /// Gets or sets the focus scope mode for this container.
    /// When set to Trap or TrapWithEscape, tab navigation cycles within this container.
    /// </summary>
    internal FocusScopeMode FocusScope { get; set; }

    /// <summary>
    /// Sets the focus scope mode for this container.
    /// </summary>
    public UiLayoutElement SetFocusScope(FocusScopeMode mode)
    {
        FocusScope = mode;
        return this;
    }

    /// <summary>
    /// Binds the focus scope mode.
    /// </summary>
    public UiLayoutElement BindFocusScope(Expression<Func<FocusScopeMode>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FocusScope = getter());
        return this;
    }
    #endregion

    #region AccessibilityLandmark
    /// <summary>
    /// Gets or sets the accessibility landmark for this container.
    /// Landmarks help screen reader users quickly navigate between main content areas.
    /// </summary>
    internal AccessibilityLandmark AccessibilityLandmark { get; set; }

    /// <summary>
    /// Sets the accessibility landmark for this container.
    /// </summary>
    public UiLayoutElement SetAccessibilityLandmark(AccessibilityLandmark landmark)
    {
        AccessibilityLandmark = landmark;
        return this;
    }

    /// <summary>
    /// Binds the accessibility landmark.
    /// </summary>
    public UiLayoutElement BindAccessibilityLandmark(Expression<Func<AccessibilityLandmark>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AccessibilityLandmark = getter());
        return this;
    }
    #endregion

    public override INotifyPropertyChanged? Context
    {
        get => base.Context;
        internal set
        {
            base.Context = value;
            foreach (var child in Children.ToList())
            {
                child.Context = value;
            }
        }
    }

    #region children
    public virtual List<UiElement> Children { get; } = [];

    /// <summary>
    /// Returns children for debug inspection (virtual for UiPageElement override).
    /// </summary>
    protected virtual IEnumerable<UiElement> GetDebugChildrenCore() => Children;

    /// <summary>
    /// IDebugInspectable implementation - delegates to virtual method.
    /// </summary>
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() => GetDebugChildrenCore();

    public UiElement AddChild(UiElement child)
    {
        ArgumentNullException.ThrowIfNull(child);
        child.Parent = this;
        Children.Add(child);
        InvalidateMeasure();
        return this;
    }
    public UiElement RemoveChild(UiElement child)
    {
        Children.Remove(child);
        InvalidateMeasure();
        return this;
    }
    public UiElement ClearChildren()
    {
        Children.Clear();
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Invalidates arrange on all children recursively.
    /// Used when parent layout changes require children to be repositioned.
    /// </summary>
    protected void InvalidateArrangeChildren()
    {
        foreach (var child in Children.ToList())
        {
            child.NeedsArrange = true;
            if (child is UiLayoutElement layoutChild)
            {
                layoutChild.InvalidateArrangeChildren();
            }
        }
    }
    #endregion

    public override void BuildContent()
    {
        base.BuildContent();
        foreach (var child in Children.ToList())
        {
            child.BuildContent();
        }
    }

    #region rendering
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        if (IsVisible)
        {
            foreach (var child in Children.ToList())
            {
                // Save the current VisualOffset
                var childOriginalOffset = child.VisualOffset;

                // Apply parent's VisualOffset to child (additive)
                child.SetVisualOffset(new Point(
                    childOriginalOffset.X + VisualOffset.X,
                    childOriginalOffset.Y + VisualOffset.Y
                ));
                // Render the child
                child.Render(canvas);

                // Restore original VisualOffset
                child.SetVisualOffset(childOriginalOffset);
            }
        }
    }
    #endregion

    #region bindings
    protected override void UpdateBindingsInternal()
    {
        foreach (var child in Children.ToList())
        {
            child.UpdateBindings();
        }
    }
    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var child in Children.ToList())
        {
            child.UpdateBindings(propertyName);
        }
    }
    #endregion

    public override void InvalidateMeasure()
    {
        base.InvalidateMeasure();
        foreach (var child in Children.ToList())
        {
            child.InvalidateMeasure();
        }
    }

    public override void ApplyStyles()
    {
        base.ApplyStyles();
        foreach (var child in Children.ToList())
        {
            child.ApplyStyles();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose all children
            foreach (var child in Children.ToList())
            {
                child.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}
