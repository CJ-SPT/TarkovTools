using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models.Presets;
using TarkovTools.Utils;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class PresetService(
    ISptLogger<PresetService> logger,
    SettingsService settingsService,
    PathUtil pathUtil,
    JsonUtil jsonUtil
    )
{
    public List<TarkovToolsPreset> LoadedPresets { get; } = [];
    public TarkovToolsPreset? SelectedPreset { get; private set; }
    
    private bool _initialized;
    
    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }
        
        if (!Directory.Exists(pathUtil.PresetPath))
        {
            Directory.CreateDirectory(pathUtil.PresetPath);
        }
        
        LoadPresets();

        if (!string.IsNullOrEmpty(settingsService.Settings?.SelectedPreset))
        {
            var preset = LoadedPresets.FirstOrDefault(x => x.Name == settingsService.Settings.SelectedPreset);
            SelectedPreset = preset;
        }
        
        _initialized = true;
    }

    public TarkovToolsPreset CreatePreset(string name, bool select = false)
    {
        var result = new TarkovToolsPreset
        {
            Name = name,
        };
        
        LoadedPresets.Add(result);
        SavePreset(result);
        
        if (select)
        {
            SelectedPreset = result;
        }
        
        return result;
    }

    /// <summary>
    ///     Selects the provided preset
    /// </summary>
    /// <param name="preset">preset to select</param>
    public void SelectPreset(TarkovToolsPreset preset)
    {
        SelectedPreset = preset;
    }

    /// <summary>
    ///     Selects the provided preset by name
    /// </summary>
    /// <param name="name">Name of the preset to select</param>
    /// <returns>True if selected</returns>
    public bool SelectPreset(string name)
    {
        var preset = LoadedPresets.FirstOrDefault(x => x.Name == name);
        if (preset is null)
        {
            logger.Warning($"Could not find preset with name {name}");
            return false;
        }
        
        SelectedPreset = preset;
        return true;
    }
    
    /// <summary>
    ///     Loads all presets from disk
    /// </summary>
    public void LoadPresets()
    {
        var directories =  Directory.GetDirectories(pathUtil.PresetPath);
        if (directories.Length == 0)
        {
            logger.Warning("[TarkovTools] No presets found, consider creating or installing one. Mods features will be limited until you do so.");
            return;
        }
        
        foreach (var directory in directories)
        {
            var presetJson = File.ReadAllText(Path.Combine(directory, "Preset.json"));
            var preset = jsonUtil.Deserialize<TarkovToolsPreset>(presetJson);

            if (preset is null)
            {
                logger.Error($"[TarkovTools] Preset: {presetJson} could not be deserialized");
                continue;
            }
            
            LoadedPresets.Add(preset);
        }

        // Set the selected preset
        switch (LoadedPresets.Count)
        {
            // No loaded presets
            case 0:
                logger.Error("[TarkovTools] No presets found after loading...");
                return;
            // Use the first preset loaded when the selected preset doesn't exist
            case 1 when string.IsNullOrEmpty(settingsService.Settings.SelectedPreset):
                SelectedPreset = LoadedPresets[0];
                settingsService.Settings.SelectedPreset = SelectedPreset.Name;
                break;
            // Set the users selected preset
            default:
                if (!string.IsNullOrEmpty(settingsService.Settings.SelectedPreset))
                {
                    SelectPreset(settingsService.Settings.SelectedPreset);
                }
                break;
        }
        
        logger.Info($"[TarkovTools] Loaded {LoadedPresets.Count} presets");
    }

    public void SavePresets()
    {
        if (LoadedPresets.Count == 0)
        {
            return;
        }
        
        foreach (var preset in LoadedPresets)
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
    }

    public void DeletePreset(TarkovToolsPreset preset)
    {
        LoadedPresets.Remove(preset);
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
        }
    }
    
    public bool DeletePreset(string name)
    {
        var preset = LoadedPresets.FirstOrDefault(x => x.Name == name);
        if (preset is null)
        {
            return false;
        }
        
        DeletePreset(preset);
        return true;
    }
}