namespace PlusUi.core.Services;

public interface IClipboardService
{
    string? GetText();
    void SetText(string text);
}
