namespace PlusUi.core.Animations;

public interface IPageTransition
{
    TimeSpan Duration { get; }
    Easing Easing { get; }

    void ApplyOutgoing(UiPageElement page, float progress);
    void ApplyIncoming(UiPageElement page, float progress);
    void Reset(UiPageElement page);
    IPageTransition GetReversed();
}
