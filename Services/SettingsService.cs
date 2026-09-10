using System;
using System.IO;
using System.Text.Json;
using ValorantAutoLock.Models;

namespace ValorantAutoLock.Services;

public sealed class SettingsService
{
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private Settings? _cachedSettings;

    public SettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "ValorantAutoLock");
        Directory.CreateDirectory(dir);
        _settingsPath = Path.Combine(dir, "settings.json");
    }

    public Settings Load()
    {
        if (_cachedSettings != null) return _cachedSettings;

        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                _cachedSettings = JsonSerializer.Deserialize<Settings>(json, _jsonOptions) ?? new Settings();
                return _cachedSettings;
            }
        }
        catch { }

        _cachedSettings = new Settings();
        return _cachedSettings;
    }

    public void Save(Settings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
        catch { }
    }

    public void Reset()
    {
        _cachedSettings = new Settings();
        Save(_cachedSettings);
    }
}