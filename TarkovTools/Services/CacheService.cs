using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using TarkovTools.Utils;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class CacheService(
    ISptLogger<CacheService> logger,
    DatabaseService  databaseService,
    LocaleService  localeService,
    PathUtil pathUtil
    )
{
    public bool Hydrated { get; private set; }
    
    public Dictionary<string, string>? GlobalLocales { get; private set; }

    public void Hydrate()
    {
        if (Hydrated)
        {
            return;
        }
        
        GlobalLocales = localeService.GetLocaleDb();
        
        if (!Directory.Exists(pathUtil.ImageCachePath))
        {
            Directory.CreateDirectory(pathUtil.ImageCachePath);
        }
        
        CacheStockTraderImages();
        
        Hydrated = true;
    }

    private void CacheStockTraderImages()
    {
        var sptImagePath = pathUtil.SptTraderImagePath;
        var traderImageCachePath = Path.Combine(pathUtil.ImageCachePath, "traders");

        if (!Directory.Exists(traderImageCachePath))
        {
            Directory.CreateDirectory(traderImageCachePath);
        }
        
        foreach (var file in Directory.GetFiles(sptImagePath))
        {
            var fileName = Path.GetFileName(file);
            var cachedImagePath = Path.Combine(traderImageCachePath, fileName);
            
            File.Copy(file, cachedImagePath, true);
        }
    }
}