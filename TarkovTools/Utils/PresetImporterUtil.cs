using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
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
            var presetJson = await fileUtil.ReadFileAsync(Path.Combine(directory, "Preset.json"));
            var preset = jsonUtil.Deserialize<TarkovToolsPreset>(presetJson);
            
            if (preset is null)
            {
                logger.Error($"[TarkovTools] Preset: {presetJson} could not be deserialized");
                continue;
            }
            
            preset.RootPath = directory;
            await ImportTraders(preset.TraderPreset, directory);
            
            result.Add(preset.Name, preset);
        }
        
        return result;
    }

    private async Task ImportTraders(TraderPreset preset, string path)
    {
        var tradersDir = Path.Combine(path, "traders");

        if (!Directory.Exists(tradersDir))
        {
            return;
        }
        
        foreach (var file in Directory.GetFiles(tradersDir))
        {
            var text = await fileUtil.ReadFileAsync(file);
            var trader =  jsonUtil.Deserialize<KeyValuePair<MongoId, Trader>>(text);
            
            preset.ModifiedTraders.Add(trader.Key);
            preset.Traders.Add(trader.Key, trader.Value);
        }
    }
}