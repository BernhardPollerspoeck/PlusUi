using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Checkbox : UiElement, IToggleButtonControl
{
    protected override bool SkipBackground => true;

    public Checkbox()
    {
        SetDesiredSize(new(22, 22));
        SetBackgroundColor(SKColors.White);
    }

    #region IsChecked
    internal bool IsChecked
    {
        get => field;
        set
        {
            field = value;
        }
    }
    public Checkbox SetIsChecked(bool isChecked)
    {
        IsChecked = isChecked;
        return this;
    }
    public Checkbox BindIsChecked(string propertyName, Func<bool> propertyGetter, Action<bool> propertySetter)
    {
        RegisterBinding(propertyName, () => IsChecked = propertyGetter());
        RegisterSetter(nameof(IsChecked), propertySetter);
        return this;
    }
    #endregion

    public void Toggle()
    {
        IsChecked = !IsChecked;
        if (_setter.TryGetValue(nameof(IsChecked), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(IsChecked);
            }
        }
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }
        BackgroundPaint.StrokeWidth = 2;
        BackgroundPaint.Style = SKPaintStyle.Stroke;
        var rect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        canvas.DrawRoundRect(rect, 5, 5, BackgroundPaint);

        if (IsChecked)
        {
            var checkPath = new SKPath();
            checkPath.MoveTo(Position.X + VisualOffset.X + 4, Position.Y + VisualOffset.Y + (ElementSize.Height / 2));
            checkPath.LineTo(Position.X + VisualOffset.X + (ElementSize.Width / 3), Position.Y + VisualOffset.Y + ElementSize.Height - 4);
            checkPath.LineTo(Position.X + VisualOffset.X + ElementSize.Width - 4, Position.Y + VisualOffset.Y + 4);
            canvas.DrawPath(checkPath, BackgroundPaint);
        }
    }

}
