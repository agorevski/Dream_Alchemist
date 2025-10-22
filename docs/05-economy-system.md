# 05 - Economy System

## Overview

The economy system simulates a dynamic market where ingredient prices fluctuate based on multiple factors including city modifiers, events, time, and player actions.

## Price Calculation Formula

From the GDD, the core formula is:

```
price(i,c) = base_value(i) × rarity_modifier(i) × city_modifier(c,i) × event_multiplier × noise(0.9–1.1)
```

Where:
- `i` = ingredient
- `c` = city
- `base_value` = ingredient's base value
- `rarity_modifier` = multiplier based on rarity (1.0 to 7.0)
- `city_modifier` = city's preference for ingredient tags
- `event_multiplier` = active event effects (0.5 to 2.0)
- `noise` = daily random fluctuation (0.9 to 1.1)

## IMarketService Interface

```csharp
namespace DreamAlchemist.Services.Game;

public interface IMarketService
{
    Task<List<MarketPriceDto>> GetCurrentPricesAsync(string cityId);
    Task<MarketPriceDto?> GetIngredientPriceAsync(string cityId, string ingredientId);
    Task<bool> BuyIngredientAsync(string ingredientId, int quantity);
    Task<bool> SellIngredientAsync(string ingredientId, int quantity);
    Task<bool> SellCraftedDreamAsync(string dreamId);
    Task UpdateMarketPricesAsync();
    Task<Dictionary<string, decimal>> GetPriceHistoryAsync(string ingredientId, int days);
    Task<List<MarketTrendDto>> GetMarketTrendsAsync(string cityId);
}
```

## MarketService Implementation

```csharp
namespace DreamAlchemist.Services.Game;

public class MarketService : IMarketService
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;
    private readonly IEventService _eventService;
    private readonly Dictionary<string, List<decimal>> _priceHistory = new();

    public MarketService(
        IDatabaseService databaseService,
        IGameStateService gameStateService,
        IEventService eventService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
        _eventService = eventService;
        
        // Subscribe to day progression
        _gameStateService.DayProgressed += OnDayProgressed;
    }

    private async void OnDayProgressed(object? sender, int days)
    {
        await UpdateMarketPricesAsync();
    }

    public async Task<List<MarketPriceDto>> GetCurrentPricesAsync(string cityId)
    {
        var ingredients = await _databaseService.GetIngredientsAsync();
        var city = await _databaseService.GetCityAsync(cityId);
        
        if (city == null)
            throw new ArgumentException("City not found", nameof(cityId));

        var currentDay = _gameStateService.CurrentDay;
        var prices = new List<MarketPriceDto>();

        foreach (var ingredient in ingredients)
        {
            var currentPrice = CalculatePrice(ingredient, city, currentDay);
            var basePrice = ingredient.BaseValue;
            var changePercent = ((currentPrice - basePrice) / basePrice) * 100;

            prices.Add(new MarketPriceDto
            {
                IngredientId = ingredient.Id,
                IngredientName = ingredient.Name,
                Rarity = ingredient.Rarity,
                CurrentPrice = currentPrice,
                BasePrice = basePrice,
                PriceChangePercent = changePercent,
                IsTrending = Math.Abs(changePercent) > 20,
                AvailableQuantity = GetAvailableQuantity(ingredient, city),
                Tags = ingredient.Tags,
                Color = ingredient.Color,
                IconId = ingredient.IconId
            });
        }

        return prices.OrderBy(p => p.Rarity).ThenBy(p => p.IngredientName).ToList();
    }

    public async Task<MarketPriceDto?> GetIngredientPriceAsync(string cityId, string ingredientId)
    {
        var prices = await GetCurrentPricesAsync(cityId);
        return prices.FirstOrDefault(p => p.IngredientId == ingredientId);
    }

    public async Task<bool> BuyIngredientAsync(string ingredientId, int quantity)
    {
        if (quantity <= 0)
            return false;

        var ingredient = await _databaseService.GetIngredientAsync(ingredientId);
        if (ingredient == null)
            return false;

        var playerState = _gameStateService.PlayerState;
        var currentCity = _gameStateService.CurrentCity;
        
        // Calculate total cost
        var pricePerUnit = CalculatePrice(ingredient, currentCity, _gameStateService.CurrentDay);
        var totalCost = (int)Math.Ceiling(pricePerUnit * quantity);

        // Check if player can afford
        if (playerState.Coins < totalCost)
            return false;

        // Check inventory capacity
        if (!playerState.CanAddToInventory(ingredient, quantity))
            return false;

        // Execute transaction
        playerState.Coins -= totalCost;
        playerState.AddToInventory(ingredient, quantity);

        await _gameStateService.SaveGameAsync();

        // Track price history
        TrackPrice(ingredientId, pricePerUnit);

        return true;
    }

    public async Task<bool> SellIngredientAsync(string ingredientId, int quantity)
    {
        if (quantity <= 0)
            return false;

        var ingredient = await _databaseService.GetIngredientAsync(ingredientId);
        if (ingredient == null)
            return false;

        var playerState = _gameStateService.PlayerState;
        
        // Check if player has enough
        if (!playerState.Inventory.ContainsKey(ingredientId) || 
            playerState.Inventory[ingredientId] < quantity)
            return false;

        var currentCity = _gameStateService.CurrentCity;
        
        // Calculate sell price (80% of buy price)
        var pricePerUnit = CalculatePrice(ingredient, currentCity, _gameStateService.CurrentDay);
        var sellPrice = pricePerUnit * 0.8m;
        var totalRevenue = (int)Math.Floor(sellPrice * quantity);

        // Execute transaction
        playerState.RemoveFromInventory(ingredientId, quantity);
        playerState.CurrentWeight -= ingredient.Weight * quantity;
        playerState.Coins += totalRevenue;

        await _gameStateService.SaveGameAsync();

        return true;
    }

    public async Task<bool> SellCraftedDreamAsync(string dreamId)
    {
        var playerState = _gameStateService.PlayerState;
        var dream = playerState.CraftedDreams.FirstOrDefault(d => d.Id == dreamId);
        
        if (dream == null)
            return false;

        // Crafted dreams sell for full value
        var salePrice = (int)Math.Floor(dream.Value);
        
        playerState.Coins += salePrice;
        playerState.CraftedDreams.Remove(dream);

        // Reputation gain for selling rare dreams
        var reputationGain = dream.Rarity switch
        {
            Rarity.Rare => 2,
            Rarity.Epic => 5,
            Rarity.Legendary => 10,
            _ => 1
        };

        await _gameStateService.UpdateReputationAsync(reputationGain, 0, reputationGain);
        await _gameStateService.SaveGameAsync();

        return true;
    }

    public async Task UpdateMarketPricesAsync()
    {
        // Called when day progresses
        // Prices are calculated dynamically, but we can update trends here
        var currentDay = _gameStateService.CurrentDay;
        
        // Every 7 days, trigger a random market event
        if (currentDay % 7 == 0)
        {
            await _eventService.TriggerRandomMarketEventAsync();
        }

        await Task.CompletedTask;
    }

    public async Task<Dictionary<string, decimal>> GetPriceHistoryAsync(string ingredientId, int days)
    {
        var history = new Dictionary<string, decimal>();
        var currentDay = _gameStateService.CurrentDay;
        var ingredient = await _databaseService.GetIngredientAsync(ingredientId);
        var city = _gameStateService.CurrentCity;

        if (ingredient == null)
            return history;

        for (int i = days - 1; i >= 0; i--)
        {
            var day = currentDay - i;
            var price = CalculatePrice(ingredient, city, day);
            history[$"Day {day}"] = price;
        }

        return history;
    }

    public async Task<List<MarketTrendDto>> GetMarketTrendsAsync(string cityId)
    {
        var prices = await GetCurrentPricesAsync(cityId);
        
        return prices
            .Where(p => p.IsTrending)
            .OrderByDescending(p => Math.Abs(p.PriceChangePercent))
            .Take(5)
            .Select(p => new MarketTrendDto
            {
                IngredientId = p.IngredientId,
                IngredientName = p.IngredientName,
                Direction = p.PriceChangePercent > 0 ? "Rising" : "Falling",
                ChangePercent = p.PriceChangePercent
            })
            .ToList();
    }

    // Private helper methods

    private decimal CalculatePrice(Ingredient ingredient, City city, int day)
    {
        var basePrice = ingredient.BaseValue;
        var rarityModifier = GetRarityModifier(ingredient.Rarity);
        var cityModifier = GetCityModifier(ingredient, city);
        var eventMultiplier = _eventService.GetEventMultiplier(ingredient.Id, city.Id);
        var noise = GetPriceNoise(day, ingredient.Id);

        var price = basePrice * rarityModifier * cityModifier * eventMultiplier * noise;

        // Clamp to reasonable bounds
        var minPrice = basePrice * GameConstants.MIN_PRICE_MULTIPLIER;
        var maxPrice = basePrice * GameConstants.MAX_PRICE_MULTIPLIER;

        return Math.Clamp(price, minPrice, maxPrice);
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
        var seed = day * 1000 + ingredientId.GetHashCode();
        var random = new Random(seed);
        return (decimal)(0.9 + random.NextDouble() * 0.2); // 0.9 to 1.1
    }

    private int GetAvailableQuantity(Ingredient ingredient, City city)
    {
        // Simulate market availability
        // Rarer items are less available
        return ingredient.Rarity switch
        {
            Rarity.Common => 50,
            Rarity.Uncommon => 30,
            Rarity.Rare => 15,
            Rarity.Epic => 8,
            Rarity.Legendary => 3,
            _ => 20
        };
    }

    private void TrackPrice(string ingredientId, decimal price)
    {
        if (!_priceHistory.ContainsKey(ingredientId))
        {
            _priceHistory[ingredientId] = new List<decimal>();
        }

        _priceHistory[ingredientId].Add(price);

        // Keep only last 30 days
        if (_priceHistory[ingredientId].Count > 30)
        {
            _priceHistory[ingredientId].RemoveAt(0);
        }
    }
}
```

## Supporting DTOs

### MarketTrendDto

```csharp
namespace DreamAlchemist.Models.DTOs;

public class MarketTrendDto
{
    public string IngredientId { get; set; } = string.Empty;
    public string IngredientName { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty; // "Rising" or "Falling"
    public decimal ChangePercent { get; set; }
}
```

## Market Simulation Examples

### Example 1: Basic Price Calculation

```csharp
// Ingredient: Echo Dust
// Base Value: 120
// Rarity: Uncommon (1.5x)
// City: Somnia Terminal (Memory: 1.2x, Sound: 1.1x)
// Event: None (1.0x)
// Noise: 1.05

price = 120 × 1.5 × (1.2 × 1.1) × 1.0 × 1.05
price = 120 × 1.5 × 1.32 × 1.05
price = 249.48 LucidCoins
```

### Example 2: With Event Modifier

```csharp
// Same as above, but with "Lucidity Surge" event
// Event multiplies calming ingredients by 3.0x

price = 120 × 1.5 × 1.32 × 3.0 × 1.05
price = 748.44 LucidCoins (3x increase!)
```

## Market Update Flow

```
Day Progresses
    ↓
UpdateMarketPricesAsync()
    ↓
Check for Weekly Events (Day % 7 == 0)
    ↓
Recalculate All Prices
    ↓
Update Price History
    ↓
Notify ViewModels via Events
```

## Performance Considerations

### Caching Strategy

```csharp
private readonly Dictionary<string, (DateTime timestamp, List<MarketPriceDto> prices)> _priceCache = new();
private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

public async Task<List<MarketPriceDto>> GetCurrentPricesAsync(string cityId)
{
    // Check cache
    if (_priceCache.TryGetValue(cityId, out var cached))
    {
        if (DateTime.UtcNow - cached.timestamp < _cacheExpiration)
        {
            return cached.prices;
        }
    }

    // Calculate prices
    var prices = await CalculatePricesAsync(cityId);
    
    // Update cache
    _priceCache[cityId] = (DateTime.UtcNow, prices);
    
    return prices;
}
```

### Bulk Price Calculation

Instead of calculating prices individually, batch calculate for all ingredients:

```csharp
private async Task<List<MarketPriceDto>> CalculatePricesInBatchAsync(string cityId)
{
    var ingredients = await _databaseService.GetIngredientsAsync();
    var city = await _databaseService.GetCityAsync(cityId);
    var currentDay = _gameStateService.CurrentDay;

    // Parallel calculation for large ingredient sets
    var prices = await Task.WhenAll(
        ingredients.Select(async ingredient => 
            await Task.Run(() => CalculatePrice(ingredient, city, currentDay))
        )
    );

    return MapToDtos(ingredients, prices);
}
```

## Testing Market System

### Unit Tests

```csharp
[Fact]
public void CalculatePrice_WithNoModifiers_ReturnsBaseValueTimesRarity()
{
    // Arrange
    var ingredient = new Ingredient 
    { 
        BaseValue = 100, 
        Rarity = Rarity.Uncommon 
    };
    var city = new City { TagModifiers = new() };
    
    // Act
    var price = _marketService.CalculatePrice(ingredient, city, 1);
    
    // Assert (1.5x for Uncommon, plus noise ~1.0)
    Assert.InRange(price, 135m, 165m);
}

[Fact]
public async Task BuyIngredient_WithInsufficientFunds_ReturnsFalse()
{
    // Arrange
    _gameStateService.PlayerState.Coins = 100;
    
    // Act
    var result = await _marketService.BuyIngredientAsync("expensive_item", 10);
    
    // Assert
    Assert.False(result);
    Assert.Equal(100, _gameStateService.PlayerState.Coins);
}
```

## Next Steps

Proceed to **06-crafting-system.md** for dream synthesis implementation.
