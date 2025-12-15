using PlusUi.core.Animations;

namespace PlusUi.core;

public interface ITransitionService
{
    bool IsTransitioning { get; }
    UiPageElement? OutgoingPage { get; }

    void StartTransition(UiPageElement outgoingPage, UiPageElement incomingPage, IPageTransition transition);
    void Update();
}
