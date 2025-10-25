using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.Services;

/// <summary>
/// Unit tests for TravelService
/// Tests city navigation, travel costs, and event triggering
/// </summary>
public class TravelServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IGameStateService> _mockGameStateService;
    private readonly Mock<IEventService> _mockEventService;
    private readonly TravelService _sut;

    public TravelServiceTests()
    {
        _mockDatabaseService = MockFactory.CreateMockDatabaseService();
        _mockGameStateService = MockFactory.CreateMockGameStateService();
        _mockEventService = MockFactory.CreateMockEventService();
        
        _sut = new TravelService(
            _mockDatabaseService.Object,
            _mockGameStateService.Object,
            _mockEventService.Object);
    }

    #region TravelToCityAsync Tests

    [Fact]
    public async Task TravelToCity_ValidCity_Success()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;

        // Act
        var result = await _sut.TravelToCityAsync("city2");

        // Assert
        result.Success.Should().BeTrue();
        result.NewCityId.Should().Be("city2");
    }

    [Fact]
    public async Task TravelToCity_InvalidCity_Fails()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetCityAsync("invalid"))
            .ReturnsAsync((City?)null);

        // Act
        var result = await _sut.TravelToCityAsync("invalid");

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task TravelToCity_InsufficientFunds_Fails()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Expensive");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 10; // Not enough

        // Act
        var result = await _sut.TravelToCityAsync("city2");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("funds");
    }

    [Fact]
    public async Task TravelToCity_UpdatesCurrentCity()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;
        playerState.CurrentCityId = "city1";

        // Act
        await _sut.TravelToCityAsync("city2");

        // Assert
        playerState.CurrentCityId.Should().Be("city2");
    }

    [Fact]
    public async Task TravelToCity_DeductsCoins()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;
        playerState.CurrentCityId = "city1";

        // Act
        var result = await _sut.TravelToCityAsync("city2");

        // Assert
        playerState.Coins.Should().BeLessThan(1000);
        result.CostPaid.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TravelToCity_ProgressesDay()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;

        // Act
        var result = await _sut.TravelToCityAsync("city2");

        // Assert
        result.DaysPassed.Should().BeGreaterThan(0);
        _mockGameStateService.Verify(x => x.ProgressDayAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task TravelToCity_MayTriggerEvent()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var travelEvent = TestDataBuilder.CreateTestEvent("event1", "Travel Event");
        _mockEventService.Setup(x => x.TriggerRandomEventAsync())
            .ReturnsAsync(travelEvent);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;

        // Act
        var result = await _sut.TravelToCityAsync("city2");

        // Assert
        result.EventTriggered.Should().NotBeNull();
        result.EventTriggered!.Id.Should().Be("event1");
    }

    [Fact]
    public async Task TravelToCity_SavesGame()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;

        // Act
        await _sut.TravelToCityAsync("city2");

        // Assert
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    #endregion

    #region GetTravelCost Tests

    [Theory]
    [InlineData("city1", "city2", 50)]
    [InlineData("city1", "city3", 75)]
    [InlineData("city2", "city3", 50)]
    public void GetTravelCost_DifferentCities_ReturnsExpectedCost(string from, string to, int expectedMin)
    {
        // Act
        var cost = _sut.GetTravelCost(from, to);

        // Assert
        cost.Should().BeGreaterOrEqualTo(expectedMin);
    }

    [Fact]
    public void GetTravelCost_SameCity_ReturnsZero()
    {
        // Act
        var cost = _sut.GetTravelCost("city1", "city1");

        // Assert
        cost.Should().Be(0);
    }

    #endregion

    #region GetAvailableCities Tests

    [Fact]
    public async Task GetAvailableCities_ReturnsUnlockedCities()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Trust = 50; // Enough to unlock some cities

        // Act
        var result = await _sut.GetAvailableCitiesAsync();

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAvailableCities_ExcludesCurrentCity()
    {
        // Arrange
        _mockGameStateService.Setup(x => x.CurrentCity)
            .Returns(TestDataBuilder.CreateTestCity("city1", "Current"));

        // Act
        var result = await _sut.GetAvailableCitiesAsync();

        // Assert
        result.Should().NotContain(c => c.Id == "city1");
    }

    #endregion

    #region CanTravelTo Tests

    [Fact]
    public async Task CanTravelTo_SufficientFundsAndUnlocked_ReturnsTrue()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        targetCity.UnlockRequirement = 0;
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;

        // Act
        var result = await _sut.CanTravelToAsync("city2");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanTravelTo_InsufficientFunds_ReturnsFalse()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Destination");
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 10;

        // Act
        var result = await _sut.CanTravelToAsync("city2");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanTravelTo_CityLocked_ReturnsFalse()
    {
        // Arrange
        var targetCity = TestDataBuilder.CreateTestCity("city2", "Locked");
        targetCity.UnlockRequirement = 100;
        MockFactory.SetupCityRetrieval(_mockDatabaseService, targetCity);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 1000;
        playerState.Trust = 50; // Below requirement

        // Act
        var result = await _sut.CanTravelToAsync("city2");

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
