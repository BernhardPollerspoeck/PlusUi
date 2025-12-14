using SkiaSharp;
using System.ComponentModel;

namespace PlusUi.core;

public abstract class UiLayoutElement<T> : UiLayoutElement where T : UiLayoutElement<T>
{
    public new T AddChild(UiElement child)
    {
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

}

public abstract class UiLayoutElement : UiElement
{

    public override INotifyPropertyChanged? Context
    {
        get => base.Context;
        internal set
        {
            base.Context = value;
            foreach (var child in Children)
            {
                child.Context = value;
            }
        }
    }

    #region children
    public virtual List<UiElement> Children { get; } = [];
    public UiElement AddChild(UiElement child)
    {
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
    #endregion

    public override void BuildContent()
    {
        base.BuildContent();
        foreach (var child in Children)
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
            foreach (var child in Children)
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
        foreach (var child in Children)
        {
            child.UpdateBindings();
        }
    }
    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var child in Children)
        {
            child.UpdateBindings(propertyName);
        }
    }
    #endregion

    public override void InvalidateMeasure()
    {
        base.InvalidateMeasure();
        foreach (var child in Children)
        {
            child.InvalidateMeasure();
        }
    }

    public override void ApplyStyles()
    {
        base.ApplyStyles();
        foreach (var child in Children)
        {
            child.ApplyStyles();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose all children
            foreach (var child in Children)
            {
                child.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}
