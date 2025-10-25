using DreamAlchemist.ViewModels;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Tests.TestHelpers;
using CommunityToolkit.Mvvm.Input;

namespace DreamAlchemist.Tests.Unit.ViewModels;

/// <summary>
/// Unit tests for MarketViewModel
/// Tests market display, buy/sell operations, and UI interactions
/// </summary>
public class MarketViewModelTests
{
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IMarketService> _mockMarketService;
    private readonly Mock<IGameStateService> _mockGameStateService;
    private readonly MarketViewModel _sut;

    public MarketViewModelTests()
    {
        _mockNavigationService = MockFactory.CreateMockNavigationService();
        _mockMarketService = MockFactory.CreateMockMarketService();
        _mockGameStateService = MockFactory.CreateMockGameStateService();
        
        _sut = new MarketViewModel(
            _mockNavigationService.Object,
            _mockMarketService.Object,
            _mockGameStateService.Object);
    }

    #region OnAppearingAsync Tests

    [Fact]
    public async Task OnAppearingAsync_LoadsPrices()
    {
        // Act
        await _sut.OnAppearingAsync();

        // Assert
        _mockMarketService.Verify(
            x => x.GetCurrentPricesAsync(It.IsAny<string>()), 
            Times.Once);
    }

    [Fact]
    public async Task OnAppearingAsync_UpdatesPlayerCoins()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Coins = 500;

        // Act
        await _sut.OnAppearingAsync();

        // Assert
        _sut.PlayerCoins.Should().Be(500);
    }

    [Fact]
    public async Task OnAppearingAsync_SetsTitleToCurrentCity()
    {
        // Arrange
        var city = TestDataBuilder.CreateTestCity("city1", "Test City");
        _mockGameStateService.Setup(x => x.CurrentCity).Returns(city);

        // Act
        await _sut.OnAppearingAsync();

        // Assert
        _sut.Title.Should().Contain("Test City");
    }

    #endregion

    #region LoadPricesCommand Tests

    [Fact]
    public async Task LoadPricesCommand_PopulatesPricesList()
    {
        // Arrange
        var prices = TestDataBuilder.CreateTestMarketPrices();
        _mockMarketService.Setup(x => x.GetCurrentPricesAsync(It.IsAny<string>()))
            .ReturnsAsync(prices);

        // Act
        await _sut.LoadPricesCommand.ExecuteAsync(null);

        // Assert
        _sut.Prices.Should().HaveCount(prices.Count);
    }

    [Fact]
    public async Task LoadPricesCommand_SetsIsBusyDuringExecution()
    {
        // Arrange
        var taskSource = new TaskCompletionSource<List<MarketPriceDto>>();
        _mockMarketService.Setup(x => x.GetCurrentPricesAsync(It.IsAny<string>()))
            .Returns(taskSource.Task);

        // Act
        var executeTask = _sut.LoadPricesCommand.ExecuteAsync(null);
        
        // Assert - IsBusy should be true during execution
        _sut.IsBusy.Should().BeTrue();
        
        // Complete the task
        taskSource.SetResult(TestDataBuilder.CreateTestMarketPrices());
        await executeTask;
        
        // IsBusy should be false after completion
        _sut.IsBusy.Should().BeFalse();
    }

    [Fact]
    public async Task LoadPricesCommand_HandlesErrors()
    {
        // Arrange
        _mockMarketService.Setup(x => x.GetCurrentPricesAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        await _sut.LoadPricesCommand.ExecuteAsync(null);

        // Assert
        _sut.ErrorMessage.Should().NotBeEmpty();
        _sut.IsBusy.Should().BeFalse();
    }

    #endregion

    #region BuyCommand Tests

    [Fact]
    public async Task BuyCommand_SuccessfulPurchase_RefreshesPrices()
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        _sut.SelectedPrice = price;
        _sut.BuyQuantity = 2;
        
        _mockMarketService.Setup(x => x.BuyIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        await _sut.BuyCommand.ExecuteAsync(null);

        // Assert
        _mockMarketService.Verify(
            x => x.GetCurrentPricesAsync(It.IsAny<string>()), 
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task BuyCommand_FailedPurchase_ShowsError()
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        _sut.SelectedPrice = price;
        _sut.BuyQuantity = 2;
        
        _mockMarketService.Setup(x => x.BuyIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act
        await _sut.BuyCommand.ExecuteAsync(null);

        // Assert
        _sut.ErrorMessage.Should().NotBeEmpty();
    }

    [Fact]
    public void BuyCommand_NoItemSelected_CannotExecute()
    {
        // Arrange
        _sut.SelectedPrice = null;

        // Act & Assert
        _sut.BuyCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void BuyCommand_ZeroQuantity_CannotExecute()
    {
        // Arrange
        _sut.SelectedPrice = TestDataBuilder.CreateTestMarketPrices().First();
        _sut.BuyQuantity = 0;

        // Act & Assert
        _sut.BuyCommand.CanExecute(null).Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task BuyCommand_VariousQuantities_CallsServiceWithCorrectAmount(int quantity)
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        _sut.SelectedPrice = price;
        _sut.BuyQuantity = quantity;
        
        _mockMarketService.Setup(x => x.BuyIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        await _sut.BuyCommand.ExecuteAsync(null);

        // Assert
        _mockMarketService.Verify(
            x => x.BuyIngredientAsync(price.IngredientId, quantity), 
            Times.Once);
    }

    #endregion

    #region SellCommand Tests

    [Fact]
    public async Task SellCommand_SuccessfulSale_RefreshesPrices()
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        _sut.SelectedPrice = price;
        _sut.SellQuantity = 1;
        
        _mockMarketService.Setup(x => x.SellIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        await _sut.SellCommand.ExecuteAsync(null);

        // Assert
        _mockMarketService.Verify(
            x => x.GetCurrentPricesAsync(It.IsAny<string>()), 
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task SellCommand_FailedSale_ShowsError()
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        _sut.SelectedPrice = price;
        _sut.SellQuantity = 1;
        
        _mockMarketService.Setup(x => x.SellIngredientAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act
        await _sut.SellCommand.ExecuteAsync(null);

        // Assert
        _sut.ErrorMessage.Should().NotBeEmpty();
    }

    #endregion

    #region Property Change Tests

    [Fact]
    public void SelectedPrice_WhenChanged_NotifiesCommandsCanExecuteChanged()
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        var canExecuteChangedRaised = false;
        _sut.BuyCommand.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        // Act
        _sut.SelectedPrice = price;

        // Assert
        canExecuteChangedRaised.Should().BeTrue();
    }

    [Fact]
    public void BuyQuantity_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_sut.BuyQuantity))
                propertyChangedRaised = true;
        };

        // Act
        _sut.BuyQuantity = 5;

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    #endregion

    #region TotalCost Calculation Tests

    [Fact]
    public void TotalCost_CalculatedCorrectly()
    {
        // Arrange
        var price = TestDataBuilder.CreateTestMarketPrices().First();
        price.CurrentPrice = 100m;
        _sut.SelectedPrice = price;
        _sut.BuyQuantity = 3;

        // Act
        var totalCost = _sut.TotalCost;

        // Assert
        totalCost.Should().Be(300);
    }

    [Fact]
    public void TotalCost_NoSelection_ReturnsZero()
    {
        // Arrange
        _sut.SelectedPrice = null;
        _sut.BuyQuantity = 5;

        // Act
        var totalCost = _sut.TotalCost;

        // Assert
        totalCost.Should().Be(0);
    }

    #endregion

    #region Filtering and Sorting Tests

    [Fact]
    public async Task FilterByRarity_FiltersCorrectly()
    {
        // Arrange
        var prices = TestDataBuilder.CreateTestMarketPrices();
        _mockMarketService.Setup(x => x.GetCurrentPricesAsync(It.IsAny<string>()))
            .ReturnsAsync(prices);
        await _sut.LoadPricesCommand.ExecuteAsync(null);

        // Act
        _sut.SelectedRarityFilter = Rarity.Common;

        // Assert
        _sut.FilteredPrices.Should().OnlyContain(p => p.Rarity == Rarity.Common);
    }

    [Fact]
    public async Task SortByPrice_SortsCorrectly()
    {
        // Arrange
        var prices = TestDataBuilder.CreateTestMarketPrices();
        _mockMarketService.Setup(x => x.GetCurrentPricesAsync(It.IsAny<string>()))
            .ReturnsAsync(prices);
        await _sut.LoadPricesCommand.ExecuteAsync(null);

        // Act
        _sut.SortByPrice = true;

        // Assert
        var sortedPrices = _sut.FilteredPrices.ToList();
        sortedPrices.Should().BeInAscendingOrder(p => p.CurrentPrice);
    }

    #endregion
}
