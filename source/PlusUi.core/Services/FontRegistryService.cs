using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Reflection;

namespace PlusUi.core.Services;

public class FontRegistryService(IServiceProvider serviceProvider, ILogger<FontRegistryService>? logger = null) : IFontRegistryService
{
    private readonly ConcurrentDictionary<string, SKTypeface> _fontCache = new();
    private SKTypeface? _defaultTypeface;
    private bool _initialized = false;
    private readonly object _initLock = new();

    /// <summary>
    /// The default font family name used by PlusUi.
    /// </summary>
    public const string DefaultFontFamily = "Inter";

    private void EnsureInitialized()
    {
        if (_initialized) return;

        lock (_initLock)
        {
            if (_initialized) return;

            // Load embedded Inter font as default
            LoadDefaultFont();

            // Load all registered fonts
            var registrations = serviceProvider.GetServices<IFontRegistration>();
            foreach (var registration in registrations)
            {
                RegisterFont(registration.ResourcePath, registration.FontFamily, registration.FontWeight, registration.FontStyle);
            }

            _initialized = true;
        }
    }

    private void LoadDefaultFont()
    {
        try
        {
            var assembly = typeof(FontRegistryService).Assembly;
            var resourceName = "PlusUi.core.Fonts.InterVariable.ttf";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                _defaultTypeface = SKTypeface.FromStream(stream);
                if (_defaultTypeface != null)
                {
                    // Register Inter for all common weights (variable font supports all)
                    var key = GetFontKey(DefaultFontFamily, FontWeight.Regular, FontStyle.Normal);
                    _fontCache[key] = _defaultTypeface;
                    logger?.LogDebug("Loaded default font: Inter (Variable)");
                }
            }
            else
            {
                logger?.LogWarning("Default font resource not found: {ResourceName}", resourceName);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error loading default font");
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
                logger?.LogDebug("Registered font: {FontFamily} {FontWeight} {FontStyle}", fontFamily, fontWeight, fontStyle);
            }
            else
            {
                logger?.LogWarning("Failed to create typeface from stream for font: {FontFamily} {FontWeight} {FontStyle}", fontFamily, fontWeight, fontStyle);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error registering font from stream: {FontFamily} {FontWeight} {FontStyle}", fontFamily, fontWeight, fontStyle);
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
                logger?.LogWarning("Font resource not found: {ResourcePath} in assembly {AssemblyName}", resourcePath, assembly.GetName().Name);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error registering font from resource: {ResourcePath} {FontFamily} {FontWeight} {FontStyle}", resourcePath, fontFamily, fontWeight, fontStyle);
        }
    }

    public SKTypeface? GetTypeface(string? fontFamily, FontWeight fontWeight, FontStyle fontStyle)
    {
        EnsureInitialized();

        // If no font family specified, use default
        if (string.IsNullOrEmpty(fontFamily))
        {
            return _defaultTypeface;
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

        // Fallback to default font (Inter) if no match found
        return _defaultTypeface;
    }

    private static string GetFontKey(string fontFamily, FontWeight fontWeight, FontStyle fontStyle)
    {
        return $"{fontFamily}|{(int)fontWeight}|{fontStyle}";
    }
}
