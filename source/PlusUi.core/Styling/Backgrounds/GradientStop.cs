namespace PlusUi.core;

/// <summary>
/// Represents a single color stop in the gradient.
/// Immutable record type.
/// </summary>
/// <param name="Color">The color at this stop</param>
/// <param name="Position">The position of the stop (0-1)</param>
public record GradientStop(Color Color, float Position);
