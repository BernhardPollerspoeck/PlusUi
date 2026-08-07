using PlusUi.core;

namespace PlusUi.desktop.Tests;

/// <summary>
/// Covers the paths that are reachable without a real window. Everything that actually
/// moves a window needs a live GLFW window and therefore belongs in a manual smoke test
/// (see the "Window &amp; Displays" page in the demo app), not here — a unit test that
/// creates an OpenGL window would be a display-dependent test, not a unit test.
/// </summary>
[TestClass]
public class DesktopWindowServiceTests
{
    [TestMethod]
    public void IsSupported_WithoutWindow_IsFalse()
    {
        // Arrange - this is the state between DI construction and WindowManager.StartAsync
        var service = new DesktopWindowService();

        // Act & Assert
        Assert.IsFalse(service.IsSupported);
    }

    [TestMethod]
    public void IsBorderless_WithoutWindow_IsFalse()
    {
        var service = new DesktopWindowService();

        Assert.IsFalse(service.IsBorderless);
    }

    [TestMethod]
    public void EnterBorderless_WithoutWindow_DoesNotThrow()
    {
        // A page could call this from its constructor, before the window exists.
        var service = new DesktopWindowService();

        service.EnterBorderless(new Rect(0, 0, 1920, 1080), topMost: true);

        // And must not claim it worked.
        Assert.IsFalse(service.IsBorderless);
    }

    [TestMethod]
    public void RestoreNormal_WithoutWindow_DoesNotThrow()
    {
        var service = new DesktopWindowService();

        service.RestoreNormal();
    }

    [TestMethod]
    public void RestoreNormal_WithoutPriorEnterBorderless_DoesNotThrow()
    {
        var service = new DesktopWindowService();

        service.RestoreNormal();
        service.RestoreNormal();
    }

    [TestMethod]
    public void MoveToAndResize_WithoutWindow_DoNotThrow()
    {
        var service = new DesktopWindowService();

        service.MoveTo(120, 80);
        service.Resize(800, 600);
    }

    [TestMethod]
    public void Resize_WithoutWindow_IgnoresDegenerateSizes()
    {
        // A drag handle pulled past the opposite edge produces these on every frame.
        var service = new DesktopWindowService();

        service.Resize(0, 400);
        service.Resize(400, -200);
    }

    [TestMethod]
    public void GetDisplays_WithoutWindow_IsEmpty()
    {
        // Deliberately different from NoOpWindowService: on desktop an empty list means
        // "GLFW is not up yet", which is a temporary state, not a platform property.
        // Inventing a display here would hide a call made too early.
        var service = new DesktopWindowService();

        Assert.AreEqual(0, service.GetDisplays().Count);
    }

    [TestMethod]
    public void VirtualDesktopBounds_WithoutWindow_IsEmpty()
    {
        var service = new DesktopWindowService();

        var bounds = service.VirtualDesktopBounds;

        Assert.AreEqual(0, bounds.X);
        Assert.AreEqual(0, bounds.Y);
        Assert.AreEqual(0, bounds.Width);
        Assert.AreEqual(0, bounds.Height);
    }
}
