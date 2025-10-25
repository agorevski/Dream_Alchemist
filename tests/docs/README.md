# Dream Alchemist - Test Suite Documentation

## Overview

This test suite provides comprehensive coverage for the Dream Alchemist game, including:
- **Unit Tests**: Service layer and ViewModel logic
- **Integration Tests**: End-to-end workflows
- **UI Tests**: MAUI page and control testing

**Test Framework**: xUnit  
**Mocking**: Moq  
**Assertions**: FluentAssertions  
**Coverage Target**: 80%+ overall, 90%+ for critical services

---

## Test Structure

```
tests/DreamAlchemist.Tests/
├── TestHelpers/
│   ├── MockFactory.cs          - Mock service creation
│   ├── TestDataBuilder.cs      - Test data generation
│   └── AssertExtensions.cs     - Custom assertions (future)
├── Unit/
│   ├── Services/               - Service layer tests
│   │   ├── MarketServiceTests.cs         ✅ (35 tests)
│   │   ├── CraftingServiceTests.cs       ✅ (25 tests)
│   │   ├── InventoryServiceTests.cs      📝 (18 tests planned)
│   │   ├── TravelServiceTests.cs         📝 (15 tests planned)
│   │   ├── EventServiceTests.cs          📝 (20 tests planned)
│   │   ├── GameStateServiceTests.cs      📝 (22 tests planned)
│   │   └── DatabaseServiceTests.cs       📝 (25 tests planned)
│   ├── ViewModels/             - ViewModel tests
│   │   ├── MarketViewModelTests.cs       ✅ (20 tests)
│   │   ├── LabViewModelTests.cs          📝 (12 tests planned)
│   │   ├── InventoryViewModelTests.cs    📝 (10 tests planned)
│   │   ├── TravelViewModelTests.cs       📝 (12 tests planned)
│   │   └── MainViewModelTests.cs         📝 (10 tests planned)
│   └── Helpers/
│       └── ConverterTests.cs             📝 (10 tests planned)
├── Integration/
│   ├── DatabaseIntegrationTests.cs       📝 (planned)
│   ├── GameFlowTests.cs                  📝 (planned)
│   └── SaveLoadTests.cs                  📝 (planned)
└── UI/
    ├── MarketPageTests.cs                📝 (planned)
    ├── LabPageTests.cs                   📝 (planned)
    └── NavigationTests.cs                📝 (planned)
```

**Current Status**: 80 tests implemented, ~170 tests planned

---

## Running Tests

### Command Line

```bash
# Run all tests
cd tests/DreamAlchemist.Tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~MarketServiceTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate coverage report
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report
```

### Visual Studio

1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All" or right-click specific tests
3. View results in Test Explorer window
4. Use Code Coverage tool (Test → Analyze Code Coverage)

### VS Code

1. Install .NET Core Test Explorer extension
2. Tests appear in sidebar
3. Click play button to run tests
4. View results inline

---

## Test Patterns

### Unit Test Structure

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and mocks
    var ingredient = TestDataBuilder.CreateTestIngredient("ing1", "Test");
    MockFactory.SetupIngredientRetrieval(_mockDb, ingredient);
    
    // Act - Execute the method being tested
    var result = await _sut.BuyIngredientAsync("ing1", 2);
    
    // Assert - Verify expected outcomes
    result.Should().BeTrue();
    _mockGameState.Verify(x => x.SaveGameAsync(), Times.Once);
}
```

### Theory Tests (Parameterized)

```csharp
[Theory]
[InlineData(Rarity.Common, 1)]
[InlineData(Rarity.Rare, 2)]
[InlineData(Rarity.Epic, 5)]
public async Task SellDream_DifferentRarities_GrantsAppropriateReputation(
    Rarity rarity, int expectedReputation)
{
    // Test with multiple input combinations
}
```

### Async Testing

```csharp
[Fact]
public async Task AsyncMethod_SetsIsBusy_DuringExecution()
{
    // Arrange
    var tcs = new TaskCompletionSource<bool>();
    _mockService.Setup(x => x.DoWorkAsync()).Returns(tcs.Task);
    
    // Act - Start async operation
    var task = _sut.ExecuteAsync();
    
    // Assert - Check intermediate state
    _sut.IsBusy.Should().BeTrue();
    
    // Complete operation
    tcs.SetResult(true);
    await task;
    
    // Assert - Check final state
    _sut.IsBusy.Should().BeFalse();
}
```

---

## Test Coverage by Component

### Services (Target: 90%+)

#### ✅ MarketService (35 tests)
- Price calculation algorithm
- Buy/sell operations with validation
- Market trends and history
- Event multiplier integration
- Edge cases (insufficient funds, capacity limits)

#### ✅ CraftingService (25 tests)
- Recipe matching (exact and experimental)
- Dream value calculation
- Rarity determination
- Inventory updates
- Recipe discovery system

#### 📝 InventoryService (18 tests planned)
- Capacity management
- Weight calculations
- Item add/remove operations
- Tier-based upgrades
- Validation logic

#### 📝 TravelService (15 tests planned)
- City navigation
- Travel cost calculation
- Day progression
- Random event triggering
- Distance-based mechanics

#### 📝 EventService (20 tests planned)
- Event generation (probability-based)
- Effect application
- Duration tracking
- Choice consequences
- Market multipliers

#### 📝 GameStateService (22 tests planned)
- State initialization
- Save/load operations
- Reputation system
- Day progression
- Active event management

#### 📝 DatabaseService (25 tests planned)
- CRUD operations for all entities
- Seed data loading
- Query operations
- Error handling
- Transaction management

### ViewModels (Target: 85%+)

#### ✅ MarketViewModel (20 tests)
- Price loading and display
- Buy/sell commands
- Command can-execute logic
- Property change notifications
- Filter/sort functionality

#### 📝 LabViewModel (12 tests planned)
- Ingredient selection
- Craft command
- Recipe display
- Result handling
- Validation

#### 📝 InventoryViewModel (10 tests planned)
- Item listing
- Sell actions
- Capacity display
- Filtering/sorting

#### 📝 TravelViewModel (12 tests planned)
- City selection
- Travel command
- Cost display
- Event handling

#### 📝 MainViewModel (10 tests planned)
- Dashboard data
- Navigation commands
- Player stats display

---

## Test Data Builders

### TestDataBuilder Methods

```csharp
// Ingredients
CreateTestIngredient(id, name, rarity, baseValue, weight)
CreateTestIngredients() // Returns list of 5 ingredients

// Recipes
CreateTestRecipe(id, name, ingredientIds, rarity)
CreateTestRecipes() // Returns list of 3 recipes

// Cities
CreateTestCity(id, name)
CreateTestCities() // Returns list of 3 cities

// Events
CreateTestEvent(id, title, eventType)
CreateTestEvents() // Returns list of 3 events

// Player State
CreateTestPlayerState() // Returns initialized player state

// DTOs
CreateTestMarketPrices() // Returns list of price DTOs
CreateTestCraftResult(success, dreamId)
CreateTestTravelResult(success)
CreateTestReputation()

// Supporting Models
CreateTestCraftedDream(id, name, rarity, value)
CreateTestActiveEvent(eventId, remainingDays)
```

### MockFactory Methods

```csharp
// Service Mocks
CreateMockDatabaseService()
CreateMockGameStateService()
CreateMockNavigationService()
CreateMockMarketService()
CreateMockCraftingService()
CreateMockInventoryService()
CreateMockTravelService()
CreateMockEventService()

// Setup Helpers
SetupIngredientRetrieval(mockDb, ingredient)
SetupCityRetrieval(mockDb, city)
SetupRecipeRetrieval(mockDb, recipe)
SetupPlayerStateWithInventory(mockGameState, inventory)
SetupPlayerStateWithCoins(mockGameState, coins)
```

---

## Integration Testing

### Database Integration Tests

```csharp
[Fact]
public async Task SaveAndLoad_PreservesGameState()
{
    // Arrange - Create real database service
    var dbService = new DatabaseService(testDbPath);
    var gameState = TestDataBuilder.CreateTestPlayerState();
    
    // Act - Save and load
    await dbService.SavePlayerStateAsync(gameState);
    var loaded = await dbService.LoadPlayerStateAsync();
    
    // Assert - State matches
    loaded.Should().BeEquivalentTo(gameState);
}
```

### End-to-End Game Flow Tests

```csharp
[Fact]
public async Task CompleteGameFlow_NewPlayerToFirstSale_Success()
{
    // 1. Start new game
    // 2. Navigate to market
    // 3. Buy ingredients
    // 4. Navigate to lab
    // 5. Craft dream
    // 6. Navigate to inventory
    // 7. Sell dream
    // 8. Verify coin balance increased
}
```

---

## UI Testing (MAUI)

### Approach 1: MAUI DeviceTests

```csharp
[Fact]
public async Task MarketPage_BuyButton_EnabledWhenItemSelected()
{
    // Arrange
    await App.NavigateToAsync("///Market");
    
    // Act
    var listView = App.WaitForElement<CollectionView>("MarketList");
    listView.SelectItem(0);
    
    // Assert
    var buyButton = App.FindElement<Button>("BuyButton");
    buyButton.IsEnabled.Should().BeTrue();
}
```

### Approach 2: Manual Test Scenarios

Create guided manual test checklists:
1. Navigation flow verification
2. Visual rendering checks
3. Touch interaction testing
4. Accessibility validation
5. Performance benchmarks

---

## Continuous Integration

### GitHub Actions Workflow

```yaml
name: Test Suite

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '9.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      - name: Upload coverage
        uses: codecov/codecov-action@v2
```

---

## Best Practices

### DO:
✅ Follow AAA pattern (Arrange, Act, Assert)  
✅ Use descriptive test names  
✅ Test one thing per test  
✅ Use TestDataBuilder for consistent test data  
✅ Mock external dependencies  
✅ Assert using FluentAssertions  
✅ Test both success and failure paths  
✅ Include edge cases and boundary conditions  

### DON'T:
❌ Test implementation details  
❌ Create interdependent tests  
❌ Use hard-coded magic values  
❌ Skip test setup/teardown  
❌ Ignore async/await  
❌ Test framework code  
❌ Write brittle tests that break easily  

---

## Extending the Test Suite

### Adding New Service Tests

1. Create test file in `Unit/Services/`
2. Follow existing pattern (see MarketServiceTests)
3. Create mocks using MockFactory
4. Test all public methods
5. Include edge cases
6. Aim for 90%+ coverage

### Adding New ViewModel Tests

1. Create test file in `Unit/ViewModels/`
2. Mock all service dependencies
3. Test commands and properties
4. Verify INotifyPropertyChanged
5. Test CanExecute logic
6. Check error handling

### Adding New Test Data

1. Add builder method to TestDataBuilder
2. Follow existing naming conventions
3. Provide sensible defaults
4. Allow parameterization
5. Document usage

---

## Troubleshooting

### Tests Not Discovered
- Ensure `IsTestProject` is true in `.csproj`
- Check test method has `[Fact]` or `[Theory]` attribute
- Rebuild solution
- Clear test cache

### Mocks Not Working
- Verify `Setup()` before `Object` usage
- Check method signatures match exactly
- Use `It.IsAny<T>()` for flexible matching
- Verify `Returns` or `ReturnsAsync`

### Async Tests Hanging
- Always await async operations
- Use TaskCompletionSource for controllable async
- Set timeout: `[Fact(Timeout = 5000)]`
- Check for deadlocks

### FluentAssertions Errors
- Ensure using `FluentAssertions`
- Use correct assertion method
- Check type compatibility
- Review comparison logic

---

## Coverage Reports

### Generate HTML Coverage Report

```bash
# Install report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate report
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html

# Open report
start coverage-report/index.html  # Windows
open coverage-report/index.html   # macOS
```

---

## Performance Testing

### Benchmark Tests

```csharp
[Fact]
public async Task MarketService_GetPrices_CompletesUnder200ms()
{
    var stopwatch = Stopwatch.StartNew();
    
    await _sut.GetCurrentPricesAsync("city1");
    
    stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromMilliseconds(200));
}
```

---

## Future Enhancements

- [ ] Add mutation testing (Stryker.NET)
- [ ] Implement property-based testing (FsCheck)
- [ ] Create load/stress tests
- [ ] Add visual regression testing
- [ ] Implement E2E UI automation
- [ ] Set up test data snapshots
- [ ] Add performance benchmarks
- [ ] Create test coverage dashboard

---

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [.NET MAUI Testing](https://learn.microsoft.com/en-us/dotnet/maui/testing)
- [Test Automation Patterns](https://martinfowler.com/articles/practical-test-pyramid.html)

---

**Last Updated**: October 25, 2025  
**Test Suite Version**: 1.0.0  
**Total Tests**: 80 implemented, 170+ planned
