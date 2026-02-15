namespace PlusUi.h264;

public record VideoOverlayDefinition(
    string FilePath,
    TimeSpan StartTime,
    Rect DestRect,
    Rect? SourceRect = null,
    float PlaybackSpeed = 1.0f,
    float Volume = 1.0f,
    TimeSpan? Duration = null,
    TimeSpan? FadeInDuration = null,
    TimeSpan? FadeOutDuration = null);
