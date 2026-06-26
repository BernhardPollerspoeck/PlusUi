using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class CursorResolverTests
{
    private sealed class FakeProvider(bool canProvide) : ICursorProvider
    {
        public CursorType? Applied { get; private set; }
        public bool CanProvide(CursorType cursor) => canProvide;
        public void Apply(CursorType cursor) => Applied = cursor;
    }

    [TestMethod]
    public void Apply_UsesFirstProviderThatCanProvide()
    {
        var first = new FakeProvider(false);
        var second = new FakeProvider(true);
        var third = new FakeProvider(true);
        var resolver = new CursorResolver([first, second, third]);

        resolver.Apply(CursorType.Hand);

        Assert.IsNull(first.Applied, "A provider that cannot provide must be skipped.");
        Assert.AreEqual(CursorType.Hand, second.Applied, "The first capable provider must apply the cursor.");
        Assert.IsNull(third.Applied, "Resolution must stop at the first capable provider.");
    }

    [TestMethod]
    public void Apply_FallsThroughToLaterProvider()
    {
        var glfw = new FakeProvider(false);   // doesn't support this cursor
        var selfDrawn = new FakeProvider(true); // backstop
        var resolver = new CursorResolver([glfw, selfDrawn]);

        resolver.Apply(CursorType.Wait);

        Assert.AreEqual(CursorType.Wait, selfDrawn.Applied,
            "When earlier providers can't provide, resolution must fall through to the backstop.");
    }

    [TestMethod]
    public void Apply_NoProviderCanProvide_DoesNothing()
    {
        var only = new FakeProvider(false);
        var resolver = new CursorResolver([only]);

        resolver.Apply(CursorType.Wait); // must not throw

        Assert.IsNull(only.Applied);
    }
}
