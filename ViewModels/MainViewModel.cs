using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ValorantAutoLock.Models;
using ValorantAutoLock.Services;

namespace ValorantAutoLock.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly AutoLockService _autoLock;
    private readonly SettingsService _settingsService;
    private readonly LocalizationService _loc;
    private readonly DataService _dataService;
    private Settings _settings;
    private bool _disposed;

    public MainViewModel(AutoLockService autoLock, SettingsService settingsService, LocalizationService loc, DataService dataService)
    {
        _autoLock = autoLock;
        _settingsService = settingsService;
        _loc = loc;
        _dataService = dataService;
        _settings = _settingsService.Load();

        _autoLock.StatusChanged += OnStatusChanged;
        _autoLock.MapDetected += OnMapDetected;
        _autoLock.AgentLocked += OnAgentLocked;
        _autoLock.ErrorOccurred += OnErrorOccurred;
        _autoLock.ArmedStateChanged += OnArmedStateChanged;
        _autoLock.LocksCountChanged += OnLocksCountChanged;

        ArmCommand = new RelayCommand(_ => Arm(), _ => !IsArmed);
        DisarmCommand = new RelayCommand(_ => Disarm(), _ => IsArmed);
        OpenSettingsCommand = new RelayCommand(_ => SettingsRequested?.Invoke(this, EventArgs.Empty));
        LanguageChangedCommand = new RelayCommand(p => ChangeLanguage((Language)p!));
        SaveCommand = new RelayCommand(_ => SaveSettings());
        ShowDashboardCommand = new RelayCommand(_ => ShowDashboardRequested?.Invoke(this, EventArgs.Empty));

        LoadDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? SettingsRequested;
    public event EventHandler? ShowDashboardRequested;

    public ICommand ArmCommand { get; }
    public ICommand DisarmCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand LanguageChangedCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ShowDashboardCommand { get; }

    private string _status = "Waiting for Valorant...";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private string? _currentMap;
    public string? CurrentMap
    {
        get => _currentMap;
        set { _currentMap = value; OnPropertyChanged(); }
    }

    private string? _currentAgent;
    public string? CurrentAgent
    {
        get => _currentAgent;
        set { _currentAgent = value; OnPropertyChanged(); }
    }

    private bool _isArmed;
    public bool IsArmed
    {
        get => _isArmed;
        set { _isArmed = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanArm)); ((RelayCommand)ArmCommand).RaiseCanExecuteChanged(); ((RelayCommand)DisarmCommand).RaiseCanExecuteChanged(); }
    }

    public bool CanArm => IsArmed == false;

    private string? _lastError;
    public string? LastError
    {
        get => _lastError;
        set { _lastError = value; OnPropertyChanged(); }
    }

    private int _locksThisSession;
    public int LocksThisSession
    {
        get => _locksThisSession;
        set { _locksThisSession = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Agent> Agents { get; } = new();
    public ObservableCollection<Map> Maps { get; } = new();
    public ObservableCollection<MapAgentViewModel> MapAgents { get; } = new();

    public SelectionMode SelectionMode
    {
        get => _settings.SelectionMode;
        set { _settings.SelectionMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsGlobalMode)); OnPropertyChanged(nameof(IsPerMapMode)); }
    }

    public bool IsGlobalMode => SelectionMode == SelectionMode.Global;
    public bool IsPerMapMode => SelectionMode == SelectionMode.PerMap;

    public string? GlobalAgentUuid
    {
        get => _settings.GlobalAgentUuid;
        set { _settings.GlobalAgentUuid = value; OnPropertyChanged(); }
    }

    public int MinDelayMs
    {
        get => _settings.MinDelayMs;
        set { _settings.MinDelayMs = Math.Max(0, value); OnPropertyChanged(); }
    }

    public int MaxDelayMs
    {
        get => _settings.MaxDelayMs;
        set { _settings.MaxDelayMs = Math.Max(MinDelayMs, value); OnPropertyChanged(); }
    }

    public bool SelectBeforeLock
    {
        get => _settings.SelectBeforeLock;
        set { _settings.SelectBeforeLock = value; OnPropertyChanged(); }
    }

    public bool AutoReArm
    {
        get => _settings.AutoReArm;
        set { _settings.AutoReArm = value; OnPropertyChanged(); }
    }

    public int PollIntervalMs
    {
        get => _settings.PollIntervalMs;
        set { _settings.PollIntervalMs = Math.Max(100, value); OnPropertyChanged(); }
    }

    public Language CurrentLanguage
    {
        get => _settings.Language;
        set { _settings.Language = value; _loc.CurrentLanguage = value; OnPropertyChanged(); RefreshLocalization(); }
    }

    public bool StartMinimized
    {
        get => _settings.StartMinimized;
        set { _settings.StartMinimized = value; OnPropertyChanged(); }
    }

    public bool MinimizeToTray
    {
        get => _settings.MinimizeToTray;
        set { _settings.MinimizeToTray = value; OnPropertyChanged(); }
    }

    public IEnumerable<Language> Languages => Enum.GetValues<Language>();

    private void OnStatusChanged(string status) => Status = status;

    private void OnMapDetected(string mapId, string mapName) => CurrentMap = mapName;

    private void OnAgentLocked(string agentName) => CurrentAgent = agentName;

    private void OnErrorOccurred(string error) => LastError = error;

    private void OnArmedStateChanged(bool isArmed) => IsArmed = isArmed;

    private void OnLocksCountChanged(int count) => LocksThisSession = count;

    private async Task LoadDataAsync()
    {
        var agents = await _dataService.GetAgentsAsync();
        var maps = await _dataService.GetMapsAsync();

        foreach (var a in agents.OrderBy(a => a.DisplayName))
            Agents.Add(a);

        foreach (var m in maps.OrderBy(m => m.DisplayName))
            Maps.Add(m);

        RefreshMapAgents();
    }

    private void RefreshMapAgents()
    {
        MapAgents.Clear();
        foreach (var map in Maps)
        {
            _settings.PerMapAgentUuids.TryGetValue(map.Uuid, out var agentUuid);
            MapAgents.Add(new MapAgentViewModel(map, agentUuid, Agents, _loc, OnMapAgentChanged));
        }
    }

    private void OnMapAgentChanged(string mapUuid, string? agentUuid)
    {
        if (string.IsNullOrEmpty(agentUuid))
            _settings.PerMapAgentUuids.Remove(mapUuid);
        else
            _settings.PerMapAgentUuids[mapUuid] = agentUuid;
        OnPropertyChanged(nameof(MapAgents));
    }

    public void SaveSettings()
    {
        _settingsService.Save(_settings);
    }

    public void Arm() => _autoLock.Arm();
    public void Disarm() => _autoLock.Disarm();

    private void ChangeLanguage(Language lang)
    {
        CurrentLanguage = lang;
        RefreshLocalization();
    }

    private void RefreshLocalization()
    {
        Status = _loc.Get(IsArmed ? "StatusArmed" : "StatusWaiting");
        OnPropertyChanged(nameof(SelectionMode));
        OnPropertyChanged(nameof(IsGlobalMode));
        OnPropertyChanged(nameof(IsPerMapMode));
        foreach (var vm in MapAgents)
            vm.RefreshLocalization();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _autoLock.Dispose();
            _disposed = true;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class MapAgentViewModel : INotifyPropertyChanged
{
    private readonly Map _map;
    private readonly ObservableCollection<Agent> _agents;
    private readonly LocalizationService _loc;
    private readonly Action<string, string?> _onChanged;
    private string? _selectedAgentUuid;

    public MapAgentViewModel(Map map, string? selectedAgentUuid, ObservableCollection<Agent> agents, LocalizationService loc, Action<string, string?> onChanged)
    {
        _map = map;
        _agents = agents;
        _loc = loc;
        _onChanged = onChanged;
        _selectedAgentUuid = selectedAgentUuid;
        ClearCommand = new RelayCommand(_ => SelectedAgentUuid = null);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string MapName => _map.DisplayName;
    public string MapUuid => _map.Uuid;

    public string? SelectedAgentUuid
    {
        get => _selectedAgentUuid;
        set { _selectedAgentUuid = value; OnPropertyChanged(); _onChanged(_map.Uuid, value); }
    }

    public Agent? SelectedAgent => _agents.FirstOrDefault(a => a.Uuid == _selectedAgentUuid);

    public IEnumerable<Agent> AvailableAgents => new[] { (Agent?)null }.Concat(_agents.Where(a => a != null)!);

    public ICommand ClearCommand { get; }

    public void RefreshLocalization() => OnPropertyChanged(nameof(MapName));

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _execute(parameter);
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}