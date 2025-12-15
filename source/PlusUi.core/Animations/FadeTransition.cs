namespace PlusUi.core.Animations;

public class FadeTransition : IPageTransition
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(250);
    public Easing Easing { get; set; } = Easing.EaseInOut;

    public FadeTransition SetDuration(TimeSpan duration)
    {
        Duration = duration;
        return this;
    }

    public FadeTransition SetEasing(Easing easing)
    {
        Easing = easing;
        return this;
    }

    public void ApplyOutgoing(UiPageElement page, float progress)
    {
        page.Opacity = 1f - progress;
    }

    public void ApplyIncoming(UiPageElement page, float progress)
    {
        page.Opacity = progress;
    }

    public void Reset(UiPageElement page)
    {
        page.Opacity = 1f;
    }

    public IPageTransition GetReversed() => new FadeTransition
    {
        Duration = Duration,
        Easing = Easing
    };
}
