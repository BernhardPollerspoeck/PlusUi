using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Reflection;

namespace PlusUi.core.Services;

public class FontRegistryService : IFontRegistryService
{
    private readonly ConcurrentDictionary<string, SKTypeface> _fontCache = new();
    private bool _initialized = false;
    private readonly object _initLock = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FontRegistryService>? _logger;

    public FontRegistryService(IServiceProvider serviceProvider, ILogger<FontRegistryService>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private void EnsureInitialized()
    {
        if (_initialized) return;

        lock (_initLock)
        {
            if (_initialized) return;

            // Load all registered fonts
            var registrations = _serviceProvider.GetServices<IFontRegistration>();
            foreach (var registration in registrations)
            {
                RegisterFont(registration.ResourcePath, registration.FontFamily, registration.FontWeight, registration.FontStyle);
            }

            _initialized = true;
        }
    }

    public void RegisterFont(Stream fontStream, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal)
    {
        try
        {
            var typeface = SKTypeface.FromStream(fontStream);
            if (typeface != null)
            {
                var key = GetFontKey(fontFamily, fontWeight, fontStyle);
                _fontCache[key] = typeface;
                _logger?.LogDebug("Registered font: {FontFamily} {FontWeight} {FontStyle}", fontFamily, fontWeight, fontStyle);
            }
            else
            {
                _logger?.LogWarning("Failed to create typeface from stream for font: {FontFamily} {FontWeight} {FontStyle}", fontFamily, fontWeight, fontStyle);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error registering font from stream: {FontFamily} {FontWeight} {FontStyle}", fontFamily, fontWeight, fontStyle);
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
            else
            {
                _logger?.LogWarning("Font resource not found: {ResourcePath} in assembly {AssemblyName}", resourcePath, assembly.GetName().Name);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error registering font from resource: {ResourcePath} {FontFamily} {FontWeight} {FontStyle}", resourcePath, fontFamily, fontWeight, fontStyle);
        }
    }

    public SKTypeface? GetTypeface(string? fontFamily, FontWeight fontWeight, FontStyle fontStyle)
    {
        EnsureInitialized();

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
