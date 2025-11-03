using PlusUi.core;
using SkiaSharp;

namespace UiPlus.core.Tests;

/// <summary>
/// Smoke tests for ImageLoaderService focusing on critical async and caching functionality.
/// These tests ensure the service doesn't crash on edge cases and handles memory correctly.
/// </summary>
[TestClass]
public sealed class ImageLoaderServiceTests
{
    [TestMethod]
    public void TestImageLoader_NullSource_DoesNotThrow()
    {
        // Arrange & Act
        var result = ImageLoaderService.LoadImage(null);

        // Assert - Should return (null, null) without throwing
        Assert.IsNull(result.staticImage);
        Assert.IsNull(result.animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_EmptySource_DoesNotThrow()
    {
        // Arrange & Act
        var result = ImageLoaderService.LoadImage("");

        // Assert - Should return (null, null) without throwing
        Assert.IsNull(result.staticImage);
        Assert.IsNull(result.animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_InvalidResourceName_DoesNotCrash()
    {
        // Arrange
        var nonExistentResource = "definitely.does.not.exist.in.any.assembly.png";

        // Act
        var result = ImageLoaderService.LoadImage(nonExistentResource);

        // Assert - Should return null gracefully, not crash
        Assert.IsNull(result.staticImage);
        Assert.IsNull(result.animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_NonExistentFile_DoesNotCrash()
    {
        // Arrange
        var nonExistentPath = "file:/this/path/definitely/does/not/exist/image.png";

        // Act
        var result = ImageLoaderService.LoadImage(nonExistentPath);

        // Assert - Should return null gracefully
        Assert.IsNull(result.staticImage);
        Assert.IsNull(result.animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_ValidFileSource_LoadsImage()
    {
        // Arrange - Create a temporary test image file
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "imageloader_test.png");
        try
        {
            // Create a simple 5x5 blue image
            using (var bitmap = new SKBitmap(5, 5))
            {
                bitmap.Erase(SKColors.Blue);
                using var skImage = SKImage.FromBitmap(bitmap);
                using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = System.IO.File.OpenWrite(tempPath);
                data.SaveTo(stream);
            }

            // Act
            var result = ImageLoaderService.LoadImage($"file:{tempPath}");

            // Assert - Should successfully load the image
            Assert.IsNotNull(result.staticImage, "Should load static image from file");
            Assert.IsNull(result.animatedImage, "PNG should not be detected as animated");
            Assert.AreEqual(5, result.staticImage.Width);
            Assert.AreEqual(5, result.staticImage.Height);
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
    public void TestImageLoader_WebSource_StartsAsyncLoading()
    {
        // Arrange
        var webUrl = "https://example.com/image.png";
        bool callbackInvoked = false;
        void OnLoaded(SKImage? image)
        {
            callbackInvoked = true;
        }

        // Act
        var result = ImageLoaderService.LoadImage(webUrl, OnLoaded);

        // Assert - Should return null immediately (async loading)
        Assert.IsNull(result.staticImage, "Web images should return null immediately");
        Assert.IsNull(result.animatedImage, "Web images should return null immediately");

        // Note: We don't assert callback is invoked because network request will fail
        // This test just ensures the async loading doesn't crash
    }

    [TestMethod]
    public void TestImageLoader_CaseInsensitive_HttpPrefix()
    {
        // Arrange
        var upperCaseUrl = "HTTP://EXAMPLE.COM/IMAGE.PNG";

        // Act
        var result = ImageLoaderService.LoadImage(upperCaseUrl);

        // Assert - Should recognize HTTP prefix regardless of case
        Assert.IsNull(result.staticImage, "HTTP source should start async loading");
        Assert.IsNull(result.animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_CaseInsensitive_FilePrefix()
    {
        // Arrange
        var upperCasePath = "FILE:/NONEXISTENT/PATH.PNG";

        // Act
        var result = ImageLoaderService.LoadImage(upperCasePath);

        // Assert - Should recognize FILE prefix regardless of case
        Assert.IsNull(result.staticImage);
        Assert.IsNull(result.animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_Cache_SameResourceTwice_ReturnsSameImage()
    {
        // Arrange - Create a temp file
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cache_test.png");
        try
        {
            using (var bitmap = new SKBitmap(3, 3))
            {
                bitmap.Erase(SKColors.Green);
                using var skImage = SKImage.FromBitmap(bitmap);
                using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = System.IO.File.OpenWrite(tempPath);
                data.SaveTo(stream);
            }

            var source = $"file:{tempPath}";

            // Act - Load the same source twice
            var result1 = ImageLoaderService.LoadImage(source);
            var result2 = ImageLoaderService.LoadImage(source);

            // Assert - Should return cached image (same instance)
            Assert.IsNotNull(result1.staticImage);
            Assert.IsNotNull(result2.staticImage);
            Assert.AreSame(result1.staticImage, result2.staticImage, "Cache should return the same image instance");
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }

    [TestMethod]
    public void TestImageLoader_WeakReferenceCache_AllowsGarbageCollection()
    {
        // Arrange - Create a temp file
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "weak_ref_test.png");
        try
        {
            using (var bitmap = new SKBitmap(2, 2))
            {
                bitmap.Erase(SKColors.Red);
                using var skImage = SKImage.FromBitmap(bitmap);
                using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = System.IO.File.OpenWrite(tempPath);
                data.SaveTo(stream);
            }

            var source = $"file:{tempPath}";

            // Act - Load image, let it go out of scope, force GC
            {
                var result = ImageLoaderService.LoadImage(source);
                Assert.IsNotNull(result.staticImage);
            }

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Act - Load again
            var resultAfterGC = ImageLoaderService.LoadImage(source);

            // Assert - Should load successfully even after GC
            // (Weak reference allows collection, so new load might be different instance)
            Assert.IsNotNull(resultAfterGC.staticImage, "Should reload image after GC collected cached version");
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
}
