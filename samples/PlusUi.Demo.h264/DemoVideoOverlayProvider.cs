using PlusUi.h264;

namespace Sandbox.h264;

public class DemoVideoOverlayProvider : IVideoOverlayProvider
{
    public IEnumerable<VideoOverlayDefinition> GetVideoOverlays()
    {
        yield return new VideoOverlayDefinition(
            FilePath: "sample_overlay.mp4",
            StartTime: TimeSpan.FromSeconds(1),
            DestRect: new Rect(580, 240, 200, 200),
            SourceRect: new Rect(160, 20, 320, 320),
            Volume: 0.3f,
            Duration: TimeSpan.FromSeconds(4),
            FadeInDuration: TimeSpan.FromSeconds(2),
            FadeOutDuration: TimeSpan.FromSeconds(2));
    }
}
