using SkiaSharp;

namespace PlusUi.core;

internal class PopupConfiguration : IPopupConfiguration
{
    public bool CloseOnBackgroundClick { get; set; }
    public bool CloseOnEscape { get; set; }
    public SKColor BackgroundColor { get; set; } = new SKColor(0, 0, 0, 220);
}
