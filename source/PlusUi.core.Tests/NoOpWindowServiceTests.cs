using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.core.Tests;

[TestClass]
public class NoOpWindowServiceTests
{
    private sealed class FakePlatformService(
        PlatformType platform = PlatformType.Android,
        float width = 412,
        float height = 915,
        float density = 2.5f) : IPlatformService
    {
        public PlatformType Platform => platform;
        public Size WindowSize => new(width, height);
        public float DisplayDensity => density;
        public bool OpenUrl(string url) => false;
    }

    [TestMethod]
    public void IsSupported_IsFalse()
    {
        // Arrange
        var service = new NoOpWindowService(new FakePlatformService());

        // Act & Assert
        Assert.IsFalse(service.IsSupported);
    }

    [TestMethod]
    public void EnterBorderless_DoesNotThrow_AndDoesNotChangeState()
    {
        // Arrange
        var service = new NoOpWindowService(new FakePlatformService());

        // Act
        service.EnterBorderless(new Rect(0, 0, 1920, 1080), topMost: true);

        // Assert - the whole point of the no-op: callers may call it unconditionally,
        // and nothing about the service changes as a result.
        Assert.IsFalse(service.IsBorderless);
    }

    [TestMethod]
    public void RestoreNormal_WithoutPriorCall_DoesNotThrow()
    {
        // Arrange
        var service = new NoOpWindowService(new FakePlatformService());

        // Act & Assert
        service.RestoreNormal();
    }

    [TestMethod]
    public void MoveToAndResize_DoNotThrow()
    {
        // The point of the no-op: an application with its own window chrome calls these on
        // every drag, and must not have to ask first whether it is on a desktop.
        var service = new NoOpWindowService(new FakePlatformService());

        service.MoveTo(120, 80);
        service.Resize(800, 600);
        service.SetSizeLimits(480, 340, null, null);
    }

    [TestMethod]
    public void GetDisplays_ReturnsExactlyOneEntry_DescribingTheSurface()
    {
        // Arrange - a caller on mobile still asks "what am I drawing on?"
        var service = new NoOpWindowService(new FakePlatformService(PlatformType.Android, 412, 915, 2.5f));

        // Act
        var displays = service.GetDisplays();

        // Assert
        Assert.AreEqual(1, displays.Count);

        var display = displays[0];
        Assert.AreEqual(0, display.Index);
        Assert.AreEqual(0, display.X);
        Assert.AreEqual(0, display.Y);
        Assert.AreEqual(412, display.Width);
        Assert.AreEqual(915, display.Height);
        Assert.AreEqual(2.5f, display.Scale);
        Assert.IsTrue(display.IsPrimary);
    }

    [TestMethod]
    public void GetDisplays_IsNeverEmpty()
    {
        // The empty list would be the formally correct answer on a platform without
        // displays - and would force every caller into a special case. Guard against
        // someone "fixing" it later.
        var service = new NoOpWindowService(new FakePlatformService(PlatformType.Headless, 0, 0, 1f));

        Assert.AreEqual(1, service.GetDisplays().Count);
    }

    [TestMethod]
    public void VirtualDesktopBounds_MatchesTheSurface()
    {
        // Arrange
        var service = new NoOpWindowService(new FakePlatformService(PlatformType.Web, 1280, 720, 1f));

        // Act
        var bounds = service.VirtualDesktopBounds;

        // Assert
        Assert.AreEqual(0, bounds.X);
        Assert.AreEqual(0, bounds.Y);
        Assert.AreEqual(1280, bounds.Width);
        Assert.AreEqual(720, bounds.Height);
    }

    [TestMethod]
    public void VirtualDesktopBounds_MatchesTheSingleDisplay()
    {
        // The two ways of asking for "the available area" must not disagree.
        var service = new NoOpWindowService(new FakePlatformService(PlatformType.iOS, 393, 852, 3f));

        var bounds = service.VirtualDesktopBounds;
        var display = service.GetDisplays()[0];

        Assert.AreEqual(display.Bounds.X, bounds.X);
        Assert.AreEqual(display.Bounds.Y, bounds.Y);
        Assert.AreEqual(display.Bounds.Width, bounds.Width);
        Assert.AreEqual(display.Bounds.Height, bounds.Height);
    }
}
