using PlusUi.core.Enumerations;

namespace PlusUi.Headless.Services;

/// <summary>
/// Zentrales Interface für Headless-Betrieb von PlusUi-Anwendungen.
/// Ermöglicht programmatische Input-Simulation und Frame-Capturing.
/// Frame-Größe und Format werden beim Startup konfiguriert.
/// </summary>
public interface IPlusUiHeadlessService
{
    /// <summary>
    /// Rendert den aktuellen Frame on-demand und gibt ihn als Bilddaten zurück.
    /// Führt intern Measure, Arrange und Render aus.
    /// </summary>
    /// <returns>Frame als Byte-Array im konfigurierten Format (PNG/JPEG/etc.)</returns>
    Task<byte[]> GetCurrentFrameAsync();

    /// <summary>
    /// Bewegt die Maus zur angegebenen Position.
    /// </summary>
    void MouseMove(float x, float y);

    /// <summary>
    /// Erzeugt MouseDown-Event an der aktuellen Mausposition.
    /// </summary>
    void MouseDown();

    /// <summary>
    /// Erzeugt MouseUp-Event an der aktuellen Mausposition.
    /// </summary>
    void MouseUp();

    /// <summary>
    /// Erzeugt Mouse-Wheel-Event an der aktuellen Mausposition.
    /// </summary>
    void MouseWheel(float deltaX, float deltaY);

    /// <summary>
    /// Simuliert Tastendruck (Enter, Backspace, etc.).
    /// </summary>
    void KeyPress(PlusKey key);

    /// <summary>
    /// Simuliert Zeichen-Eingabe (für Text-Input).
    /// </summary>
    void CharInput(char c);
}
