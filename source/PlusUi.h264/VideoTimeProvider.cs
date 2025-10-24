using Microsoft.Extensions.Options;

namespace PlusUi.h264;

internal class VideoTimeProvider
    (IOptions<VideoConfiguration> videoOptions,
    FrameInformationService frameInformationService)
    : TimeProvider
{

    public override DateTimeOffset GetUtcNow()
    {
        return new DateTimeOffset(
            new DateOnly(1, 1, 1),
            TimeOnly.FromTimeSpan(TimeSpan.FromSeconds((double)frameInformationService.CurrentFrame / videoOptions.Value.FrameRate)),
            TimeSpan.Zero);        
    }

    public override long GetTimestamp()
    {
        return (long)(frameInformationService.CurrentFrame * TimeSpan.TicksPerSecond / videoOptions.Value.FrameRate);
    }

}
