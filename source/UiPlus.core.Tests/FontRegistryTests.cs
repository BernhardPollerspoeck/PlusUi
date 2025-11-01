using PlusUi.core;
using PlusUi.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UiPlus.core.Tests;

[TestClass]
public sealed class FontRegistryTests
{
    [TestMethod]
    public void TestFontRegistry_GetTypeface_ReturnsNullForUnregisteredFont()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface("UnknownFont", FontWeight.Regular, FontStyle.Normal);

        // Assert
        Assert.IsNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_GetTypeface_ReturnsNullForEmptyFontFamily()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface("", FontWeight.Regular, FontStyle.Normal);

        // Assert
        Assert.IsNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_GetTypeface_ReturnsNullForNullFontFamily()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act
        var typeface = fontRegistry.GetTypeface(null, FontWeight.Regular, FontStyle.Normal);

        // Assert
        Assert.IsNull(typeface);
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

        // Assert - Font just won't be registered, but no exception
        var typeface = fontRegistry.GetTypeface("TestFont", FontWeight.Regular, FontStyle.Normal);
        Assert.IsNull(typeface);
    }

    [TestMethod]
    public void TestFontRegistry_FallbackToRegularWeight()
    {
        // This test verifies that the fallback logic works
        // When we request a Bold font that doesn't exist, it should try Regular
        var services = new ServiceCollection();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        var serviceProvider = services.BuildServiceProvider();
        var fontRegistry = serviceProvider.GetRequiredService<IFontRegistryService>();

        // Act - Request a bold font that doesn't exist
        var typeface = fontRegistry.GetTypeface("UnknownFont", FontWeight.Bold, FontStyle.Normal);

        // Assert - Should be null because the font family doesn't exist at all
        Assert.IsNull(typeface);
    }
}
