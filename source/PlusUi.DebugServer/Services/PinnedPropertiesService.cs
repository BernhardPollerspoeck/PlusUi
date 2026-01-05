using System.Text.Json;

namespace PlusUi.DebugServer.Services;

public class PinnedPropertiesService
{
    private readonly string _settingsPath;
    private Dictionary<string, HashSet<string>> _pinnedProperties = new();

    public PinnedPropertiesService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var debugServerPath = Path.Combine(appDataPath, "PlusUi.DebugServer");
        Directory.CreateDirectory(debugServerPath);
        _settingsPath = Path.Combine(debugServerPath, "pinned-properties.json");
        Load();
    }

    public bool IsPinned(string elementType, string propertyPath)
    {
        return _pinnedProperties.TryGetValue(elementType, out var props) && props.Contains(propertyPath);
    }

    public void TogglePin(string elementType, string propertyPath)
    {
        if (!_pinnedProperties.ContainsKey(elementType))
        {
            _pinnedProperties[elementType] = new HashSet<string>();
        }

        var props = _pinnedProperties[elementType];
        if (props.Contains(propertyPath))
        {
            props.Remove(propertyPath);
        }
        else
        {
            props.Add(propertyPath);
        }

        Save();
    }

    public HashSet<string> GetPinnedProperties(string elementType)
    {
        return _pinnedProperties.TryGetValue(elementType, out var props)
            ? props
            : new HashSet<string>();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var dict = JsonSerializer.Deserialize<Dictionary<string, HashSet<string>>>(json);
                if (dict != null)
                {
                    _pinnedProperties = dict;
                }
            }
        }
        catch
        {
            _pinnedProperties = new Dictionary<string, HashSet<string>>();
        }
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_pinnedProperties, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Ignore save errors
        }
    }
}
