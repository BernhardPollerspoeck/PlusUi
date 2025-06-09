using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.RawControl;
internal class RawUserControlPage(RawUserControlPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new TestRawUserControl()
            .SetDebug();
    }
}

internal class RawUserControlPageViewModel : ObservableObject
{
}

internal class TestRawUserControl : RawUserControl
{
    public override Size Size => new(64, 64);

    public override void RenderControl(SKBitmap bitmap)
    {
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.Transparent);
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.Red;
                canvas.DrawCircle(Size.Width / 2, Size.Height / 2, Size.Width / 2, paint);
            }
        }
    }
}