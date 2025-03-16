using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace PlusUi.core;

public class RenderService(NavigationContainer navigationContainer)
{

    public float DisplayDensity { get; set; } = 1.0f;

    public void Render(GL gl, SKCanvas canvas, GRContext? grContext, Vector2D<int> canvasSize)
    {
        canvas.Save();
        canvas.Scale(DisplayDensity);

        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        canvas.Clear(SKColors.Transparent);

        navigationContainer.Page.Measure(new Size(canvasSize.X, canvasSize.Y));
        navigationContainer.Page.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));
        navigationContainer.Page.Render(canvas);

        canvas.Flush();
        grContext?.Flush();

        canvas.Restore();
    }

}
