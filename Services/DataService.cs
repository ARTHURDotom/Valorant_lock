using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ValorantAutoLock.Models;

namespace ValorantAutoLock.Services;

public sealed class DataService
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private List<Agent>? _agents;
    private List<Map>? _maps;

    public async Task<List<Agent>> GetAgentsAsync()
    {
        if (_agents != null) return _agents;

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("ValorantAutoLock.Data.agents.json");
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<AgentData>(json, _jsonOptions);
                _agents = data?.Data ?? GetFallbackAgents();
                return _agents;
            }
        }
        catch { }

        _agents = GetFallbackAgents();
        return _agents;
    }

    public async Task<List<Map>> GetMapsAsync()
    {
        if (_maps != null) return _maps;

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("ValorantAutoLock.Data.maps.json");
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<MapData>(json, _jsonOptions);
                _maps = data?.Data ?? GetFallbackMaps();
                return _maps;
            }
        }
        catch { }

        _maps = GetFallbackMaps();
        return _maps;
    }

    private List<Agent> GetFallbackAgents()
    {
        return new List<Agent>
        {
            new("add6443a-41bd-e414-f636-536f34c8a8f9", "Brimstone", "", "Controller", "Controller", "", "", "", "", "", ""),
            new("8e253930-4c05-31dd-1b6c-968525494517", "Viper", "", "Controller", "Controller", "", "", "", "", "", ""),
            new("569fdd95-4d10-43ab-ca70-79becc718b46", "Omen", "", "Controller", "Controller", "", "", "", "", "", ""),
            new("117ed9e3-49f3-6512-3ccf-0cada7e3823b", "Killjoy", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("4d7d9e30-4c8b-318c-9f8f-889969e3c6d1", "Cypher", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("320b2a48-4d9b-a075-30f1-1f93a9b638fa", "Sova", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("569fdd95-4d10-43ab-ca70-79becc718b46", "Sage", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("eb93336a-449b-9c1b-0a54-a891f7921d69", "Phoenix", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("cc8b64c8-4b25-4ff9-6e7f-37b4da43d235", "Jett", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("a3bfb853-43b2-7238-a4f1-ad90e9e46bcc", "Reyna", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("f94c3b30-42be-e959-889c-5aa313dba261", "Raze", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("5f8d3a7f-467b-97f3-063c-067f8a7b9f1e", "Breach", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("6f2a04ca-43e0-be17-7f36-b3908627744d", "Skye", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("7f94d92c-4234-0a36-9646-3a8732b5e745", "Yoru", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("41fb69c1-4189-7b37-f117-bcf1e96f1b4d", "Astra", "", "Controller", "Controller", "", "", "", "", "", ""),
            new("601dbbe7-43ce-be57-2a40-4abd24953621", "KAY/O", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("22697a3d-45bf-8dd7-4fec-84a9e28c69d7", "Chamber", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("bb2a4828-46eb-8cd9-e765-15d48c57e8e1", "Neon", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("dade69b4-4f5a-8528-247b-219e5a1facd6", "Fade", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("95b78ed5-4637-86d9-7e41-7c9b7c8a8d8e", "Harbor", "", "Controller", "Controller", "", "", "", "", "", ""),
            new("e370fa57-4757-3604-3648-499e1f642d3f", "Gekko", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("cc8b64c8-4b25-4ff9-6e7f-37b4da43d235", "Deadlock", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("0f797a1a-4d01-4b3f-8601-7e7e3b4d8c1e", "Iso", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("1dbf3b6e-4d31-4f6b-8d62-2f5b4c8a9e7f", "Clove", "", "Controller", "Controller", "", "", "", "", "", ""),
            new("2e7b8f4d-4c1a-4e8f-9b2d-5e7a8f9b1c3d", "Vyse", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("3f8c9e1b-4a2d-4f7e-8c3b-6d9e0f1a2b4c", "Tejo", "", "Initiator", "Initiator", "", "", "", "", "", ""),
            new("4a9d0f2c-4b3e-4c8f-9d4e-7f0a1b2c3d5e", "Waylay", "", "Duelist", "Duelist", "", "", "", "", "", ""),
            new("5b0e1f3d-4c4f-4d9e-8e5f-8a1b2c3d4e6f", "Veto", "", "Sentinel", "Sentinel", "", "", "", "", "", ""),
            new("6c1f2a4e-4d5a-4e0f-9f6a-9b2c3d4e5f7a", "Miks", "", "Controller", "Controller", "", "", "", "", "", "")
        };
    }

    private List<Map> GetFallbackMaps()
    {
        return new List<Map>
        {
            new("7eaecc1b-4337-bbf6-6ab9-3b8e9d6e5f4a", "Ascent", "", "", "", "", "", "", "", "", "/Game/Maps/Ascent", true),
            new("2f92e3b4-4c5d-4e6f-8a7b-9c0d1e2f3a4b", "Bind", "", "", "", "", "", "", "", "", "/Game/Maps/Bind", false),
            new("3a8b4c9d-4e5f-4a6b-8c7d-9e0f1a2b3c4d", "Haven", "", "", "", "", "", "", "", "", "/Game/Maps/Haven", true),
            new("4b9c5d0e-4f6a-4b7c-8d8e-9f0a1b2c3d4e", "Icebox", "", "", "", "", "", "", "", "", "/Game/Maps/Icebox", false),
            new("5c0d6e1f-4a7b-4c8d-8e9f-9a0b1c2d3e4f", "Breeze", "", "", "", "", "", "", "", "", "/Game/Maps/Breeze", false),
            new("6d1e7f2a-4b8c-4d9e-8f0a-9b1c2d3e4f5a", "Fracture", "", "", "", "", "", "", "", "", "/Game/Maps/Fracture", false),
            new("7e2f8a3b-4c9d-4e0f-8a1b-9c2d3e4f5a6b", "Pearl", "", "", "", "", "", "", "", "", "/Game/Maps/Pearl", false),
            new("8f3a9b4c-4d0e-4f1a-8b2c-9d3e4f5a6b7c", "Lotus", "", "", "", "", "", "", "", "", "/Game/Maps/Lotus", true),
            new("9a4b0c5d-4e1f-4a2b-8c3d-9e4f5a6b7c8d", "Split", "", "", "", "", "", "", "", "", "/Game/Maps/Split", true),
            new("ab5c1d6e-4f2a-4b3c-8d4e-9f5a6b7c8d9e", "Sunset", "", "", "", "", "", "", "", "", "/Game/Maps/Sunset", true),
            new("bc6d2e7f-4a3b-4c4d-8e5f-9a6b7c8d9e0f", "Abyss", "", "", "", "", "", "", "", "", "/Game/Maps/Abyss", true),
            new("cd7e3f8a-4b4c-4d5e-8f6a-9b7c8d9e0f1a", "Summit", "", "", "", "", "", "", "", "", "/Game/Maps/Summit", false)
        };
    }
}

public sealed class AgentData
{
    public List<Agent>? Data { get; set; }
}

public sealed class MapData
{
    public List<Map>? Data { get; set; }
}