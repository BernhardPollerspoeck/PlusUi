namespace PlusUi.core.Services;

/// <summary>
/// Service for launching URLs in the system's default browser
/// </summary>
public interface IUrlLauncherService
{
    /// <summary>
    /// Opens the specified URL in the system's default browser
    /// </summary>
    /// <param name="url">The URL to open</param>
    /// <returns>True if the URL was successfully opened, false otherwise</returns>
    bool OpenUrl(string url);
}
