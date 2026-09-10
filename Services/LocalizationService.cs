using System.Collections.Generic;
using System.Globalization;
using ValorantAutoLock.Models;

namespace ValorantAutoLock.Services;

public sealed class LocalizationService
{
    private readonly Dictionary<Language, Dictionary<string, string>> _translations = new();
    private Language _currentLanguage = Language.English;

    public LocalizationService()
    {
        InitializeTranslations();
    }

    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set => _currentLanguage = value;
    }

    public string Get(string key)
    {
        if (_translations.TryGetValue(_currentLanguage, out var lang) && lang.TryGetValue(key, out var value))
            return value;
        if (_translations.TryGetValue(Language.English, out var en) && en.TryGetValue(key, out value))
            return value;
        return key;
    }

    public CultureInfo GetCulture() => new CultureInfo(_currentLanguage.ToString());

    private void InitializeTranslations()
    {
        _translations[Language.English] = new Dictionary<string, string>
        {
            ["AppTitle"] = "Valorant Auto Lock",
            ["StatusWaiting"] = "Waiting for Valorant...",
            ["StatusDetecting"] = "Detecting match...",
            ["StatusMapFound"] = "Map found: {0}",
            ["StatusAgentLocked"] = "Agent locked: {0}",
            ["StatusError"] = "Error: {0}",
            ["StatusArmed"] = "Armed and ready",
            ["StatusDisarmed"] = "Disarmed",
            ["BtnArm"] = "Arm",
            ["BtnDisarm"] = "Disarm",
            ["BtnSettings"] = "Settings",
            ["TabDashboard"] = "Dashboard",
            ["TabSettings"] = "Settings",
            ["TabGeneral"] = "General",
            ["TabAgents"] = "Agents",
            ["TabMaps"] = "Maps",
            ["TabAdvanced"] = "Advanced",
            ["SelectionMode"] = "Selection Mode",
            ["ModeGlobal"] = "Global Agent",
            ["ModePerMap"] = "Per Map",
            ["GlobalAgent"] = "Default Agent",
            ["AgentPerMap"] = "Agent per Map",
            ["NoAgent"] = "No agent (manual pick)",
            ["MinDelay"] = "Min Delay (ms)",
            ["MaxDelay"] = "Max Delay (ms)",
            ["SelectBeforeLock"] = "Select before lock",
            ["AutoReArm"] = "Auto re-arm after match",
            ["PollInterval"] = "Poll Interval (ms)",
            ["Language"] = "Language",
            ["StartMinimized"] = "Start minimized",
            ["MinimizeToTray"] = "Minimize to tray",
            ["Save"] = "Save",
            ["Cancel"] = "Cancel",
            ["Map"] = "Map",
            ["Agent"] = "Agent",
            ["Clear"] = "Clear",
            ["Ready"] = "Ready",
            ["Locking"] = "Locking...",
            ["AlreadyLocked"] = "Already locked",
            ["SkippedMap"] = "Skipped (no agent set for this map)",
            ["LockFailed"] = "Lock failed",
            ["SessionLocks"] = "Locks this session",
            ["StatusTitle"] = "Status",
            ["CurrentMapTitle"] = "Current Map",
            ["LockedAgentTitle"] = "Locked Agent",
            ["ArmedStatusTitle"] = "Armed Status"
        };

        _translations[Language.French] = new Dictionary<string, string>
        {
            ["AppTitle"] = "Valorant Auto Lock",
            ["StatusWaiting"] = "En attente de Valorant...",
            ["StatusDetecting"] = "Detection de la partie...",
            ["StatusMapFound"] = "Map trouvee : {0}",
            ["StatusAgentLocked"] = "Agent verrouille : {0}",
            ["StatusError"] = "Erreur : {0}",
            ["StatusArmed"] = "Arme et pret",
            ["StatusDisarmed"] = "Desarme",
            ["BtnArm"] = "Armer",
            ["BtnDisarm"] = "Desarmer",
            ["BtnSettings"] = "Parametres",
            ["TabDashboard"] = "Tableau de bord",
            ["TabSettings"] = "Parametres",
            ["TabGeneral"] = "General",
            ["TabAgents"] = "Agents",
            ["TabMaps"] = "Maps",
            ["TabAdvanced"] = "Avance",
            ["SelectionMode"] = "Mode de selection",
            ["ModeGlobal"] = "Agent global",
            ["ModePerMap"] = "Par map",
            ["GlobalAgent"] = "Agent par defaut",
            ["AgentPerMap"] = "Agent par map",
            ["NoAgent"] = "Aucun agent (choix manuel)",
            ["MinDelay"] = "Delai min (ms)",
            ["MaxDelay"] = "Delai max (ms)",
            ["SelectBeforeLock"] = "Selectionner avant de verrouiller",
            ["AutoReArm"] = "Rearmer automatiquement",
            ["PollInterval"] = "Intervalle de sondage (ms)",
            ["Language"] = "Langue",
            ["StartMinimized"] = "Demarrer minimise",
            ["MinimizeToTray"] = "Minimiser dans la barre",
            ["Save"] = "Enregistrer",
            ["Cancel"] = "Annuler",
            ["Map"] = "Map",
            ["Agent"] = "Agent",
            ["Clear"] = "Effacer",
            ["Ready"] = "Pret",
            ["Locking"] = "Verrouillage...",
            ["AlreadyLocked"] = "Deja verrouille",
            ["SkippedMap"] = "Ignore (aucun agent defini pour cette map)",
            ["LockFailed"] = "Echec du verrouillage",
            ["SessionLocks"] = "Verrouillages cette session",
            ["StatusTitle"] = "Statut",
            ["CurrentMapTitle"] = "Map Actuelle",
            ["LockedAgentTitle"] = "Agent Verrouille",
            ["ArmedStatusTitle"] = "Statut Arme"
        };

        _translations[Language.Spanish] = new Dictionary<string, string>
        {
            ["AppTitle"] = "Valorant Auto Lock",
            ["StatusWaiting"] = "Esperando a Valorant...",
            ["StatusDetecting"] = "Detectando partida...",
            ["StatusMapFound"] = "Mapa encontrado: {0}",
            ["StatusAgentLocked"] = "Agente bloqueado: {0}",
            ["StatusError"] = "Error: {0}",
            ["StatusArmed"] = "Armado y listo",
            ["StatusDisarmed"] = "Desarmado",
            ["BtnArm"] = "Armar",
            ["BtnDisarm"] = "Desarmar",
            ["BtnSettings"] = "Ajustes",
            ["TabDashboard"] = "Panel",
            ["TabSettings"] = "Ajustes",
            ["TabGeneral"] = "General",
            ["TabAgents"] = "Agentes",
            ["TabMaps"] = "Mapas",
            ["TabAdvanced"] = "Avanzado",
            ["SelectionMode"] = "Modo de seleccion",
            ["ModeGlobal"] = "Agente global",
            ["ModePerMap"] = "Por mapa",
            ["GlobalAgent"] = "Agente por defecto",
            ["AgentPerMap"] = "Agente por mapa",
            ["NoAgent"] = "Sin agente (seleccion manual)",
            ["MinDelay"] = "Retraso min (ms)",
            ["MaxDelay"] = "Retraso max (ms)",
            ["SelectBeforeLock"] = "Seleccionar antes de bloquear",
            ["AutoReArm"] = "Re-armar automaticamente",
            ["PollInterval"] = "Intervalo de sondeo (ms)",
            ["Language"] = "Idioma",
            ["StartMinimized"] = "Iniciar minimizado",
            ["MinimizeToTray"] = "Minimizar a bandeja",
            ["Save"] = "Guardar",
            ["Cancel"] = "Cancelar",
            ["Map"] = "Mapa",
            ["Agent"] = "Agente",
            ["Clear"] = "Limpiar",
            ["Ready"] = "Listo",
            ["Locking"] = "Bloqueando...",
            ["AlreadyLocked"] = "Ya bloqueado",
            ["SkippedMap"] = "Omitido (sin agente para este mapa)",
            ["LockFailed"] = "Fallo al bloquear",
            ["SessionLocks"] = "Bloqueos esta sesion",
            ["StatusTitle"] = "Estado",
            ["CurrentMapTitle"] = "Mapa Actual",
            ["LockedAgentTitle"] = "Agente Bloqueado",
            ["ArmedStatusTitle"] = "Estado Armado"
        };

        _translations[Language.German] = new Dictionary<string, string>
        {
            ["AppTitle"] = "Valorant Auto Lock",
            ["StatusWaiting"] = "Warte auf Valorant...",
            ["StatusDetecting"] = "Erkenne Match...",
            ["StatusMapFound"] = "Map gefunden: {0}",
            ["StatusAgentLocked"] = "Agent gesperrt: {0}",
            ["StatusError"] = "Fehler: {0}",
            ["StatusArmed"] = "Scharf und bereit",
            ["StatusDisarmed"] = "Unscharf",
            ["BtnArm"] = "Scharfschalten",
            ["BtnDisarm"] = "Entscharfen",
            ["BtnSettings"] = "Einstellungen",
            ["TabDashboard"] = "Dashboard",
            ["TabSettings"] = "Einstellungen",
            ["TabGeneral"] = "Allgemein",
            ["TabAgents"] = "Agenten",
            ["TabMaps"] = "Maps",
            ["TabAdvanced"] = "Erweitert",
            ["SelectionMode"] = "Selektion-Modus",
            ["ModeGlobal"] = "Globaler Agent",
            ["ModePerMap"] = "Pro Map",
            ["GlobalAgent"] = "Standard-Agent",
            ["AgentPerMap"] = "Agent pro Map",
            ["NoAgent"] = "Kein Agent (manuelle Wahl)",
            ["MinDelay"] = "Min. Verzogerung (ms)",
            ["MaxDelay"] = "Max. Verzogerung (ms)",
            ["SelectBeforeLock"] = "Vor dem Sperren auswahlen",
            ["AutoReArm"] = "Automatisch neu scharfschalten",
            ["PollInterval"] = "Abfrageintervall (ms)",
            ["Language"] = "Sprache",
            ["StartMinimized"] = "Minimiert starten",
            ["MinimizeToTray"] = "In Taskleiste minimieren",
            ["Save"] = "Speichern",
            ["Cancel"] = "Abbrechen",
            ["Map"] = "Map",
            ["Agent"] = "Agent",
            ["Clear"] = "Loschen",
            ["Ready"] = "Bereit",
            ["Locking"] = "Sperre...",
            ["AlreadyLocked"] = "Bereits gesperrt",
            ["SkippedMap"] = "Ubersprungen (kein Agent fur diese Map)",
            ["LockFailed"] = "Sperren fehlgeschlagen",
            ["SessionLocks"] = "Sperren in dieser Sitzung",
            ["StatusTitle"] = "Status",
            ["CurrentMapTitle"] = "Aktuelle Map",
            ["LockedAgentTitle"] = "Gesperrter Agent",
            ["ArmedStatusTitle"] = "Scharf-Status"
        };
    }
}