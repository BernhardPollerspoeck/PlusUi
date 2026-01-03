namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// Event args for accessibility settings changes.
/// </summary>
public class AccessibilitySettingsChangedEventArgs(string settingName) : EventArgs
{
    /// <summary>
    /// Gets the name of the setting that changed.
    /// </summary>
    public string SettingName { get; } = settingName;
}
