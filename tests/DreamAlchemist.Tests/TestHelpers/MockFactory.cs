using DreamAlchemist.Models.Entities;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Services.Game;

namespace DreamAlchemist.Tests.TestHelpers;

/// <summary>
/// Factory for creating mock objects and test data
/// </summary>
public static class MockFactory
{
    #region Mock Services

    public static Mock<IDatabaseService> CreateMockDatabaseService()
    {
        var mock = new Mock<IDatabaseService>();
        
        // Setup default behaviors
        mock.Setup(x => x.InitializeDatabaseAsync()).ReturnsAsync(true);
        mock.Setup(x => x.GetIngredientsAsync()).ReturnsAsync(TestDataBuilder.CreateTestIngredients());
        mock.Setup(x => x.GetRecipesAsync()).ReturnsAsync(TestDataBuilder.CreateTestRecipes());
        mock.Setup(x => x.GetCitiesAsync()).ReturnsAsync(TestDataBuilder.CreateTestCities());
        mock.Setup(x => x.GetEventsAsync()).ReturnsAsync(TestDataBuilder.CreateTestEvents());
        
        return mock;
    }

    public static Mock<IGameStateService> CreateMockGameStateService()
    {
        var mock = new Mock<IGameStateService>();
        
        // Setup default state
        var playerState = TestDataBuilder.CreateTestPlayerState();
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        
        mock.Setup(x => x.PlayerState).Returns(playerState);
        mock.Setup(x => x.CurrentCity).Returns(city);
        mock.Setup(x => x.CurrentDay).Returns(1);
        mock.Setup(x => x.SaveGameAsync()).ReturnsAsync(true);
        mock.Setup(x => x.LoadGameAsync()).ReturnsAsync(true);
        
        return mock;
    }

    public static Mock<INavigationService> CreateMockNavigationService()
    {
        var mock = new Mock<INavigationService>();
        
        mock.Setup(x => x.NavigateToAsync(It.IsAny<string>())).ReturnsAsync(true);
        mock.Setup(x => x.GoBackAsync()).ReturnsAsync(true);
        
        return mock;
    }

    public static Mock<IMarketService> CreateMockMarketService()
    {
        var mock = new Mock<IMarketService>();
        
        mock.Setup(x => x.GetCurrentPricesAsync(It.IsAny<string>()))
            .ReturnsAsync(TestDataBuilder.CreateTestMarketPrices());
        mock.Setup(x => x.BuyIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        mock.Setup(x => x.SellIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        
        return mock;
    }

    public static Mock<ICraftingService> CreateMockCraftingService()
    {
        var mock = new Mock<ICraftingService>();
        
        mock.Setup(x => x.CraftDreamAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(TestDataBuilder.CreateTestCraftResult(true));
        
        return mock;
    }

    public static Mock<IInventoryService> CreateMockInventoryService()
    {
        var mock = new Mock<IInventoryService>();
        
        mock.Setup(x => x.GetCurrentCapacity()).Returns(100m);
        mock.Setup(x => x.GetMaxCapacity()).Returns(200m);
        mock.Setup(x => x.CanAddItem(It.IsAny<Ingredient>(), It.IsAny<int>()))
            .Returns(true);
        
        return mock;
    }

    public static Mock<ITravelService> CreateMockTravelService()
    {
        var mock = new Mock<ITravelService>();
        
        mock.Setup(x => x.TravelToCityAsync(It.IsAny<string>()))
            .ReturnsAsync(TestDataBuilder.CreateTestTravelResult(true));
        mock.Setup(x => x.GetTravelCost(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(50);
        
        return mock;
    }

    public static Mock<IEventService> CreateMockEventService()
    {
        var mock = new Mock<IEventService>();
        
        mock.Setup(x => x.GetEventMultiplier(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(1.0m);
        mock.Setup(x => x.TriggerRandomEventAsync())
            .ReturnsAsync((GameEvent?)null);
        
        return mock;
    }

    #endregion

    #region Setup Helpers

    public static void SetupIngredientRetrieval(Mock<IDatabaseService> mockDb, Ingredient ingredient)
    {
        mockDb.Setup(x => x.GetIngredientAsync(ingredient.Id))
            .ReturnsAsync(ingredient);
    }

    public static void SetupCityRetrieval(Mock<IDatabaseService> mockDb, City city)
    {
        mockDb.Setup(x => x.GetCityAsync(city.Id))
            .ReturnsAsync(city);
    }

    public static void SetupRecipeRetrieval(Mock<IDatabaseService> mockDb, Recipe recipe)
    {
        mockDb.Setup(x => x.GetRecipeAsync(recipe.Id))
            .ReturnsAsync(recipe);
    }

    public static void SetupPlayerStateWithInventory(Mock<IGameStateService> mockGameState, Dictionary<string, int> inventory)
    {
        var playerState = TestDataBuilder.CreateTestPlayerState();
        playerState.Inventory = new Dictionary<string, int>(inventory);
        
        mockGameState.Setup(x => x.PlayerState).Returns(playerState);
    }

    public static void SetupPlayerStateWithCoins(Mock<IGameStateService> mockGameState, int coins)
    {
        var playerState = TestDataBuilder.CreateTestPlayerState();
        playerState.Coins = coins;
        
        mockGameState.Setup(x => x.PlayerState).Returns(playerState);
    }

    #endregion
}
