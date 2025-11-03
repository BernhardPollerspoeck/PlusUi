namespace PlusUi.core.Services;

/// <summary>
/// Represents the platform type
/// </summary>
public enum PlatformType
{
    Desktop,
    Android,
    iOS,
    Web
}

/// <summary>
/// Service providing platform-specific information
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Gets the current platform type
    /// </summary>
    PlatformType Platform { get; }

    /// <summary>
    /// Gets the current window/screen size
    /// </summary>
    Size WindowSize { get; }

    /// <summary>
    /// Gets the display density (scale factor)
    /// </summary>
    float DisplayDensity { get; }

    /// <summary>
    /// Opens a URL in the system's default browser
    /// </summary>
    /// <param name="url">The URL to open</param>
    /// <returns>True if successful, false otherwise</returns>
    bool OpenUrl(string url);
}
