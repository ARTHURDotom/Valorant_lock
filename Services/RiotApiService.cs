using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ValorantAutoLock.Models;

namespace ValorantAutoLock.Services;

public sealed class RiotApiService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private string? _baseUrl;
    private string? _authHeader;
    private bool _disposed;

    public RiotApiService()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
    }

    public bool Initialize()
    {
        try
        {
            var lockfilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Riot Games", "Riot Client", "Config", "lockfile");

            if (!File.Exists(lockfilePath))
                return false;

            var content = File.ReadAllText(lockfilePath);
            var parts = content.Split(':');
            if (parts.Length < 5)
                return false;

            var port = parts[2];
            var password = parts[3];
            _baseUrl = $"https://127.0.0.1:{port}";
            _authHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(_authHeader);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PregameSession?> GetPregameSessionAsync(CancellationToken ct = default)
    {
        if (_baseUrl == null) return null;

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/product-session/v1/external-sessions", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var sessions = JsonSerializer.Deserialize<List<PregameSession>>(json, _jsonOptions);
            return sessions?.FirstOrDefault(s => s.Phase == "pregame");
        }
        catch
        {
            return null;
        }
    }

    public async Task<PregameMatch?> GetMatchAsync(string matchId, CancellationToken ct = default)
    {
        if (_baseUrl == null) return null;

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/pregame/v1/matches/{matchId}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<PregameMatch>(json, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> SelectAgentAsync(string matchId, string agentId, CancellationToken ct = default)
    {
        if (_baseUrl == null) return false;

        try
        {
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/pregame/v1/matches/{matchId}/select/{agentId}", content, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LockAgentAsync(string matchId, string agentId, CancellationToken ct = default)
    {
        if (_baseUrl == null) return false;

        try
        {
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/pregame/v1/matches/{matchId}/lock/{agentId}", content, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PlayerInfo?> GetPlayerInfoAsync(CancellationToken ct = default)
    {
        if (_baseUrl == null) return null;

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/chat/v4/presences", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var presences = JsonSerializer.Deserialize<PresenceResponse>(json, _jsonOptions);
            var localPlayer = presences?.Presences.FirstOrDefault(p => p.Puid == GetLocalPuid());
            if (localPlayer == null) return null;

            return new PlayerInfo
            {
                Subject = localPlayer.Puid,
                GameName = localPlayer.Private?.GameName ?? "",
                TagLine = localPlayer.Private?.TagLine ?? ""
            };
        }
        catch
        {
            return null;
        }
    }

    private string? GetLocalPuid()
    {
        try
        {
            var lockfilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Riot Games", "Riot Client", "Config", "lockfile");
            var content = File.ReadAllText(lockfilePath);
            var parts = content.Split(':');
            return parts.Length > 1 ? parts[1] : null;
        }
        catch
        {
            return null;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}

public sealed class PregameSession
{
    public string? Subject { get; set; }
    public string? MatchId { get; set; }
    public string? Phase { get; set; }
    public string? QueueId { get; set; }
    public bool IsValid { get; set; }
}

public sealed class PregameMatch
{
    public string? ID { get; set; }
    public string? MapID { get; set; }
    public string? Mode { get; set; }
    public bool IsRanked { get; set; }
    public string? PregameState { get; set; }
    public List<Team>? Teams { get; set; }
    public AllyTeam? AllyTeam { get; set; }
}

public sealed class Team
{
    public string? TeamID { get; set; }
    public List<TeamPlayer>? Players { get; set; }
}

public sealed class TeamPlayer
{
    public string? Subject { get; set; }
    public string? CharacterID { get; set; }
    public string? CharacterSelectionState { get; set; }
}

public sealed class AllyTeam
{
    public List<TeamPlayer>? Players { get; set; }
}

public sealed class PresenceResponse
{
    public List<Presence>? Presences { get; set; }
}

public sealed class Presence
{
    public string? Puid { get; set; }
    public PrivateInfo? Private { get; set; }
}

public sealed class PrivateInfo
{
    public string? GameName { get; set; }
    public string? TagLine { get; set; }
}

public sealed class PlayerInfo
{
    public string? Subject { get; set; }
    public string? GameName { get; set; }
    public string? TagLine { get; set; }
}