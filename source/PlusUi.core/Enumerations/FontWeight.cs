namespace PlusUi.core;

/// <summary>
/// Specifies the weight (thickness) of a font. Values correspond to OpenType font weight specifications.
/// </summary>
public enum FontWeight
{
    /// <summary>
    /// Thin font weight (100). The lightest available weight.
    /// </summary>
    Thin = 100,

    /// <summary>
    /// Light font weight (300). Lighter than regular but heavier than thin.
    /// </summary>
    Light = 300,

    /// <summary>
    /// Regular font weight (400). The standard weight for normal text.
    /// </summary>
    Regular = 400,

    /// <summary>
    /// Medium font weight (500). Slightly bolder than regular.
    /// </summary>
    Medium = 500,

    /// <summary>
    /// Semi-bold font weight (600). Between medium and bold.
    /// </summary>
    SemiBold = 600,

    /// <summary>
    /// Bold font weight (700). Standard bold weight for emphasis.
    /// </summary>
    Bold = 700,

    /// <summary>
    /// Black font weight (900). The heaviest available weight.
    /// </summary>
    Black = 900
}
