# 03 - Data Models

## Overview

This document defines all data models, entities, enums, and DTOs used throughout Dream Alchemist.

## Entity Models (Database)

### Ingredient Entity

```csharp
using SQLite;
using Newtonsoft.Json;

namespace DreamAlchemist.Models.Entities;

[Table("Ingredients")]
public class Ingredient
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Rarity Rarity { get; set; }

    public decimal BaseValue { get; set; }

    /// <summary>
    /// Weight for inventory capacity calculation
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Whether this ingredient can trigger volatility events
    /// </summary>
    public bool Volatile { get; set; }

    /// <summary>
    /// Tags for market modifiers and recipe matching
    /// Stored as JSON string in SQLite
    /// </summary>
    [Ignore]
    public List<DreamTag> Tags { get; set; } = new();

    [Column("TagsJson")]
    public string TagsJson
    {
        get => JsonConvert.SerializeObject(Tags);
        set => Tags = JsonConvert.DeserializeObject<List<DreamTag>>(value) ?? new();
    }

    /// <summary>
    /// Visual representation color (hex)
    /// </summary>
    public string Color { get; set; } = "#8B5CF6";

    /// <summary>
    /// Icon identifier for UI
    /// </summary>
    public string IconId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### Recipe Entity

```csharp
using SQLite;
using Newtonsoft.Json;

namespace DreamAlchemist.Models.Entities;

[Table("Recipes")]
public class Recipe
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Rarity Rarity { get; set; }

    /// <summary>
    /// Required ingredient IDs (2-3 ingredients)
    /// Stored as JSON array
    /// </summary>
    [Ignore]
    public List<string> RequiredIngredients { get; set; } = new();

    [Column("RequiredIngredientsJson")]
    public string RequiredIngredientsJson
    {
        get => JsonConvert.SerializeObject(RequiredIngredients);
        set => RequiredIngredients = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Dream alignment (e.g., melancholic, joyful, fearful)
    /// </summary>
    public string Alignment { get; set; } = string.Empty;

    /// <summary>
    /// Multiplier applied to base ingredient values
    /// </summary>
    public decimal ValueMultiplier { get; set; }

    /// <summary>
    /// Whether player has discovered this recipe
    /// </summary>
    public bool Discovered { get; set; }

    /// <summary>
    /// Tags inherited from ingredients plus unique tags
    /// </summary>
    [Ignore]
    public List<DreamTag> Tags { get; set; } = new();

    [Column("TagsJson")]
    public string TagsJson
    {
        get => JsonConvert.SerializeObject(Tags);
        set => Tags = JsonConvert.DeserializeObject<List<DreamTag>>(value) ?? new();
    }

    /// <summary>
    /// Special effect when consumed (optional)
    /// </summary>
    public string? EffectId { get; set; }

    /// <summary>
    /// AI-generated narrative description (optional)
    /// </summary>
    public string? NarrativeText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### City Entity

```csharp
using SQLite;
using Newtonsoft.Json;

namespace DreamAlchemist.Models.Entities;

[Table("Cities")]
public class City
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Visual theme color
    /// </summary>
    public string ThemeColor { get; set; } = "#8B5CF6";

    /// <summary>
    /// Ambient music track identifier
    /// </summary>
    public string MusicTrackId { get; set; } = string.Empty;

    /// <summary>
    /// Market modifiers for dream tags
    /// Key: DreamTag, Value: Multiplier
    /// </summary>
    [Ignore]
    public Dictionary<DreamTag, decimal> TagModifiers { get; set; } = new();

    [Column("TagModifiersJson")]
    public string TagModifiersJson
    {
        get => JsonConvert.SerializeObject(TagModifiers);
        set => TagModifiers = JsonConvert.DeserializeObject<Dictionary<DreamTag, decimal>>(value) ?? new();
    }

    /// <summary>
    /// Available event IDs for this city
    /// </summary>
    [Ignore]
    public List<string> EventPool { get; set; } = new();

    [Column("EventPoolJson")]
    public string EventPoolJson
    {
        get => JsonConvert.SerializeObject(EventPool);
        set => EventPool = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Travel cost from starting city
    /// </summary>
    public int TravelCost { get; set; }

    /// <summary>
    /// Days required to travel here
    /// </summary>
    public int TravelDays { get; set; }

    /// <summary>
    /// Minimum reputation required to access
    /// </summary>
    public int RequiredReputation { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### GameEvent Entity

```csharp
using SQLite;
using Newtonsoft.Json;

namespace DreamAlchemist.Models.Entities;

[Table("GameEvents")]
public class GameEvent
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public EventType Type { get; set; }

    /// <summary>
    /// Probability of this event occurring (0.0 to 1.0)
    /// </summary>
    public double Probability { get; set; }

    /// <summary>
    /// Duration in days
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Price modifiers by ingredient ID
    /// Key: IngredientId, Value: Multiplier
    /// </summary>
    [Ignore]
    public Dictionary<string, decimal> PriceModifiers { get; set; } = new();

    [Column("PriceModifiersJson")]
    public string PriceModifiersJson
    {
        get => JsonConvert.SerializeObject(PriceModifiers);
        set => PriceModifiers = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(value) ?? new();
    }

    /// <summary>
    /// Tag-based modifiers (affects all ingredients with tag)
    /// </summary>
    [Ignore]
    public Dictionary<DreamTag, decimal> TagModifiers { get; set; } = new();

    [Column("TagModifiersJson")]
    public string TagModifiersJson
    {
        get => JsonConvert.SerializeObject(TagModifiers);
        set => TagModifiers = JsonConvert.DeserializeObject<Dictionary<DreamTag, decimal>>(value) ?? new();
    }

    /// <summary>
    /// Reputation change when event triggers
    /// </summary>
    public int ReputationEffect { get; set; }

    /// <summary>
    /// Narrative text for event popup
    /// </summary>
    public string NarrativeText { get; set; } = string.Empty;

    /// <summary>
    /// Optional choices for player
    /// </summary>
    [Ignore]
    public List<EventChoice>? Choices { get; set; }

    [Column("ChoicesJson")]
    public string? ChoicesJson
    {
        get => Choices != null ? JsonConvert.SerializeObject(Choices) : null;
        set => Choices = !string.IsNullOrEmpty(value) 
            ? JsonConvert.DeserializeObject<List<EventChoice>>(value) 
            : null;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### PlayerState Entity

```csharp
using SQLite;
using Newtonsoft.Json;

namespace DreamAlchemist.Models.Entities;

[Table("PlayerState")]
public class PlayerState
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string PlayerName { get; set; } = "Novice Peddler";

    public int Coins { get; set; } = 5000;

    public int CurrentDay { get; set; } = 1;

    public string CurrentCityId { get; set; } = "somnia_terminal";

    /// <summary>
    /// Player progression tier (1-5)
    /// </summary>
    public int Tier { get; set; } = 1;

    /// <summary>
    /// Reputation scores
    /// </summary>
    public int TrustReputation { get; set; }
    public int InfamyReputation { get; set; }
    public int LucidityReputation { get; set; }

    /// <summary>
    /// Total inventory weight
    /// </summary>
    public int CurrentWeight { get; set; }

    /// <summary>
    /// Maximum inventory capacity
    /// </summary>
    public int MaxWeight { get; set; } = 100;

    /// <summary>
    /// Inventory items: Key = IngredientId, Value = Quantity
    /// </summary>
    [Ignore]
    public Dictionary<string, int> Inventory { get; set; } = new();

    [Column("InventoryJson")]
    public string InventoryJson
    {
        get => JsonConvert.SerializeObject(Inventory);
        set => Inventory = JsonConvert.DeserializeObject<Dictionary<string, int>>(value) ?? new();
    }

    /// <summary>
    /// Crafted dreams in storage
    /// </summary>
    [Ignore]
    public List<CraftedDream> CraftedDreams { get; set; } = new();

    [Column("CraftedDreamsJson")]
    public string CraftedDreamsJson
    {
        get => JsonConvert.SerializeObject(CraftedDreams);
        set => CraftedDreams = JsonConvert.DeserializeObject<List<CraftedDream>>(value) ?? new();
    }

    /// <summary>
    /// Discovered recipe IDs
    /// </summary>
    [Ignore]
    public List<string> DiscoveredRecipes { get; set; } = new();

    [Column("DiscoveredRecipesJson")]
    public string DiscoveredRecipesJson
    {
        get => JsonConvert.SerializeObject(DiscoveredRecipes);
        set => DiscoveredRecipes = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Unlocked cities
    /// </summary>
    [Ignore]
    public List<string> UnlockedCities { get; set; } = new() { "somnia_terminal" };

    [Column("UnlockedCitiesJson")]
    public string UnlockedCitiesJson
    {
        get => JsonConvert.SerializeObject(UnlockedCities);
        set => UnlockedCities = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Active event instances
    /// </summary>
    [Ignore]
    public List<ActiveEvent> ActiveEvents { get; set; } = new();

    [Column("ActiveEventsJson")]
    public string ActiveEventsJson
    {
        get => JsonConvert.SerializeObject(ActiveEvents);
        set => ActiveEvents = JsonConvert.DeserializeObject<List<ActiveEvent>>(value) ?? new();
    }

    public DateTime LastSaved { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Helper methods
    public bool CanAddToInventory(Ingredient ingredient, int quantity)
    {
        var totalWeight = CurrentWeight + (ingredient.Weight * quantity);
        return totalWeight <= MaxWeight;
    }

    public void AddToInventory(Ingredient ingredient, int quantity)
    {
        if (Inventory.ContainsKey(ingredient.Id))
        {
            Inventory[ingredient.Id] += quantity;
        }
        else
        {
            Inventory[ingredient.Id] = quantity;
        }
        CurrentWeight += ingredient.Weight * quantity;
    }

    public bool RemoveFromInventory(string ingredientId, int quantity)
    {
        if (!Inventory.ContainsKey(ingredientId) || Inventory[ingredientId] < quantity)
            return false;

        Inventory[ingredientId] -= quantity;
        if (Inventory[ingredientId] == 0)
        {
            Inventory.Remove(ingredientId);
        }
        
        return true;
    }
}
```

## Supporting Models

### CraftedDream

```csharp
namespace DreamAlchemist.Models;

public class CraftedDream
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RecipeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public Rarity Rarity { get; set; }
    public string NarrativeText { get; set; } = string.Empty;
    public DateTime CraftedAt { get; set; } = DateTime.UtcNow;
}
```

### EventChoice

```csharp
namespace DreamAlchemist.Models;

public class EventChoice
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int CoinsCost { get; set; }
    public int ReputationEffect { get; set; }
    public string? ResultText { get; set; }
    public Dictionary<string, int>? ItemRewards { get; set; }
}
```

### ActiveEvent

```csharp
namespace DreamAlchemist.Models;

public class ActiveEvent
{
    public string EventId { get; set; } = string.Empty;
    public string CityId { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
}
```

## Enumerations

### Rarity Enum

```csharp
namespace DreamAlchemist.Models.Enums;

public enum Rarity
{
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Epic = 4,
    Legendary = 5
}
```

### DreamTag Enum

```csharp
namespace DreamAlchemist.Models.Enums;

public enum DreamTag
{
    Memory,
    Fear,
    Joy,
    Melancholy,
    Sound,
    Vision,
    Touch,
    Childhood,
    Loss,
    Hope,
    Chaos,
    Order,
    Nature,
    Urban,
    Surreal,
    Lucid,
    Nightmare,
    Fantasy
}
```

### EventType Enum

```csharp
namespace DreamAlchemist.Models.Enums;

public enum EventType
{
    Market,           // Affects prices
    Random,           // Random encounter
    Story,            // Narrative progression
    Raid,             // Police/danger
    Opportunity,      // Special deal
    Discovery,        // New recipe/city
    Reputation        // Affects standing
}
```

### PlayerTier Enum

```csharp
namespace DreamAlchemist.Models.Enums;

public enum PlayerTier
{
    NovicePeddler = 1,
    DreamArtisan = 2,
    DreamBroker = 3,
    DreamCartelLeader = 4,
    LucidArchitect = 5
}
```

## Data Transfer Objects (DTOs)

### MarketPriceDto

```csharp
namespace DreamAlchemist.Models.DTOs;

public class MarketPriceDto
{
    public string IngredientId { get; set; } = string.Empty;
    public string IngredientName { get; set; } = string.Empty;
    public Rarity Rarity { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal BasePrice { get; set; }
    public decimal PriceChangePercent { get; set; }
    public bool IsTrending { get; set; }
    public int AvailableQuantity { get; set; }
    public List<DreamTag> Tags { get; set; } = new();
    public string Color { get; set; } = string.Empty;
    public string IconId { get; set; } = string.Empty;
}
```

### CraftResultDto

```csharp
namespace DreamAlchemist.Models.DTOs;

public class CraftResultDto
{
    public bool Success { get; set; }
    public string? RecipeId { get; set; }
    public string? RecipeName { get; set; }
    public bool NewDiscovery { get; set; }
    public CraftedDream? CraftedDream { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? NarrativeText { get; set; }
}
```

### TravelResultDto

```csharp
namespace DreamAlchemist.Models.DTOs;

public class TravelResultDto
{
    public bool Success { get; set; }
    public string DestinationCityId { get; set; } = string.Empty;
    public string DestinationCityName { get; set; } = string.Empty;
    public int DaysPassed { get; set; }
    public int CoinsCost { get; set; }
    public List<string> EventsTriggered { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
```

### ReputationDto

```csharp
namespace DreamAlchemist.Models.DTOs;

public class ReputationDto
{
    public int Trust { get; set; }
    public int Infamy { get; set; }
    public int Lucidity { get; set; }
    public string TrustLevel { get; set; } = string.Empty;
    public string InfamyLevel { get; set; } = string.Empty;
    public string LucidityLevel { get; set; } = string.Empty;
    public List<string> UnlockedFeatures { get; set; } = new();
}
```

## Model Extensions

### IngredientExtensions

```csharp
namespace DreamAlchemist.Models.Extensions;

public static class IngredientExtensions
{
    public static string GetRarityColor(this Ingredient ingredient)
    {
        return ingredient.Rarity switch
        {
            Rarity.Common => "#9CA3AF",
            Rarity.Uncommon => "#10B981",
            Rarity.Rare => "#3B82F6",
            Rarity.Epic => "#A855F7",
            Rarity.Legendary => "#F59E0B",
            _ => "#9CA3AF"
        };
    }

    public static string GetRarityName(this Ingredient ingredient)
    {
        return ingredient.Rarity.ToString();
    }

    public static bool HasTag(this Ingredient ingredient, DreamTag tag)
    {
        return ingredient.Tags.Contains(tag);
    }
}
```

### RecipeExtensions

```csharp
namespace DreamAlchemist.Models.Extensions;

public static class RecipeExtensions
{
    public static bool CanCraft(this Recipe recipe, Dictionary<string, int> inventory)
    {
        foreach (var ingredientId in recipe.RequiredIngredients)
        {
            if (!inventory.ContainsKey(ingredientId) || inventory[ingredientId] < 1)
            {
                return false;
            }
        }
        return true;
    }

    public static decimal CalculateValue(this Recipe recipe, List<Ingredient> ingredients)
    {
        var baseValue = ingredients
            .Where(i => recipe.RequiredIngredients.Contains(i.Id))
            .Sum(i => i.BaseValue);
        
        return baseValue * recipe.ValueMultiplier;
    }
}
```

## Constants

```csharp
namespace DreamAlchemist.Models;

public static class GameConstants
{
    // Starting values
    public const int STARTING_COINS = 5000;
    public const int STARTING_WEIGHT_CAPACITY = 100;
    public const string STARTING_CITY = "somnia_terminal";

    // Tier thresholds
    public static readonly Dictionary<int, string> TierNames = new()
    {
        { 1, "Novice Peddler" },
        { 2, "Dream Artisan" },
        { 3, "Dream Broker" },
        { 4, "Dream Cartel Leader" },
        { 5, "Lucid Architect" }
    };

    // Reputation levels
    public const int REPUTATION_MAX = 100;
    public const int REPUTATION_MIN = -100;

    // Economy
    public const decimal MIN_PRICE_MULTIPLIER = 0.5m;
    public const decimal MAX_PRICE_MULTIPLIER = 3.0m;

    // Crafting
    public const int MIN_INGREDIENTS_PER_RECIPE = 2;
    public const int MAX_INGREDIENTS_PER_RECIPE = 3;

    // Events
    public const double BASE_EVENT_PROBABILITY = 0.15;
    public const int MAX_SIMULTANEOUS_EVENTS = 3;
}
```

## Next Steps

Proceed to **04-core-services.md** for service implementations.
