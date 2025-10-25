using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.Services;

/// <summary>
/// Unit tests for InventoryService
/// Tests capacity management, item operations, and weight calculations
/// </summary>
public class InventoryServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IGameStateService> _mockGameStateService;
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _mockDatabaseService = MockFactory.CreateMockDatabaseService();
        _mockGameStateService = MockFactory.CreateMockGameStateService();
        
        _sut = new InventoryService(
            _mockDatabaseService.Object,
            _mockGameStateService.Object);
    }

    #region GetCurrentCapacity Tests

    [Fact]
    public void GetCurrentCapacity_ReturnsPlayerWeight()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 50m;

        // Act
        var result = _sut.GetCurrentCapacity();

        // Assert
        result.Should().Be(50m);
    }

    [Fact]
    public void GetCurrentCapacity_EmptyInventory_ReturnsZero()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 0m;

        // Act
        var result = _sut.GetCurrentCapacity();

        // Assert
        result.Should().Be(0m);
    }

    #endregion

    #region GetMaxCapacity Tests

    [Fact]
    public void GetMaxCapacity_ReturnsPlayerMaxCapacity()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.MaxCapacity = 200m;

        // Act
        var result = _sut.GetMaxCapacity();

        // Assert
        result.Should().Be(200m);
    }

    [Theory]
    [InlineData(PlayerTier.Apprentice, 100)]
    [InlineData(PlayerTier.Adept, 150)]
    [InlineData(PlayerTier.Artisan, 200)]
    public void GetMaxCapacity_DifferentTiers_ReturnsCorrectCapacity(PlayerTier tier, int expectedCapacity)
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentTier = tier;
        playerState.MaxCapacity = expectedCapacity;

        // Act
        var result = _sut.GetMaxCapacity();

        // Assert
        result.Should().Be(expectedCapacity);
    }

    #endregion

    #region GetCapacityPercentage Tests

    [Fact]
    public void GetCapacityPercentage_HalfFull_Returns50()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 50m;
        playerState.MaxCapacity = 100m;

        // Act
        var result = _sut.GetCapacityPercentage();

        // Assert
        result.Should().Be(50m);
    }

    [Fact]
    public void GetCapacityPercentage_Empty_ReturnsZero()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 0m;
        playerState.MaxCapacity = 100m;

        // Act
        var result = _sut.GetCapacityPercentage();

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void GetCapacityPercentage_Full_Returns100()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 100m;
        playerState.MaxCapacity = 100m;

        // Act
        var result = _sut.GetCapacityPercentage();

        // Assert
        result.Should().Be(100m);
    }

    #endregion

    #region CanAddItem Tests

    [Fact]
    public void CanAddItem_WithinCapacity_ReturnsTrue()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test", Rarity.Common, 10m, 5m);
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 50m;
        playerState.MaxCapacity = 100m;

        // Act
        var result = _sut.CanAddItem(ingredient, 5); // 5 * 5 = 25 weight

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAddItem_ExceedsCapacity_ReturnsFalse()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Heavy", Rarity.Common, 10m, 20m);
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 90m;
        playerState.MaxCapacity = 100m;

        // Act
        var result = _sut.CanAddItem(ingredient, 1); // 90 + 20 = 110 > 100

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAddItem_ExactlyAtCapacity_ReturnsTrue()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test", Rarity.Common, 10m, 10m);
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 90m;
        playerState.MaxCapacity = 100m;

        // Act
        var result = _sut.CanAddItem(ingredient, 1); // 90 + 10 = 100

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAddItem_ZeroQuantity_ReturnsTrue()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test");
        var playerState = _mockGameStateService.Object.PlayerState;

        // Act
        var result = _sut.CanAddItem(ingredient, 0);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public async Task AddItem_Success_UpdatesInventory()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test");
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.MaxCapacity = 100m;

        // Act
        var result = await _sut.AddItemAsync(ingredient, 3);

        // Assert
        result.Should().BeTrue();
        playerState.Inventory.Should().ContainKey("ing1");
        playerState.Inventory["ing1"].Should().Be(3);
    }

    [Fact]
    public async Task AddItem_ExistingItem_AddsToQuantity()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test");
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.MaxCapacity = 100m;

        // Act
        var result = await _sut.AddItemAsync(ingredient, 3);

        // Assert
        result.Should().BeTrue();
        playerState.Inventory["ing1"].Should().Be(8);
    }

    [Fact]
    public async Task AddItem_UpdatesWeight()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test", Rarity.Common, 10m, 5m);
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.CurrentWeight = 10m;
        playerState.MaxCapacity = 100m;

        // Act
        await _sut.AddItemAsync(ingredient, 3); // 3 * 5 = 15

        // Assert
        playerState.CurrentWeight.Should().Be(25m);
    }

    [Fact]
    public async Task AddItem_SavesGame()
    {
        // Arrange
        var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test");
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.MaxCapacity = 100m;

        // Act
        await _sut.AddItemAsync(ingredient, 1);

        // Assert
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public async Task RemoveItem_Success_UpdatesInventory()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.CurrentWeight = 10m;

        // Act
        var result = await _sut.RemoveItemAsync("ing1", 2);

        // Assert
        result.Should().BeTrue();
        playerState.Inventory["ing1"].Should().Be(3);
    }

    [Fact]
    public async Task RemoveItem_RemovesAllQuantity_RemovesKey()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 3;

        // Act
        await _sut.RemoveItemAsync("ing1", 3);

        // Assert
        playerState.Inventory.Should().NotContainKey("ing1");
    }

    [Fact]
    public async Task RemoveItem_InsufficientQuantity_ReturnsFalse()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 2;

        // Act
        var result = await _sut.RemoveItemAsync("ing1", 5);

        // Assert
        result.Should().BeFalse();
        playerState.Inventory["ing1"].Should().Be(2); // Unchanged
    }

    [Fact]
    public async Task RemoveItem_NotInInventory_ReturnsFalse()
    {
        // Act
        var result = await _sut.RemoveItemAsync("nonexistent", 1);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetItemQuantity Tests

    [Fact]
    public void GetItemQuantity_ItemExists_ReturnsQuantity()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 7;

        // Act
        var result = _sut.GetItemQuantity("ing1");

        // Assert
        result.Should().Be(7);
    }

    [Fact]
    public void GetItemQuantity_ItemDoesNotExist_ReturnsZero()
    {
        // Act
        var result = _sut.GetItemQuantity("nonexistent");

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region UpgradeCapacity Tests

    [Fact]
    public async Task UpgradeCapacity_IncreasesMaxCapacity()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.MaxCapacity = 100m;

        // Act
        await _sut.UpgradeCapacityAsync(50m);

        // Assert
        playerState.MaxCapacity.Should().Be(150m);
    }

    [Fact]
    public async Task UpgradeCapacity_SavesGame()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.MaxCapacity = 100m;

        // Act
        await _sut.UpgradeCapacityAsync(25m);

        // Assert
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    #endregion
}
