namespace TarkovTools.Models;

public record SearchResult
{
    public required string Id { get; init; }
    public required string LocalizedName { get; init; }
    public required SearchResultType ResultType { get; init; }
}

public enum SearchResultType
{
    Item,
    ItemParent,
    Trader
}