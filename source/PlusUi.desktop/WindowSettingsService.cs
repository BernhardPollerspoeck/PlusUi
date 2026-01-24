using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlusUi.core;
using System.Text.Json;

namespace PlusUi.desktop;

internal record WindowSettings(int X, int Y, int Width, int Height, bool IsMaximized);

internal class WindowSettingsService(
    IOptions<PlusUiConfiguration> config,
    ILogger<WindowSettingsService> logger)
{
    private const string SettingsFileName = "window-settings.json";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public WindowSettings? Load()
    {
        if (!config.Value.RememberWindowPosition)
            return null;

        var filePath = GetSettingsFilePath();
        if (!File.Exists(filePath))
            return null;

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<WindowSettings>(json);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load window settings from {Path}", filePath);
            return null;
        }
    }

    public void Save(WindowSettings settings)
    {
        if (!config.Value.RememberWindowPosition)
            return;

        var filePath = GetSettingsFilePath();

        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save window settings to {Path}", filePath);
        }
    }

    private string GetSettingsFilePath()
    {
        var appId = config.Value.ApplicationId
            ?? AppDomain.CurrentDomain.FriendlyName
            ?? "PlusUiApp";

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, appId, SettingsFileName);
    }
}
