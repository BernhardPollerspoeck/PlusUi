using PlusUi.core.Services;

namespace PlusUi.core.Tests;

public class FakeClipboardService : IClipboardService
{
    public string? Text { get; set; }

    public string? GetText() => Text;

    public void SetText(string text) => Text = text;
}
