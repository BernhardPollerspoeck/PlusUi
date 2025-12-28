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
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Dialog;

    public INotifyPropertyChanged ViewModel { get; }
    private UiElement _tree = new NullElement();

    protected override bool SkipBackgroundRendering => true;

    public bool CloseOnBackgroundClick { get; private set; }
    public bool CloseOnEscape { get; private set; }

    protected UiPopupElement(INotifyPropertyChanged vm)
    {
        ViewModel = vm;
        SetHorizontalAlignment(HorizontalAlignment.Stretch);
        SetVerticalAlignment(VerticalAlignment.Stretch);
    }

    internal void SetConfiguration(IPopupConfiguration configuration)
    {
        CloseOnBackgroundClick = configuration.CloseOnBackgroundClick;
        CloseOnEscape = configuration.CloseOnEscape;
        Background = new SolidColorBackground(configuration.BackgroundColor);
    }

    internal void BuildPopup()
    {
        _tree = new Grid()
            .AddChild(Build(), 0, 0, 1, 1)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
        _tree.Context = ViewModel;
        _tree.BuildContent();
        _tree.ApplyStyles();
        InvalidateMeasure();
        Appearing();
    }

    public virtual void Appearing() { }
    public virtual void Disappearing() { }

    protected abstract UiElement Build();
    public abstract void Close(bool success);

    public override void Render(SKCanvas canvas)
    {
        // Draw fullscreen overlay background
        if (Background is SolidColorBackground solidBg)
        {
            using var overlayPaint = new SKPaint
            {
                Color = solidBg.Color,
                IsAntialias = true
            };
            canvas.DrawRect(canvas.DeviceClipBounds, overlayPaint);
        }

        base.Render(canvas);
        _tree.Render(canvas);
    }

    public override void InvalidateMeasure()
    {
        base.InvalidateMeasure();
        _tree.InvalidateMeasure();
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