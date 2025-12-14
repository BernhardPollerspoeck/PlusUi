using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Service for managing overlay elements that render above the page but below popups.
/// </summary>
public class OverlayService : IOverlayService
{
    private readonly List<UiElement> _overlays = new();

    internal IReadOnlyList<UiElement> Overlays => _overlays;

    public void RegisterOverlay(UiElement element)
    {
        if (!_overlays.Contains(element))
        {
            _overlays.Add(element);
        }
    }

    public void UnregisterOverlay(UiElement element)
    {
        _overlays.Remove(element);
    }

    internal void RenderOverlays(SKCanvas canvas)
    {
        foreach (var overlay in _overlays)
        {
            overlay.Render(canvas);
        }
    }
}
