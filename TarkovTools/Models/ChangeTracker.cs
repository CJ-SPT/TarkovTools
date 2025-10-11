using SPTarkov.Server.Core.Utils;

namespace TarkovTools.Models;

public record ChangeTracker<T> : IChangeTracker
{
    private readonly string? _originalJson;
    private readonly JsonUtil _jsonUtil;
    
    public ChangeTracker(T objectToTrack, JsonUtil jsonUtil)
    {
        _jsonUtil = jsonUtil;
        _originalJson = jsonUtil.Serialize(objectToTrack);
    }
    
    public ChangeTracker(string originalJson, JsonUtil jsonUtil)
    {
        _jsonUtil = jsonUtil;
        _originalJson = originalJson;
    }

    public bool HasChanges(T objectToCheck)
    {
        var currentJson = _jsonUtil.Serialize(objectToCheck);
        return currentJson != _originalJson;
    }

    public bool HasChanges(object objectToCheck)
    {
        if (objectToCheck is T typedObject)
        {
            return HasChanges(typedObject);
        }
        
        return false;
    }
}

public interface IChangeTracker
{
    bool HasChanges(object objectToCheck);
}