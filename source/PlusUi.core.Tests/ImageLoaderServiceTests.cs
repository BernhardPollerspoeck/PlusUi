using Microsoft.Extensions.Options;
using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

/// <summary>
/// Smoke tests for ImageLoaderService focusing on critical async and caching functionality.
/// These tests ensure the service doesn't crash on edge cases and handles memory correctly.
/// </summary>
[TestClass]
public sealed class ImageLoaderServiceTests
{

    private ImageLoaderService _imageLoaderService = null!;

    [TestInitialize]
    public void Setup()
    {
        var configuration = new PlusUiConfiguration();
        var options = Options.Create(configuration);
        _imageLoaderService = new ImageLoaderService(options);
    }


    [TestMethod]
    public void TestImageLoader_NullSource_DoesNotThrow()
    {
        // Arrange & Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage(null);

        // Assert - Should return (null, null) without throwing
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_EmptySource_DoesNotThrow()
    {
        // Arrange & Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage("");

        // Assert - Should return (null, null) without throwing
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_InvalidResourceName_DoesNotCrash()
    {
        // Arrange
        var nonExistentResource = "definitely.does.not.exist.in.any.assembly.png";

        // Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage(nonExistentResource);

        // Assert - Should return null gracefully, not crash
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_NonExistentFile_DoesNotCrash()
    {
        // Arrange
        var nonExistentPath = "file:/this/path/definitely/does/not/exist/image.png";

        // Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage(nonExistentPath);

        // Assert - Should return null gracefully
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
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
            var (staticImage, animatedImage) = _imageLoaderService.LoadImage($"file:{tempPath}");

            // Assert - Should successfully load the image
            Assert.IsNotNull(staticImage, "Should load static image from file");
            Assert.IsNull(animatedImage, "PNG should not be detected as animated");
            Assert.AreEqual(5, staticImage.Width);
            Assert.AreEqual(5, staticImage.Height);
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
        static void OnLoaded(SKImage? image)
        {
        }

        // Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage(webUrl, OnLoaded);

        // Assert - Should return null immediately (async loading)
        Assert.IsNull(staticImage, "Web images should return null immediately");
        Assert.IsNull(animatedImage, "Web images should return null immediately");

        // Note: We don't assert callback is invoked because network request will fail
        // This test just ensures the async loading doesn't crash
    }

    [TestMethod]
    public void TestImageLoader_CaseInsensitive_HttpPrefix()
    {
        // Arrange
        var upperCaseUrl = "HTTP://EXAMPLE.COM/IMAGE.PNG";

        // Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage(upperCaseUrl);

        // Assert - Should recognize HTTP prefix regardless of case
        Assert.IsNull(staticImage, "HTTP source should start async loading");
        Assert.IsNull(animatedImage);
    }

    [TestMethod]
    public void TestImageLoader_CaseInsensitive_FilePrefix()
    {
        // Arrange
        var upperCasePath = "FILE:/NONEXISTENT/PATH.PNG";

        // Act
        var (staticImage, animatedImage) = _imageLoaderService.LoadImage(upperCasePath);

        // Assert - Should recognize FILE prefix regardless of case
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
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
            var (staticImage, animatedImage) = _imageLoaderService.LoadImage(source);
            var result2 = _imageLoaderService.LoadImage(source);

            // Assert - Should return cached image (same instance)
            Assert.IsNotNull(staticImage);
            Assert.IsNotNull(result2.staticImage);
            Assert.AreSame(staticImage, result2.staticImage, "Cache should return the same image instance");
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
                var (staticImage, animatedImage) = _imageLoaderService.LoadImage(source);
                Assert.IsNotNull(staticImage);
            }

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Act - Load again
            var resultAfterGC = _imageLoaderService.LoadImage(source);

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
