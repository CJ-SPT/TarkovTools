using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using TarkovTools.Models;
using LogLevel = SPTarkov.Server.Core.Models.Spt.Logging.LogLevel;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class ChangeTrackerService(
    ISptLogger<ChangeTrackerService> logger,
    JsonUtil jsonUtil
    )
{
    /// <summary>
    ///     Key is the type name, value is the change tracker
    /// </summary>
    public Dictionary<string, IChangeTracker> TrackedChanges { get; private set; } = [];

    /// <summary>
    ///     Track an object, if it already exists untrack it and start fresh
    /// </summary>
    /// <param name="obj">object to track</param>
    /// <param name="id">id to assign to store it</param>
    /// <typeparam name="T">type of object</typeparam>
    public void TrackObject<T>(T obj, string id)
    {
        TrackedChanges.Remove(id);
        var tracker = new ChangeTracker<T>(obj, jsonUtil);
        TrackedChanges.Add(id, tracker);
    }
    
    /// <summary>
    ///     Does the provided object have changes?
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <param name="id">Id assigned when the object was tracked</param>
    /// <typeparam name="T">Type of object</typeparam>
    /// <returns>True if it was changed</returns>
    public bool HasChanges<T>(T obj, string id)
    {
        return TrackedChanges.TryGetValue(id, out var tracker) && tracker.HasChanges(obj);
    }
    
    /// <summary>
    ///     Does the provided object have changes?
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <param name="id">Id assigned when the object was tracked</param>
    /// <returns>True if it was changed</returns>
    public bool HasChanges(object obj, string id)
    {
        return TrackedChanges.TryGetValue(id, out var tracker) && tracker.HasChanges(obj);
    }
}