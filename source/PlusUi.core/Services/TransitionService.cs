using PlusUi.core.Animations;

namespace PlusUi.core;

internal class TransitionService : ITransitionService
{
    private DateTime _transitionStart;
    private IPageTransition? _activeTransition;
    private UiPageElement? _outgoingPage;
    private UiPageElement? _incomingPage;

    public bool IsTransitioning => _activeTransition != null;
    public UiPageElement? OutgoingPage => _outgoingPage;

    public void StartTransition(UiPageElement outgoingPage, UiPageElement incomingPage, IPageTransition transition)
    {
        _outgoingPage = outgoingPage;
        _incomingPage = incomingPage;
        _activeTransition = transition;
        _transitionStart = DateTime.Now;
    }

    public void Update()
    {
        if (_activeTransition == null || _outgoingPage == null || _incomingPage == null)
        {
            return;
        }

        var elapsed = (DateTime.Now - _transitionStart).TotalMilliseconds;
        var duration = _activeTransition.Duration.TotalMilliseconds;

        if (duration <= 0)
        {
            CompleteTransition();
            return;
        }

        var rawProgress = Math.Min(1f, (float)(elapsed / duration));
        var progress = EasingFunctions.Apply(_activeTransition.Easing, rawProgress);

        _activeTransition.ApplyOutgoing(_outgoingPage, progress);
        _activeTransition.ApplyIncoming(_incomingPage, progress);

        if (rawProgress >= 1f)
        {
            CompleteTransition();
        }
    }

    private void CompleteTransition()
    {
        if (_activeTransition != null)
        {
            if (_outgoingPage != null)
            {
                _activeTransition.Reset(_outgoingPage);
            }
            if (_incomingPage != null)
            {
                _activeTransition.Reset(_incomingPage);
            }
        }

        _outgoingPage = null;
        _incomingPage = null;
        _activeTransition = null;
    }
}
