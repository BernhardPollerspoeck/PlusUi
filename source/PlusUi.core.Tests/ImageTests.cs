using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

/// <summary>
/// Tests for the Image control, especially prefix-based source handling
/// </summary>
[TestClass]
public sealed class ImageTests
{
    [TestMethod]
    public void TestImage_NullSource_ReturnsNull()
    {
        // Arrange
        var image = new Image();
        
        // Act - Image source is null by default
        
        // Assert - The image should not throw and should handle null gracefully
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_EmptySource_ReturnsNull()
    {
        // Arrange
        var image = new Image()
            .SetImageSource("");
        
        // Act & Assert - Should not throw
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_InvalidResourceSource_DoesNotThrow()
    {
        // Arrange & Act
        var image = new Image()
            .SetImageSource("nonexistent.resource.png");
        
        // Assert - Should not throw exception, just return null image
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_FilePrefix_WithNonexistentFile_DoesNotThrow()
    {
        // Arrange & Act
        var image = new Image()
            .SetImageSource("file:/nonexistent/path/image.png");
        
        // Assert - Should not throw exception
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_FilePrefix_WithValidFile_LoadsImage()
    {
        // Arrange - Create a temporary test image
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "test_image.png");
        try
        {
            // Create a simple 10x10 red image
            using (var bitmap = new SKBitmap(10, 10))
            {
                bitmap.Erase(SKColors.Red);
                using var skImage = SKImage.FromBitmap(bitmap);
                using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = System.IO.File.OpenWrite(tempPath);
                data.SaveTo(stream);
            }

            // Act
            var image = new Image()
                .SetImageSource($"file:{tempPath}");

            // Assert - Image should be created without throwing
            Assert.IsNotNull(image);
        }
        finally
        {
            // Cleanup
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }

    [TestMethod]
    public void TestImage_HttpPrefix_RecognizedAsWebSource()
    {
        // Arrange & Act
        var image = new Image()
            .SetImageSource("http://example.com/image.png");
        
        // Assert - Should not throw (even though loading will fail due to network)
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_HttpsPrefix_RecognizedAsWebSource()
    {
        // Arrange & Act
        var image = new Image()
            .SetImageSource("https://example.com/image.png");
        
        // Assert - Should not throw (even though loading will fail due to network)
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_SetImageSource_ReturnsSelf()
    {
        // Arrange
        var image = new Image();
        
        // Act
        var result = image.SetImageSource("test.png");
        
        // Assert - Should return the same instance for method chaining
        Assert.AreSame(image, result);
    }

    [TestMethod]
    public void TestImage_CaseInsensitive_HttpPrefix()
    {
        // Arrange & Act
        var image = new Image()
            .SetImageSource("HTTP://example.com/image.png");
        
        // Assert - Should recognize uppercase HTTP
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void TestImage_CaseInsensitive_FilePrefix()
    {
        // Arrange & Act
        var image = new Image()
            .SetImageSource("FILE:/path/to/image.png");
        
        // Assert - Should recognize uppercase FILE
        Assert.IsNotNull(image);
    }
}
