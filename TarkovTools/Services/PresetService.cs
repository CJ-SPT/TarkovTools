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
    private readonly List<TarkovToolsPreset> _loadedPresets = [];
    private TarkovToolsPreset? _selectedPreset;
    
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
            var preset = _loadedPresets.FirstOrDefault(x => x.Name == settingsService.Settings.SelectedPreset);
            _selectedPreset = preset;
        }
        
        _initialized = true;
    }

    public TarkovToolsPreset CreatePreset(string name, bool select = false)
    {
        var result = new TarkovToolsPreset
        {
            Name = name,
        };

        if (select)
        {
            _selectedPreset = result;
        }
        
        return result;
    }
    
    /// <summary>
    ///     Loads all presets from disk
    /// </summary>
    public void LoadPresets()
    {
        logger.Info("[TarkovTools] Loading presets");
        
        var directories =  Directory.GetDirectories(pathUtil.PresetPath);
        if (directories.Length == 0)
        {
            logger.Warning("[TarkovTools] No presets found, consider creating or installing one to start taking advantage of the mods features");
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
            
            _loadedPresets.Add(preset);
        }
        
        logger.Info($"[TarkovTools] Loaded {directories.Length} presets");
    }

    public void SavePresets()
    {
        if (_loadedPresets.Count == 0)
        {
            return;
        }
        
        foreach (var preset in _loadedPresets)
        {
            SavePreset(preset);
        }
        
        logger.Info($"[TarkovTools] Saved {_loadedPresets.Count} presets");
    }

    public void SavePreset(TarkovToolsPreset preset)
    {
        // Create the preset directory if it doesn't exist
        if (!Directory.Exists(Path.Combine(pathUtil.PresetPath, preset.Name)))
        {
            Directory.CreateDirectory(Path.Combine(pathUtil.PresetPath, preset.Name));
        }
        
        var json =  jsonUtil.Serialize(preset);
        File.WriteAllText(Path.Combine(pathUtil.PresetPath, preset.Name, "preset.json"), json);
    }
}