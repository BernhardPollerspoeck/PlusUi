using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

/// <summary>
/// Verifies the render half of the ContextMenu pipeline: an overlay registered with the
/// OverlayService is actually drawn by RenderOverlays (the exact call RenderService makes).
/// </summary>
[TestClass]
public sealed class MenuOverlayRenderTests
{
    [TestMethod]
    public void OverlayService_RendersRegisteredMenuOverlay_DrawsPixels()
    {
        var items = new List<object>
        {
            new MenuItem().SetText("Cut"),
            new MenuItem().SetText("Copy"),
        };
        var overlay = new MenuOverlay(items, new Point(20, 20));
        var service = new OverlayService();
        service.RegisterOverlay(overlay);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));
        surface.Canvas.Clear(SKColors.Transparent);
        service.RenderOverlays(surface.Canvas);

        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);

        var drawn = 0;
        for (var y = 0; y < bitmap.Height; y += 4)
            for (var x = 0; x < bitmap.Width; x += 4)
                if (bitmap.GetPixel(x, y).Alpha > 0)
                    drawn++;

        Assert.IsGreaterThan(0, drawn,
            "A MenuOverlay registered with the OverlayService must draw visible pixels via RenderOverlays.");
    }
}
