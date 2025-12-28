using SkiaSharp;
using System.Reflection;

namespace PlusUi.core.Services;

public interface IFontRegistryService
{
    void RegisterFont(Stream fontStream, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal);
    void RegisterFont(string resourcePath, string fontFamily, FontWeight fontWeight = FontWeight.Regular, FontStyle fontStyle = FontStyle.Normal, Assembly? assembly = null);
    SKTypeface? GetTypeface(string? fontFamily, FontWeight fontWeight, FontStyle fontStyle);
}
