# Remaining Test Templates

This document provides complete test templates for the remaining 90 tests. Each template follows the established patterns and can be implemented by filling in the test logic.

## Service Tests Remaining

### EventServiceTests.cs (20 tests)

```csharp
using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.Services;

public class EventServiceTests
{
    private readonly Mock<IDatabaseService> _mockDb;
    private readonly Mock<IGameStateService> _mockGameState;
    private readonly EventService _sut;

    public EventServiceTests()
    {
        _mockDb = MockFactory.CreateMockDatabaseService();
        _mockGameState = MockFactory.CreateMockGameStateService();
        _sut = new EventService(_mockDb.Object, _mockGameState.Object);
    }

    // TriggerRandomEventAsync (5 tests)
    [Fact] public async Task TriggerRandomEvent_ReturnsProbabilityBasedEvent() { }
    [Fact] public async Task TriggerRandomEvent_MayReturnNull() { }
    [Fact] public async Task TriggerRandomEvent_FiltersAppropriateTypes() { }
    [Fact] public async Task TriggerRandomEvent_ActivatesEvent() { }
    [Fact] public async Task TriggerRandomEvent_SavesGame() { }

    // GetEventMultiplier (4 tests)
    [Fact] public void GetEventMultiplier_NoActiveEvents_ReturnsOne() { }
    [Fact] public void GetEventMultiplier_ActiveEvent_ReturnsMultiplier() { }
    [Fact] public void GetEventMultiplier_MultipleEvents_CombinesMultipliers() { }
    [Fact] public void GetEventMultiplier_ExpiredEvent_Ignored() { }

    // ProcessEventChoiceAsync (4 tests)
    [Fact] public async Task ProcessEventChoice_AppliesConsequences() { }
    [Fact] public async Task ProcessEventChoice_UpdatesReputation() { }
    [Fact] public async Task ProcessEventChoice_ModifiesCoins() { }
    [Fact] public async Task ProcessEventChoice_RemovesEvent() { }

    // UpdateActiveEventsAsync (3 tests)
    [Fact] public async Task UpdateActiveEvents_DecrementsRemainingDays() { }
    [Fact] public async Task UpdateActiveEvents_RemovesExpiredEvents() { }
    [Fact] public async Task UpdateActiveEvents_SavesGame() { }

    // GetActiveEventsAsync (2 tests)
    [Fact] public async Task GetActiveEvents_ReturnsPlayerEvents() { }
    [Fact] public async Task GetActiveEvents_FiltersExpired() { }

    // ActivateEventAsync (2 tests)
    [Fact] public async Task ActivateEvent_AddsToActiveList() { }
    [Fact] public async Task ActivateEvent_SetsDuration() { }
}
```

### GameStateServiceTests.cs (22 tests)

```csharp
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.Services;

public class GameStateServiceTests
{
    private readonly Mock<IDatabaseService> _mockDb;
    private readonly GameStateService _sut;

    public GameStateServiceTests()
    {
        _mockDb = MockFactory.CreateMockDatabaseService();
        _sut = new GameStateService(_mockDb.Object);
    }

    // Initialization (3 tests)
    [Fact] public async Task Initialize_CreatesNewPlayerState() { }
    [Fact] public async Task Initialize_LoadsSeedData() { }
    [Fact] public async Task Initialize_SetsDefaultValues() { }

    // Save/Load (4 tests)
    [Fact] public async Task SaveGame_PersistsPlayerState() { }
    [Fact] public async Task LoadGame_RestoresPlayerState() { }
    [Fact] public async Task LoadGame_NoSave_CreatesNewState() { }
    [Fact] public async Task SaveGame_SerializesComplexData() { }

    // ProgressDayAsync (4 tests)
    [Fact] public async Task ProgressDay_IncrementsCurrentDay() { }
    [Fact] public async Task ProgressDay_UpdatesActiveEvents() { }
    [Fact] public async Task ProgressDay_RaisesEvent() { }
    [Fact] public async Task ProgressDay_SavesGame() { }

    // UpdateReputationAsync (5 tests)
    [Fact] public async Task UpdateReputation_ModifiesTrust() { }
    [Fact] public async Task UpdateReputation_ModifiesInfamy() { }
    [Fact] public async Task UpdateReputation_ModifiesLucidity() { }
    [Fact] public async Task UpdateReputation_ClampsToBounds() { }
    [Fact] public async Task UpdateReputation_ChecksTierProgression() { }

    // Tier Progression (3 tests)
    [Fact] public async Task CheckTierProgression_EligibleForUpgrade_Upgrades() { }
    [Fact] public async Task CheckTierProgression_NotEligible_NoChange() { }
    [Fact] public async Task CheckTierProgression_UpgradesCapacity() { }

    // CurrentCity Property (2 tests)
    [Fact] public async Task CurrentCity_LoadsFromDatabase() { }
    [Fact] public async Task CurrentCity_CachesResult() { }

    // DayProgressed Event (1 test)
    [Fact] public async Task DayProgressed_RaisesWithCorrectArgs() { }
}
```

### DatabaseServiceTests.cs (25 tests)

```csharp
using DreamAlchemist.Services.Data;
using DreamAlchemist.Models.Entities;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.Services;

public class DatabaseServiceTests
{
    private readonly DatabaseService _sut;
    private readonly string _testDbPath;

    public DatabaseServiceTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _sut = new DatabaseService(_testDbPath);
    }

    // Ingredients CRUD (5 tests)
    [Fact] public async Task GetIngredients_ReturnsAll() { }
    [Fact] public async Task GetIngredient_ById_ReturnsMatch() { }
    [Fact] public async Task SaveIngredient_PersistsData() { }
    [Fact] public async Task UpdateIngredient_ModifiesExisting() { }
    [Fact] public async Task DeleteIngredient_RemovesFromDb() { }

    // Recipes CRUD (5 tests)
    [Fact] public async Task GetRecipes_ReturnsAll() { }
    [Fact] public async Task GetRecipe_ById_ReturnsMatch() { }
    [Fact] public async Task SaveRecipe_PersistsData() { }
    [Fact] public async Task UpdateRecipe_ModifiesExisting() { }
    [Fact] public async Task GetRecipesByIngredient_FiltersCorrectly() { }

    // Cities CRUD (4 tests)
    [Fact] public async Task GetCities_ReturnsAll() { }
    [Fact] public async Task GetCity_ById_ReturnsMatch() { }
    [Fact] public async Task SaveCity_PersistsData() { }
    [Fact] public async Task GetCitiesByUnlockRequirement_FiltersCorrectly() { }

    // Events CRUD (4 tests)
    [Fact] public async Task GetEvents_ReturnsAll() { }
    [Fact] public async Task GetEvent_ById_ReturnsMatch() { }
    [Fact] public async Task GetEventsByType_FiltersCorrectly() { }
    [Fact] public async Task GetEventsByProbability_FiltersCorrectly() { }

    // PlayerState (4 tests)
    [Fact] public async Task SavePlayerState_PersistsData() { }
    [Fact] public async Task LoadPlayerState_RestoresData() { }
    [Fact] public async Task LoadPlayerState_NoData_ReturnsNull() { }
    [Fact] public async Task SavePlayerState_SerializesComplexTypes() { }

    // Seed Data (2 tests)
    [Fact] public async Task LoadSeedData_PopulatesDatabase() { }
    [Fact] public async Task LoadSeedData_PreventsDuplicates() { }

    // Error Handling (1 test)
    [Fact] public async Task DatabaseOperations_InvalidData_HandlesGracefully() { }
}
```

## ViewModel Tests Remaining

### LabViewModelTests.cs (12 tests)

```csharp
using DreamAlchemist.ViewModels;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.ViewModels;

public class LabViewModelTests
{
    private readonly Mock<INavigationService> _mockNav;
    private readonly Mock<ICraftingService> _mockCrafting;
    private readonly Mock<IGameStateService> _mockGameState;
    private readonly LabViewModel _sut;

    public LabViewModelTests()
    {
        _mockNav = MockFactory.CreateMockNavigationService();
        _mockCrafting = MockFactory.CreateMockCraftingService();
        _mockGameState = MockFactory.CreateMockGameStateService();
        _sut = new LabViewModel(_mockNav.Object, _mockCrafting.Object, _mockGameState.Object);
    }

    // OnAppearingAsync (2 tests)
    [Fact] public async Task OnAppearing_LoadsAvailableIngredients() { }
    [Fact] public async Task OnAppearing_LoadsKnownRecipes() { }

    // Ingredient Selection (3 tests)
    [Fact] public void SelectIngredient_AddsToSelected() { }
    [Fact] public void SelectIngredient_MaxFive_PreventsMore() { }
    [Fact] public void DeselectIngredient_RemovesFromSelected() { }

    // CraftCommand (4 tests)
    [Fact] public async Task CraftCommand_Success_ShowsResult() { }
    [Fact] public async Task CraftCommand_Failure_ShowsError() { }
    [Fact] public async Task CraftCommand_NewRecipe_ShowsDiscovery() { }
    [Fact] public void CraftCommand_NoIngredients_CannotExecute() { }

    // Recipe Display (2 tests)
    [Fact] public async Task LoadRecipes_DisplaysKnownRecipes() { }
    [Fact] public async Task LoadRecipes_ShowsCraftableIndicator() { }

    // Property Updates (1 test)
    [Fact] public void SelectedIngredients_NotifyChanges() { }
}
```

### InventoryViewModelTests.cs (10 tests)

```csharp
using DreamAlchemist.ViewModels;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.ViewModels;

public class InventoryViewModelTests
{
    private readonly Mock<INavigationService> _mockNav;
    private readonly Mock<IInventoryService> _mockInventory;
    private readonly Mock<IMarketService> _mockMarket;
    private readonly Mock<IGameStateService> _mockGameState;
    private readonly InventoryViewModel _sut;

    public InventoryViewModelTests()
    {
        _mockNav = MockFactory.CreateMockNavigationService();
        _mockInventory = MockFactory.CreateMockInventoryService();
        _mockMarket = MockFactory.CreateMockMarketService();
        _mockGameState = MockFactory.CreateMockGameStateService();
        _sut = new InventoryViewModel(_mockNav.Object, _mockInventory.Object, 
            _mockMarket.Object, _mockGameState.Object);
    }

    // OnAppearingAsync (2 tests)
    [Fact] public async Task OnAppearing_LoadsInventoryItems() { }
    [Fact] public async Task OnAppearing_UpdatesCapacityDisplay() { }

    // SellIngredientCommand (2 tests)
    [Fact] public async Task SellIngredient_Success_RefreshesDisplay() { }
    [Fact] public async Task SellIngredient_Failure_ShowsError() { }

    // SellDreamCommand (2 tests)
    [Fact] public async Task SellDream_Success_UpdatesCoins() { }
    [Fact] public async Task SellDream_Failure_ShowsError() { }

    // Filtering (2 tests)
    [Fact] public void FilterByRarity_FiltersCorrectly() { }
    [Fact] public void FilterByType_SeparatesIngredientsAndDreams() { }

    // Sorting (1 test)
    [Fact] public void SortByValue_OrdersDescending() { }

    // Capacity Display (1 test)
    [Fact] public void CapacityPercentage_CalculatesCorrectly() { }
}
```

### TravelViewModelTests.cs (12 tests)

```csharp
using DreamAlchemist.ViewModels;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.ViewModels;

public class TravelViewModelTests
{
    private readonly Mock<INavigationService> _mockNav;
    private readonly Mock<ITravelService> _mockTravel;
    private readonly Mock<IGameStateService> _mockGameState;
    private readonly TravelViewModel _sut;

    public TravelViewModelTests()
    {
        _mockNav = MockFactory.CreateMockNavigationService();
        _mockTravel = MockFactory.CreateMockTravelService();
        _mockGameState = MockFactory.CreateMockGameStateService();
        _sut = new TravelViewModel(_mockNav.Object, _mockTravel.Object, _mockGameState.Object);
    }

    // OnAppearingAsync (2 tests)
    [Fact] public async Task OnAppearing_LoadsAvailableCities() { }
    [Fact] public async Task OnAppearing_ExcludesCurrentCity() { }

    // City Selection (2 tests)
    [Fact] public void SelectCity_UpdatesTravelCost() { }
    [Fact] public void SelectCity_EnablesTravelCommand() { }

    // TravelCommand (4 tests)
    [Fact] public async Task TravelCommand_Success_NavigatesToNewCity() { }
    [Fact] public async Task TravelCommand_Failure_ShowsError() { }
    [Fact] public async Task TravelCommand_TriggersEvent_ShowsEventModal() { }
    [Fact] public void TravelCommand_NoCity_CannotExecute() { }

    // Cost Display (2 tests)
    [Fact] public void TravelCost_UpdatesOnCitySelection() { }
    [Fact] public void TravelCost_InsufficientFunds_ShowsWarning() { }

    // Locked Cities (2 tests)
    [Fact] public void LockedCity_ShowsRequirement() { }
    [Fact] public void LockedCity_DisablesTravelButton() { }
}
```

### MainViewModelTests.cs (10 tests)

```csharp
using DreamAlchemist.ViewModels;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.ViewModels;

public class MainViewModelTests
{
    private readonly Mock<INavigationService> _mockNav;
    private readonly Mock<IGameStateService> _mockGameState;
    private readonly MainViewModel _sut;

    public MainViewModelTests()
    {
        _mockNav = MockFactory.CreateMockNavigationService();
        _mockGameState = MockFactory.CreateMockGameStateService();
        _sut = new MainViewModel(_mockNav.Object, _mockGameState.Object);
    }

    // OnAppearingAsync (2 tests)
    [Fact] public async Task OnAppearing_LoadsPlayerStats() { }
    [Fact] public async Task OnAppearing_LoadsActiveEvents() { }

    // Navigation Commands (4 tests)
    [Fact] public async Task NavigateToMarket_Success() { }
    [Fact] public async Task NavigateToLab_Success() { }
    [Fact] public async Task NavigateToInventory_Success() { }
    [Fact] public async Task NavigateToTravel_Success() { }

    // Player Stats Display (2 tests)
    [Fact] public void PlayerStats_UpdatesFromGameState() { }
    [Fact] public void ReputationDisplay_FormatsCorrectly() { }

    // Event Display (1 test)
    [Fact] public void ActiveEvents_DisplaysCount() { }

    // Tier Display (1 test)
    [Fact] public void TierDisplay_ShowsCurrentTier() { }
}
```

### ConverterTests.cs (10 tests)

```csharp
using DreamAlchemist.Helpers.Converters;
using Microsoft.Maui.Graphics;

namespace DreamAlchemist.Tests.Unit.Helpers;

public class ConverterTests
{
    // BoolToColorConverter (2 tests)
    [Fact] public void BoolToColor_True_ReturnsSuccessColor() { }
    [Fact] public void BoolToColor_False_ReturnsErrorColor() { }

    // InvertedBoolConverter (2 tests)
    [Fact] public void InvertedBool_True_ReturnsFalse() { }
    [Fact] public void InvertedBool_False_ReturnsTrue() { }

    // IsNotNullConverter (2 tests)
    [Fact] public void IsNotNull_NotNull_ReturnsTrue() { }
    [Fact] public void IsNotNull_Null_ReturnsFalse() { }

    // PriceChangeToColorConverter (2 tests)
    [Fact] public void PriceChangeToColor_Positive_ReturnsGreen() { }
    [Fact] public void PriceChangeToColor_Negative_ReturnsRed() { }

    // StringIsNotNullOrEmptyConverter (2 tests)
    [Fact] public void StringNotEmpty_ValidString_ReturnsTrue() { }
    [Fact] public void StringNotEmpty_EmptyOrNull_ReturnsFalse() { }
}
```

## Integration Tests

### DatabaseIntegrationTests.cs

```csharp
using DreamAlchemist.Services.Data;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Integration;

public class DatabaseIntegrationTests
{
    [Fact] public async Task SaveAndLoad_PreservesCompleteState() { }
    [Fact] public async Task SeedData_LoadsFromJson() { }
    [Fact] public async Task ConcurrentOperations_HandlesCorrectly() { }
    [Fact] public async Task LargeDataSet_PerformsWell() { }
}
```

### GameFlowTests.cs

```csharp
using DreamAlchemist.Services;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Integration;

public class GameFlowTests
{
    [Fact] public async Task CompleteGameFlow_BuyCraftSell_Success() { }
    [Fact] public async Task TravelFlow_ChangeCityAffectsPrices() { }
    [Fact] public async Task ProgressionFlow_TierUpgrade_Works() { }
    [Fact] public async Task EventFlow_TriggerAndResolve_Success() { }
}
```

## Implementation Instructions

1. **Copy template to appropriate test file**
2. **Implement test body following AAA pattern**:
   ```csharp
   // Arrange - Set up test data using TestDataBuilder
   var data = TestDataBuilder.CreateTestData();
   
   // Act - Execute method under test
   var result = await _sut.MethodAsync(data);
   
   // Assert - Verify with FluentAssertions
   result.Should().BeTrue();
   ```
3. **Use MockFactory for consistent mocking**
4. **Follow naming conventions**
5. **Add regional comments for organization**
6. **Run tests frequently to catch issues early**

All templates are ready for implementation following the established patterns in the existing test files (MarketServiceTests, CraftingServiceTests, MarketViewModelTests, InventoryServiceTests, TravelServiceTests).
