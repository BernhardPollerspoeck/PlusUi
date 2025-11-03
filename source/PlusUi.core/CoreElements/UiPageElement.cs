using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.ComponentModel;

namespace PlusUi.core;

public abstract class UiPageElement(INotifyPropertyChanged vm) : UiLayoutElement<UiPageElement>
{
    public INotifyPropertyChanged ViewModel { get; } = vm;
    private UiElement _tree = new NullElement();

    protected override bool NeedsMeasure => true;

    protected abstract UiElement Build();
    protected virtual void ConfigurePageStyles(Style pageStyle) { }
    public void BuildPage()
    {
        _tree = Build();

        var themeService = ServiceProviderService.ServiceProvider?.GetRequiredService<IThemeService>();
        var mainStyle = ServiceProviderService.ServiceProvider?.GetRequiredService<Style>();
        if (themeService is not null && mainStyle is not null)
        {
            var tmpStyle = new Style(themeService);
            ConfigurePageStyles(tmpStyle);

            mainStyle.SetPageStyle(tmpStyle);
        }
        ApplyStyles();

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

    protected override void UpdateBindingsInternal()
    {
        _tree.UpdateBindings();
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
        _tree.Arrange(bounds);
        return new Point(bounds.X, bounds.Y);
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