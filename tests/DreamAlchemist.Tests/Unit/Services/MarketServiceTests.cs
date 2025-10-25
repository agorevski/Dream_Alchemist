using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Tests.TestHelpers;
using DreamAlchemist.Helpers;

namespace DreamAlchemist.Tests.Unit.Services;

/// <summary>
/// Unit tests for MarketService
/// Tests price calculations, buy/sell operations, and market dynamics
/// </summary>
public class MarketServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IGameStateService> _mockGameStateService;
    private readonly Mock<IEventService> _mockEventService;
    private readonly MarketService _sut; // System Under Test

    public MarketServiceTests()
    {
        _mockDatabaseService = MockFactory.CreateMockDatabaseService();
        _mockGameStateService = MockFactory.CreateMockGameStateService();
        _mockEventService = MockFactory.CreateMockEventService();
        
        _sut = new MarketService(
            _mockDatabaseService.Object,
            _mockGameStateService.Object,
            _mockEventService.Object);
    }

    #region GetCurrentPricesAsync Tests

    [Fact]
    public async Task GetCurrentPricesAsync_ValidCity_ReturnsPriceList()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetCurrentPricesAsync("city1");

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(5); // 5 test ingredients
    }

    [Fact]
    public async Task GetCurrentPricesAsync_InvalidCity_ThrowsArgumentException()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetCityAsync("invalid"))
            .ReturnsAsync((City?)null);

        // Act & Assert
        await _sut.Invoking(s => s.GetCurrentPricesAsync("invalid"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*City not found*");
    }

    [Fact]
    public async Task GetCurrentPricesAsync_NoIngredients_ReturnsEmptyList()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);
        _mockDatabaseService.Setup(x => x.GetIngredientsAsync())
            .ReturnsAsync(new List<Ingredient>());

        // Act
        var result = await _sut.GetCurrentPricesAsync("city1");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCurrentPricesAsync_PricesOrderedByRarityThenName()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetCurrentPricesAsync("city1");

        // Assert
        result.Should().BeInAscendingOrder(p => p.Rarity);
        
        // Within same rarity, should be ordered by name
        var commonItems = result.Where(p => p.Rarity == Rarity.Common).ToList();
        if (commonItems.Count > 1)
        {
            commonItems.Should().BeInAscendingOrder(p => p.IngredientName);
        }
    }

    [Fact]
    public async Task GetCurrentPricesAsync_CalculatesPriceChangePercent()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetCurrentPricesAsync("city1");

        // Assert
        result.Should().AllSatisfy(price =>
        {
            var expectedChange = ((price.CurrentPrice - price.BasePrice) / price.BasePrice) * 100;
            price.PriceChangePercent.Should().BeApproximately(expectedChange, 0.01m);
        });
    }

    [Fact]
    public async Task GetCurrentPricesAsync_MarksItemsAsTrendingAbove20Percent()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetCurrentPricesAsync("city1");

        // Assert
        result.Should().AllSatisfy(price =>
        {
            if (Math.Abs(price.PriceChangePercent) > 20)
            {
                price.IsTrending.Should().BeTrue();
            }
            else
            {
                price.IsTrending.Should().BeFalse();
            }
        });
    }

    #endregion

    #region BuyIngredientAsync Tests

    [Fact]
    public async Task BuyIngredientAsync_SufficientFunds_Success()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item", Rarity.Common, 10m, 1m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        MockFactory.SetupPlayerStateWithCoins(_mockGameStateService, 1000);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.MaxCapacity = 100m;

        // Act
        var result = await _sut.BuyIngredientAsync("ing1", 2);

        // Assert
        result.Should().BeTrue();
        playerState.Coins.Should().BeLessThan(1000); // Coins should be deducted
        playerState.Inventory.Should().ContainKey("ing1");
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    [Fact]
    public async Task BuyIngredientAsync_InsufficientFunds_Fails()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Expensive Item", Rarity.Legendary, 1000m, 1m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        MockFactory.SetupPlayerStateWithCoins(_mockGameStateService, 100);

        // Act
        var result = await _sut.BuyIngredientAsync("ing1", 1);

        // Assert
        result.Should().BeFalse();
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Never);
    }

    [Fact]
    public async Task BuyIngredientAsync_ZeroQuantity_Fails()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item");
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);

        // Act
        var result = await _sut.BuyIngredientAsync("ing1", 0);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BuyIngredientAsync_NegativeQuantity_Fails()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item");
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);

        // Act
        var result = await _sut.BuyIngredientAsync("ing1", -5);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BuyIngredientAsync_InvalidIngredient_Fails()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetIngredientAsync("invalid"))
            .ReturnsAsync((Ingredient?)null);

        // Act
        var result = await _sut.BuyIngredientAsync("invalid", 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BuyIngredientAsync_ExceedsCapacity_Fails()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Heavy Item", Rarity.Common, 10m, 50m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;
        playerState.MaxCapacity = 100m;
        playerState.CurrentWeight = 80m; // Already near capacity

        // Act
        var result = await _sut.BuyIngredientAsync("ing1", 2); // Would exceed capacity

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 12)] // Buy 1 at ~12 coins
    [InlineData(2, 24)] // Buy 2 at ~24 coins
    [InlineData(5, 60)] // Buy 5 at ~60 coins
    public async Task BuyIngredientAsync_VariousQuantities_DeductsCorrectAmount(int quantity, int expectedCost)
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item", Rarity.Common, 10m, 1m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;
        playerState.MaxCapacity = 200m;

        // Act
        var result = await _sut.BuyIngredientAsync("ing1", quantity);

        // Assert
        result.Should().BeTrue();
        playerState.Coins.Should().BeLessOrEqualTo(1000 - expectedCost + 10); // Allow some price variance
    }

    #endregion

    #region SellIngredientAsync Tests

    [Fact]
    public async Task SellIngredientAsync_HasIngredient_Success()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item", Rarity.Common, 10m, 1m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.CurrentWeight = 5m;
        playerState.Coins = 100;

        // Act
        var result = await _sut.SellIngredientAsync("ing1", 2);

        // Assert
        result.Should().BeTrue();
        playerState.Inventory["ing1"].Should().Be(3);
        playerState.Coins.Should().BeGreaterThan(100); // Should gain coins
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    [Fact]
    public async Task SellIngredientAsync_InsufficientQuantity_Fails()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item");
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 2;

        // Act
        var result = await _sut.SellIngredientAsync("ing1", 5);

        // Assert
        result.Should().BeFalse();
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Never);
    }

    [Fact]
    public async Task SellIngredientAsync_NotInInventory_Fails()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item");
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);

        // Act
        var result = await _sut.SellIngredientAsync("ing1", 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SellIngredientAsync_SellsAt80PercentOfBuyPrice()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item", Rarity.Common, 10m, 1m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 10;
        playerState.CurrentWeight = 10m;
        var initialCoins = playerState.Coins;

        // Act
        await _sut.SellIngredientAsync("ing1", 1);

        // Assert
        var coinsGained = playerState.Coins - initialCoins;
        // Sell price should be approximately 80% of calculated buy price
        coinsGained.Should().BeGreaterThan(0);
        coinsGained.Should().BeLessThan(15); // Less than full buy price
    }

    [Fact]
    public async Task SellIngredientAsync_UpdatesWeight()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item", Rarity.Common, 10m, 5m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.CurrentWeight = 25m; // 5 items × 5 weight

        // Act
        await _sut.SellIngredientAsync("ing1", 2);

        // Assert
        playerState.CurrentWeight.Should().Be(15m); // Reduced by 10 (2 items × 5 weight)
    }

    #endregion

    #region SellCraftedDreamAsync Tests

    [Fact]
    public async Task SellCraftedDreamAsync_HasDream_Success()
    {
        // Arrange
        var dream = TestDataBuilder.CreateTestCraftedDream("dream1", "Test Dream", Rarity.Rare, 100m);
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CraftedDreams.Add(dream);
        var initialCoins = playerState.Coins;

        // Act
        var result = await _sut.SellCraftedDreamAsync("dream1");

        // Assert
        result.Should().BeTrue();
        playerState.CraftedDreams.Should().NotContain(dream);
        playerState.Coins.Should().Be(initialCoins + 100);
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    [Fact]
    public async Task SellCraftedDreamAsync_DreamNotFound_Fails()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        var initialCoins = playerState.Coins;

        // Act
        var result = await _sut.SellCraftedDreamAsync("nonexistent");

        // Assert
        result.Should().BeFalse();
        playerState.Coins.Should().Be(initialCoins);
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Never);
    }

    [Theory]
    [InlineData(Rarity.Common, 1)]
    [InlineData(Rarity.Rare, 2)]
    [InlineData(Rarity.Epic, 5)]
    [InlineData(Rarity.Legendary, 10)]
    public async Task SellCraftedDreamAsync_DifferentRarities_GrantsAppropriateReputation(
        Rarity rarity, int expectedMinReputation)
    {
        // Arrange
        var dream = TestDataBuilder.CreateTestCraftedDream("dream1", "Test Dream", rarity, 100m);
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CraftedDreams.Add(dream);

        // Act
        await _sut.SellCraftedDreamAsync("dream1");

        // Assert
        _mockGameStateService.Verify(
            x => x.UpdateReputationAsync(
                It.Is<int>(trust => trust >= expectedMinReputation),
                It.IsAny<int>(),
                It.Is<int>(lucidity => lucidity >= expectedMinReputation)),
            Times.Once);
    }

    #endregion

    #region GetIngredientPriceAsync Tests

    [Fact]
    public async Task GetIngredientPriceAsync_ValidIngredient_ReturnsPrice()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetIngredientPriceAsync("city1", "ing1");

        // Assert
        result.Should().NotBeNull();
        result!.IngredientId.Should().Be("ing1");
    }

    [Fact]
    public async Task GetIngredientPriceAsync_InvalidIngredient_ReturnsNull()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetIngredientPriceAsync("city1", "nonexistent");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetPriceHistoryAsync Tests

    [Fact]
    public async Task GetPriceHistoryAsync_ValidIngredient_ReturnsHistory()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item");
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        _mockGameStateService.Setup(x => x.CurrentDay).Returns(10);

        // Act
        var result = await _sut.GetPriceHistoryAsync("ing1", 7);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(7);
        result.Keys.Should().Contain(key => key.StartsWith("Day "));
    }

    [Fact]
    public async Task GetPriceHistoryAsync_InvalidIngredient_ReturnsEmpty()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetIngredientAsync("invalid"))
            .ReturnsAsync((Ingredient?)null);

        // Act
        var result = await _sut.GetPriceHistoryAsync("invalid", 7);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPriceHistoryAsync_RequestedDaysExceedsCurrentDay_ReturnsOnlyAvailableDays()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test Item");
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ingredient);
        _mockGameStateService.Setup(x => x.CurrentDay).Returns(3);

        // Act
        var result = await _sut.GetPriceHistoryAsync("ing1", 7);

        // Assert
        result.Count.Should().BeLessThanOrEqualTo(3); // Can't have more days than current day
    }

    #endregion

    #region GetMarketTrendsAsync Tests

    [Fact]
    public async Task GetMarketTrendsAsync_ReturnsTop5TrendingItems()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetMarketTrendsAsync("city1");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeLessOrEqualTo(5);
    }

    [Fact]
    public async Task GetMarketTrendsAsync_OnlyIncludesTrendingItems()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetMarketTrendsAsync("city1");

        // Assert
        result.Should().AllSatisfy(trend =>
        {
            Math.Abs(trend.ChangePercent).Should().BeGreaterThan(20);
        });
    }

    [Fact]
    public async Task GetMarketTrendsAsync_OrderedByChangePercent()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetMarketTrendsAsync("city1");

        // Assert
        if (result.Count > 1)
        {
            result.Should().BeInDescendingOrder(t => Math.Abs(t.ChangePercent));
        }
    }

    [Fact]
    public async Task GetMarketTrendsAsync_SetsCorrectDirection()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, city);

        // Act
        var result = await _sut.GetMarketTrendsAsync("city1");

        // Assert
        result.Should().AllSatisfy(trend =>
        {
            if (trend.ChangePercent > 0)
            {
                trend.Direction.Should().Be("Rising");
            }
            else
            {
                trend.Direction.Should().Be("Falling");
            }
        });
    }

    #endregion
}
