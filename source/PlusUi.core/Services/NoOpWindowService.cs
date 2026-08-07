namespace PlusUi.core.Services;

/// <summary>
/// Default implementation for platforms without a window concept — mobile, web, headless.
/// <para>
/// Window control does nothing. <see cref="GetDisplays"/> still returns one entry: the
/// drawing surface itself. An empty list would be the formally more correct but less useful
/// answer — it would force every caller that merely wants the dimensions into a special
/// case, and thus back into exactly the platform switch this interface exists to avoid.
/// </para>
/// </summary>
internal sealed class NoOpWindowService(IPlatformService platformService) : IWindowService
{
    public bool IsSupported => false;

    public bool IsBorderless => false;

    public void EnterBorderless(Rect bounds, bool topMost)
    {
        // Intentionally empty.
    }

    public void RestoreNormal()
    {
        // Intentionally empty.
    }

    public Rect Bounds
    {
        get
        {
            var size = platformService.WindowSize;
            return new Rect(0, 0, size.Width, size.Height);
        }
    }

    public void MoveTo(float x, float y)
    {
        // Intentionally empty.
    }

    public void Resize(float width, float height)
    {
        // Intentionally empty.
    }

    public IReadOnlyList<DisplayInfo> GetDisplays()
    {
        var size = platformService.WindowSize;
        return
        [
            new DisplayInfo(
                Index: 0,
                Name: platformService.Platform.ToString(),
                X: 0,
                Y: 0,
                Width: (int)size.Width,
                Height: (int)size.Height,
                Scale: platformService.DisplayDensity,
                IsPrimary: true)
        ];
    }

    public Rect VirtualDesktopBounds
    {
        get
        {
            var size = platformService.WindowSize;
            return new Rect(0, 0, size.Width, size.Height);
        }
    }
}
