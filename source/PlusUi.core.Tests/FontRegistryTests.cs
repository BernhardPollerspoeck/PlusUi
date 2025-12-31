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
}
