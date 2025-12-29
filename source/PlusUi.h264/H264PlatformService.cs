using Microsoft.Extensions.Options;
using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.h264;

/// <summary>
/// H.264 video rendering platform service implementation
/// </summary>
public class H264PlatformService(RenderService renderService, IOptions<VideoConfiguration> videoConfiguration) : IPlatformService
{
    private readonly VideoConfiguration _videoConfiguration = videoConfiguration.Value;

    public PlatformType Platform => PlatformType.Desktop; // H.264 is typically used for desktop video rendering

    public Size WindowSize => new Size(_videoConfiguration.Width, _videoConfiguration.Height);

    public float DisplayDensity => renderService.DisplayDensity;

    public bool OpenUrl(string url)
    {
        // URL opening is not supported in video rendering context
        return false;
    }
}
