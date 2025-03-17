using SkiaSharp;

namespace PlusUi.core;

public interface IPopupConfiguration
{
    bool CloseOnBackgroundClick { get; set; }
    bool CloseOnEscape { get; set; }
    SKColor BackgroundColor { get; set; }
}
