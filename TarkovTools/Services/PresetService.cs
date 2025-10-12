using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models.Presets;
using TarkovTools.Utils;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class PresetService(
    ISptLogger<PresetService> logger,
    DatabaseService databaseService,
    SettingsService settingsService,
    PresetImporterUtil presetImporterUtil,
    PathUtil pathUtil,
    JsonUtil jsonUtil
    )
{
    public Dictionary<string, TarkovToolsPreset> LoadedPresets { get; private set; } = [];
    
    public bool IsPresetLoaded => SelectedPreset != null;
    public TarkovToolsPreset? SelectedPreset { get; private set; }
    
    private bool _initialized;
    
    public async Task ImportPresets()
    {
        if (_initialized)
        {
            return;
        }
        
        if (!Directory.Exists(pathUtil.PresetPath))
        {
            Directory.CreateDirectory(pathUtil.PresetPath);
            _initialized = true;
            return;
        }

        LoadedPresets = await presetImporterUtil.LoadPresetDefinitions();

        // Check if the default preset is present and apply it
        if (!string.IsNullOrEmpty(settingsService.Settings.SelectedPreset))
        {
            if (LoadedPresets.TryGetValue(settingsService.Settings.SelectedPreset, out var preset))
            {
                SelectedPreset = preset;
                UpdateDatabase(preset);
                
                logger.Success($"[TarkovTools] Preset {preset.Name} applied");
            }
        }
        
        _initialized = true;
    }

    public bool CreatePreset(string name, bool select = false)
    {
        var result = new TarkovToolsPreset
        {
            Name = name,
            Version = 1,
            TraderPreset = new TraderPreset
            {
                Traders = []
            }
        };
        
        if (!LoadedPresets.TryAdd(name, result))
        {
            logger.Error($"[TarkovTools] preset `{name}` already exists");
            return false;
        }
        
        SavePreset(result);
        
        if (select)
        {
            SelectedPreset = result;
        }
            
        return true;

    }
    
    /// <summary>
    ///     Selects the provided preset by name
    /// </summary>
    /// <param name="name">Name of the preset to select</param>
    /// <returns>True if selected</returns>
    public bool SelectPreset(string name)
    {
        if (LoadedPresets.TryGetValue(name, out var preset))
        {
            SelectedPreset = preset;
            UpdateDatabase(preset);
            
            logger.Success($"[TarkovTools] Preset {preset.Name} applied");
            return true;
        }
        
        logger.Warning($"[TarkovTools] Could not find preset with name: {name}");
        return false;
    }
    
    public void SavePresets()
    {
        if (LoadedPresets.Count == 0)
        {
            return;
        }
        
        foreach (var (_, preset) in LoadedPresets)
        {
            SavePreset(preset);
        }
        
        logger.Info($"[TarkovTools] Saved {LoadedPresets.Count} presets");
    }

    public void SavePreset(TarkovToolsPreset preset)
    {
        // Create the preset directory if it doesn't exist
        if (!Directory.Exists(Path.Combine(pathUtil.PresetPath, preset.Name)))
        {
            Directory.CreateDirectory(Path.Combine(pathUtil.PresetPath, preset.Name));
        }
        
        var json =  jsonUtil.Serialize(preset, true);
        File.WriteAllText(Path.Combine(pathUtil.PresetPath, preset.Name, "preset.json"), json);
        
        logger.Success($"[TarkovTools] Preset {preset.Name} saved");
    }
    
    /// <summary>
    ///     Deletes a preset
    /// </summary>
    /// <param name="name">preset name to remove</param>
    /// <returns>True if deleted</returns>
    public bool DeletePreset(string name)
    {
        if (LoadedPresets.Remove(name, out var preset))
        {
            DeletePreset(preset);
        }
        
        return true;
    }
    
    /// <summary>
    ///     Handles removing all references to the deleted preset
    /// </summary>
    /// <param name="preset">preset to delete</param>
    private void DeletePreset(TarkovToolsPreset preset)
    {
        if (SelectedPreset?.Name == preset.Name)
        {
            SelectedPreset = null;
        }
        
        var presetPath = Path.Combine(pathUtil.PresetPath, preset.Name);
        if (Directory.Exists(presetPath))
        {
            Directory.Delete(presetPath, true);
        }
        
        if (settingsService.Settings.SelectedPreset == preset.Name)
        {
            settingsService.Settings.SelectedPreset = string.Empty;
            settingsService.SaveSettings();
            
            logger.Warning($"[TarkovTools] Preset {preset.Name} deleted, please restart the server.");
        }
    }

    private void UpdateDatabase(TarkovToolsPreset preset)
    {
        UpdateTraders(preset);
    }

    private void UpdateTraders(TarkovToolsPreset preset)
    {
        var presetTraders = preset.TraderPreset.Traders;
        var dbTraders = databaseService.GetTraders();
        
        foreach (var (presetId, presetTrader) in presetTraders)
        {
            foreach (var (databaseId, _) in dbTraders)
            {
                if (presetId == databaseId)
                {
                    dbTraders[presetId] = presetTrader;
                }
            }
        }
    }
}