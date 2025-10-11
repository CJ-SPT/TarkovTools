using System.Diagnostics;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using TarkovTools.Models;

namespace TarkovTools.Services;

[Injectable]
public class SearchService(
    ISptLogger<SearchService> logger,
    DatabaseService databaseService,
    CacheService cacheService,
    LocaleService localeService
    )
{
    private readonly Dictionary<string, string> _localizedItemNames = [];
    private readonly Dictionary<string, string> _localizedItemParentNames = [];
    
    public void CacheSearchIndexes()
    {
        logger.Info("[TarkovTools] Caching search indexes this might take a minute...");
        
        var items = databaseService.GetTables().Templates.Items;

        var sw = Stopwatch.StartNew();
        
        foreach (var item in items)
        {
            _localizedItemNames[item.Key] = GetLocalizedName(item.Key);
        }
        
        logger.Info($"[TarkovTools] Caching {_localizedItemNames.Count} localized items for search indexing took {sw.ElapsedMilliseconds}ms");
        sw.Restart();
        
        var parents = items
            .Select(x => x.Value.Parent);
        
        foreach (var parent in parents)
        {
            _localizedItemParentNames[parent] = GetLocalizedName(parent);
        }
        
        logger.Info($"[TarkovTools] Caching {_localizedItemParentNames.Count} localized parent items for search indexing took {sw.ElapsedMilliseconds}ms");
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
        // Return nothing
        if (string.IsNullOrWhiteSpace(query)) return [];
        
        // Order the dictionary by value in descending order
        var sortedDictionaryDescending = _localizedItemNames.OrderByDescending(pair => pair.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        var matches = sortedDictionaryDescending.Where(x => x.Key.Contains(query))
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
        // Return nothing
        if (string.IsNullOrWhiteSpace(query)) return [];
        
        // Order the dictionary by value in descending order
        var sortedDictionaryDescending = _localizedItemParentNames.OrderByDescending(pair => pair.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        var matches = sortedDictionaryDescending.Where(x => x.Key.Contains(query))
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
        
        logger.Debug($"ItemParent search yielded {results.Count} parents");
        
        return results;
    }
    
    public async Task<List<SearchResult>> SearchTraders(string query)
    {
        return [];
    }

    public string GetLocalizedName(string id)
    {
        if (cacheService.GlobalLocales?.TryGetValue($"{id} Name", out var locale) ?? false)
        {
            return locale;
        }
        
        logger.Debug($"Could not find locale `{id} Name`");
        return string.Empty;
    }

    public string GetLocalizedNickname(string id)
    {
        var found = localeService.GetLocaleDb().TryGetValue($"{id} Nickname", out var locale);
        
        if (!found || locale is null)
        {
            logger.Error($"Could not find locale `{id} Nickname`");
            return string.Empty;
        }
        
        return locale;
    }
    
    private static List<SearchResult> GetSearchResults(Dictionary<string, string>  matchList)
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