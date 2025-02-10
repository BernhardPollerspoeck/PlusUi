using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.UiElements;

public class VStack : UiElement
{
    private readonly List<UiElement> _children = [];

    #region Spacing
    public float Spacing
    {
        get => field;
        set
        {
            field = value;
            Measure();
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
        _children.AddRange(elements);
    }

    public new VStack SetMargin(Margin margin)
    {
        base.SetMargin(margin);
        return this;
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var child in _children)
        {
            child.UpdateBindings(propertyName);
        }
    }
    public override void Render(SKCanvas canvas, SKPoint location)
    {
        var offset = 0f;
        foreach (var child in _children)
        {
            child.Render(canvas, new(location.X + Margin.Left, location.Y + Margin.Top + offset));
            offset += child.Size.Height + Spacing;
        }
    }

    protected override Size MeasureInternal()
    {
        var width = 0f;
        var height = 0f;
        foreach (var child in _children)
        {
            var size = child.Measure();
            width = Math.Max(width, size.Width);
            height += size.Height;
        }
        return new Size(width, height);
    }
}
