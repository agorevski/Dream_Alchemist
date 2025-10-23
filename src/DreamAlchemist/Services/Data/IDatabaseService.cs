using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.Services.Data;

public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task ClearDatabaseAsync();
    
    // Ingredients
    Task<List<Ingredient>> GetIngredientsAsync();
    Task<Ingredient?> GetIngredientAsync(string id);
    Task<int> SaveIngredientAsync(Ingredient ingredient);
    
    // Recipes
    Task<List<Recipe>> GetRecipesAsync();
    Task<Recipe?> GetRecipeAsync(string id);
    Task<List<Recipe>> GetDiscoveredRecipesAsync();
    Task<int> SaveRecipeAsync(Recipe recipe);
    
    // Cities
    Task<List<City>> GetCitiesAsync();
    Task<City?> GetCityAsync(string id);
    Task<int> SaveCityAsync(City city);
    
    // Events
    Task<List<GameEvent>> GetEventsAsync();
    Task<GameEvent?> GetEventAsync(string id);
    Task<int> SaveEventAsync(GameEvent gameEvent);
    
    // Player State
    Task<PlayerState?> GetPlayerStateAsync();
    Task<int> SavePlayerStateAsync(PlayerState playerState);
    
    // Utility
    Task<int> DeleteAllDataAsync();
}
