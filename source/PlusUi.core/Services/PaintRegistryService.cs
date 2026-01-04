using System.Collections.Concurrent;
using SkiaSharp;

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

    public int CacheCount => _cache.Count;

    public (SKPaint paint, SKFont font) GetOrCreate(
        SKColor color,
        float size,
        SKTypeface? typeface = null,
        bool isAntialias = true,
        bool subpixel = true,
        SKFontEdging edging = SKFontEdging.SubpixelAntialias,
        SKFontHinting hinting = SKFontHinting.Full,
        SKTextAlign textAlign = SKTextAlign.Left)
    {
        var key = new PaintCacheKey(color, size, typeface, isAntialias, subpixel, edging, hinting, textAlign);

        var entry = _cache.AddOrUpdate(
            key,
            // Add: Create new entry
            k => CreateEntry(k),
            // Update: Increment RefCount
            (k, existing) =>
            {
                Interlocked.Increment(ref existing.RefCount);
                return existing;
            }
        );

        return (entry.Paint, entry.Font);
    }

    public void Release(SKPaint paint, SKFont font)
    {
        // Find entry by paint reference
        var entry = _cache.Values.FirstOrDefault(e => ReferenceEquals(e.Paint, paint));
        if (entry == null) return;

        var newCount = Interlocked.Decrement(ref entry.RefCount);
        if (newCount <= 0)
        {
            lock (_disposeLock)
            {
                // Double-check after acquiring lock
                if (entry.RefCount <= 0 && _cache.TryRemove(entry.Key, out var removed))
                {
                    removed.Dispose();
                }
            }
        }
    }

    public void ClearAll()
    {
        lock (_disposeLock)
        {
            foreach (var entry in _cache.Values)
            {
                entry.Dispose();
            }
            _cache.Clear();
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
}
