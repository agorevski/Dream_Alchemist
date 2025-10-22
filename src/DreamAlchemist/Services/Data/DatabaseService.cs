using SQLite;
using Newtonsoft.Json;
using DreamAlchemist.Models.Entities;
using DreamAlchemist.Helpers;

namespace DreamAlchemist.Services.Data;

public class DatabaseService : IDatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _initialized = false;

    public DatabaseService()
    {
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "dreamalchemist.db3");
        
        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeDatabaseAsync()
    {
        if (_initialized)
            return;

        // TEMPORARY FIX: Force database reset to fix deserialization issues
        // Remove this after confirming the fix works
        try
        {
            await _database.DropTableAsync<Ingredient>();
            await _database.DropTableAsync<Recipe>();
            await _database.DropTableAsync<City>();
            await _database.DropTableAsync<GameEvent>();
            await _database.DropTableAsync<PlayerState>();
            System.Diagnostics.Debug.WriteLine("Database tables dropped for reset");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error dropping tables (may not exist): {ex.Message}");
        }

        await _database.CreateTableAsync<Ingredient>();
        await _database.CreateTableAsync<Recipe>();
        await _database.CreateTableAsync<City>();
        await _database.CreateTableAsync<GameEvent>();
        await _database.CreateTableAsync<PlayerState>();

        _initialized = true;

        // Check if data exists, if not, seed it
        var ingredientCount = await _database.Table<Ingredient>().CountAsync();
        if (ingredientCount == 0)
        {
            await SeedDataAsync();
        }
    }

    public async Task SeedDataAsync()
    {
        await SeedIngredientsAsync();
        await SeedRecipesAsync();
        await SeedCitiesAsync();
        await SeedEventsAsync();
        await CreateInitialPlayerStateAsync();
    }

    private async Task SeedIngredientsAsync()
    {
        try
        {
            var json = await LoadEmbeddedResourceAsync("ingredients.json");
            System.Diagnostics.Debug.WriteLine($"Loaded ingredients JSON, length: {json?.Length ?? 0}");
            
            var ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(json);
            System.Diagnostics.Debug.WriteLine($"Deserialized {ingredients?.Count ?? 0} ingredients");
            
            if (ingredients != null && ingredients.Count > 0)
            {
                // Log first ingredient to verify deserialization
                var first = ingredients[0];
                System.Diagnostics.Debug.WriteLine($"First ingredient: {first.Name}, Tags count: {first.Tags?.Count ?? 0}");
                if (first.Tags != null && first.Tags.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"First tag: {first.Tags[0]}");
                }
                
                await _database.InsertAllAsync(ingredients);
                System.Diagnostics.Debug.WriteLine($"Successfully inserted {ingredients.Count} ingredients into database");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No ingredients to seed - list is null or empty");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding ingredients: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private async Task SeedRecipesAsync()
    {
        try
        {
            var json = await LoadEmbeddedResourceAsync("recipes.json");
            var recipes = JsonConvert.DeserializeObject<List<Recipe>>(json);
            
            if (recipes != null && recipes.Count > 0)
            {
                await _database.InsertAllAsync(recipes);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding recipes: {ex.Message}");
        }
    }

    private async Task SeedCitiesAsync()
    {
        try
        {
            var json = await LoadEmbeddedResourceAsync("cities.json");
            var cities = JsonConvert.DeserializeObject<List<City>>(json);
            
            if (cities != null && cities.Count > 0)
            {
                await _database.InsertAllAsync(cities);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding cities: {ex.Message}");
        }
    }

    private async Task SeedEventsAsync()
    {
        try
        {
            var json = await LoadEmbeddedResourceAsync("events.json");
            var events = JsonConvert.DeserializeObject<List<GameEvent>>(json);
            
            if (events != null && events.Count > 0)
            {
                await _database.InsertAllAsync(events);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding events: {ex.Message}");
        }
    }

    private async Task CreateInitialPlayerStateAsync()
    {
        var playerState = new PlayerState
        {
            PlayerName = "Novice Peddler",
            Coins = GameConstants.STARTING_COINS,
            CurrentDay = 1,
            CurrentCityId = GameConstants.STARTING_CITY,
            Tier = 1,
            MaxWeight = GameConstants.STARTING_WEIGHT_CAPACITY
        };

        await _database.InsertAsync(playerState);
    }

    private async Task<string> LoadEmbeddedResourceAsync(string filename)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync($"Data/{filename}");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    // Ingredient methods
    public async Task<List<Ingredient>> GetIngredientsAsync()
    {
        return await _database.Table<Ingredient>().ToListAsync();
    }

    public async Task<Ingredient?> GetIngredientAsync(string id)
    {
        return await _database.Table<Ingredient>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveIngredientAsync(Ingredient ingredient)
    {
        var existing = await GetIngredientAsync(ingredient.Id);
        if (existing != null)
        {
            return await _database.UpdateAsync(ingredient);
        }
        return await _database.InsertAsync(ingredient);
    }

    // Recipe methods
    public async Task<List<Recipe>> GetRecipesAsync()
    {
        return await _database.Table<Recipe>().ToListAsync();
    }

    public async Task<Recipe?> GetRecipeAsync(string id)
    {
        return await _database.Table<Recipe>()
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Recipe>> GetDiscoveredRecipesAsync()
    {
        return await _database.Table<Recipe>()
            .Where(r => r.Discovered)
            .ToListAsync();
    }

    public async Task<int> SaveRecipeAsync(Recipe recipe)
    {
        var existing = await GetRecipeAsync(recipe.Id);
        if (existing != null)
        {
            return await _database.UpdateAsync(recipe);
        }
        return await _database.InsertAsync(recipe);
    }

    // City methods
    public async Task<List<City>> GetCitiesAsync()
    {
        return await _database.Table<City>().ToListAsync();
    }

    public async Task<City?> GetCityAsync(string id)
    {
        return await _database.Table<City>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveCityAsync(City city)
    {
        var existing = await GetCityAsync(city.Id);
        if (existing != null)
        {
            return await _database.UpdateAsync(city);
        }
        return await _database.InsertAsync(city);
    }

    // Event methods
    public async Task<List<GameEvent>> GetEventsAsync()
    {
        return await _database.Table<GameEvent>().ToListAsync();
    }

    public async Task<GameEvent?> GetEventAsync(string id)
    {
        return await _database.Table<GameEvent>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveEventAsync(GameEvent gameEvent)
    {
        var existing = await GetEventAsync(gameEvent.Id);
        if (existing != null)
        {
            return await _database.UpdateAsync(gameEvent);
        }
        return await _database.InsertAsync(gameEvent);
    }

    // Player State methods
    public async Task<PlayerState?> GetPlayerStateAsync()
    {
        return await _database.Table<PlayerState>().FirstOrDefaultAsync();
    }

    public async Task<int> SavePlayerStateAsync(PlayerState playerState)
    {
        playerState.LastSaved = DateTime.UtcNow;
        
        var existing = await GetPlayerStateAsync();
        if (existing != null)
        {
            playerState.Id = existing.Id;
            return await _database.UpdateAsync(playerState);
        }
        return await _database.InsertAsync(playerState);
    }

    public async Task<int> DeleteAllDataAsync()
    {
        await _database.DropTableAsync<Ingredient>();
        await _database.DropTableAsync<Recipe>();
        await _database.DropTableAsync<City>();
        await _database.DropTableAsync<GameEvent>();
        await _database.DropTableAsync<PlayerState>();
        
        _initialized = false;
        await InitializeDatabaseAsync();
        
        return 1;
    }
}
