using System.Collections.Concurrent;
using SkiaSharp;
using PlusUi.core.Services.DebugBridge;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.core.Services;

/// <summary>
/// Entry in the paint cache with reference counting.
/// </summary>
internal class PaintEntry
{
    public required SKPaint Paint { get; init; }
    public required SKFont Font { get; init; }
    public int RefCount;
    public required PaintCacheKey Key { get; init; }

    public void Dispose()
    {
        Paint.Dispose();
        Font.Dispose();
    }
}

/// <summary>
/// Thread-safe service for centralized SKPaint/SKFont management with caching and reference counting.
/// </summary>
public class PaintRegistryService : IPaintRegistryService
{
    private readonly ConcurrentDictionary<PaintCacheKey, PaintEntry> _cache = new();
    private readonly object _disposeLock = new();
    private readonly DebugBridgeClient? _debugBridgeClient;

    public int CacheCount => _cache.Count;

    /// <summary>
    /// Public constructor for use when Debug Bridge is not available.
    /// </summary>
    public PaintRegistryService() : this(null)
    {
    }

    /// <summary>
    /// Internal constructor for DI with Debug Bridge support.
    /// </summary>
    internal PaintRegistryService(DebugBridgeClient? debugBridgeClient)
    {
        _debugBridgeClient = debugBridgeClient;
    }

    public (SKPaint paint, SKFont font) GetOrCreate(
        SKColor color,
        float size,
        SKTypeface? typeface = null,
        bool isAntialias = true,
        bool subpixel = true,
        SKFontEdging edging = SKFontEdging.Antialias,
        SKFontHinting hinting = SKFontHinting.Full,
        SKTextAlign textAlign = SKTextAlign.Left)
    {
        var key = new PaintCacheKey(color, size, typeface, isAntialias, subpixel, edging, hinting, textAlign);

        bool isNewEntry = false;
        var entry = _cache.AddOrUpdate(
            key,
            // Add: Create new entry
            k =>
            {
                isNewEntry = true;
                return CreateEntry(k);
            },
            // Update: Increment RefCount
            (k, existing) =>
            {
                var newCount = Interlocked.Increment(ref existing.RefCount);
                LogDebug($"Cache HIT - RefCount: {newCount}, Color: {color}, Size: {size}");
                return existing;
            }
        );

        if (isNewEntry)
        {
            LogDebug($"NEW Entry - RefCount: 1, Color: {color}, Size: {size}, CacheSize: {_cache.Count}");
        }

        return (entry.Paint, entry.Font);
    }

    public void Release(SKPaint paint, SKFont font)
    {
        // Find entry by paint reference
        var entry = _cache.Values.FirstOrDefault(e => ReferenceEquals(e.Paint, paint));
        if (entry == null)
        {
            LogWarning("Release called on paint not in cache!");
            return;
        }

        var newCount = Interlocked.Decrement(ref entry.RefCount);
        LogDebug($"RELEASE - RefCount: {newCount}, Color: {entry.Key.Color}, Size: {entry.Key.Size}");

        if (newCount <= 0)
        {
            lock (_disposeLock)
            {
                // Double-check after acquiring lock
                if (entry.RefCount <= 0 && _cache.TryRemove(entry.Key, out var removed))
                {
                    removed.Dispose();
                    LogDebug($"DISPOSED - Color: {entry.Key.Color}, Size: {entry.Key.Size}, CacheSize: {_cache.Count}");
                }
            }
        }
    }

    public void ClearAll()
    {
        lock (_disposeLock)
        {
            var count = _cache.Count;
            LogDebug($"CLEAR ALL - Disposing {count} entries");

            foreach (var entry in _cache.Values)
            {
                entry.Dispose();
            }
            _cache.Clear();

            LogDebug($"CLEAR ALL Complete - CacheSize: 0");
        }
    }

    private PaintEntry CreateEntry(PaintCacheKey key)
    {
        var paint = new SKPaint
        {
            Color = key.Color,
            IsAntialias = key.IsAntialias,
        };

        var font = new SKFont(key.Typeface ?? SKTypeface.Default)
        {
            Size = key.Size,
            Hinting = key.Hinting,
            Subpixel = key.Subpixel,
            Edging = key.Edging,
        };

        return new PaintEntry
        {
            Paint = paint,
            Font = font,
            Key = key,
            RefCount = 1
        };
    }

    /// <summary>
    /// Determines if a color is "light" (bright) based on luminance.
    /// Light colors on dark backgrounds suffer from blooming and appear thinner.
    /// </summary>
    private static bool IsLightColor(SKColor color)
    {
        // Calculate relative luminance using standard weights (ITU-R BT.709)
        var luminance = (0.299 * color.Red) + (0.587 * color.Green) + (0.114 * color.Blue);

        // Consider color "light" if luminance > 50%
        return luminance > 128;
    }

    private void LogDebug(string message)
    {
        if (_debugBridgeClient == null) return;

        _ = _debugBridgeClient.SendAsync(new DebugMessage
        {
            Type = "log_entry",
            Data = new LogEntryDto
            {
                Timestamp = DateTimeOffset.Now,
                Level = "Debug",
                Message = $"[PaintRegistry] {message}"
            }
        });
    }

    private void LogWarning(string message)
    {
        if (_debugBridgeClient == null) return;

        _ = _debugBridgeClient.SendAsync(new DebugMessage
        {
            Type = "log_entry",
            Data = new LogEntryDto
            {
                Timestamp = DateTimeOffset.Now,
                Level = "Warning",
                Message = $"[PaintRegistry] {message}"
            }
        });
    }

    /// <summary>
    /// Gets current metrics snapshot for debugging.
    /// </summary>
    internal PaintRegistryMetricsDto GetMetrics()
    {
        var entries = _cache.Values.Select(e => new PaintEntryMetricsDto
        {
            Color = $"#{e.Key.Color.Red:X2}{e.Key.Color.Green:X2}{e.Key.Color.Blue:X2}{e.Key.Color.Alpha:X2}",
            Size = e.Key.Size,
            RefCount = e.RefCount,
            Hinting = e.Key.Hinting.ToString(),
            Edging = e.Key.Edging.ToString()
        }).ToList();

        return new PaintRegistryMetricsDto
        {
            Timestamp = DateTimeOffset.Now,
            TotalInstances = _cache.Count,
            TotalRefCount = entries.Sum(e => e.RefCount),
            Entries = entries
        };
    }
}
