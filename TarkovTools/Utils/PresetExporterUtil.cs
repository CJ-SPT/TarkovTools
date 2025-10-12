using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models.Presets;

namespace TarkovTools.Utils;

[Injectable]
public class PresetExporterUtil(
    ISptLogger<PresetExporterUtil> logger,
    PathUtil pathUtil,
    FileUtil fileUtil,
    JsonUtil jsonUtil
    )
{
    public async Task ExportPreset(TarkovToolsPreset preset)
    {
        var presetDir = Path.Combine(pathUtil.PresetPath, preset.Name);
        
        // Create the preset directory if it doesn't exist
        if (!Directory.Exists(presetDir))
        {
            Directory.CreateDirectory(Path.Combine(pathUtil.PresetPath, preset.Name));
        }

        try
        {
            var json =  jsonUtil.Serialize(preset, true);
            await File.WriteAllTextAsync(Path.Combine(pathUtil.PresetPath, preset.Name, "preset.json"), json);
        
            await ExportTraders(preset.TraderPreset, presetDir);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task ExportTraders(TraderPreset preset, string path)
    {
        var tradersDir = Path.Combine(path, "traders");

        if (!Directory.Exists(tradersDir))
        {
            Directory.CreateDirectory(tradersDir);
        }
        
        foreach (var trader in preset.Traders)
        {
            var json =  jsonUtil.Serialize(trader, true);
            await File.WriteAllTextAsync(Path.Combine(tradersDir, $"{trader.Key.ToString()}.json"), json);
        }
    }
}