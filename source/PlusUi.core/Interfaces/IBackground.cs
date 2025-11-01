using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Base interface for all background types (solid colors, gradients, patterns, etc.).
/// Enables extensible background system for all UI elements.
/// </summary>
public interface IBackground
{
    /// <summary>
    /// Renders the background to the canvas with the specified bounds and corner radius.
    /// </summary>
    /// <param name="canvas">The canvas to render to</param>
    /// <param name="bounds">The rectangular bounds to fill</param>
    /// <param name="cornerRadius">Corner radius for rounded backgrounds (0 for sharp corners)</param>
    void Render(SKCanvas canvas, SKRect bounds, float cornerRadius);
}
