namespace PlusUi.h264;

public interface IVideoOverlayProvider
{
    IEnumerable<VideoOverlayDefinition> GetVideoOverlays();
}
