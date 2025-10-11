using SPTarkov.Server.Core.Models.Common;

namespace TarkovTools.Models.Presets;

public record TarkovToolsPreset
{
    public required string Name { get; set; }
    
    public List<MongoId>? TradersModified { get; set; }
}