using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models.Presets;
using Path = System.IO.Path;

namespace TarkovTools.Utils;

[Injectable]
public class PresetImporterUtil(
    ISptLogger<PresetImporterUtil> logger,
    FileUtil fileUtil,
    JsonUtil jsonUtil,
    PathUtil pathUtil
    )
{
    public async Task<Dictionary<string, TarkovToolsPreset>> LoadPresetDefinitions()
    {
        var directories =  Directory.GetDirectories(pathUtil.PresetPath);
        if (directories.Length == 0)
        {
            logger.Warning("[TarkovTools] No presets found, consider creating or installing one. Mods features will be limited until you do so.");
            return [];
        }
        
        var result = new Dictionary<string, TarkovToolsPreset>();
        foreach (var directory in directories)
        {
            var presetJson = await File.ReadAllTextAsync(Path.Combine(directory, "Preset.json"));
            var preset = jsonUtil.Deserialize<TarkovToolsPreset>(presetJson);
            
            if (preset is null)
            {
                logger.Error($"[TarkovTools] Preset: {presetJson} could not be deserialized");
                continue;
            }
            
            preset.RootPath = directory;
            result.Add(preset.Name, preset);
        }
        
        return result;
    }

    /// <summary>
    ///     Loads all the relevant data for a preset
    /// </summary>
    /// <param name="preset">Preset to load the data for</param>
    public void LoadPreset(TarkovToolsPreset preset)
    {
        ImportTraders(preset);
    }
    
    private void ImportTraders(TarkovToolsPreset preset)
    {
        foreach (var id in preset.TraderPreset.AlteredTraders)
        {
            var traderPath = Path.Combine(preset.RootPath, "traders", $"{id.ToString()}.json");
            var trader = Import<Trader>(traderPath);
            preset.TraderPreset.PresetTraders.Add(trader);
        }
    }

    private T Import<T>(string path)
    {
        var text = fileUtil.ReadFile(path);
        var t = jsonUtil.Deserialize<T>(text);

        return t ?? throw new Exception($"[TarkovTools] {path} could not be deserialized");
    }
}