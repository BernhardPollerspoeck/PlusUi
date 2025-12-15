namespace PlusUi.core.Animations;

public class NoneTransition : IPageTransition
{
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public Easing Easing { get; set; } = Easing.Linear;

    public NoneTransition SetDuration(TimeSpan duration)
    {
        Duration = duration;
        return this;
    }

    public NoneTransition SetEasing(Easing easing)
    {
        Easing = easing;
        return this;
    }

    public void ApplyOutgoing(UiPageElement page, float progress)
    {
        // No visual effect - instant transition
    }

    public void ApplyIncoming(UiPageElement page, float progress)
    {
        // No visual effect - instant transition
    }

    public void Reset(UiPageElement page)
    {
        // Nothing to reset
    }

    public IPageTransition GetReversed() => new NoneTransition
    {
        Duration = Duration,
        Easing = Easing
    };
}
