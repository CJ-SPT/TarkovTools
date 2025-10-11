using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;

namespace TarkovTools.Utils;

[Injectable]
public class PathUtil(ISptLogger<PathUtil> logger)
{
    public string WwwRootPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot");
    public string SptDataPath => Path.Combine(Directory.GetCurrentDirectory(), "SPT_Data");
    public string TraderImagePath => Path.Combine(SptDataPath, "images", "trader", "avatar");
    
    public string ImageCachePath => Path.Combine(WwwRootPath, "images");
}