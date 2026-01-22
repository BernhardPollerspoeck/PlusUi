using PlusUi.core.Attributes;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Creates reusable custom controls by composing other UI elements.
/// Define your control's UI by implementing the Build method.
/// </summary>
/// <remarks>
/// UserControl enables component composition, allowing you to create complex, reusable UI components
/// from simpler elements.
/// </remarks>
/// <example>
/// <code>
/// public class UserCard : UserControl
/// {
///     public string Name { get; set; }
///     public string Email { get; set; }
///
///     protected override UiElement Build() =>
///         new VStack(
///             new Label().SetText(Name).SetFontWeight(FontWeight.Bold),
///             new Label().SetText(Email).SetTextSize(12).SetMargin(new Margin(0, 4, 0, 0))
///         )
///         .SetMargin(new Margin(12));
/// }
///
/// // Usage
/// new UserCard { Name = "John Doe", Email = "john@example.com" }
/// </code>
/// </example>
[GenerateGenericWrapper]
public abstract partial class UserControl : UiElement<UserControl>, IDebugInspectable
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Container;

    private UiElement _content = new NullElement();
    protected abstract UiElement Build();

    /// <summary>
    /// Returns the content element for debug inspection.
    /// </summary>
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() => [_content];


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
    protected override void UpdateBindingsInternal()
    {
        _content.UpdateBindings();
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Use canvas translation instead of VisualOffset propagation
        // This ensures all child rendering is offset correctly
        canvas.Save();
        canvas.Translate((float)VisualOffset.X, (float)VisualOffset.Y);
        _content.Render(canvas);
        canvas.Restore();
    }
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _content.Measure(availableSize, dontStretch);
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

    public override void InvalidateMeasure()
    {
        base.InvalidateMeasure();
        _content.InvalidateMeasure();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose content
            _content.Dispose();
        }
        base.Dispose(disposing);
    }
}
