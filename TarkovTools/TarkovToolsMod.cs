using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;
using TarkovTools.Services;

namespace TarkovTools;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class TarkovToolsMod(
    ISptLogger<TarkovToolsMod> logger, 
    CacheService cacheService,
    SearchService searchService
    ) : IOnLoad
{
    public const string Version = "0.1.0";
    public const string SptVersion = "~4.0";
    
    public Task OnLoad()
    {
        try
        {
            cacheService.Hydrate();
            searchService.CacheSearchIndexes();
            
            logger.Info("TarkovTools is loaded");
        }
        catch (Exception e)
        {
            logger.Critical("TarkovTools encountered a critical error and couldn't be loaded", e);
            throw;
        }
        
        return Task.CompletedTask;
    }
}