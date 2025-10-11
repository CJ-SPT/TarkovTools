using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;

namespace TarkovTools.Utils;

[Injectable]
public class PathUtil(ISptLogger<PathUtil> logger)
{
    /// <summary>
    ///     wwwroot/
    /// </summary>
    public string WwwRootPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot");
    
    /// <summary>
    ///     wwwroot/presets
    /// </summary>
    public string SettingsJsonPath => Path.Combine(WwwRootPath, "settings.json");
    
    /// <summary>
    ///     SPT_Data/
    /// </summary>
    public string SptDataPath => Path.Combine(Directory.GetCurrentDirectory(), "SPT_Data");
    
    /// <summary>
    ///     SPT_Data/images/trader/avatar
    /// </summary>
    public string TraderImagePath => Path.Combine(SptDataPath, "images", "trader", "avatar");
    
    /// <summary>
    ///     wwwroot/images
    /// </summary>
    public string ImageCachePath => Path.Combine(WwwRootPath, "images");
    
    /// <summary>
    ///     wwwroot/presets
    /// </summary>
    public string PresetPath => Path.Combine(WwwRootPath, "presets");
}