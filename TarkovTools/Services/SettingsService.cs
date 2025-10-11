using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models.Editor;
using TarkovTools.Utils;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class SettingsService
{
    private bool _initialized;
    public required TarkovToolsSettings Settings { get; set; }

    private readonly ISptLogger<SettingsService> _logger;
    private readonly PathUtil _pathUtil;
    private readonly JsonUtil _jsonUtil;
    
    public SettingsService(
        ISptLogger<SettingsService> logger, 
        PathUtil pathUtil,
        JsonUtil jsonUtil
    )
    {
        _logger = logger;
        _pathUtil = pathUtil;
        _jsonUtil = jsonUtil;
        
        if (!File.Exists(_pathUtil.SettingsJsonPath))
        {
            SaveSettings(true);
            
            _initialized = true;
            return;
        }
        
        Settings = LoadSettings() ?? throw new NullReferenceException("[TarkovTools] Settings is null when constructing SettingsService");
    }
    
    public void SaveSettings(bool createNew = false)
    {
        if (createNew)
        {
            Settings = CreateSettings();
        }
        
        if (!File.Exists(_pathUtil.SettingsJsonPath))
        {
            File.Create(_pathUtil.SettingsJsonPath).Close();
        }
        
        var json = _jsonUtil.Serialize(Settings, true);
        File.WriteAllText(_pathUtil.SettingsJsonPath, json);
        
        _logger.Info("[TarkovTools] Settings file created.");
    }

    public TarkovToolsSettings? LoadSettings()
    {
        if (File.Exists(_pathUtil.SettingsJsonPath))
        {
            try
            {
                var text = File.ReadAllText(_pathUtil.SettingsJsonPath);
                var tmpSettings = _jsonUtil.Deserialize<TarkovToolsSettings>(text);
                if (tmpSettings is null)
                {
                    _logger.Error("[TarkovTools] Settings file could not be loaded.");
                    return null;
                };
                
                Settings = tmpSettings;
                _initialized = true;
                _logger.Info("[TarkovTools] Settings file loaded.");
                return tmpSettings;
            }
            catch (Exception e)
            {
                _logger.Warning("[TarkovTools] Error loading settings. Most likely out of date settings file.");
                _logger.Warning("[TarkovTools] Creating a backup and new creating default settings...");
                
                File.Copy(_pathUtil.SettingsJsonPath, $"{_pathUtil.SettingsJsonPath}.bak");
                
                SaveSettings(true);
                return Settings;
            }
        }
        
        _logger.Error("[TarkovTools] Settings file not found when attempting to load settings.");
        return null;
    }

    private static TarkovToolsSettings CreateSettings()
    {
        return new TarkovToolsSettings
        {
            SelectedPreset = ""
        };
    }
}