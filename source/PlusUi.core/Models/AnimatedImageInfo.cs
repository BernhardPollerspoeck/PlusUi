using SkiaSharp;

namespace PlusUi.core.Models;

/// <summary>
/// Contains information about an animated image (e.g., GIF) including all frames and their delays.
/// </summary>
internal class AnimatedImageInfo : IDisposable
{
    /// <summary>
    /// Array of image frames.
    /// </summary>
    public SKImage[] Frames { get; }

    /// <summary>
    /// Array of delays in milliseconds for each frame.
    /// </summary>
    public int[] FrameDelays { get; }

    /// <summary>
    /// Total number of frames in the animation.
    /// </summary>
    public int FrameCount => Frames.Length;

    /// <summary>
    /// Width of the animation frames.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Height of the animation frames.
    /// </summary>
    public int Height { get; }

    public AnimatedImageInfo(SKImage[] frames, int[] frameDelays, int width, int height)
    {
        if (frames.Length != frameDelays.Length)
        {
            throw new ArgumentException("Frames and frame delays must have the same length.");
        }

        Frames = frames;
        FrameDelays = frameDelays;
        Width = width;
        Height = height;
    }

    public void Dispose()
    {
        foreach (var frame in Frames)
        {
            frame?.Dispose();
        }
    }
}
