namespace PlusUi.core.Services;

internal class FontRegistration : IFontRegistration
{
    public required string ResourcePath { get; init; }
    public required string FontFamily { get; init; }
    public FontWeight FontWeight { get; init; }
    public FontStyle FontStyle { get; init; }
}
