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
    public List<UiElement> Children { get; } = [];
    public UiElement AddChild(UiElement child)
    {
        child.Parent = this;
        Children.Add(child);
        return this;
    }
    public UiElement RemoveChild(UiElement child)
    {
        Children.Remove(child);
        return this;
    }
    public UiElement ClearChildren()
    {
        Children.Clear();
        return this;
    }
    #endregion

    #region rendering
    public override void Render(SKCanvas canvas)
    {
        foreach (var child in Children)
        {
            child.Render(canvas);
        }
    }
    #endregion

    #region bindings
    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var child in Children)
        {
            child.UpdateBindings(propertyName);
        }
    }
    #endregion    
}
