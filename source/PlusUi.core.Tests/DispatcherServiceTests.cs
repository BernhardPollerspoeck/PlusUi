using PlusUi.core.Services;

namespace PlusUi.core.Tests;

[TestClass]
public class DispatcherServiceTests
{
    [TestMethod]
    public void IsOnUiThread_BeforeMarking_IsFalse()
    {
        // Nothing has claimed the UI thread yet, so no thread may claim to be it - otherwise
        // work posted during startup would run wherever it happened to be posted from.
        var dispatcher = new DispatcherService();

        Assert.IsFalse(dispatcher.IsOnUiThread);
    }

    [TestMethod]
    public void Post_BeforeMarking_QueuesInsteadOfRunning()
    {
        var dispatcher = new DispatcherService();
        var ran = false;

        dispatcher.Post(() => ran = true);

        Assert.IsFalse(ran, "work must not run before the UI thread is known");

        dispatcher.Drain();

        Assert.IsTrue(ran);
    }

    [TestMethod]
    public void Post_OnUiThread_StillWaitsForTheDrain()
    {
        // The property the whole class rests on. The render thread is the UI thread, so
        // running inline "because we are already there" would execute the work inside the
        // draw callback it was posted from - which is exactly the place Post exists to escape.
        var dispatcher = new DispatcherService();
        dispatcher.MarkUiThread();

        var ran = false;
        dispatcher.Post(() => ran = true);

        Assert.IsFalse(ran, "Post must never run inline, not even on the UI thread");

        dispatcher.Drain();

        Assert.IsTrue(ran);
    }

    [TestMethod]
    public void Post_FromAnotherThread_DoesNotRunUntilDrained()
    {
        var dispatcher = new DispatcherService();
        dispatcher.MarkUiThread();

        var ran = false;
        var poster = new Thread(() => dispatcher.Post(() => ran = true));
        poster.Start();
        poster.Join();

        Assert.IsFalse(ran, "work from another thread must wait for the pump");

        dispatcher.Drain();

        Assert.IsTrue(ran);
    }

    [TestMethod]
    public void Drain_RunsInPostOrder()
    {
        var dispatcher = new DispatcherService();
        var order = new List<int>();

        dispatcher.Post(() => order.Add(1));
        dispatcher.Post(() => order.Add(2));
        dispatcher.Post(() => order.Add(3));
        dispatcher.Drain();

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, order);
    }

    [TestMethod]
    public void Post_FromMixedThreads_KeepsPostOrder()
    {
        // One queue for everyone, so work runs in the order it was posted regardless of who
        // posted it. A shortcut for the UI thread would let a later item overtake an earlier
        // one from a background thread.
        var dispatcher = new DispatcherService();
        var order = new List<int>();

        var poster = new Thread(() => dispatcher.Post(() => order.Add(1)));
        poster.Start();
        poster.Join();

        dispatcher.MarkUiThread();
        dispatcher.Post(() => order.Add(2));
        dispatcher.Drain();

        CollectionAssert.AreEqual(new[] { 1, 2 }, order);
    }

    [TestMethod]
    public void Drain_WorkThatThrows_DoesNotEscape()
    {
        // The pump runs from the frame loop: an escaping exception would close the window
        // rather than fail the operation.
        var dispatcher = new DispatcherService();
        var second = false;

        dispatcher.Post(() => throw new InvalidOperationException("boom"));
        dispatcher.Post(() => second = true);

        dispatcher.Drain();

        Assert.IsTrue(second, "a failing item must not stop the ones behind it");
    }

    [TestMethod]
    public void Drain_WorkThatPostsMore_DefersToNextDrain()
    {
        // Otherwise an item that re-posts itself would spin inside one frame forever.
        var dispatcher = new DispatcherService();
        var runs = 0;

        void Republish()
        {
            runs++;
            if (runs < 3)
                dispatcher.Post(Republish);
        }

        dispatcher.Post(Republish);

        dispatcher.Drain();
        Assert.AreEqual(1, runs);

        dispatcher.Drain();
        Assert.AreEqual(2, runs);
    }

    [TestMethod]
    public void Post_Null_Throws()
    {
        var dispatcher = new DispatcherService();

        Assert.ThrowsExactly<ArgumentNullException>(() => dispatcher.Post(null!));
    }
}
