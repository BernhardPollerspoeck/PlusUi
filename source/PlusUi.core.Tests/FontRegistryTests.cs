using PlusUi.core;
using PlusUi.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class FontRegistryTests
{
    [TestMethod]
    public void TestFontRegistry_GetTypeface_ReturnsDefaultFontForUnregisteredFont()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface("UnknownFont", FontWeight.Regular, FontStyle.Normal);

        // Assert - Returns default Inter font instead of null
        Assert.IsNotNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_GetTypeface_ReturnsDefaultFontForEmptyFontFamily()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface("", FontWeight.Regular, FontStyle.Normal);

        // Assert - Returns default Inter font for empty font family
        Assert.IsNotNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_GetTypeface_ReturnsDefaultFontForNullFontFamily()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface(null, FontWeight.Regular, FontStyle.Normal);

        // Assert - Returns default Inter font for null font family
        Assert.IsNotNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_RegisterFont_DoesNotThrowOnInvalidPath()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act - Should not throw
        fontRegistry.RegisterFont("NonExistent.Path.ttf", "TestFont", FontWeight.Regular, FontStyle.Normal);

        // Assert - Font not registered, but returns default font
        var typeface = fontRegistry.GetTypeface("TestFont", FontWeight.Regular, FontStyle.Normal);
        Assert.IsNotNull(typeface); // Returns default Inter font
    }

    [TestMethod]
    public void TestFontRegistry_FallbackToDefaultFont()
    {
        // This test verifies that the fallback logic works
        // When we request a font that doesn't exist, we get the default Inter font
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act - Request a bold font that doesn't exist
        var typeface = fontRegistry.GetTypeface("UnknownFont", FontWeight.Bold, FontStyle.Normal);

        // Assert - Returns default Inter font
        Assert.IsNotNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_DefaultFontIsInter()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface(FontRegistryService.DefaultFontFamily, FontWeight.Regular, FontStyle.Normal);

        // Assert - Inter font should be available
        Assert.IsNotNull(typeface);
    }

    private static IFontRegistryService CreateRegistry()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        return services.BuildServiceProvider().GetRequiredService<IFontRegistryService>();
    }

    [TestMethod]
    public void TestFontRegistry_VariableFont_DerivesDistinctTypefacePerWeight()
    {
        // Arrange
        var fontRegistry = CreateRegistry();

        // Act - the embedded Inter Variable font should yield a distinct instance per weight
        var regular = fontRegistry.GetTypeface(FontRegistryService.DefaultFontFamily, FontWeight.Regular, FontStyle.Normal);
        var bold = fontRegistry.GetTypeface(FontRegistryService.DefaultFontFamily, FontWeight.Bold, FontStyle.Normal);

        // Assert
        Assert.IsNotNull(regular);
        Assert.IsNotNull(bold);
        Assert.AreNotSame(regular, bold, "Bold should be a distinct derived typeface, not the regular fallback");
    }

    [TestMethod]
    public void TestFontRegistry_VariableFont_HeavierWeightRendersWider()
    {
        // Arrange
        var fontRegistry = CreateRegistry();
        const string sample = "Weight";

        // Act - derive Light and Black from the variable base and measure their advances
        var light = fontRegistry.GetTypeface(FontRegistryService.DefaultFontFamily, FontWeight.Light, FontStyle.Normal)!;
        var black = fontRegistry.GetTypeface(FontRegistryService.DefaultFontFamily, FontWeight.Black, FontStyle.Normal)!;
        using var lightFont = light.ToFont(32f, 1f, 0f);
        using var blackFont = black.ToFont(32f, 1f, 0f);
        var lightWidth = lightFont.MeasureText(sample);
        var blackWidth = blackFont.MeasureText(sample);

        // Assert - the weight axis actually affects rendering, not just the cache key
        Assert.IsTrue(blackWidth > lightWidth, $"Black ({blackWidth}) should render wider than Light ({lightWidth})");
    }

    [TestMethod]
    public void TestFontRegistry_VariableFont_AppliesWeightForEmptyFamily()
    {
        // Arrange - the common case binds weight without an explicit font family
        var fontRegistry = CreateRegistry();

        // Act
        var regular = fontRegistry.GetTypeface("", FontWeight.Regular, FontStyle.Normal);
        var bold = fontRegistry.GetTypeface("", FontWeight.Bold, FontStyle.Normal);

        // Assert
        Assert.AreNotSame(regular, bold, "Weight derivation should apply even without an explicit family");
    }
}
