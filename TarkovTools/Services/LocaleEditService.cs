using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace TarkovTools.Services;

[Injectable]
public class LocaleEditService(
    ISptLogger<LocaleEditService> logger,
    DatabaseService  databaseService
    )
{
    public void AddOrModifyLocale(string lang, string key, string value)
    {
        if (!databaseService.GetLocales().Global.TryGetValue(lang, out var lazyloadedValue))
        {
            return;
        }
        
        lazyloadedValue.AddTransformer(lazyloadedLocaleData =>
        {
            if (!lazyloadedLocaleData?.TryAdd(key, value) ?? false)
            {
                lazyloadedLocaleData[key] = value;
            }
            
            return lazyloadedLocaleData;
        });
    }
}