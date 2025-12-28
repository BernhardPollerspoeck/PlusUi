using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlusUi.core.Services;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace PlusUi.h264;

internal class FrameRenderService(
    ChannelReader<IVideoFrame> frameReader,
    IOptions<VideoConfiguration> videoOptions,
    ICommandLineService commandLineService,
    AudioSequenceConverter audioSequenceConverter,
    VideoRenderingProgressService progressService,
    IAudioSequenceProvider? audioSequenceProvider = null)
    : BackgroundService
{
    private int _processedFrameCount = 0;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (CheckForSingleFrameExport(stoppingToken))
        {
            Environment.Exit(0);
            return;
        }
        await Task.Yield();

        ConfigureFFmpeg();
        await RenderFrames(stoppingToken);

        //end the application after processing
        Environment.Exit(0);
    }

    private async Task RenderFrames(CancellationToken stoppingToken)
    {
        progressService.ReportMessage("Configuring video encoder...", MessageType.Warning);
        
        var videoFramesSource = new RawVideoPipeSource(GenerateVideoFrames(stoppingToken))
        {
            FrameRate = videoOptions.Value.FrameRate,
        };

        var arguments = FFMpegArguments
            .FromPipeInput(videoFramesSource);

        var complexFilter = (string?)null;
        if (audioSequenceProvider is not null)
        {
            progressService.ReportMessage("Adding audio to video...", MessageType.Info);
            var audioSequence = audioSequenceProvider.GetAudioSequence().ToList();

            foreach (var audioFile in audioSequence)
            {
                arguments.AddFileInput(audioFile.FilePath);
                progressService.ReportMessage($"- Added audio file: {audioFile.FilePath}", MessageType.Info);
            }
            complexFilter = audioSequenceConverter.GetComplexFilter(audioSequence);
        }

        progressService.ReportMessage("Starting video encoding process", MessageType.Success);
        
        await arguments
            .OutputToFile(
                videoOptions.Value.OutputFilePath,
                true,
                options =>
                {
                    options
                        .WithVideoCodec("libx264")
                        .WithConstantRateFactor(23)
                        .WithVideoBitrate(2000)
                        .ForcePixelFormat("yuv420p")
                        .WithFramerate(videoOptions.Value.FrameRate)
                        .WithVideoFilters(filterOptions => filterOptions
                            .Scale(videoOptions.Value.Width, videoOptions.Value.Height));
                    if (audioSequenceProvider is not null)
                    {
                        options
                            .WithAudioCodec("aac")
                            .WithAudioBitrate(128)
                            .WithCustomArgument($"-filter_complex \"{complexFilter}\"")
                            .WithCustomArgument("-map 0:v -map \"[aout]\"");
                    }
                    options.WithFastStart();
                })
            .ProcessAsynchronously();
            
        progressService.ReportEncodingComplete();
    }

    private bool CheckForSingleFrameExport(CancellationToken stoppingToken)
    {
        if (!commandLineService.HasFlag("--frameOutput"))
        {
            return false;
        }
        var frameTimestamp = commandLineService.GetOptionValue("--frameTimestamp");
        if (frameTimestamp is null || !TimeSpan.TryParseExact(frameTimestamp, "c", null, out var parsedTimestamp))
        {
            return false;
        }

        var outputPath = Path.GetDirectoryName(videoOptions.Value.OutputFilePath)
                ?? throw new InvalidOperationException("Output file path is invalid.");
        var fileName = Path.GetFileNameWithoutExtension(videoOptions.Value.OutputFilePath);
        var outputFilePath = Path.Combine(outputPath, $"{fileName}_{parsedTimestamp}.png").Replace(':', '_');

        progressService.ReportMessage($"Exporting single frame at timestamp: {parsedTimestamp}", MessageType.Info);
        
        var frames = GenerateVideoFrames(stoppingToken).ToList();
        if (frames.Count != 1)
        {
            throw new InvalidOperationException("Expected exactly one frame for the specified timestamp, but found multiple.");
        }

        if (frames[0] is not ISkiaVideoFrame skiaFrame)
        {
            throw new InvalidOperationException("Expected SkiaSharpVideoFrame type for frame serialization.");
        }

        using var image = skiaFrame.Bitmap;
        using var imageStream = File.OpenWrite(outputFilePath);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(imageStream);

        progressService.ReportSingleFrameExported(outputFilePath);
        return true;
    }

    private IEnumerable<IVideoFrame> GenerateVideoFrames(CancellationToken stoppingToken)
    {
        var frameQueue = new ConcurrentQueue<IVideoFrame>();
        
        // Start frame reading task
        var frameReaderTask = Task.Run(async () =>
        {
            try
            {
                // Read frames asynchronously without blocking the main thread
                await foreach (var frame in frameReader.ReadAllAsync(stoppingToken))
                {
                    frameQueue.Enqueue(frame);
                    if (frame is SkiaSharpVideoFrame { IsEofFrame: true })
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation, no need to handle
            }
            catch (Exception ex)
            {
                progressService.ReportMessage($"Error reading frames: {ex.Message}", MessageType.Error);
            }
        }, stoppingToken);

        // Start progress display in a separate task
        var progressTask = Task.Run(() =>
        {
            progressService.StartProgressDisplay();
        }, stoppingToken);

        // This is the synchronous iterator that will be consumed by RawVideoPipeSource
        // It yields frames as they become available in the queue
        while (!stoppingToken.IsCancellationRequested)
        {
            if (frameQueue.TryDequeue(out var frame))
            {
                if (frame is SkiaSharpVideoFrame { IsEofFrame: true })
                {
                    progressService.ReportMessage("End of frames reached - finishing encoding", MessageType.Warning);
                    progressService.CompleteProgress();
                    yield break;
                }
                
                _processedFrameCount++;
                
                // Update progress via service
                progressService.UpdateEncodingProgress(_processedFrameCount);
                
                yield return frame;
            }
            else
            {
                // No frames available yet, wait a short time to avoid CPU spinning
                // This doesn't block the writers, just this consumer thread
                Thread.Sleep(10);
            }
        }
    }

    private void ConfigureFFmpeg()
    {
        var appDir = AppContext.BaseDirectory;
        string ffmpegPath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ffmpegPath = Path.Combine(appDir, "ffmpeg.exe");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ffmpegPath = Path.Combine(appDir, "ffmpeg");
            // Executable-Berechtigung setzen
            if (File.Exists(ffmpegPath))
            {
                try
                {
                    var process = System.Diagnostics.Process.Start("chmod", $"+x \"{ffmpegPath}\"");
                    process?.WaitForExit();
                }
                catch { } // Ignoriere Fehler
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ffmpegPath = Path.Combine(appDir, "ffmpeg");
            // Executable-Berechtigung setzen
            if (File.Exists(ffmpegPath))
            {
                try
                {
                    var process = System.Diagnostics.Process.Start("chmod", $"+x \"{ffmpegPath}\"");
                    process?.WaitForExit();
                }
                catch { } // Ignoriere Fehler
            }
        }
        else
        {
            throw new PlatformNotSupportedException("Plattform wird nicht unterstützt");
        }

        if (File.Exists(ffmpegPath))
        {
            GlobalFFOptions.Configure(options => options.BinaryFolder = appDir);
            progressService.ReportFFmpegConfiguration(ffmpegPath, true);
        }
        else
        {
            progressService.ReportFFmpegConfiguration(ffmpegPath, false);
            throw new FileNotFoundException($"FFmpeg Binary nicht gefunden: {ffmpegPath}");
        }
    }
}

