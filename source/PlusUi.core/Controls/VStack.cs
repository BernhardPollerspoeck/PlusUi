using PlusUi.core.CoreElements;
using PlusUi.core.Structures;

namespace PlusUi.core.Controls;

public class VStack : UiLayoutElement<VStack>
{
    #region Spacing
    public float Spacing
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public VStack SetSpacing(float spacing)
    {
        Spacing = spacing;
        return this;
    }
    public VStack BindSpacing(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Spacing = propertyGetter());
        return this;
    }
    #endregion

    public VStack(params UiElement[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent = this;
        }
        _children.AddRange(elements);
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var child in _children)
        {
            child.UpdateBindings(propertyName);
        }
    }


    public override UiElement? HitTest(Point point)
    {
        foreach (var child in _children)
        {
            var result = child.HitTest(point);
            if (result is not null)
            {
                return result;
            }
        }
        return base.HitTest(point);
    }

    
}
