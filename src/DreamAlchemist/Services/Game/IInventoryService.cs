using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.Services.Game;

public interface IInventoryService
{
    /// <summary>
    /// Get all items in inventory with quantities
    /// </summary>
    Task<Dictionary<Ingredient, int>> GetInventoryItemsAsync();
    
    /// <summary>
    /// Get current inventory weight
    /// </summary>
    int GetCurrentWeight();
    
    /// <summary>
    /// Get maximum inventory capacity
    /// </summary>
    int GetMaxCapacity();
    
    /// <summary>
    /// Get available inventory space
    /// </summary>
    int GetAvailableSpace();
    
    /// <summary>
    /// Check if item can be added to inventory
    /// </summary>
    Task<bool> CanAddItemAsync(string ingredientId, int quantity);
    
    /// <summary>
    /// Get quantity of specific ingredient in inventory
    /// </summary>
    int GetItemQuantity(string ingredientId);
    
    /// <summary>
    /// Sort inventory by specified criteria
    /// </summary>
    Task<Dictionary<Ingredient, int>> GetSortedInventoryAsync(InventorySortMode sortMode);
}

public enum InventorySortMode
{
    Name,
    Rarity,
    Quantity,
    Value,
    Weight
}
