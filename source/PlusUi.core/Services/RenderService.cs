using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkiaSharp;
using System.Numerics;

namespace PlusUi.core;

//TODO: interface
public class RenderService(NavigationContainer navigationContainer, PlusUiPopupService popupService)
{

    public float DisplayDensity { get; set; } = 1.0f;

    public void Render(GL? gl, SKCanvas canvas, GRContext? grContext, Vector2 canvasSize)
    {
        canvas.Save();
        canvas.Scale(DisplayDensity);

        gl?.Clear((uint)ClearBufferMask.ColorBufferBit);
        canvas.Clear(SKColors.Transparent);

        navigationContainer.Page.Measure(new Size(canvasSize.X, canvasSize.Y));
        navigationContainer.Page.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));
        navigationContainer.Page.Render(canvas);

        var popup = popupService.CurrentPopup;
        if (popup is not null)
        {
            popup.Measure(new Size(canvasSize.X, canvasSize.Y));
            popup.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));
            popup.Render(canvas);
        }

        canvas.Flush();
        grContext?.Flush();

        canvas.Restore();
    }

}
