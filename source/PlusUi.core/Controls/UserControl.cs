using SkiaSharp;

namespace PlusUi.core;

public abstract class UserControl : UiElement<UserControl>
{
    private UiElement _content = new NullElement();
    protected abstract UiElement Build();


    public override void BuildContent()
    {
        _content = Build();
        _content.BuildContent();
        _content.Parent = this;
        InvalidateMeasure();
    }

    protected override void UpdateBindingsInternal(string propertyName)
    {
        _content.UpdateBindings(propertyName);
    }
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        _content.Render(canvas);
    }
    public override Size MeasureInternal(Size availableSize)
    {
        _content.Measure(availableSize);
        var width = _content.ElementSize.Width + _content.Margin.Left + _content.Margin.Right;
        var height = _content.ElementSize.Height + _content.Margin.Top + _content.Margin.Bottom;
        return new Size(width, height);
    }
    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => bounds.Left + ((bounds.Width - ElementSize.Width) / 2),
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.Left + Margin.Left,
        };
        var positionY = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.Top + ((bounds.Height - ElementSize.Height) / 2),
            VerticalAlignment.Bottom => bounds.Bottom - ElementSize.Height - Margin.Bottom,
            _ => bounds.Top + Margin.Top,
        };
        _content.Arrange(new Rect(positionX, positionY, ElementSize.Width, ElementSize.Height));
        return new Point(positionX, positionY);
    }
    public override UiElement? HitTest(Point point)
    {
        return _content.HitTest(point);
    }
    public override void ApplyStyles()
    {
        base.ApplyStyles();
        _content.ApplyStyles();
    }


}
