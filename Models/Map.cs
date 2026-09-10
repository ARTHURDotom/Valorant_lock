namespace ValorantAutoLock.Models;

public sealed record Map(
    string Uuid,
    string DisplayName,
    string NarrativeDescription,
    string TacticalDescription,
    string Coordinates,
    string DisplayIcon,
    string ListViewIcon,
    string ListViewIconTall,
    string Splash,
    string AssetPath,
    string MapUrl,
    bool IsRankedMap
);