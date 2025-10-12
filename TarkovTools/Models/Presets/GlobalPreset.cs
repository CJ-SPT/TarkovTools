using System.Text.Json.Serialization;

namespace TarkovTools.Models.Presets;

public record GlobalPreset
{
    public PresetType PresetType => PresetType.Global;

    public bool HasLocaleModifications { get; set; }
    
    [JsonIgnore] 
    public Dictionary<string, Dictionary<string, string>> ModifiedLocales { get; set; } = [];
}