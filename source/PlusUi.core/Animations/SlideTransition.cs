namespace PlusUi.core.Animations;

public class SlideTransition : IPageTransition
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(300);
    public Easing Easing { get; set; } = Easing.EaseOut;
    public SlideDirection Direction { get; set; } = SlideDirection.Left;

    public SlideTransition SetDuration(TimeSpan duration)
    {
        Duration = duration;
        return this;
    }

    public SlideTransition SetEasing(Easing easing)
    {
        Easing = easing;
        return this;
    }

    public SlideTransition SetDirection(SlideDirection direction)
    {
        Direction = direction;
        return this;
    }

    public void ApplyOutgoing(UiPageElement page, float progress)
    {
        var offset = CalculateOutgoingOffset(page.ElementSize, progress);
        page.SetVisualOffset(offset);
    }

    public void ApplyIncoming(UiPageElement page, float progress)
    {
        var offset = CalculateIncomingOffset(page.ElementSize, 1f - progress);
        page.SetVisualOffset(offset);
    }

    public void Reset(UiPageElement page)
    {
        page.SetVisualOffset(new Point(0, 0));
    }

    public IPageTransition GetReversed() => new SlideTransition
    {
        Duration = Duration,
        Easing = Easing,
        Direction = Direction switch
        {
            SlideDirection.Left => SlideDirection.Right,
            SlideDirection.Right => SlideDirection.Left,
            SlideDirection.Up => SlideDirection.Down,
            SlideDirection.Down => SlideDirection.Up,
            _ => Direction
        }
    };

    private Point CalculateOutgoingOffset(Size size, float progress)
    {
        return Direction switch
        {
            SlideDirection.Left => new Point(-size.Width * progress, 0),
            SlideDirection.Right => new Point(size.Width * progress, 0),
            SlideDirection.Up => new Point(0, -size.Height * progress),
            SlideDirection.Down => new Point(0, size.Height * progress),
            _ => new Point(0, 0)
        };
    }

    private Point CalculateIncomingOffset(Size size, float remainingProgress)
    {
        return Direction switch
        {
            SlideDirection.Left => new Point(size.Width * remainingProgress, 0),
            SlideDirection.Right => new Point(-size.Width * remainingProgress, 0),
            SlideDirection.Up => new Point(0, size.Height * remainingProgress),
            SlideDirection.Down => new Point(0, -size.Height * remainingProgress),
            _ => new Point(0, 0)
        };
    }
}
