using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Web;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace TarkovTools;

public record TarkovToolsMetadata : AbstractModMetadata, IModWebMetadata
{
    public override string ModGuid { get; init; } = "com.Cj.TarkovTools";
    public override string Name { get; init; } = "Tarkov Tools Blazor";
    public override string Author { get; init; } = "Cj";
    public override List<string>? Contributors { get; init; }
    public override Version Version { get; init; } = new(TarkovToolsMod.Version);
    public override Range SptVersion { get; init; } = new(TarkovToolsMod.SptVersion);
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}