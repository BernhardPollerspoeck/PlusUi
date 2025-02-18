using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace PlusUi.core;

public class RenderService(NavigationContainer navigationContainer)
{
    public void Render(GL gl, SKCanvas canvas, GRContext grContext, Vector2D<int> canvasSize)
    {
        // Clear the OpenGL buffer
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        // Clear the canvas
        canvas.Clear(SKColors.Black);

        // Render the UI

        navigationContainer.Page.Measure(new Size(canvasSize.X, canvasSize.Y));
        navigationContainer.Page.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));

        navigationContainer.Page.Render(canvas);

        // Flush the surface
        canvas.Flush();
        grContext.Flush();
    }

}
