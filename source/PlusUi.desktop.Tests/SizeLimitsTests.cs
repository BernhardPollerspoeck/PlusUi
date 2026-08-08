namespace PlusUi.desktop.Tests;

/// <summary>
/// Covers the clamping that <see cref="DesktopWindowService.Resize"/> applies before touching
/// the window.
/// <para>
/// It exists because GLFW's own size limits are enforced through the window manager's
/// interactive resize, and a borderless window has none: measured on Windows, a decorated
/// window honours a 900 × 700 minimum while a borderless one shrinks straight past it to
/// 300 × 200. An application that asked for a minimum gets one either way.
/// </para>
/// </summary>
[TestClass]
public class SizeLimitsTests
{
    [TestMethod]
    public void NoLimits_LeavesTheSizeAlone()
    {
        var limits = new DesktopWindowService.SizeLimits(null, null, null, null);

        var (width, height) = limits.Clamp(300, 200);

        Assert.AreEqual(300, width);
        Assert.AreEqual(200, height);
    }

    [TestMethod]
    public void BelowMinimum_IsRaised()
    {
        var limits = new DesktopWindowService.SizeLimits(660, 460, null, null);

        var (width, height) = limits.Clamp(300, 200);

        Assert.AreEqual(660, width);
        Assert.AreEqual(460, height);
    }

    [TestMethod]
    public void AboveMinimum_IsUntouched()
    {
        var limits = new DesktopWindowService.SizeLimits(660, 460, null, null);

        var (width, height) = limits.Clamp(1200, 800);

        Assert.AreEqual(1200, width);
        Assert.AreEqual(800, height);
    }

    [TestMethod]
    public void AboveMaximum_IsLowered()
    {
        var limits = new DesktopWindowService.SizeLimits(null, null, 1024, 768);

        var (width, height) = limits.Clamp(1600, 1200);

        Assert.AreEqual(1024, width);
        Assert.AreEqual(768, height);
    }

    [TestMethod]
    public void EachAxisIsClampedOnItsOwn()
    {
        // A window dragged past the bottom edge but not the right one is the ordinary case,
        // not an edge case - the two axes must not borrow each other's verdict.
        var limits = new DesktopWindowService.SizeLimits(660, 460, null, null);

        var (width, height) = limits.Clamp(900, 100);

        Assert.AreEqual(900, width);
        Assert.AreEqual(460, height);
    }

    [TestMethod]
    public void ContradictingLimits_LetTheMinimumWin()
    {
        // Nonsense input, but it has to resolve to something: a window at the minimum is
        // usable, a window at a maximum below the minimum is not.
        var limits = new DesktopWindowService.SizeLimits(800, 600, 400, 300);

        var (width, height) = limits.Clamp(1000, 1000);

        Assert.AreEqual(800, width);
        Assert.AreEqual(600, height);
    }

    [TestMethod]
    public void NonPositiveLimits_AreTreatedAsAbsent()
    {
        // Zero and negatives are how "no bound" arrives from callers that compute their
        // limits; pinning a window to zero would be the one reading nobody intends.
        var limits = new DesktopWindowService.SizeLimits(0, -1, 0, -1);

        var (width, height) = limits.Clamp(300, 200);

        Assert.AreEqual(300, width);
        Assert.AreEqual(200, height);
    }

    [TestMethod]
    public void SetSizeLimits_WithoutWindow_IsRememberedForLater()
    {
        // Attach-time calls happen before the native handle exists. The GLFW part is skipped
        // then, but the values must survive - otherwise the limit silently never applies.
        var service = new DesktopWindowService();

        service.SetSizeLimits(660, 460, null, null);

        var (width, height) = service.CurrentLimits.Clamp(300, 200);
        Assert.AreEqual(660, width);
        Assert.AreEqual(460, height);
    }
}
