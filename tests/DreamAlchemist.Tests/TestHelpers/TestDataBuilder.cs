using DreamAlchemist.Models.Entities;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Models.Supporting;

namespace DreamAlchemist.Tests.TestHelpers;

/// <summary>
/// Builder for creating test data objects
/// </summary>
public static class TestDataBuilder
{
    #region Ingredients

    public static List<Ingredient> CreateTestIngredients()
    {
        return new List<Ingredient>
        {
            CreateTestIngredient("ing1", "Starlight Essence", Rarity.Common, 10m, 1m),
            CreateTestIngredient("ing2", "Moonstone Shard", Rarity.Uncommon, 25m, 2m),
            CreateTestIngredient("ing3", "Phoenix Feather", Rarity.Rare, 50m, 3m),
            CreateTestIngredient("ing4", "Dragon Scale", Rarity.Epic, 100m, 5m),
            CreateTestIngredient("ing5", "Void Crystal", Rarity.Legendary, 250m, 10m)
        };
    }

    public static Ingredient CreateTestIngredient(
        string id,
        string name,
        Rarity rarity = Rarity.Common,
        decimal baseValue = 10m,
        decimal weight = 1m)
    {
        return new Ingredient
        {
            Id = id,
            Name = name,
            Description = $"Test ingredient: {name}",
            Rarity = rarity,
            BaseValue = baseValue,
            Weight = weight,
            Tags = new List<DreamTag> { DreamTag.Mystical, DreamTag.Ethereal },
            Color = "#8B5CF6",
            IconId = "icon_test"
        };
    }

    #endregion

    #region Recipes

    public static List<Recipe> CreateTestRecipes()
    {
        return new List<Recipe>
        {
            CreateTestRecipe("recipe1", "Basic Dream", new[] { "ing1", "ing2" }, Rarity.Common),
            CreateTestRecipe("recipe2", "Mystic Vision", new[] { "ing2", "ing3" }, Rarity.Uncommon),
            CreateTestRecipe("recipe3", "Epic Dream", new[] { "ing3", "ing4" }, Rarity.Epic)
        };
    }

    public static Recipe CreateTestRecipe(
        string id,
        string name,
        string[] ingredientIds,
        Rarity resultRarity = Rarity.Common)
    {
        return new Recipe
        {
            Id = id,
            Name = name,
            Description = $"Test recipe: {name}",
            RequiredIngredientIds = ingredientIds.ToList(),
            ResultRarity = resultRarity,
            ResultValueMultiplier = 2.0m,
            IsDiscovered = true
        };
    }

    #endregion

    #region Cities

    public static List<City> CreateTestCities()
    {
        return new List<City>
        {
            CreateTestCity("city1", "Crystal Spire"),
            CreateTestCity("city2", "Shadow Market"),
            CreateTestCity("city3", "Moonlight Bay")
        };
    }

    public static City CreateTestCity(string id, string name)
    {
        return new City
        {
            Id = id,
            Name = name,
            Description = $"Test city: {name}",
            TagModifiers = new Dictionary<DreamTag, decimal>
            {
                { DreamTag.Mystical, 1.2m },
                { DreamTag.Ethereal, 1.1m },
                { DreamTag.Dark, 0.9m }
            },
            UnlockRequirement = 0
        };
    }

    #endregion

    #region Events

    public static List<GameEvent> CreateTestEvents()
    {
        return new List<GameEvent>
        {
            CreateTestEvent("event1", "Market Boom", EventType.Market),
            CreateTestEvent("event2", "Strange Encounter", EventType.Personal),
            CreateTestEvent("event3", "Lucky Find", EventType.Random)
        };
    }

    public static GameEvent CreateTestEvent(
        string id,
        string title,
        EventType eventType = EventType.Random)
    {
        return new GameEvent
        {
            Id = id,
            Title = title,
            Description = $"Test event: {title}",
            EventType = eventType,
            Probability = 0.1,
            DurationDays = 3,
            Effects = new Dictionary<string, decimal>
            {
                { "marketMultiplier", 1.5m }
            },
            Choices = new List<EventChoice>
            {
                new EventChoice
                {
                    Text = "Accept",
                    Consequences = new Dictionary<string, object>
                    {
                        { "coins", 100 }
                    }
                },
                new EventChoice
                {
                    Text = "Decline",
                    Consequences = new Dictionary<string, object>
                    {
                        { "reputation", 5 }
                    }
                }
            }
        };
    }

    #endregion

    #region Player State

    public static PlayerState CreateTestPlayerState()
    {
        return new PlayerState
        {
            Id = 1,
            PlayerName = "Test Player",
            Coins = 1000,
            CurrentCityId = "city1",
            CurrentDay = 1,
            CurrentTier = PlayerTier.Apprentice,
            Trust = 50,
            Infamy = 10,
            Lucidity = 30,
            Inventory = new Dictionary<string, int>(),
            DiscoveredRecipes = new List<string>(),
            CraftedDreams = new List<CraftedDream>(),
            ActiveEvents = new List<ActiveEvent>(),
            MaxCapacity = 100m,
            CurrentWeight = 0m
        };
    }

    #endregion

    #region DTOs

    public static List<MarketPriceDto> CreateTestMarketPrices()
    {
        return new List<MarketPriceDto>
        {
            new MarketPriceDto
            {
                IngredientId = "ing1",
                IngredientName = "Starlight Essence",
                Rarity = Rarity.Common,
                CurrentPrice = 12m,
                BasePrice = 10m,
                PriceChangePercent = 20m,
                IsTrending = true,
                AvailableQuantity = 50,
                Tags = new List<DreamTag> { DreamTag.Mystical },
                Color = "#8B5CF6",
                IconId = "icon_test"
            },
            new MarketPriceDto
            {
                IngredientId = "ing2",
                IngredientName = "Moonstone Shard",
                Rarity = Rarity.Uncommon,
                CurrentPrice = 24m,
                BasePrice = 25m,
                PriceChangePercent = -4m,
                IsTrending = false,
                AvailableQuantity = 30,
                Tags = new List<DreamTag> { DreamTag.Ethereal },
                Color = "#06B6D4",
                IconId = "icon_test"
            }
        };
    }

    public static CraftResultDto CreateTestCraftResult(bool success, string? dreamId = null)
    {
        if (success)
        {
            return new CraftResultDto
            {
                Success = true,
                CraftedDream = new CraftedDream
                {
                    Id = dreamId ?? "dream1",
                    Name = "Test Dream",
                    Description = "A crafted test dream",
                    Rarity = Rarity.Uncommon,
                    Value = 50m,
                    IngredientIds = new List<string> { "ing1", "ing2" },
                    CraftedOn = 1
                },
                WasNewRecipe = false,
                Message = "Dream crafted successfully!"
            };
        }
        else
        {
            return new CraftResultDto
            {
                Success = false,
                CraftedDream = null,
                WasNewRecipe = false,
                Message = "Crafting failed"
            };
        }
    }

    public static TravelResultDto CreateTestTravelResult(bool success)
    {
        if (success)
        {
            return new TravelResultDto
            {
                Success = true,
                NewCityId = "city2",
                DaysPassed = 1,
                CostPaid = 50,
                EventTriggered = null,
                Message = "Travel successful"
            };
        }
        else
        {
            return new TravelResultDto
            {
                Success = false,
                NewCityId = string.Empty,
                DaysPassed = 0,
                CostPaid = 0,
                EventTriggered = null,
                Message = "Travel failed"
            };
        }
    }

    public static ReputationDto CreateTestReputation()
    {
        return new ReputationDto
        {
            Trust = 50,
            Infamy = 10,
            Lucidity = 30,
            TrustDescription = "Trusted",
            InfamyDescription = "Curious",
            LucidityDescription = "Aware"
        };
    }

    #endregion

    #region Crafted Dreams

    public static CraftedDream CreateTestCraftedDream(
        string id = "dream1",
        string name = "Test Dream",
        Rarity rarity = Rarity.Common,
        decimal value = 50m)
    {
        return new CraftedDream
        {
            Id = id,
            Name = name,
            Description = $"A test dream: {name}",
            Rarity = rarity,
            Value = value,
            IngredientIds = new List<string> { "ing1", "ing2" },
            CraftedOn = 1
        };
    }

    #endregion

    #region Active Events

    public static ActiveEvent CreateTestActiveEvent(
        string eventId = "event1",
        int remainingDays = 3)
    {
        return new ActiveEvent
        {
            EventId = eventId,
            ActivatedOnDay = 1,
            RemainingDays = remainingDays,
            Effects = new Dictionary<string, decimal>
            {
                { "marketMultiplier", 1.5m }
            }
        };
    }

    #endregion
}
