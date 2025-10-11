using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TarkovTools.Utils;

[Injectable]
public class PathUtil
{
    #region ROOT

    /// <summary>
    ///     root/
    /// </summary>
    public string RootModPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    
    /// <summary>
    ///     root/settings.json
    /// </summary>
    public string SettingsJsonPath => Path.Combine(RootModPath, "settings.json");

    /// <summary>
    ///     root/presets
    /// </summary>
    public string PresetPath => Path.Combine(RootModPath, "presets");
    
    #endregion
    
    #region WWWROOT

    /// <summary>
    ///      root/wwwroot/
    /// </summary>
    public string WwwRootPath => Path.Combine(RootModPath, "wwwroot");
    
    /// <summary>
    ///     root/wwwroot/images
    /// </summary>
    public string ImageCachePath => Path.Combine(WwwRootPath, "images");

    #endregion
    
    #region SPT

    /// <summary>
    ///     SPT_ROOT/
    /// </summary>
    public string SptRoot => Directory.GetCurrentDirectory();
    
    /// <summary>
    ///     SPT_ROOT/SPT_Data/
    /// </summary>
    public string SptDataPath => Path.Combine(SptRoot, "SPT_Data");
    
    /// <summary>
    ///     SPT_ROOT/SPT_Data/images/trader/avatar
    /// </summary>
    public string SptTraderImagePath => Path.Combine(SptDataPath, "images", "trader", "avatar");

    #endregion
}