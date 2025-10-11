using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace TarkovTools.Extensions;

public static class ItemExtensions
{
    /// <summary>
    ///     Finds all children items for the provided parent item (Non-Recursive)
    /// </summary>
    /// <param name="rootItem">Item to find the children for</param>
    /// <param name="itemsToSearch">List of items to search</param>
    /// <returns>List of child items</returns>
    public static List<Item> FindChildrenItems(this Item rootItem, List<Item> itemsToSearch)
    {
        var result = new List<Item>();
        
        foreach (var item in itemsToSearch)
        {
            // This is our item, it's not a child
            if (item.ParentId != rootItem.Id) continue;
            
            // Add our item
            result.Add(item);
        }
        
        return result;
    }
    
    /// <summary>
    ///     Returns a flat list of all children from a root object
    /// </summary>
    /// <param name="item">Item to get the children for</param>
    /// <param name="itemsToSearch">Items to search</param>
    /// <returns>Recursive list of children</returns>
    public static List<Item> FindChildrenRecursively(this Item item, List<Item> itemsToSearch)
    {
        var result = new List<Item>();
        
        foreach (var child in FindChildrenItems(item, itemsToSearch))
        {
            result.Add(child);
            result.AddRange(FindChildrenRecursively(child, itemsToSearch));
        }
        
        return result;
    }
    
    /// <summary>
    ///     Is this item a child of or recursively a child of this object
    /// </summary>
    /// <param name="rootItem">Item to check against</param>
    /// <param name="item">Item to check</param>
    /// <param name="itemsToSearch">Items to search</param>
    /// <returns>True if the item is a child item of the root item</returns>
    public static bool IsChildOfItem(this Item rootItem, Item item, List<Item> itemsToSearch)
    {
        return FindChildrenRecursively(rootItem, itemsToSearch)
            .Any(c => c.Id == item.ParentId);
    }
    
    /// <summary>
    ///     Checks if an item is a root item
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <returns>True if the item is a child</returns>
    public static bool IsChildItem(this Item item)
    {
        return item.ParentId != "hideout";
    }
}