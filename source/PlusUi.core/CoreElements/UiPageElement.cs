using SkiaSharp;

namespace PlusUi.core;

public abstract class UiPageElement(ViewModelBase vm) : UiLayoutElement<UiPageElement>
{
    public ViewModelBase ViewModel { get; } = vm;
    private UiElement _tree = new NullElement();

    protected abstract UiElement Build();
    public void BuildPage()
    {
        _tree = Build();
        _tree.Parent = this;
        InvalidateMeasure();
        Appearing();
    }

    public virtual void Appearing()
    {
    }
    public virtual void Disappearing()
    {
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        _tree.UpdateBindings(propertyName);
    }

    public override void Render(SKCanvas canvas)
    {
        _tree.Render(canvas);
    }

    protected override Size MeasureInternal(Size availableSize)
    {
        return _tree.Measure(availableSize);
    }
    protected override Point ArrangeInternal(Rect bounds)
    {
        return _tree.Arrange(bounds);
    }
    public override UiElement? HitTest(Point point)
    {
        return _tree.HitTest(point);
    }

}