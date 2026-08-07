namespace PlusUi.core.Services;

/// <summary>
/// Moves work onto the UI thread.
/// <para>
/// Anything that touches the window, the navigation stack or the element tree has to run on
/// the thread that owns them. Background work that wants to act on its result — a completed
/// download, a global hotkey, a timer, a message from another process — therefore needs a way
/// back, and without one applications improvise: usually by doing the work inside a render
/// callback, which is the one place they can be sure of the thread.
/// </para>
/// <para>
/// That improvisation is worse than it looks. A render callback runs <b>during</b> the walk of
/// the element tree, so work posted there navigates, resizes or replaces the very structures
/// being drawn. On desktop, resizing the window from inside a draw disposes the surface that
/// draw is writing into.
/// </para>
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Whether the calling thread is the one that owns the UI. False before the UI thread has
    /// been established, so that early work is queued rather than run somewhere arbitrary.
    /// </summary>
    bool IsOnUiThread { get; }

    /// <summary>
    /// Runs the work on the UI thread — immediately if already there, otherwise at the start
    /// of the next frame, before anything is drawn.
    /// <para>
    /// Exceptions escaping the work are caught and logged rather than propagated: the queue is
    /// drained from the frame loop, so an exception let through would take the window down
    /// instead of the operation.
    /// </para>
    /// </summary>
    void Post(Action work);
}
