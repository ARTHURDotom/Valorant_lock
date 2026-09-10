using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ValorantAutoLock.Models;

public enum SelectionMode
{
    Global,
    PerMap
}

public enum Language
{
    English,
    French,
    Spanish,
    German
}

public sealed class Settings : INotifyPropertyChanged
{
    private SelectionMode _selectionMode = SelectionMode.Global;
    private string? _globalAgentUuid = "";
    private List<string> _globalAllowedMapUuids = new();
    private Dictionary<string, string> _perMapAgentUuids = new();
    private int _minDelayMs = 300;
    private int _maxDelayMs = 800;
    private bool _selectBeforeLock = true;
    private bool _autoReArm = true;
    private Language _language = Language.English;
    private bool _startMinimized = false;
    private bool _minimizeToTray = true;
    private int _pollIntervalMs = 600;

    public SelectionMode SelectionMode
    {
        get => _selectionMode;
        set { _selectionMode = value; OnPropertyChanged(); }
    }

    public string? GlobalAgentUuid
    {
        get => _globalAgentUuid;
        set { _globalAgentUuid = value; OnPropertyChanged(); }
    }

    public List<string> GlobalAllowedMapUuids
    {
        get => _globalAllowedMapUuids;
        set { _globalAllowedMapUuids = value; OnPropertyChanged(); }
    }

    public Dictionary<string, string> PerMapAgentUuids
    {
        get => _perMapAgentUuids;
        set { _perMapAgentUuids = value; OnPropertyChanged(); }
    }

    public int MinDelayMs
    {
        get => _minDelayMs;
        set { _minDelayMs = Math.Max(0, value); OnPropertyChanged(); }
    }

    public int MaxDelayMs
    {
        get => _maxDelayMs;
        set { _maxDelayMs = Math.Max(MinDelayMs, value); OnPropertyChanged(); }
    }

    public bool SelectBeforeLock
    {
        get => _selectBeforeLock;
        set { _selectBeforeLock = value; OnPropertyChanged(); }
    }

    public bool AutoReArm
    {
        get => _autoReArm;
        set { _autoReArm = value; OnPropertyChanged(); }
    }

    public Language Language
    {
        get => _language;
        set { _language = value; OnPropertyChanged(); }
    }

    public bool StartMinimized
    {
        get => _startMinimized;
        set { _startMinimized = value; OnPropertyChanged(); }
    }

    public bool MinimizeToTray
    {
        get => _minimizeToTray;
        set { _minimizeToTray = value; OnPropertyChanged(); }
    }

    public int PollIntervalMs
    {
        get => _pollIntervalMs;
        set { _pollIntervalMs = Math.Max(100, value); OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}