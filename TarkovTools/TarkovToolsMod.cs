using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;
using TarkovTools.Services;

namespace TarkovTools;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class TarkovToolsMod(
    ISptLogger<TarkovToolsMod> logger,
    CacheService cacheService,
    SearchService searchService,
    PresetService presetService
    ) : IOnLoad
{
    public const string Version = "0.1.0";
    public const string SptVersion = "~4.0";
    
    public async Task OnLoad()
    {
        try
        {
            logger.Warning("[TarkovTools] is an alpha mod, no support should be granted by SPT support when using this product.");
            logger.Warning("[TarkovTools] Do NOT report bugs to SPT developers while using this mod.");
            
            cacheService.Hydrate();
            searchService.CacheSearchIndexes();
            await presetService.ImportPresets();
            
            logger.Success($"[TarkovTools] {Version} for SPT: {SptVersion} loaded.");
            logger.Success("[TarkovTools] Visit: https://127.0.0.1:6969/tarkovtools to get started");
        }
        catch (Exception e)
        {
            logger.Critical("[TarkovTools] Encountered a critical error and couldn't be loaded", e);
            throw;
        }
    }
}