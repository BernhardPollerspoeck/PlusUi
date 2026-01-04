using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Animations;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;
using System.ComponentModel;

namespace PlusUi.core;

public abstract class UiPageElement(INotifyPropertyChanged vm) : UiLayoutElement<UiPageElement>
{
    public INotifyPropertyChanged ViewModel { get; } = vm;
    private UiElement _tree = new NullElement();

    /// <summary>
    /// Gets the root element of the page's content tree for traversal purposes.
    /// </summary>
    internal UiElement ContentTree => _tree;

    /// <summary>
    /// Pages return ContentTree for debug inspection, not Children.
    /// </summary>
    protected override IEnumerable<UiElement> GetDebugChildrenCore() => [ContentTree];

    /// <summary>
    /// Optional page-specific transition that overrides the global default transition.
    /// </summary>
    public virtual IPageTransition? Transition { get; set; }

    public UiPageElement SetTransition(IPageTransition? transition)
    {
        Transition = transition;
        return this;
    }

    protected override bool NeedsMeasure => true;

    protected abstract UiElement Build();
    protected virtual void ConfigurePageStyles(Style pageStyle) { }
    public void BuildPage()
    {
        _tree = Build();
        _tree.Context = ViewModel;

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

    /// <summary>
    /// Called when the page is about to appear on screen.
    /// This is called after the page tree is built and before rendering begins.
    /// </summary>
    public virtual void Appearing()
    {
    }

    /// <summary>
    /// Called when the page is about to disappear from screen.
    /// This is called before navigating to another page.
    /// </summary>
    public virtual void Disappearing()
    {
    }

    /// <summary>
    /// Called when the page is navigated to with an optional parameter.
    /// This method is called after Appearing() and after the page tree is built.
    /// Use this to handle navigation parameters and initialize page state based on the parameter.
    /// </summary>
    /// <param name="parameter">
    /// The parameter passed during navigation via NavigateTo&lt;TPage&gt;(parameter).
    /// Can be null if no parameter was provided.
    /// </param>
    /// <remarks>
    /// This method is part of the navigation lifecycle and is called every time the page is navigated to,
    /// including when using GoBack() if the navigation stack is enabled.
    /// </remarks>
    public virtual void OnNavigatedTo(object? parameter)
    {
    }

    /// <summary>
    /// Called when the page is being navigated away from.
    /// This is called before Disappearing() and before the new page is shown.
    /// Use this to clean up resources, save state, or cancel ongoing operations.
    /// </summary>
    /// <remarks>
    /// This method is part of the navigation lifecycle and is called every time navigation occurs away from this page.
    /// Paint registry is automatically cleared here to free up cached SKPaint/SKFont instances.
    /// </remarks>
    public virtual void OnNavigatedFrom()
    {
        // Clear entire paint registry - all paints disposed, cache cleared
        PaintRegistry?.ClearAll();
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
        // Check if we need visual transformations for transitions
        var hasOffset = VisualOffset.X != 0 || VisualOffset.Y != 0;
        var hasOpacity = Opacity < 1f;

        // Apply opacity layer if needed (wraps entire page including children)
        if (hasOpacity)
        {
            canvas.SaveLayer(new SKPaint { Color = SKColors.White.WithAlpha((byte)(Opacity * 255)) });
        }

        // Apply visual offset translation (moves entire page including children)
        if (hasOffset)
        {
            canvas.Save();
            canvas.Translate(VisualOffset.X, VisualOffset.Y);
        }

        // Temporarily clear offset so base.Render doesn't double-apply it
        var savedOffset = VisualOffset;
        VisualOffset = new Point(0, 0);

        // Render background
        base.Render(canvas);

        // Render children
        _tree.Render(canvas);

        // Restore offset
        VisualOffset = savedOffset;

        if (hasOffset)
        {
            canvas.Restore();
        }

        if (hasOpacity)
        {
            canvas.Restore();
        }
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose the page tree
            _tree.Dispose();
        }
        base.Dispose(disposing);
    }
}