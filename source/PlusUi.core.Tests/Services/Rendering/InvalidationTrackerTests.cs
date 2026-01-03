using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core.Services.Rendering;

namespace PlusUi.core.Tests.Services.Rendering;

[TestClass]
public class InvalidationTrackerTests
{
    [TestMethod]
    public void NeedsRendering_Initially_ReturnsFalse()
    {
        // Arrange
        var tracker = new InvalidationTracker();

        // Act
        var needsRendering = tracker.NeedsRendering;

        // Assert
        Assert.IsFalse(needsRendering, "InvalidationTracker should not need rendering initially");
    }

    [TestMethod]
    public void RequestRender_SetsNeedsRenderingToTrue()
    {
        // Arrange
        var tracker = new InvalidationTracker();

        // Act
        tracker.RequestRender();

        // Assert
        Assert.IsTrue(tracker.NeedsRendering, "NeedsRendering should be true after RequestRender");
    }

    [TestMethod]
    public void FrameRendered_ClearsManualRenderRequest()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        tracker.RequestRender();
        Assert.IsTrue(tracker.NeedsRendering, "Precondition: NeedsRendering should be true");

        // Act
        tracker.FrameRendered();

        // Assert
        Assert.IsFalse(tracker.NeedsRendering, "NeedsRendering should be false after FrameRendered");
    }

    [TestMethod]
    public void RequestRender_FiresRenderingRequiredChanged_WhenStateChanges()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var eventFired = false;
        tracker.RenderingRequiredChanged += (_, _) => eventFired = true;

        // Act
        tracker.RequestRender();

        // Assert
        Assert.IsTrue(eventFired, "RenderingRequiredChanged should fire when state changes from false to true");
    }

    [TestMethod]
    public void RequestRender_DoesNotFireEvent_WhenAlreadyNeedsRendering()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        tracker.RequestRender(); // First request

        var eventFireCount = 0;
        tracker.RenderingRequiredChanged += (_, _) => eventFireCount++;

        // Act
        tracker.RequestRender(); // Second request while already needing rendering

        // Assert
        Assert.AreEqual(0, eventFireCount, "Event should not fire when already needing rendering");
    }

    [TestMethod]
    public void Register_AddsInvalidatorToTracking()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator = new TestInvalidator { NeedsRendering = true };

        // Act
        tracker.Register(invalidator);

        // Assert
        Assert.IsTrue(tracker.NeedsRendering, "Tracker should need rendering when invalidator needs rendering");
    }

    [TestMethod]
    public void Unregister_RemovesInvalidatorFromTracking()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator = new TestInvalidator { NeedsRendering = true };
        tracker.Register(invalidator);
        Assert.IsTrue(tracker.NeedsRendering, "Precondition: Should need rendering");

        // Act
        tracker.Unregister(invalidator);

        // Assert
        Assert.IsFalse(tracker.NeedsRendering, "Should not need rendering after unregistering");
    }

    [TestMethod]
    public void InvalidatorChanged_TriggersRenderingRequiredChanged()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator = new TestInvalidator { NeedsRendering = false };
        tracker.Register(invalidator);

        var eventFired = false;
        tracker.RenderingRequiredChanged += (_, _) => eventFired = true;

        // Act
        invalidator.NeedsRendering = true;
        invalidator.RaiseInvalidationChanged();

        // Wait for debounce timer (20ms should be enough for 16ms debounce)
        Thread.Sleep(20);

        // Assert
        Assert.IsTrue(eventFired, "RenderingRequiredChanged should fire when invalidator changes");
        Assert.IsTrue(tracker.NeedsRendering, "Tracker should need rendering");
    }

    [TestMethod]
    public void MultipleInvalidators_AnyNeedsRendering_TrackerNeedsRendering()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator1 = new TestInvalidator { NeedsRendering = false };
        var invalidator2 = new TestInvalidator { NeedsRendering = false };
        var invalidator3 = new TestInvalidator { NeedsRendering = false };

        tracker.Register(invalidator1);
        tracker.Register(invalidator2);
        tracker.Register(invalidator3);

        // Act - only one needs rendering
        invalidator2.NeedsRendering = true;

        // Assert
        Assert.IsTrue(tracker.NeedsRendering, "Tracker should need rendering if any invalidator needs it");
    }

    [TestMethod]
    public void MultipleInvalidators_AllIdle_TrackerIdle()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator1 = new TestInvalidator { NeedsRendering = true };
        var invalidator2 = new TestInvalidator { NeedsRendering = true };

        tracker.Register(invalidator1);
        tracker.Register(invalidator2);
        Assert.IsTrue(tracker.NeedsRendering, "Precondition: Should need rendering");

        // Act - all become idle
        invalidator1.NeedsRendering = false;
        invalidator2.NeedsRendering = false;

        // Assert
        Assert.IsFalse(tracker.NeedsRendering, "Tracker should be idle when all invalidators are idle");
    }

    [TestMethod]
    public void FrameRendered_DoesNotClearInvalidatorRendering()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator = new TestInvalidator { NeedsRendering = true };
        tracker.Register(invalidator);
        Assert.IsTrue(tracker.NeedsRendering, "Precondition");

        // Act
        tracker.FrameRendered();

        // Assert
        Assert.IsTrue(tracker.NeedsRendering, "Invalidator is still active, so tracker should still need rendering");
        Assert.IsTrue(invalidator.NeedsRendering, "Invalidator state should not change");
    }

    [TestMethod]
    public void CombinedScenario_ManualAndInvalidator()
    {
        // Arrange
        var tracker = new InvalidationTracker();
        var invalidator = new TestInvalidator { NeedsRendering = false };
        tracker.Register(invalidator);

        // Act & Assert - Manual request
        tracker.RequestRender();
        Assert.IsTrue(tracker.NeedsRendering, "Manual request should make tracker need rendering");

        // Frame rendered - manual cleared
        tracker.FrameRendered();
        Assert.IsFalse(tracker.NeedsRendering, "After frame, no rendering needed");

        // Invalidator becomes active
        invalidator.NeedsRendering = true;
        Assert.IsTrue(tracker.NeedsRendering, "Invalidator active means tracker needs rendering");

        // Frame rendered - invalidator still active
        tracker.FrameRendered();
        Assert.IsTrue(tracker.NeedsRendering, "Still needs rendering because invalidator is active");

        // Invalidator becomes idle
        invalidator.NeedsRendering = false;
        Assert.IsFalse(tracker.NeedsRendering, "All idle, no rendering needed");
    }

    // Helper class for testing
    private class TestInvalidator : IInvalidator
    {
        private bool _needsRendering;

        public bool NeedsRendering
        {
            get => _needsRendering;
            set => _needsRendering = value;
        }

        public event EventHandler? InvalidationChanged;

        public void RaiseInvalidationChanged()
        {
            InvalidationChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
