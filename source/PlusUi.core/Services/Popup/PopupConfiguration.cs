namespace PlusUi.core;

internal class PopupConfiguration : IPopupConfiguration
{
    public bool CloseOnBackgroundClick { get; set; }
    public bool CloseOnEscape { get; set; }
    public Color BackgroundColor { get; set; } = new Color(0, 0, 0, 220);
}
