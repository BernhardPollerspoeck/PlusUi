namespace PlusUi.h264;

public class VideoConfiguration
{
    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;
    public string OutputFilePath { get; set; } = "output.mp4";
    public int FrameRate { get; set; } = 30;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);
}
