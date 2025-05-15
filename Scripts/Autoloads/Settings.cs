using Godot;
using System.Collections.Generic;

public partial class Settings : Node
{
    // Dictionary to store settings
    public Dictionary<string, object> SettingsData = new Dictionary<string, object>
            {
                { "DisplayMode", 1 },
                { "Vsync", true },
                { "DisplayFps", false },
                { "MaxFps", 9},
                { "MasterVolume", 0 },
                { "MusicVolume", 0},
                { "SfxVolume", 0}
            };

    public void LoadSettings()
    {

    }

    // Set a setting
    public void SetSetting(string key, object value)
    {
        if (SettingsData.ContainsKey(key))
        {
            SettingsData[key] = value;
        }
    }

    // Get a setting
    public T GetSetting<T>(string key)
    {
        if (SettingsData.TryGetValue(key, out var value) && value is T)
        {
            return (T)value;
        }
        GD.PrintErr($"Setting '{key}' not found or type mismatch.");
        return default;
    }
}
