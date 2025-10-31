using FFMpegCore.Pipes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlusUi.core;
using Silk.NET.Maths;
using SkiaSharp;
using System.Numerics;
using System.Threading.Channels;

namespace PlusUi.h264;

internal class VideoMainHandler(
    FrameInformationService frameInformationService,
    RenderService renderService,
    IOptions<VideoConfiguration> videoOptions,
    ChannelWriter<IVideoFrame> frameWriter,
    PlusUiNavigationService plusUiNavigationService,
    NavigationContainer navigationContainer,
    VideoRenderingProgressService progressService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        plusUiNavigationService.Initialize();

        // Generate the canvas
        var bitmap = new SKBitmap(videoOptions.Value.Width, videoOptions.Value.Height);
        var canvas = new SKCanvas(bitmap);
        var canvasSize = new Vector2(videoOptions.Value.Width, videoOptions.Value.Height);

        // Display initial information
        progressService.DisplayVideoInformation();

        // Process frames
        foreach (var _ in frameInformationService.GetNextFrame())
        {

            // Update progress using the service
            progressService.UpdateRenderingProgress(frameInformationService.CurrentFrame);

            // Update UI bindings and render frame
            navigationContainer.Page.UpdateBindings();
            renderService.Render(null, canvas, null, canvasSize);

            // Create and write video frame
            var videoFrame = new SkiaSharpVideoFrame(bitmap.Copy());
            var writeResult = frameWriter.TryWrite(videoFrame);

            // Give UI time to update
            await Task.Delay(1, stoppingToken);
        }

        // Write the end of file frame
        progressService.ReportMessage("Writing end of file frame", MessageType.Warning);

        var videoFrameEof = new SkiaSharpVideoFrame(bitmap.Copy())
        {
            IsEofFrame = true
        };

        frameWriter.TryWrite(videoFrameEof);
        frameWriter.Complete();

        progressService.ReportRenderingComplete();
    }
}
