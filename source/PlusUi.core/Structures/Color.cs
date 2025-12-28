using SkiaSharp;
using System.Diagnostics;

namespace PlusUi.core;

/// <summary>
/// Represents an ARGB color value.
/// </summary>
[DebuggerDisplay("Color: R={R}, G={G}, B={B}, A={A}")]
public readonly struct Color(byte r, byte g, byte b, byte a = 255) : IEquatable<Color>
{
    /// <summary>
    /// Gets the red component of the color (0-255).
    /// </summary>
    public byte R { get; } = r;

    /// <summary>
    /// Gets the green component of the color (0-255).
    /// </summary>
    public byte G { get; } = g;

    /// <summary>
    /// Gets the blue component of the color (0-255).
    /// </summary>
    public byte B { get; } = b;

    /// <summary>
    /// Gets the alpha component of the color (0-255). 0 is fully transparent, 255 is fully opaque.
    /// </summary>
    public byte A { get; } = a;

    /// <summary>
    /// Alias for A (alpha component).
    /// </summary>
    public byte Alpha => A;

    /// <summary>
    /// Creates a color from ARGB components.
    /// </summary>
    public static Color FromArgb(byte a, byte r, byte g, byte b) => new(r, g, b, a);

    /// <summary>
    /// Creates a color from RGB components with full opacity.
    /// </summary>
    public static Color FromRgb(byte r, byte g, byte b) => new(r, g, b);

    /// <summary>
    /// Creates a new color with the specified alpha value.
    /// </summary>
    public Color WithAlpha(byte alpha) => new(R, G, B, alpha);

    /// <summary>
    /// Gets the red component (0-255). Alias for R.
    /// </summary>
    public byte Red => R;

    /// <summary>
    /// Gets the green component (0-255). Alias for G.
    /// </summary>
    public byte Green => G;

    /// <summary>
    /// Gets the blue component (0-255). Alias for B.
    /// </summary>
    public byte Blue => B;

    /// <summary>
    /// Converts this color to an SKColor for internal SkiaSharp rendering.
    /// </summary>
    internal SKColor ToSkColor() => new(R, G, B, A);

    /// <summary>
    /// Creates a Color from an SKColor.
    /// </summary>
    internal static Color FromSkColor(SKColor skColor) => new(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);

    /// <summary>
    /// Implicit conversion to SKColor for internal use.
    /// </summary>
    public static implicit operator SKColor(Color color) => color.ToSkColor();

    public bool Equals(Color other) => R == other.R && G == other.G && B == other.B && A == other.A;
    public override bool Equals(object? obj) => obj is Color other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(R, G, B, A);

    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public override string ToString() => A == 255
        ? $"#{R:X2}{G:X2}{B:X2}"
        : $"#{A:X2}{R:X2}{G:X2}{B:X2}";
}
