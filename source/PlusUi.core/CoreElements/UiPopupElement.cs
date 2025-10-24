using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.ComponentModel;

namespace PlusUi.core.CoreElements;

public abstract class UiPopupElement<TArgument>(INotifyPropertyChanged vm) : UiPopupElement(vm)
{
    public TArgument? Argument { get; private set; }
    public Action? OnClosed { get; private set; }
    internal void SetArgument(TArgument? argument)
    {
        Argument = argument;
    }
    internal void SetOnClosed(Action? onClosed)
    {
        OnClosed = onClosed;
    }
    public override void Close(bool success)
    {
        if (success)
        {
            OnClosed?.Invoke();
        }
    }
}
public abstract class UiPopupElement : UiElement
{
    public INotifyPropertyChanged ViewModel { get; }
    private UiElement _tree = new NullElement();

    protected override bool SkipBackground => true;

    public bool CloseOnBackgroundClick { get; private set; }
    public bool CloseOnEscape { get; private set; }

    protected UiPopupElement(INotifyPropertyChanged vm)
    {
        ViewModel = vm;
        SetHorizontalAlignment(HorizontalAlignment.Center);
        SetVerticalAlignment(VerticalAlignment.Center);
    }

    internal void SetConfiguration(IPopupConfiguration configuration)
    {
        CloseOnBackgroundClick = configuration.CloseOnBackgroundClick;
        CloseOnEscape = configuration.CloseOnEscape;
        BackgroundColor = configuration.BackgroundColor;
    }

    internal void BuildPopup()
    {
        //TODO: make propper wrapper for working resize calculations
        _tree = new Grid()
            .AddChild(Build())
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
        _tree.BuildContent();
        _tree.ApplyStyles();
        Appearing();
    }

    public virtual void Appearing() { }
    public virtual void Disappearing() { }

    protected abstract UiElement Build();
    public abstract void Close(bool success);

    public override void Render(SKCanvas canvas)
    {
        canvas.DrawRect(canvas.DeviceClipBounds, BackgroundPaint);

        base.Render(canvas);
        _tree.Render(canvas);
    }


    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _tree.Measure(availableSize, true);
        return availableSize;
    }
    protected override Point ArrangeInternal(Rect bounds)
    {
        return _tree.Arrange(bounds);
    }
    public override UiElement? HitTest(Point point)
    {
        var hitControl = _tree.HitTest(point);
        if (hitControl is not null)
        {
            return hitControl;
        }
        if (CloseOnBackgroundClick)
        {
            var popupService = ServiceProviderService.ServiceProvider?.GetRequiredService<IPopupService>();
            popupService?.ClosePopup(false);
        }
        return null;
    }
    public override void ApplyStyles()
    {
        base.ApplyStyles();
        _tree.ApplyStyles();
    }
}