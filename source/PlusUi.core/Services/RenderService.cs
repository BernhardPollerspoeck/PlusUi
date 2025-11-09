using Silk.NET.OpenGL;
using SkiaSharp;
using System.Numerics;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services;

namespace PlusUi.core;

public class RenderService(NavigationContainer navigationContainer, PlusUiPopupService popupService, ILogger<RenderService>? logger = null, IAppMonitor? appMonitor = null)
{
    private readonly ILogger<RenderService>? _logger = logger;
    private readonly IAppMonitor? _appMonitor = appMonitor;

    public float DisplayDensity { get; set; } = 1.0f;

    public void Render(GL? gl, SKCanvas canvas, GRContext? grContext, Vector2 canvasSize)
    {
        var frameTimer = _appMonitor != null ? Stopwatch.StartNew() : null;

        try
        {
            canvas.Save();
            canvas.Scale(DisplayDensity);

            gl?.Clear((uint)ClearBufferMask.ColorBufferBit);
            canvas.Clear(SKColors.Transparent);

            var measureTimer = _appMonitor != null ? Stopwatch.StartNew() : null;
            navigationContainer.CurrentPage.Measure(new Size(canvasSize.X, canvasSize.Y));
            if (measureTimer != null)
            {
                measureTimer.Stop();
                _appMonitor?.ReportMeasureTime(measureTimer.Elapsed.TotalMilliseconds);
            }

            var arrangeTimer = _appMonitor != null ? Stopwatch.StartNew() : null;
            navigationContainer.CurrentPage.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));
            if (arrangeTimer != null)
            {
                arrangeTimer.Stop();
                _appMonitor?.ReportArrangeTime(arrangeTimer.Elapsed.TotalMilliseconds);
            }

            var renderTimer = _appMonitor != null ? Stopwatch.StartNew() : null;
            navigationContainer.CurrentPage.Render(canvas);

            var popup = popupService.CurrentPopup;
            if (popup is not null)
            {
                popup.Measure(new Size(canvasSize.X, canvasSize.Y));
                popup.Arrange(new Rect(0, 0, canvasSize.X, canvasSize.Y));
                popup.Render(canvas);
            }

            canvas.Flush();
            grContext?.Flush();

            if (renderTimer != null)
            {
                renderTimer.Stop();
                _appMonitor?.ReportRenderTime(renderTimer.Elapsed.TotalMilliseconds);
            }

            canvas.Restore();

            if (frameTimer != null)
            {
                frameTimer.Stop();
                _appMonitor?.ReportFrameTime(frameTimer.Elapsed.TotalMilliseconds);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Render error at canvas size {CanvasSize}", canvasSize);
            // Re-throw to let the caller handle critical rendering errors
            throw;
        }
    }

}
