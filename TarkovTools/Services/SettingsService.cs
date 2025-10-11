using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models.Editor;
using TarkovTools.Utils;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class SettingsService(
    ISptLogger<SettingsService> logger, 
    PathUtil pathUtil,
    JsonUtil jsonUtil
    )
{
    private bool _initialized;
    public TarkovToolsSettings? Settings { get; private set; }
    
    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        if (!File.Exists(pathUtil.SettingsJsonPath))
        {
            SaveSettings(true);
            
            _initialized = true;
            return;
        }
        
        LoadSettings();
    }
    
    public void SaveSettings(bool createNew = false)
    {
        if (Settings is null && !createNew)
        {
            logger.Error("[TarkovTools] Settings null when attempting to save settings.");
            return;
        }

        if (createNew)
        {
            Settings = CreateSettings();
        }
        
        if (!File.Exists(pathUtil.SettingsJsonPath))
        {
            File.Create(pathUtil.SettingsJsonPath).Close();
        }
        
        var json = jsonUtil.Serialize(Settings, true);
        File.WriteAllText(pathUtil.SettingsJsonPath, json);
        
        logger.Info("[TarkovTools] Settings file created.");
    }

    public void LoadSettings()
    {
        if (File.Exists(pathUtil.SettingsJsonPath))
        {
            try
            {
                var text = File.ReadAllText(pathUtil.SettingsJsonPath);
                Settings = jsonUtil.Deserialize<TarkovToolsSettings>(text);
            
                _initialized = true;
            
                logger.Info("[TarkovTools] Settings file loaded.");
                return;
            }
            catch (Exception e)
            {
                logger.Warning("[TarkovTools] Error loading settings. Most likely out of date settings file.");
                logger.Warning("[TarkovTools] Creating a backup and new creating default settings...");
                
                File.Copy(pathUtil.SettingsJsonPath, $"{pathUtil.SettingsJsonPath}.bak");
                
                SaveSettings(true);
                return;
            }
        }
        
        logger.Error("[TarkovTools] Settings file not found when attempting to load settings.");
    }

    private static TarkovToolsSettings CreateSettings()
    {
        return new TarkovToolsSettings
        {
            SelectedPreset = ""
        };
    }
}