using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace TarkovTools.Models.Presets;

public record TraderPreset
{
    public PresetType PresetType => PresetType.Trader;
    
    public required HashSet<MongoId> ModifiedTraders { get; set; }

    [JsonIgnore] 
    public Dictionary<MongoId, Trader> Traders { get; } = [];
}