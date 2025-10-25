# 04 - Core Services

## Overview

This document details the implementation of core services that form the foundation of Dream Alchemist's business logic layer.

## Database Service

### IDatabaseService Interface

```csharp
namespace DreamAlchemist.Services.Data;

public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task SeedDataAsync();
    
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
```

### DatabaseService Implementation

```csharp
using SQLite;

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
        // Load seed data from JSON files
        await SeedIngredientsAsync();
        await SeedRecipesAsync();
        await SeedCitiesAsync();
        await SeedEventsAsync();
        await CreateInitialPlayerStateAsync();
    }

    private async Task SeedIngredientsAsync()
    {
        var json = await LoadEmbeddedResourceAsync("ingredients.json");
        var ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(json);
        
        if (ingredients != null)
        {
            await _database.InsertAllAsync(ingredients);
        }
    }

    private async Task SeedRecipesAsync()
    {
        var json = await LoadEmbeddedResourceAsync("recipes.json");
        var recipes = JsonConvert.DeserializeObject<List<Recipe>>(json);
        
        if (recipes != null)
        {
            await _database.InsertAllAsync(recipes);
        }
    }

    private async Task SeedCitiesAsync()
    {
        var json = await LoadEmbeddedResourceAsync("cities.json");
        var cities = JsonConvert.DeserializeObject<List<City>>(json);
        
        if (cities != null)
        {
            await _database.InsertAllAsync(cities);
        }
    }

    private async Task SeedEventsAsync()
    {
        var json = await LoadEmbeddedResourceAsync("events.json");
        var events = JsonConvert.DeserializeObject<List<GameEvent>>(json);
        
        if (events != null)
        {
            await _database.InsertAllAsync(events);
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
```

## Game State Service

### IGameStateService Interface

```csharp
namespace DreamAlchemist.Services.Core;

public interface IGameStateService
{
    PlayerState PlayerState { get; }
    City CurrentCity { get; }
    int CurrentDay { get; }
    bool IsInitialized { get; }
    
    event EventHandler<PlayerState>? PlayerStateChanged;
    event EventHandler<City>? CityChanged;
    event EventHandler<int>? DayProgressed;
    
    Task InitializeAsync();
    Task LoadGameAsync();
    Task SaveGameAsync();
    Task ProgressTimeAsync(int days);
    Task TravelToCityAsync(string cityId);
    Task UpdateReputationAsync(int trust, int infamy, int lucidity);
    Task CheckAndProgressTierAsync();
}
```

### GameStateService Implementation

```csharp
namespace DreamAlchemist.Services.Core;

public class GameStateService : IGameStateService
{
    private readonly IDatabaseService _databaseService;
    private PlayerState _playerState = null!;
    private City _currentCity = null!;

    public PlayerState PlayerState => _playerState;
    public City CurrentCity => _currentCity;
    public int CurrentDay => _playerState?.CurrentDay ?? 1;
    public bool IsInitialized { get; private set; }

    public event EventHandler<PlayerState>? PlayerStateChanged;
    public event EventHandler<City>? CityChanged;
    public event EventHandler<int>? DayProgressed;

    public GameStateService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        await _databaseService.InitializeDatabaseAsync();
        await LoadGameAsync();
        
        IsInitialized = true;
    }

    public async Task LoadGameAsync()
    {
        var playerState = await _databaseService.GetPlayerStateAsync();
        
        if (playerState == null)
        {
            // Create new game
            playerState = new PlayerState
            {
                PlayerName = "Novice Peddler",
                Coins = GameConstants.STARTING_COINS,
                CurrentDay = 1,
                CurrentCityId = GameConstants.STARTING_CITY,
                Tier = 1,
                MaxWeight = GameConstants.STARTING_WEIGHT_CAPACITY
            };
            await _databaseService.SavePlayerStateAsync(playerState);
        }

        _playerState = playerState;
        
        var city = await _databaseService.GetCityAsync(playerState.CurrentCityId);
        _currentCity = city ?? throw new InvalidOperationException("Current city not found");

        PlayerStateChanged?.Invoke(this, _playerState);
        CityChanged?.Invoke(this, _currentCity);
    }

    public async Task SaveGameAsync()
    {
        await _databaseService.SavePlayerStateAsync(_playerState);
    }

    public async Task ProgressTimeAsync(int days)
    {
        _playerState.CurrentDay += days;
        
        DayProgressed?.Invoke(this, days);
        PlayerStateChanged?.Invoke(this, _playerState);
        
        await SaveGameAsync();
    }

    public async Task TravelToCityAsync(string cityId)
    {
        var city = await _databaseService.GetCityAsync(cityId);
        if (city == null)
            throw new ArgumentException("City not found", nameof(cityId));

        if (!_playerState.UnlockedCities.Contains(cityId))
            throw new InvalidOperationException("City not unlocked");

        _playerState.CurrentCityId = cityId;
        _currentCity = city;

        CityChanged?.Invoke(this, _currentCity);
        PlayerStateChanged?.Invoke(this, _playerState);
        
        await SaveGameAsync();
    }

    public async Task UpdateReputationAsync(int trust, int infamy, int lucidity)
    {
        _playerState.TrustReputation = Math.Clamp(
            _playerState.TrustReputation + trust,
            GameConstants.REPUTATION_MIN,
            GameConstants.REPUTATION_MAX);

        _playerState.InfamyReputation = Math.Clamp(
            _playerState.InfamyReputation + infamy,
            GameConstants.REPUTATION_MIN,
            GameConstants.REPUTATION_MAX);

        _playerState.LucidityReputation = Math.Clamp(
            _playerState.LucidityReputation + lucidity,
            GameConstants.REPUTATION_MIN,
            GameConstants.REPUTATION_MAX);

        PlayerStateChanged?.Invoke(this, _playerState);
        
        await CheckAndProgressTierAsync();
        await SaveGameAsync();
    }

    public async Task CheckAndProgressTierAsync()
    {
        // Calculate tier based on total reputation
        var totalReputation = _playerState.TrustReputation + 
                            _playerState.InfamyReputation + 
                            _playerState.LucidityReputation;

        var newTier = totalReputation switch
        {
            < 50 => 1,
            < 120 => 2,
            < 200 => 3,
            < 300 => 4,
            _ => 5
        };

        if (newTier > _playerState.Tier)
        {
            _playerState.Tier = newTier;
            _playerState.PlayerName = GameConstants.TierNames[newTier];
            
            // Increase capacity on tier up
            _playerState.MaxWeight += 50;
            
            PlayerStateChanged?.Invoke(this, _playerState);
            await SaveGameAsync();
        }
    }
}
```

## Save Service

### ISaveService Interface

```csharp
namespace DreamAlchemist.Services.Core;

public interface ISaveService
{
    Task<bool> SaveGameAsync();
    Task<bool> LoadGameAsync();
    Task<bool> DeleteSaveAsync();
    Task<bool> HasSaveDataAsync();
    Task<SaveMetadata?> GetSaveMetadataAsync();
    Task ExportSaveAsync(string filePath);
    Task<bool> ImportSaveAsync(string filePath);
}
```

### SaveService Implementation

```csharp
namespace DreamAlchemist.Services.Core;

public class SaveService : ISaveService
{
    private readonly IGameStateService _gameStateService;
    private readonly IDatabaseService _databaseService;
    private const string SAVE_METADATA_KEY = "save_metadata";

    public SaveService(
        IGameStateService gameStateService,
        IDatabaseService databaseService)
    {
        _gameStateService = gameStateService;
        _databaseService = databaseService;
    }

    public async Task<bool> SaveGameAsync()
    {
        try
        {
            await _gameStateService.SaveGameAsync();
            
            var metadata = new SaveMetadata
            {
                LastSaved = DateTime.UtcNow,
                PlayerName = _gameStateService.PlayerState.PlayerName,
                CurrentDay = _gameStateService.CurrentDay,
                Coins = _gameStateService.PlayerState.Coins,
                Tier = _gameStateService.PlayerState.Tier
            };

            await Preferences.SetAsync(SAVE_METADATA_KEY, 
                JsonConvert.SerializeObject(metadata));

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> LoadGameAsync()
    {
        try
        {
            await _gameStateService.LoadGameAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteSaveAsync()
    {
        try
        {
            await _databaseService.DeleteAllDataAsync();
            Preferences.Remove(SAVE_METADATA_KEY);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete save failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> HasSaveDataAsync()
    {
        var playerState = await _databaseService.GetPlayerStateAsync();
        return playerState != null;
    }

    public async Task<SaveMetadata?> GetSaveMetadataAsync()
    {
        var json = await Preferences.GetAsync(SAVE_METADATA_KEY, null);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonConvert.DeserializeObject<SaveMetadata>(json);
    }

    public async Task ExportSaveAsync(string filePath)
    {
        var playerState = await _databaseService.GetPlayerStateAsync();
        var json = JsonConvert.SerializeObject(playerState, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<bool> ImportSaveAsync(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var playerState = JsonConvert.DeserializeObject<PlayerState>(json);
            
            if (playerState != null)
            {
                await _databaseService.SavePlayerStateAsync(playerState);
                await LoadGameAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}

public class SaveMetadata
{
    public DateTime LastSaved { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int CurrentDay { get; set; }
    public int Coins { get; set; }
    public int Tier { get; set; }
}
```

## Navigation Service

```csharp
namespace DreamAlchemist.Services.Core;

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters != null)
        {
            await Shell.Current.GoToAsync(route, parameters);
        }
        else
        {
            await Shell.Current.GoToAsync(route);
        }
    }

    public async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task PopToRootAsync()
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}
```

## Service Registration

All services should be registered in `MauiProgram.cs`:

```csharp
private static void RegisterServices(IServiceCollection services)
{
    // Core Services
    services.AddSingleton<IDatabaseService, DatabaseService>();
    services.AddSingleton<IGameStateService, GameStateService>();
    services.AddSingleton<ISaveService, SaveService>();
    services.AddSingleton<INavigationService, NavigationService>();
    
    // Game Logic Services (will be implemented in subsequent docs)
    services.AddSingleton<IMarketService, MarketService>();
    services.AddSingleton<ICraftingService, CraftingService>();
    services.AddSingleton<IInventoryService, InventoryService>();
    services.AddSingleton<IEventService, EventService>();
    services.AddSingleton<IReputationService, ReputationService>();
    services.AddSingleton<ITravelService, TravelService>();
    
    // External Services
    services.AddSingleton<IAIService, AIService>();
    services.AddSingleton(AudioManager.Current);
    services.AddSingleton<IAudioService, AudioService>();
}
```

## Testing Core Services

### Example Unit Test

```csharp
using Xunit;
using Moq;

namespace DreamAlchemist.Tests.Services;

public class GameStateServiceTests
{
    [Fact]
    public async Task ProgressTime_ShouldIncrementDay()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var service = new GameStateService(mockDb.Object);
        
        // Initialize with test data
        await service.InitializeAsync();
        var initialDay = service.CurrentDay;

        // Act
        await service.ProgressTimeAsync(3);

        // Assert
        Assert.Equal(initialDay + 3, service.CurrentDay);
    }

    [Fact]
    public async Task UpdateReputation_ShouldClampValues()
    {
        // Arrange
        var mockDb = new Mock<IDatabaseService>();
        var service = new GameStateService(mockDb.Object);
        await service.InitializeAsync();

        // Act
        await service.UpdateReputationAsync(150, 0, 0);

        // Assert
        Assert.Equal(GameConstants.REPUTATION_MAX, 
            service.PlayerState.TrustReputation);
    }
}
```

## Next Steps

Proceed to **05-economy-system.md** for market simulation implementation.
