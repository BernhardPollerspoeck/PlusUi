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
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(null);

        // Assert - Should return (null, null, null) without throwing
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
        Assert.IsNull(svgImage);
    }

    [TestMethod]
    public void TestImageLoader_EmptySource_DoesNotThrow()
    {
        // Arrange & Act
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage("");

        // Assert - Should return (null, null, null) without throwing
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
        Assert.IsNull(svgImage);
    }

    [TestMethod]
    public void TestImageLoader_InvalidResourceName_DoesNotCrash()
    {
        // Arrange
        var nonExistentResource = "definitely.does.not.exist.in.any.assembly.png";

        // Act
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(nonExistentResource);

        // Assert - Should return null gracefully, not crash
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
        Assert.IsNull(svgImage);
    }

    [TestMethod]
    public void TestImageLoader_NonExistentFile_DoesNotCrash()
    {
        // Arrange
        var nonExistentPath = "file:/this/path/definitely/does/not/exist/image.png";

        // Act
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(nonExistentPath);

        // Assert - Should return null gracefully
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
        Assert.IsNull(svgImage);
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
            var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage($"file:{tempPath}");

            // Assert - Should successfully load the image
            Assert.IsNotNull(staticImage, "Should load static image from file");
            Assert.IsNull(animatedImage, "PNG should not be detected as animated");
            Assert.IsNull(svgImage, "PNG should not be detected as SVG");
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
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(webUrl, OnLoaded);

        // Assert - Should return null immediately (async loading)
        Assert.IsNull(staticImage, "Web images should return null immediately");
        Assert.IsNull(animatedImage, "Web images should return null immediately");
        Assert.IsNull(svgImage, "Web images should return null immediately");

        // Note: We don't assert callback is invoked because network request will fail
        // This test just ensures the async loading doesn't crash
    }

    [TestMethod]
    public void TestImageLoader_CaseInsensitive_HttpPrefix()
    {
        // Arrange
        var upperCaseUrl = "HTTP://EXAMPLE.COM/IMAGE.PNG";

        // Act
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(upperCaseUrl);

        // Assert - Should recognize HTTP prefix regardless of case
        Assert.IsNull(staticImage, "HTTP source should start async loading");
        Assert.IsNull(animatedImage);
        Assert.IsNull(svgImage);
    }

    [TestMethod]
    public void TestImageLoader_CaseInsensitive_FilePrefix()
    {
        // Arrange
        var upperCasePath = "FILE:/NONEXISTENT/PATH.PNG";

        // Act
        var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(upperCasePath);

        // Assert - Should recognize FILE prefix regardless of case
        Assert.IsNull(staticImage);
        Assert.IsNull(animatedImage);
        Assert.IsNull(svgImage);
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
            var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(source);
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
    public void TestImageLoader_SvgCache_DisposedSvgInfo_SecondLoadStillUsable()
    {
        // Arrange - Create a temp SVG file (white rect on transparent background)
        var tempPath = Path.Combine(Path.GetTempPath(), "dispose_bug_test.svg");
        try
        {
            File.WriteAllText(tempPath,
                """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><rect fill="white" width="24" height="24"/></svg>""");

            var source = $"file:{tempPath}";

            // Act - Load SVG, then dispose it (simulating what Image.Dispose does)
            var (_, _, svgImage) = _imageLoaderService.LoadImage(source);
            Assert.IsNotNull(svgImage, "First load should return SvgImageInfo");

            // Verify first render works and produces white pixels
            var firstRender = svgImage.RenderToImage(10, 10);
            Assert.IsNotNull(firstRender, "First render should succeed");
            using var firstBitmap = SKBitmap.FromImage(firstRender);
            var firstPixel = firstBitmap.GetPixel(5, 5);
            Assert.AreEqual(SKColors.White, firstPixel, "First render should produce white pixels");

            // This simulates what Image.Dispose() does with the shared cached resource
            svgImage.Dispose();

            // Load the same SVG again - cache returns the same (now disposed) instance
            var (_, _, svgImage2) = _imageLoaderService.LoadImage(source);
            Assert.IsNotNull(svgImage2, "Second load should return SvgImageInfo from cache");

            // BUG: After dispose, the cached SvgImageInfo has a disposed SKPicture.
            // RenderToImage should still produce valid pixels, but the SKPicture is gone.
            var secondRender = svgImage2.RenderToImage(10, 10);
            Assert.IsNotNull(secondRender, "Second render should succeed");
            using var secondBitmap = SKBitmap.FromImage(secondRender);
            var secondPixel = secondBitmap.GetPixel(5, 5);
            Assert.AreEqual(SKColors.White, secondPixel,
                "Second render after dispose should still produce white pixels - " +
                "BUG: Image.Dispose() disposes shared cached SvgImageInfo, corrupting it for subsequent users");
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
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
                var (staticImage, animatedImage, svgImage) = _imageLoaderService.LoadImage(source);
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
