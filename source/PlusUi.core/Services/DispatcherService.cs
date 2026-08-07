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

        // Running inline when already on the UI thread keeps Post usable as "make sure this
        // happens on the UI thread" without the caller having to know where it is. It also
        // preserves ordering: a queued item followed by an inline one would otherwise run in
        // the wrong order.
        if (IsOnUiThread && _pending.IsEmpty)
        {
            Execute(work);
            return;
        }

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
