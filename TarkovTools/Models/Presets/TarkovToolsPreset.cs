using System.Text.Json.Serialization;

namespace TarkovTools.Models.Presets;

public record TarkovToolsPreset
{
    public required string Name { get; set; }
    public required int Version { get; set; }

    [JsonIgnore] 
    public string RootPath { get; set; } = string.Empty;
    
    public required GlobalPreset GlobalPreset { get; set; }
    public required TraderPreset TraderPreset { get; set; }
}