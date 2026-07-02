using System;
using System.Collections.Generic;
using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

/// <summary>
/// Verifies the per-frame contract of <see cref="GameCanvas"/>: its draw hook runs every rendered
/// frame with an advancing frame count and a valid drawable size, via both the callback and a
/// subclass override.
/// </summary>
[TestClass]
public sealed class GameCanvasTests
{
    [TestMethod]
    public void GameCanvas_InvokesOnDrawEachFrame_WithAdvancingFrameCountAndSize()
    {
        var frames = new List<GameCanvasDrawContext>();
        var canvas = new GameCanvas().SetOnDraw(frames.Add);

        canvas.Measure(new Size(200, 150));
        canvas.Arrange(new Rect(0, 0, 200, 150));

        using var surface = SKSurface.Create(new SKImageInfo(200, 150));
        canvas.Render(surface.Canvas);
        canvas.Render(surface.Canvas);

        Assert.AreEqual(2, frames.Count, "OnDraw must be called once per rendered frame.");
        Assert.AreEqual(0L, frames[0].FrameCount);
        Assert.AreEqual(1L, frames[1].FrameCount);
        Assert.AreEqual(TimeSpan.Zero, frames[0].DeltaTime, "The first frame reports zero delta.");
        Assert.AreEqual(TimeSpan.Zero, frames[0].TotalTime, "The first frame reports zero total time.");
        Assert.AreEqual(200f, frames[1].Size.Width, "Size should reflect the arranged element size.");
        Assert.AreEqual(150f, frames[1].Size.Height);
    }

    [TestMethod]
    public void GameCanvas_SubclassDrawOverride_ReceivesFrames()
    {
        var canvas = new CountingGameCanvas();
        canvas.Measure(new Size(50, 50));
        canvas.Arrange(new Rect(0, 0, 50, 50));

        using var surface = SKSurface.Create(new SKImageInfo(50, 50));
        canvas.Render(surface.Canvas);

        Assert.AreEqual(1, canvas.DrawCount, "Overriding Draw should receive the per-frame call.");
    }

    private sealed class CountingGameCanvas : GameCanvas
    {
        public int DrawCount { get; private set; }
        protected override void Draw(GameCanvasDrawContext context) => DrawCount++;
    }
}
