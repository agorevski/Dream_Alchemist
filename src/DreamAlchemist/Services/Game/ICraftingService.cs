using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.Services.Game;

public interface ICraftingService
{
    /// <summary>
    /// Attempt to craft a dream from ingredients
    /// </summary>
    Task<CraftResultDto> CraftDreamAsync(List<string> ingredientIds);
    
    /// <summary>
    /// Get all discovered recipes
    /// </summary>
    Task<List<Recipe>> GetDiscoveredRecipesAsync();
    
    /// <summary>
    /// Get all available recipes (discovered and undiscovered)
    /// </summary>
    Task<List<Recipe>> GetAllRecipesAsync();
    
    /// <summary>
    /// Check if a specific recipe can be crafted with current inventory
    /// </summary>
    Task<bool> CanCraftRecipeAsync(string recipeId);
    
    /// <summary>
    /// Craft a specific known recipe
    /// </summary>
    Task<CraftResultDto> CraftRecipeAsync(string recipeId);
    
    /// <summary>
    /// Get crafting suggestions based on current inventory
    /// </summary>
    Task<List<Recipe>> GetCraftingSuggestionsAsync();
}
