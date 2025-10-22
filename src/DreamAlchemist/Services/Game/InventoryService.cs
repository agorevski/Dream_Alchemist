using DreamAlchemist.Models.Entities;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;

namespace DreamAlchemist.Services.Game;

public class InventoryService : IInventoryService
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;

    public InventoryService(
        IDatabaseService databaseService,
        IGameStateService gameStateService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
    }

    public async Task<Dictionary<Ingredient, int>> GetInventoryItemsAsync()
    {
        var playerState = _gameStateService.PlayerState;
        var result = new Dictionary<Ingredient, int>();

        foreach (var kvp in playerState.Inventory)
        {
            var ingredient = await _databaseService.GetIngredientAsync(kvp.Key);
            if (ingredient != null)
            {
                result[ingredient] = kvp.Value;
            }
        }

        return result;
    }

    public int GetCurrentWeight()
    {
        return _gameStateService.PlayerState.CurrentWeight;
    }

    public int GetMaxCapacity()
    {
        return _gameStateService.PlayerState.MaxWeight;
    }

    public int GetAvailableSpace()
    {
        return GetMaxCapacity() - GetCurrentWeight();
    }

    public async Task<bool> CanAddItemAsync(string ingredientId, int quantity)
    {
        var ingredient = await _databaseService.GetIngredientAsync(ingredientId);
        if (ingredient == null)
            return false;

        var requiredWeight = ingredient.Weight * quantity;
        return GetAvailableSpace() >= requiredWeight;
    }

    public int GetItemQuantity(string ingredientId)
    {
        var playerState = _gameStateService.PlayerState;
        return playerState.Inventory.TryGetValue(ingredientId, out var quantity) ? quantity : 0;
    }

    public async Task<Dictionary<Ingredient, int>> GetSortedInventoryAsync(InventorySortMode sortMode)
    {
        var inventory = await GetInventoryItemsAsync();

        return sortMode switch
        {
            InventorySortMode.Name => inventory
                .OrderBy(kvp => kvp.Key.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            InventorySortMode.Rarity => inventory
                .OrderByDescending(kvp => kvp.Key.Rarity)
                .ThenBy(kvp => kvp.Key.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            InventorySortMode.Quantity => inventory
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            InventorySortMode.Value => inventory
                .OrderByDescending(kvp => kvp.Key.BaseValue)
                .ThenBy(kvp => kvp.Key.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            InventorySortMode.Weight => inventory
                .OrderByDescending(kvp => kvp.Key.Weight)
                .ThenBy(kvp => kvp.Key.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            _ => inventory
        };
    }
}
