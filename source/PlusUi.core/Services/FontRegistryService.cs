using SkiaSharp;
using System.Collections.Concurrent;
using System.Reflection;

namespace PlusUi.core.Services;

public interface IFontRegistryService
{
    void RegisterFont(Stream fontStream, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal);
    void RegisterFont(string resourcePath, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal, Assembly? assembly = null);
    SKTypeface? GetTypeface(string? fontFamily, FontWeight fontWeight, FontStyle fontStyle);
}

public class FontRegistryService : IFontRegistryService
{
    private readonly ConcurrentDictionary<string, SKTypeface> _fontCache = new();

    public void RegisterFont(Stream fontStream, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal)
    {
        try
        {
            var typeface = SKTypeface.FromStream(fontStream);
            if (typeface != null)
            {
                var key = GetFontKey(fontFamily, fontWeight, fontStyle);
                _fontCache[key] = typeface;
            }
        }
        catch
        {
            // Silently fail - font registration is optional
        }
    }

    public void RegisterFont(string resourcePath, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal, Assembly? assembly = null)
    {
        try
        {
            assembly ??= Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                RegisterFont(stream, fontFamily, fontWeight, fontStyle);
            }
        }
        catch
        {
            // Silently fail - font registration is optional
        }
    }

    public SKTypeface? GetTypeface(string? fontFamily, FontWeight fontWeight, FontStyle fontStyle)
    {
        if (string.IsNullOrEmpty(fontFamily))
        {
            return null;
        }

        var key = GetFontKey(fontFamily, fontWeight, fontStyle);
        if (_fontCache.TryGetValue(key, out var typeface))
        {
            return typeface;
        }

        // Try to find closest match
        // 1. Try same family with regular weight
        if (fontWeight != FontWeight.Regular)
        {
            var regularKey = GetFontKey(fontFamily, FontWeight.Regular, fontStyle);
            if (_fontCache.TryGetValue(regularKey, out typeface))
            {
                return typeface;
            }
        }

        // 2. Try same family with normal style
        if (fontStyle != FontStyle.Normal)
        {
            var normalKey = GetFontKey(fontFamily, fontWeight, FontStyle.Normal);
            if (_fontCache.TryGetValue(normalKey, out typeface))
            {
                return typeface;
            }
        }

        // 3. Try same family with regular weight and normal style
        if (fontWeight != FontWeight.Regular || fontStyle != FontStyle.Normal)
        {
            var baseKey = GetFontKey(fontFamily, FontWeight.Regular, FontStyle.Normal);
            if (_fontCache.TryGetValue(baseKey, out typeface))
            {
                return typeface;
            }
        }

        return null;
    }

    private static string GetFontKey(string fontFamily, FontWeight fontWeight, FontStyle fontStyle)
    {
        return $"{fontFamily}|{(int)fontWeight}|{fontStyle}";
    }
}
