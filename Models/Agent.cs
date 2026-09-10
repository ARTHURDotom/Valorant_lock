namespace ValorantAutoLock.Models;

public sealed record Agent(
    string Uuid,
    string DisplayName,
    string Description,
    string Role,
    string RoleDisplayName,
    string PortraitUrl,
    string FullPortraitUrl,
    string KillfeedPortraitUrl,
    string BackgroundUrl,
    string BackgroundGradientColor,
    string AssetPath
);

public sealed record AgentRole(
    string Uuid,
    string DisplayName,
    string Description,
    string DisplayIcon
);