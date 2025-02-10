using PlusUi.core.ViewModel;
using SkiaSharp;

namespace PlusUi.core.UiElements;

public abstract class UiPage(ViewModelBase vm) : UiElement
{
    public ViewModelBase ViewModel { get; } = vm;
    private UiElement _tree = new NullElement();

    protected abstract UiElement Build();
    public void BuildPage()
    {
        _tree = Build();
        Measure();
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        _tree!.UpdateBindings(propertyName);
    }

    public override void Render(SKCanvas canvas, SKPoint location)
    {
        _tree.Render(canvas, location);
    }

    protected override Size MeasureInternal()
    {
        return _tree.Measure();
    }
}