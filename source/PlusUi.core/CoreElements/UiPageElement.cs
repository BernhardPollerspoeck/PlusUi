﻿using SkiaSharp;

namespace PlusUi.core;

public abstract class UiPageElement(ViewModelBase vm) : UiLayoutElement<UiPageElement>
{
    public ViewModelBase ViewModel { get; } = vm;
    private UiElement _tree = new NullElement();

    protected override bool NeadsMeasure => true;

    protected abstract UiElement Build();
    public void BuildPage()
    {
        _tree = Build();
        _tree.BuildContent();
        _tree.Parent = this;
        _tree.ApplyStyles();
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
        base.Render(canvas);
        _tree.Render(canvas);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _tree.Measure(availableSize);
        return availableSize;
    }
    protected override Point ArrangeInternal(Rect bounds)
    {
        return _tree.Arrange(bounds);
    }
    public override UiElement? HitTest(Point point)
    {
        return _tree.HitTest(point);
    }
    public override void ApplyStyles()
    {
        base.ApplyStyles();
        _tree.ApplyStyles();
    }

}