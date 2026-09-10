using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValorantAutoLock.Models;
using ValorantAutoLock.Services;

namespace ValorantAutoLock.Services;

public sealed class AutoLockService : IDisposable
{
    private readonly RiotApiService _riotApi;
    private readonly DataService _dataService;
    private readonly SettingsService _settingsService;
    private readonly LocalizationService _loc;
    private readonly Random _random = new();
    private readonly CancellationTokenSource _cts = new();
    private Task? _monitorTask;
    private bool _isArmed;
    private bool _disposed;
    private string? _lastMatchId;
    private string? _currentMapName;
    private int _locksThisSession;

    public bool IsArmed => _isArmed;
    public int LocksThisSession => _locksThisSession;

    public event Action<string>? StatusChanged;
    public event Action<string, string>? MapDetected;
    public event Action<string>? AgentLocked;
    public event Action<string>? ErrorOccurred;
    public event Action<bool>? ArmedStateChanged;
    public event Action<int>? LocksCountChanged;

    public AutoLockService(RiotApiService riotApi, DataService dataService, SettingsService settingsService, LocalizationService loc)
    {
        _riotApi = riotApi;
        _dataService = dataService;
        _settingsService = settingsService;
        _loc = loc;
    }

    public void Arm()
    {
        if (_isArmed) return;
        if (!_riotApi.Initialize())
        {
            ErrorOccurred?.Invoke(_loc.Get("StatusError").Replace("{0}", "Valorant not running or lockfile not found"));
            return;
        }

        _isArmed = true;
        _cts.Cancel();
        _cts.Dispose();
        var newCts = new CancellationTokenSource();
        _monitorTask = MonitorLoopAsync(newCts.Token);
        ArmedStateChanged?.Invoke(true);
        StatusChanged?.Invoke(_loc.Get("StatusArmed"));
    }

    public void Disarm()
    {
        if (!_isArmed) return;
        _isArmed = false;
        _cts.Cancel();
        ArmedStateChanged?.Invoke(false);
        StatusChanged?.Invoke(_loc.Get("StatusDisarmed"));
    }

    private async Task MonitorLoopAsync(CancellationToken ct)
    {
        var settings = _settingsService.Load();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var session = await _riotApi.GetPregameSessionAsync(ct);
                if (session != null && !string.IsNullOrEmpty(session.MatchId) && session.MatchId != _lastMatchId)
                {
                    _lastMatchId = session.MatchId;
                    await HandleMatchAsync(session.MatchId, settings, ct);
                }
                else if (session == null)
                {
                    _lastMatchId = null;
                    StatusChanged?.Invoke(_loc.Get("StatusWaiting"));
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(_loc.Get("StatusError").Replace("{0}", ex.Message));
            }

            await Task.Delay(settings.PollIntervalMs, ct);
        }
    }

    private async Task HandleMatchAsync(string matchId, Settings settings, CancellationToken ct)
    {
        StatusChanged?.Invoke(_loc.Get("StatusDetecting"));

        var match = await _riotApi.GetMatchAsync(matchId, ct);
        if (match == null || string.IsNullOrEmpty(match.MapID))
        {
            ErrorOccurred?.Invoke(_loc.Get("StatusError").Replace("{0}", "Failed to get match info"));
            return;
        }

        var maps = await _dataService.GetMapsAsync();
        var map = maps.FirstOrDefault(m => m.Uuid == match.MapID);
        _currentMapName = map?.DisplayName ?? match.MapID;

        MapDetected?.Invoke(match.MapID, _currentMapName);
        StatusChanged?.Invoke(string.Format(_loc.Get("StatusMapFound"), _currentMapName));

        if (match.PregameState != "character_select_active")
        {
            StatusChanged?.Invoke(_loc.Get("AlreadyLocked"));
            return;
        }

        var agentUuid = GetAgentForMap(match.MapID, settings);
        if (string.IsNullOrEmpty(agentUuid))
        {
            StatusChanged?.Invoke(_loc.Get("SkippedMap"));
            return;
        }

        var agents = await _dataService.GetAgentsAsync();
        var agent = agents.FirstOrDefault(a => a.Uuid == agentUuid);
        var agentName = agent?.DisplayName ?? agentUuid;

        var delay = _random.Next(settings.MinDelayMs, settings.MaxDelayMs + 1);
        await Task.Delay(delay, ct);

        StatusChanged?.Invoke(_loc.Get("Locking"));

        bool success = false;
        if (settings.SelectBeforeLock)
        {
            var selected = await _riotApi.SelectAgentAsync(matchId, agentUuid, ct);
            if (!selected)
            {
                ErrorOccurred?.Invoke(_loc.Get("LockFailed"));
                return;
            }
            await Task.Delay(100, ct);
        }

        success = await _riotApi.LockAgentAsync(matchId, agentUuid, ct);

        if (success)
        {
            _locksThisSession++;
            LocksCountChanged?.Invoke(_locksThisSession);
            AgentLocked?.Invoke(agentName);
            StatusChanged?.Invoke(string.Format(_loc.Get("StatusAgentLocked"), agentName));
        }
        else
        {
            ErrorOccurred?.Invoke(_loc.Get("LockFailed"));
        }
    }

    private string? GetAgentForMap(string mapId, Settings settings)
    {
        return settings.SelectionMode switch
        {
            SelectionMode.Global => settings.GlobalAllowedMapUuids.Count == 0 || settings.GlobalAllowedMapUuids.Contains(mapId)
                ? settings.GlobalAgentUuid
                : null,
            SelectionMode.PerMap => settings.PerMapAgentUuids.TryGetValue(mapId, out var agent) ? agent : null,
            _ => null
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Disarm();
            _cts.Dispose();
            _riotApi.Dispose();
            _disposed = true;
        }
    }
}