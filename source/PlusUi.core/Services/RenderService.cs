using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace PlusUi.core.Services;

public class RenderService(CurrentPage rootPage)
{
    public void Render(GL gl, SKCanvas canvas, GRContext grContext, Vector2D<int> canvasSize)
    {
        // Clear the OpenGL buffer
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        // Clear the canvas
        canvas.Clear(SKColors.Black);

        // Render the UI

        rootPage.Page.Measure(new Size(canvasSize.X, canvasSize.Y));
        rootPage.Page.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));

        rootPage.Page.Render(canvas);

        // Flush the surface
        canvas.Flush();
        grContext.Flush();
    }

}
