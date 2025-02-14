using PlusUi.core.Structures;
using PlusUi.core.ViewModel;
using SkiaSharp;

namespace PlusUi.core.CoreElements;

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
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        _tree!.UpdateBindings(propertyName);
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

}