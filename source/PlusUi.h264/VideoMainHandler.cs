using FFMpegCore.Pipes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlusUi.core;
using Silk.NET.Maths;
using SkiaSharp;
using System.Threading.Channels;

namespace PlusUi.h264;

internal class VideoMainHandler(
    FrameInformationService frameInformationService,
    RenderService renderService,
    IOptions<VideoConfiguration> videoOptions,
    ChannelWriter<IVideoFrame> frameWriter,
    ILogger<VideoMainHandler> logger,
    PlusUiNavigationService plusUiNavigationService,
    NavigationContainer navigationContainer)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        plusUiNavigationService.Initialize();

        //generate the canvas
        var bitmap = new SKBitmap(videoOptions.Value.Width, videoOptions.Value.Height);
        var canvas = new SKCanvas(bitmap);
        var canvasSize = new Vector2D<int>(videoOptions.Value.Width, videoOptions.Value.Height);

        //Get a frame from FrameInformationService this is a integer or null if the video is finished
        foreach (var _ in frameInformationService.GetNextFrame())
        {
            logger.LogInformation("Rendering frame {FrameNumber}", frameInformationService.CurrentFrame);

            navigationContainer.Page.UpdateBindings();

            renderService.Render(null, canvas, null, canvasSize);
            logger.LogInformation("Frame {FrameNumber} rendered", frameInformationService.CurrentFrame);

            var videoFrame = new SkiaSharpVideoFrame(bitmap.Copy());

            var writeResult = frameWriter.TryWrite(videoFrame);
            logger.LogInformation("Frame {FrameNumber} written: {WriteResult}", frameInformationService.CurrentFrame, writeResult);
        }

        //write the end of file frame. this will lead to the video encoder to finish the video
        logger.LogInformation("Writing end of file frame");
        var videoFrameEof = new SkiaSharpVideoFrame(bitmap.Copy())
        {
            IsEofFrame = true
        };
        frameWriter.TryWrite(videoFrameEof);
        frameWriter.Complete();
        logger.LogInformation("End of file frame written");
    }

}
