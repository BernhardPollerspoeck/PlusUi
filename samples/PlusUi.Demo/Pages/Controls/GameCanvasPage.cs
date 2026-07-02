using PlusUi.core;
using PlusUi.Demo.Pages.Shared;
using SkiaSharp;

namespace PlusUi.Demo.Pages.Controls;

public class GameCanvasPage(GameCanvasPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "GameCanvas";

    protected override string Description =>
        "Hands you a raw Skia canvas once per frame for imperative, immediate-mode rendering. " +
        "Input is read separately via IGlobalInputService, so canvas and input stay decoupled. " +
        "Move the circle with WASD / arrow keys, hold the left mouse button to fill it, scroll to resize.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Per-frame rendering + global input",
            new GameCanvas()
                .SetBackground(new Color(20, 20, 28))
                .SetCornerRadius(8)
                .SetDesiredHeight(360)
                .SetOnDraw(Draw)),

        Note("The canvas' Render runs every frame (~60 FPS on desktop). The circle is moved in the " +
             "view model from IGlobalInputService events; the canvas only draws the current state."),
    ];

    private void Draw(GameCanvasDrawContext ctx)
    {
        // Advance the game state using the frame delta, clamped to the current canvas size.
        vm.Step((float)ctx.DeltaTime.TotalSeconds, ctx.Size.Width, ctx.Size.Height);

        using var body = new SKPaint
        {
            Color = vm.IsPointerDown ? new SKColor(120, 200, 255) : new SKColor(40, 60, 90),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        using var ring = new SKPaint
        {
            Color = new SKColor(120, 200, 255),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3
        };
        ctx.Canvas.DrawCircle(vm.X, vm.Y, vm.Radius, body);
        ctx.Canvas.DrawCircle(vm.X, vm.Y, vm.Radius, ring);

        using var font = new SKFont { Size = 13 };
        using var hud = new SKPaint { Color = new SKColor(150, 150, 160), IsAntialias = true };
        ctx.Canvas.DrawText(
            $"frame {ctx.FrameCount}   dt {ctx.DeltaTime.TotalMilliseconds:F1} ms",
            12, 22, SKTextAlign.Left, font, hud);
    }
}
