using Microsoft.Extensions.Options;
using PlusUi.core.Services;

namespace PlusUi.h264;

internal class VideoTimeProvider
    (IOptions<VideoConfiguration> videoOptions,
    FrameInformationService frameInformationService)
    : IApplicationTimeProvider
{
    public DateTime Now => new(new DateOnly(1, 1, 1), TimeOnly.FromTimeSpan(Elapsed));
    public DateTime UtcNow => new(new DateOnly(1, 1, 1), TimeOnly.FromTimeSpan(Elapsed));
    public TimeSpan Elapsed => TimeSpan.FromSeconds((double)frameInformationService.CurrentFrame / videoOptions.Value.FrameRate);
}
