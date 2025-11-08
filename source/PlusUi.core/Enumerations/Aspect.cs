namespace PlusUi.core;

/// <summary>
/// Specifies how an image should be scaled to fit within its allocated space.
/// </summary>
public enum Aspect
{
    /// <summary>
    /// Stretches the image to fill the entire space, potentially distorting the aspect ratio.
    /// </summary>
    Fill,

    /// <summary>
    /// Scales the image uniformly to fit within the space while preserving aspect ratio. May leave empty space.
    /// </summary>
    AspectFit,

    /// <summary>
    /// Scales the image uniformly to fill the space while preserving aspect ratio. May crop the image.
    /// </summary>
    AspectFill
}
