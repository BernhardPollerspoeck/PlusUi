namespace PlusUi.core.Services;

internal interface IFontRegistration
{
    string ResourcePath { get; }
    string FontFamily { get; }
    FontWeight FontWeight { get; }
    FontStyle FontStyle { get; }
}
