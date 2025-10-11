using System.Diagnostics;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using TarkovTools.Models;

namespace TarkovTools.Services;

[Injectable(InjectionType.Singleton)]
public class SearchService(
    ISptLogger<SearchService> logger,
    DatabaseService databaseService,
    CacheService cacheService,
    LocaleService localeService,
    ItemHelper itemHelper
    )
{
    private readonly Dictionary<MongoId, string> _localizedItemNames = [];
    private readonly Dictionary<MongoId, string> _localizedItemParentNames = [];

    private bool _hydrated;
    
    public void CacheSearchIndexes()
    {
        if (_hydrated)
        {
            return;
        }
        
        logger.Info("[TarkovTools] Caching search indexes this might take a minute...");
        
        var items = databaseService.GetTables().Templates.Items
            .Where(item => itemHelper.IsValidItem(item.Key));

        var sw = Stopwatch.StartNew();
        
        foreach (var (id, _) in items)
        {
            _localizedItemNames[id] = GetLocalizedName(id);
        }
        
        logger.Info($"[TarkovTools] Caching {_localizedItemNames.Count} localized items for search indexing took {sw.ElapsedMilliseconds}ms");
        sw.Restart();
        
        foreach (var parent in items.Select(x => x.Value.Parent))
        {
            _localizedItemParentNames[parent] = GetLocalizedName(parent);
        }
        
        logger.Info($"[TarkovTools] Caching {_localizedItemParentNames.Count} localized parent items for search indexing took {sw.ElapsedMilliseconds}ms");
        
        _hydrated = true;
    }
    
    public async Task<IEnumerable<SearchResult>> SearchItems(string value, CancellationToken token)
    {
        return await SearchItems(value.ToLower());
    }
    
    public async Task<IEnumerable<SearchResult>> SearchItemParents(string value, CancellationToken token)
    {
        return await SearchItemParents(value.ToLower());
    }
    
    public async Task<List<SearchResult>> SearchItems(string query)
    {
        logger.Debug($"[TarkovTools] Item Search: {query}");
        
        // Return nothing
        if (string.IsNullOrWhiteSpace(query)) return [];
        
        // Order the dictionary by value in descending order
        var sortedDictionaryDescending = _localizedItemNames.OrderByDescending(pair => pair.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        logger.Debug($"(sortedDictionaryDescending) count {_localizedItemNames.Count}");
        
        var matches = sortedDictionaryDescending.Where(x => x.Value.Contains(query))
            .ToDictionary(x => x.Key, x => x.Value);
        
        foreach (var kvp in sortedDictionaryDescending.Where(x => 
                     x.Value.Contains(query, StringComparison.CurrentCultureIgnoreCase) || 
                     x.Value.StartsWith(query, StringComparison.CurrentCultureIgnoreCase) ||
                     x.Value.EndsWith(query, StringComparison.CurrentCultureIgnoreCase) ||
                     x.Value.Equals(query, StringComparison.CurrentCultureIgnoreCase)))
        {
            matches[kvp.Key] = kvp.Value;
        }
        
        var results = GetSearchResults(matches);
        
        logger.Debug($"Item search yielded {results.Count} items");
        
        return results;
    }
    
    public async Task<List<SearchResult>> SearchItemParents(string query)
    {
        logger.Debug($"[TarkovTools] Parent Item Search: {query}");
        
        // Return nothing
        if (string.IsNullOrWhiteSpace(query)) return [];
        
        // Order the dictionary by value in descending order
        var sortedDictionaryDescending = _localizedItemParentNames.OrderByDescending(pair => pair.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        logger.Debug($"(sortedDictionaryDescending) count {sortedDictionaryDescending.Count}");
        
        var matches = sortedDictionaryDescending.Where(x => x.Value.Contains(query))
            .ToDictionary(x => x.Key, x => x.Value);
        
        foreach (var kvp in sortedDictionaryDescending.Where(x => 
                     x.Value.Contains(query, StringComparison.CurrentCultureIgnoreCase) || 
                     x.Value.StartsWith(query, StringComparison.CurrentCultureIgnoreCase) ||
                     x.Value.EndsWith(query, StringComparison.CurrentCultureIgnoreCase) ||
                     x.Value.Equals(query, StringComparison.CurrentCultureIgnoreCase)))
        {
            matches[kvp.Key] = kvp.Value;
        }

        var results = GetSearchResults(matches);
        
        logger.Debug($"Item parent search yielded {results.Count} parents");
        
        return results;
    }
    
    public async Task<List<SearchResult>> SearchTraders(string query)
    {
        return [];
    }

    public string GetLocalizedName(MongoId id)
    {
        if (cacheService.GlobalLocales?.TryGetValue($"{id.ToString()} Name", out var locale) ?? false)
        {
            return locale;
        }
        
        return string.Empty;
    }

    public string GetLocalizedNickname(MongoId id)
    {
        if (cacheService.GlobalLocales?.TryGetValue($"{id.ToString()} Nickname", out var locale) ?? false)
        {
            return locale;
        }
        
        return string.Empty;
    }
    
    private static List<SearchResult> GetSearchResults(Dictionary<MongoId, string>  matchList)
    {
        var results = new List<SearchResult>();
        foreach (var match in matchList)
        {
            results.Add(new SearchResult
            {
                Id = match.Key,
                LocalizedName = match.Value,
                ResultType = SearchResultType.Item,
            });
        }
        
        return results;
    }
}