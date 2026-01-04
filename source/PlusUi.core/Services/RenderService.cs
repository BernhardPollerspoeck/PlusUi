using SkiaSharp;
using System.Numerics;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services;

namespace PlusUi.core;

public class RenderService(NavigationContainer navigationContainer, PlusUiPopupService popupService, OverlayService overlayService, ITransitionService transitionService, ILogger<RenderService>? logger = null, IAppMonitor? appMonitor = null)
{
    public float DisplayDensity { get; set; } = 1.0f;

    public void Render(Action? clearAction, SKCanvas canvas, GRContext? grContext, Vector2 canvasSize)
    {
        var frameTimer = appMonitor != null ? Stopwatch.StartNew() : null;

        try
        {
            // Clear BEFORE applying any transforms to ensure entire buffer is cleared
            clearAction?.Invoke();
            canvas.Clear(SKColors.Transparent);

            canvas.Save();
            canvas.Scale(DisplayDensity);

            var availableSize = new Size(canvasSize.X, canvasSize.Y);
            var bounds = new Rect(0, 0, canvasSize.X, canvasSize.Y);

            var measureTimer = appMonitor != null ? Stopwatch.StartNew() : null;
            navigationContainer.CurrentPage.Measure(availableSize);

            // If transitioning, also measure outgoing page
            if (transitionService.IsTransitioning && transitionService.OutgoingPage != null)
            {
                transitionService.OutgoingPage.Measure(availableSize);
            }

            if (measureTimer != null)
            {
                measureTimer.Stop();
                appMonitor?.ReportMeasureTime(measureTimer.Elapsed.TotalMilliseconds);
            }

            var arrangeTimer = appMonitor != null ? Stopwatch.StartNew() : null;
            navigationContainer.CurrentPage.Arrange(bounds);

            // If transitioning, also arrange outgoing page
            if (transitionService.IsTransitioning && transitionService.OutgoingPage != null)
            {
                transitionService.OutgoingPage.Arrange(bounds);
            }

            if (arrangeTimer != null)
            {
                arrangeTimer.Stop();
                appMonitor?.ReportArrangeTime(arrangeTimer.Elapsed.TotalMilliseconds);
            }

            // Update transition state AFTER measure/arrange so ElementSize is available
            transitionService.Update();

            var renderTimer = appMonitor != null ? Stopwatch.StartNew() : null;

            // If transitioning, render outgoing page first (below)
            if (transitionService.IsTransitioning && transitionService.OutgoingPage != null)
            {
                transitionService.OutgoingPage.Render(canvas);
            }

            // Render current page (on top during transition)
            navigationContainer.CurrentPage.Render(canvas);

            // Render overlays (above page, below popups)
            overlayService.RenderOverlays(canvas);

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
                appMonitor?.ReportRenderTime(renderTimer.Elapsed.TotalMilliseconds);
            }

            canvas.Restore();

            if (frameTimer != null)
            {
                frameTimer.Stop();
                appMonitor?.ReportFrameTime(frameTimer.Elapsed.TotalMilliseconds);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Render error at canvas size {CanvasSize}", canvasSize);
            // Re-throw to let the caller handle critical rendering errors
            throw;
        }
    }

}
