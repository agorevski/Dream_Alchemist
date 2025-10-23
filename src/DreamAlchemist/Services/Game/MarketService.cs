using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Models.Entities;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Helpers;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;

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
        
        // Subscribe to day progression to update market
        _gameStateService.DayProgressed += OnDayProgressed;
    }

    private async void OnDayProgressed(object? sender, int days)
    {
        await UpdateMarketPricesAsync();
    }

    public async Task<List<MarketPriceDto>> GetCurrentPricesAsync(string cityId)
    {
        System.Diagnostics.Debug.WriteLine($"MarketService: GetCurrentPricesAsync called for city: {cityId}");
        
        var ingredients = await _databaseService.GetIngredientsAsync();
        System.Diagnostics.Debug.WriteLine($"MarketService: Retrieved {ingredients?.Count ?? 0} ingredients from database");
        
        var city = await _databaseService.GetCityAsync(cityId);
        System.Diagnostics.Debug.WriteLine($"MarketService: Retrieved city: {city?.Name ?? "NULL"}");
        
        if (city == null)
            throw new ArgumentException("City not found", nameof(cityId));
        
        if (ingredients == null || ingredients.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine("No ingredients found in database");
            return new List<MarketPriceDto>();
        }

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

        System.Diagnostics.Debug.WriteLine($"MarketService: Created {prices.Count} price DTOs");
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
        var currentDay = _gameStateService.CurrentDay;
        
        // Every 7 days, check for potential market events
        if (currentDay % 7 == 0)
        {
            // Trigger random market event (will be implemented in EventService)
            System.Diagnostics.Debug.WriteLine($"Day {currentDay}: Market event check");
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
            if (day < 1) continue;
            
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
        // Formula: price = base_value × rarity_modifier × city_modifier × event_multiplier × noise
        
        var basePrice = ingredient.BaseValue;
        var rarityModifier = GetRarityModifier(ingredient.Rarity);
        var cityModifier = GetCityModifier(ingredient, city);
        var eventMultiplier = GetEventMultiplier(ingredient.Id, city.Id);
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

    private decimal GetEventMultiplier(string ingredientId, string cityId)
    {
        return _eventService.GetEventMultiplier(ingredientId, cityId);
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
        // Simulate market availability based on rarity
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
