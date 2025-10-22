# 02 - Architecture

## Overview

Dream Alchemist follows the **Model-View-ViewModel (MVVM)** architectural pattern, enhanced with dependency injection, services layer, and repository pattern for data access.

## Architecture Layers

```
┌─────────────────────────────────────────────────────┐
│                     Views (XAML)                     │
│              User Interface Components               │
└────────────────────┬────────────────────────────────┘
                     │ Data Binding
                     ▼
┌─────────────────────────────────────────────────────┐
│                   ViewModels                         │
│        CommunityToolkit.Mvvm ObservableObjects       │
└────────────────────┬────────────────────────────────┘
                     │ Service Calls
                     ▼
┌─────────────────────────────────────────────────────┐
│                   Services Layer                     │
│        Business Logic & Game Mechanics               │
└────────────────────┬────────────────────────────────┘
                     │ Data Access
                     ▼
┌─────────────────────────────────────────────────────┐
│                  Data Layer                          │
│           Repository Pattern + SQLite                │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│                    Models                            │
│              Domain Entities & DTOs                  │
└─────────────────────────────────────────────────────┘
```

## Project Structure

```
DreamAlchemist/
├── Models/
│   ├── Entities/              # Database entities
│   │   ├── Ingredient.cs
│   │   ├── Recipe.cs
│   │   ├── City.cs
│   │   ├── GameEvent.cs
│   │   └── PlayerState.cs
│   ├── DTOs/                  # Data transfer objects
│   │   ├── MarketPriceDto.cs
│   │   └── CraftResultDto.cs
│   └── Enums/                 # Enumerations
│       ├── Rarity.cs
│       ├── DreamTag.cs
│       └── EventType.cs
│
├── ViewModels/
│   ├── Base/
│   │   └── BaseViewModel.cs   # Shared ViewModel base
│   ├── MainViewModel.cs
│   ├── MarketViewModel.cs
│   ├── LabViewModel.cs
│   ├── InventoryViewModel.cs
│   ├── TravelViewModel.cs
│   └── EventViewModel.cs
│
├── Views/
│   ├── MainPage.xaml          # Hub/dashboard
│   ├── MarketPage.xaml        # Buy/sell interface
│   ├── LabPage.xaml           # Crafting interface
│   ├── InventoryPage.xaml     # Inventory management
│   ├── TravelPage.xaml        # City navigation
│   ├── EventPage.xaml         # Event display
│   └── Controls/              # Custom controls
│       ├── IngredientCard.xaml
│       ├── RecipeCard.xaml
│       └── PriceChart.xaml
│
├── Services/
│   ├── Core/
│   │   ├── IGameStateService.cs
│   │   ├── GameStateService.cs
│   │   ├── ISaveService.cs
│   │   ├── SaveService.cs
│   │   ├── INavigationService.cs
│   │   └── NavigationService.cs
│   ├── Game/
│   │   ├── IMarketService.cs
│   │   ├── MarketService.cs
│   │   ├── ICraftingService.cs
│   │   ├── CraftingService.cs
│   │   ├── IInventoryService.cs
│   │   ├── InventoryService.cs
│   │   ├── IEventService.cs
│   │   ├── EventService.cs
│   │   ├── IReputationService.cs
│   │   ├── ReputationService.cs
│   │   ├── ITravelService.cs
│   │   └── TravelService.cs
│   ├── External/
│   │   ├── IAIService.cs
│   │   ├── AIService.cs
│   │   ├── IAudioService.cs
│   │   └── AudioService.cs
│   └── Data/
│       ├── IDatabaseService.cs
│       └── DatabaseService.cs
│
├── Data/
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── Repository.cs
│   │   ├── IIngredientRepository.cs
│   │   ├── IngredientRepository.cs
│   │   ├── IRecipeRepository.cs
│   │   └── RecipeRepository.cs
│   ├── GameDbContext.cs
│   └── SeedData.cs
│
├── Helpers/
│   ├── Constants.cs
│   ├── Extensions.cs
│   ├── RandomHelper.cs
│   └── Validators/
│       └── RecipeValidator.cs
│
└── Resources/
    ├── Styles/
    ├── Images/
    ├── Audio/
    └── Data/                   # JSON seed files
        ├── ingredients.json
        ├── recipes.json
        └── cities.json
```

## MVVM Pattern Implementation

### BaseViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DreamAlchemist.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    protected INavigationService NavigationService { get; }

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    /// <summary>
    /// Called when the view appears
    /// </summary>
    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the view disappears
    /// </summary>
    public virtual Task OnDisappearingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Execute an async task with error handling
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation, string? errorMessage = null)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = errorMessage ?? ex.Message;
            // Log error
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

### Example ViewModel Implementation

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DreamAlchemist.ViewModels;

public partial class MarketViewModel : BaseViewModel
{
    private readonly IMarketService _marketService;
    private readonly IInventoryService _inventoryService;
    private readonly IGameStateService _gameStateService;

    [ObservableProperty]
    private ObservableCollection<MarketItemViewModel> marketItems = new();

    [ObservableProperty]
    private MarketItemViewModel? selectedItem;

    [ObservableProperty]
    private int playerCoins;

    [ObservableProperty]
    private string currentCityName = string.Empty;

    public MarketViewModel(
        INavigationService navigationService,
        IMarketService marketService,
        IInventoryService inventoryService,
        IGameStateService gameStateService)
        : base(navigationService)
    {
        _marketService = marketService;
        _inventoryService = inventoryService;
        _gameStateService = gameStateService;
        
        Title = "Dream Market";
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadMarketDataAsync();
    }

    [RelayCommand]
    private async Task LoadMarketDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var currentCity = _gameStateService.CurrentCity;
            CurrentCityName = currentCity.Name;
            
            var prices = await _marketService.GetCurrentPricesAsync(currentCity.Id);
            
            MarketItems.Clear();
            foreach (var price in prices)
            {
                MarketItems.Add(new MarketItemViewModel(price));
            }
            
            PlayerCoins = _gameStateService.PlayerState.Coins;
        });
    }

    [RelayCommand]
    private async Task BuyItemAsync(MarketItemViewModel item)
    {
        await ExecuteAsync(async () =>
        {
            var success = await _marketService.BuyIngredientAsync(
                item.IngredientId, 
                item.Quantity);
            
            if (success)
            {
                await LoadMarketDataAsync();
                // Show success message
            }
        }, "Failed to purchase item");
    }

    [RelayCommand]
    private async Task SellItemAsync(MarketItemViewModel item)
    {
        await ExecuteAsync(async () =>
        {
            var success = await _marketService.SellIngredientAsync(
                item.IngredientId,
                item.Quantity);
            
            if (success)
            {
                await LoadMarketDataAsync();
                // Show success message
            }
        }, "Failed to sell item");
    }

    [RelayCommand]
    private async Task NavigateToInventoryAsync()
    {
        await NavigationService.NavigateToAsync("InventoryPage");
    }
}
```

## Service Layer Pattern

### Service Interface

```csharp
namespace DreamAlchemist.Services.Game;

public interface IMarketService
{
    /// <summary>
    /// Get current market prices for a city
    /// </summary>
    Task<List<MarketPrice>> GetCurrentPricesAsync(string cityId);

    /// <summary>
    /// Purchase an ingredient
    /// </summary>
    Task<bool> BuyIngredientAsync(string ingredientId, int quantity);

    /// <summary>
    /// Sell an ingredient
    /// </summary>
    Task<bool> SellIngredientAsync(string ingredientId, int quantity);

    /// <summary>
    /// Update all market prices (called on time progression)
    /// </summary>
    Task UpdateMarketPricesAsync();

    /// <summary>
    /// Apply event modifier to market
    /// </summary>
    Task ApplyEventModifierAsync(string eventId, string cityId);
}
```

### Service Implementation

```csharp
namespace DreamAlchemist.Services.Game;

public class MarketService : IMarketService
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;
    private readonly IEventService _eventService;

    public MarketService(
        IDatabaseService databaseService,
        IGameStateService gameStateService,
        IEventService eventService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
        _eventService = eventService;
    }

    public async Task<List<MarketPrice>> GetCurrentPricesAsync(string cityId)
    {
        var ingredients = await _databaseService.GetIngredientsAsync();
        var city = await _databaseService.GetCityAsync(cityId);
        var currentDay = _gameStateService.CurrentDay;

        var prices = new List<MarketPrice>();
        
        foreach (var ingredient in ingredients)
        {
            var price = CalculatePrice(ingredient, city, currentDay);
            prices.Add(new MarketPrice
            {
                IngredientId = ingredient.Id,
                IngredientName = ingredient.Name,
                Price = price,
                Available = true,
                LastUpdated = DateTime.UtcNow
            });
        }

        return prices;
    }

    public async Task<bool> BuyIngredientAsync(string ingredientId, int quantity)
    {
        var ingredient = await _databaseService.GetIngredientAsync(ingredientId);
        var currentCity = _gameStateService.CurrentCity;
        var playerState = _gameStateService.PlayerState;

        var price = CalculatePrice(ingredient, currentCity, _gameStateService.CurrentDay);
        var totalCost = price * quantity;

        if (playerState.Coins < totalCost)
            return false;

        if (!playerState.CanAddToInventory(ingredient, quantity))
            return false;

        playerState.Coins -= totalCost;
        playerState.AddToInventory(ingredient, quantity);

        await _databaseService.SavePlayerStateAsync(playerState);

        return true;
    }

    // Additional methods...

    private decimal CalculatePrice(Ingredient ingredient, City city, int currentDay)
    {
        // Price calculation formula from GDD
        // price = base_value × rarity_modifier × city_modifier × event_multiplier × noise
        
        var basePrice = ingredient.BaseValue;
        var rarityModifier = GetRarityModifier(ingredient.Rarity);
        var cityModifier = GetCityModifier(ingredient, city);
        var eventMultiplier = _eventService.GetCurrentEventMultiplier(ingredient.Id, city.Id);
        var noise = GetPriceNoise(currentDay, ingredient.Id);

        return basePrice * rarityModifier * cityModifier * eventMultiplier * noise;
    }

    private decimal GetRarityModifier(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => 1.0m,
            Rarity.Uncommon => 1.5m,
            Rarity.Rare => 2.5m,
            Rarity.Epic => 4.0m,
            Rarity.Legendary => 7.0m,
            _ => 1.0m
        };
    }

    private decimal GetCityModifier(Ingredient ingredient, City city)
    {
        // Check ingredient tags against city modifiers
        decimal modifier = 1.0m;
        
        foreach (var tag in ingredient.Tags)
        {
            if (city.TagModifiers.TryGetValue(tag, out var tagModifier))
            {
                modifier *= tagModifier;
            }
        }

        return modifier;
    }

    private decimal GetPriceNoise(int day, string ingredientId)
    {
        // Deterministic noise based on day and ingredient
        var seed = day + ingredientId.GetHashCode();
        var random = new Random(seed);
        return (decimal)(0.9 + random.NextDouble() * 0.2); // 0.9 to 1.1
    }
}
```

## Repository Pattern

### Generic Repository Interface

```csharp
namespace DreamAlchemist.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<List<T>> GetAllAsync();
    Task<int> InsertAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
    Task<List<T>> QueryAsync(string query, params object[] args);
}
```

### Generic Repository Implementation

```csharp
using SQLite;

namespace DreamAlchemist.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, new()
{
    protected readonly SQLiteAsyncConnection _database;

    public Repository(SQLiteAsyncConnection database)
    {
        _database = database;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _database.Table<T>()
            .Where(e => ((dynamic)e).Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _database.Table<T>().ToListAsync();
    }

    public async Task<int> InsertAsync(T entity)
    {
        return await _database.InsertAsync(entity);
    }

    public async Task<int> UpdateAsync(T entity)
    {
        return await _database.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync(T entity)
    {
        return await _database.DeleteAsync(entity);
    }

    public async Task<List<T>> QueryAsync(string query, params object[] args)
    {
        return await _database.QueryAsync<T>(query, args);
    }
}
```

## Dependency Injection Container

Already configured in `MauiProgram.cs` (see 01-project-setup.md), but here's the pattern:

```csharp
// Singleton - One instance for app lifetime
services.AddSingleton<IGameStateService, GameStateService>();

// Transient - New instance every time
services.AddTransient<MainViewModel>();

// Scoped - One instance per scope (not commonly used in MAUI)
```

## Navigation Service

```csharp
namespace DreamAlchemist.Services.Core;

public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
    Task NavigateBackAsync();
    Task PopToRootAsync();
}

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

## State Management

### Game State Service

```csharp
namespace DreamAlchemist.Services.Core;

public interface IGameStateService
{
    PlayerState PlayerState { get; }
    City CurrentCity { get; }
    int CurrentDay { get; }
    
    Task InitializeAsync();
    Task LoadGameAsync();
    Task SaveGameAsync();
    Task ProgressTimeAsync(int days);
    Task TravelToCityAsync(string cityId);
}
```

This service acts as the central hub for game state, coordinating between different services.

## Communication Between Components

### Event Aggregator Pattern

For loosely coupled communication between ViewModels:

```csharp
namespace DreamAlchemist.Helpers;

public class EventAggregator
{
    private readonly Dictionary<Type, List<object>> _subscribers = new();

    public void Subscribe<T>(Action<T> action)
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
        {
            _subscribers[type] = new List<object>();
        }
        _subscribers[type].Add(action);
    }

    public void Publish<T>(T message)
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            foreach (var subscriber in _subscribers[type].Cast<Action<T>>())
            {
                subscriber(message);
            }
        }
    }
}

// Usage:
// Subscribe: _eventAggregator.Subscribe<MarketUpdatedEvent>(OnMarketUpdated);
// Publish: _eventAggregator.Publish(new MarketUpdatedEvent { CityId = "somnia" });
```

## Testing Architecture

Services should be designed with interfaces for easy mocking:

```csharp
// In test project
var mockMarketService = new Mock<IMarketService>();
mockMarketService
    .Setup(m => m.GetCurrentPricesAsync(It.IsAny<string>()))
    .ReturnsAsync(mockPrices);

var viewModel = new MarketViewModel(
    mockNavigationService.Object,
    mockMarketService.Object,
    mockInventoryService.Object,
    mockGameStateService.Object);
```

## Key Architectural Principles

1. **Separation of Concerns** - Each layer has a specific responsibility
2. **Dependency Inversion** - Depend on abstractions, not concrete implementations
3. **Single Responsibility** - Each class does one thing well
4. **Open/Closed Principle** - Open for extension, closed for modification
5. **Interface Segregation** - Small, focused interfaces
6. **DRY (Don't Repeat Yourself)** - Reuse through inheritance and composition

## Next Steps

Proceed to **03-data-models.md** to define all data models and entities.
