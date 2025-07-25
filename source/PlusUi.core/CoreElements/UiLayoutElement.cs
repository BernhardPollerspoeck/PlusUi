using SkiaSharp;

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

    public override void ApplyStyles()
    {
        base.ApplyStyles();
        foreach (var child in Children)
        {
            child.ApplyStyles();
        }
    }
}
