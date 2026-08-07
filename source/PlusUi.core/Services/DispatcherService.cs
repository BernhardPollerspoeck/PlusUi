using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PlusUi.core.Services;

/// <summary>
/// The <see cref="IDispatcher"/> implementation. Platform heads drive it: one call to
/// <see cref="MarkUiThread"/> once the UI thread exists, and one call to <see cref="Drain"/>
/// per frame from a point <b>outside</b> rendering.
/// </summary>
public sealed class DispatcherService(ILogger<DispatcherService>? logger = null) : IDispatcher
{
    private readonly ConcurrentQueue<Action> _pending = new();
    private int _uiThreadId = -1;

    public bool IsOnUiThread => _uiThreadId != -1 && Environment.CurrentManagedThreadId == _uiThreadId;

    public void Post(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);

        // Always queued, never run inline - not even when the caller is already on the UI
        // thread.
        //
        // An earlier version took that shortcut, and it defeated the entire purpose. The
        // render thread IS the UI thread, so a Post from inside a draw callback ran the work
        // immediately, in the middle of the render pass: the one place this class exists to
        // move work out of. Resizing the window from there disposes the surface the current
        // draw is writing into, and the next canvas call throws ObjectDisposedException.
        //
        // "Post" means later. Code that wants "now, if I am on the right thread" can check
        // IsOnUiThread and call directly - and then it is that code's decision, made where the
        // consequences are visible, rather than a silent optimization in here.
        _pending.Enqueue(work);
    }

    /// <summary>Called by the platform head from the thread that owns the UI.</summary>
    public void MarkUiThread() => _uiThreadId = Environment.CurrentManagedThreadId;

    /// <summary>
    /// Runs everything queued so far. Called once per frame by the platform head, from
    /// outside the render pass.
    /// <para>
    /// Snapshots the count first, so work that posts more work does not starve the frame —
    /// the newly posted items wait for the next one.
    /// </para>
    /// </summary>
    public void Drain()
    {
        var remaining = _pending.Count;

        while (remaining-- > 0 && _pending.TryDequeue(out var work))
            Execute(work);
    }

    private void Execute(Action work)
    {
        try
        {
            work();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Dispatched work threw.");
        }
    }
}
