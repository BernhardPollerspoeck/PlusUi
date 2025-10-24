using Microsoft.Extensions.Options;
using Spectre.Console;
using System.Collections.Concurrent;

namespace PlusUi.h264;

/// <summary>
/// Service responsible for managing and displaying the progress of video rendering.
/// Centralizes all UI and progress tracking functionality.
/// </summary>
public class VideoRenderingProgressService
{
    private readonly VideoConfiguration _videoConfig;
    private readonly TimeProvider _timeProvider;
    
    // Progress tracking
    private int _renderedFrameCount = 0;
    private int _processedFrameCount = 0;
    private readonly int _totalFrames;
    private readonly ConcurrentDictionary<string, ProgressTask> _progressTasks = new();
    private ProgressContext? _progressContext;
    
    // Flag to indicate if progress display is active
    private bool _isProgressActive = false;
    
    public VideoRenderingProgressService(
        IOptions<VideoConfiguration> videoOptions,
        TimeProvider timeProvider)
    {
        _videoConfig = videoOptions.Value;
        _timeProvider = timeProvider;
        _totalFrames = (int)(_videoConfig.FrameRate * _videoConfig.Duration.TotalSeconds);
    }
    
    /// <summary>
    /// Displays initial information about the video being rendered
    /// </summary>
    public void DisplayVideoInformation()
    {
        AnsiConsole.MarkupLine($"[bold]Output will be stored at:[/] [green]{Path.GetFullPath(_videoConfig.OutputFilePath)}[/]");
        AnsiConsole.MarkupLine($"[bold]Video duration:[/] [blue]{_videoConfig.Duration}[/] ([blue]{_totalFrames}[/] frames at [blue]{_videoConfig.FrameRate}[/] fps)");
        AnsiConsole.MarkupLine($"[bold]Video settings:[/] {_videoConfig.Width}x{_videoConfig.Height}, {_videoConfig.FrameRate}fps");
    }

    /// <summary>
    /// Starts the progress display
    /// </summary>
    public void StartProgressDisplay()
    {
        if (_isProgressActive)
            return;
            
        _isProgressActive = true;
        
        AnsiConsole.Progress()
            .Columns(new ProgressColumn[] 
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
                new ElapsedTimeColumn(),
                new RemainingTimeColumn()
            })
            .Start(ctx =>
            {
                _progressContext = ctx;
                
                // Create tasks for rendering and encoding with time information in their descriptions
                _progressTasks["rendering"] = ctx.AddTask($"[green]Rendering frames[/] (0:00:00 / {_videoConfig.Duration})", maxValue: _totalFrames);
                _progressTasks["encoding"] = ctx.AddTask($"[yellow]Writing frames to video[/] (0:00:00 / {_videoConfig.Duration})", maxValue: _totalFrames);
                
                // Keep the progress display active
                while (_isProgressActive)
                {
                    Thread.Sleep(100);
                }
            });
    }

    /// <summary>
    /// Completes the progress display
    /// </summary>
    public void CompleteProgress()
    {
        _isProgressActive = false;
    }
    
    /// <summary>
    /// Updates the rendering progress
    /// </summary>
    /// <param name="frameNumber">The current frame number being rendered</param>
    public void UpdateRenderingProgress(int frameNumber)
    {
        _renderedFrameCount = frameNumber;
        
        if (_progressTasks.TryGetValue("rendering", out var task))
        {
            // Update task value and description with time information
            var currentTimespan = TimeSpan.FromSeconds((double)frameNumber / _videoConfig.FrameRate);
            task.Value = frameNumber;
            task.Description = $"[green]Rendering frames[/] ([blue]{currentTimespan}[/] / [blue]{_videoConfig.Duration}[/])";
        }
    }
    
    /// <summary>
    /// Updates the encoding progress
    /// </summary>
    /// <param name="frameCount">The current frame count being encoded</param>
    public void UpdateEncodingProgress(int frameCount)
    {
        _processedFrameCount = frameCount;
        
        if (_progressTasks.TryGetValue("encoding", out var task))
        {
            // Update task value and description with time information
            var currentTimespan = TimeSpan.FromSeconds((double)frameCount / _videoConfig.FrameRate);
            task.Value = frameCount;
            task.Description = $"[yellow]Writing frames to video[/] ([blue]{currentTimespan}[/] / [blue]{_videoConfig.Duration}[/])";
        }
    }
    
    /// <summary>
    /// Reports a message with appropriate styling
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="messageType">The type of message (info, warning, error, success)</param>
    public void ReportMessage(string message, MessageType messageType = MessageType.Info)
    {
        string styledMessage = messageType switch
        {
            MessageType.Info => $"[blue]{message}[/]",
            MessageType.Warning => $"[yellow]{message}[/]",
            MessageType.Error => $"[bold red]{message}[/]",
            MessageType.Success => $"[bold green]{message}[/]",
            _ => message
        };
        
        AnsiConsole.MarkupLine(styledMessage);
    }
    
    /// <summary>
    /// Reports that single frame export has been completed
    /// </summary>
    /// <param name="outputFilePath">The path where the frame was saved</param>
    public void ReportSingleFrameExported(string outputFilePath)
    {
        AnsiConsole.MarkupLine($"[bold green]Frame saved to:[/] [blue]{Path.GetFullPath(outputFilePath)}[/]");
    }
    
    /// <summary>
    /// Reports the FFmpeg configuration status
    /// </summary>
    /// <param name="ffmpegPath">The path to the FFmpeg executable</param>
    /// <param name="exists">Whether the FFmpeg executable exists</param>
    public void ReportFFmpegConfiguration(string ffmpegPath, bool exists)
    {
        if (exists)
        {
            AnsiConsole.MarkupLine($"[bold green]FFmpeg configured:[/] [blue]{ffmpegPath}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]FFmpeg binary not found:[/] [blue]{ffmpegPath}[/]");
        }
    }
    
    /// <summary>
    /// Reports that all frames have been rendered successfully
    /// </summary>
    public void ReportRenderingComplete()
    {
        AnsiConsole.MarkupLine("[green]All frames rendered successfully![/]");
    }
    
    /// <summary>
    /// Reports that encoding is complete
    /// </summary>
    public void ReportEncodingComplete()
    {
        AnsiConsole.MarkupLine("[bold green]Video processing complete![/]");
        AnsiConsole.MarkupLine($"[bold]Final output:[/] [blue]{Path.GetFullPath(_videoConfig.OutputFilePath)}[/]");
    }
    
    /// <summary>
    /// Gets the total number of frames in the video
    /// </summary>
    public int TotalFrames => _totalFrames;
}

/// <summary>
/// Defines the types of messages that can be displayed
/// </summary>
public enum MessageType
{
    Info,
    Warning,
    Error,
    Success
}