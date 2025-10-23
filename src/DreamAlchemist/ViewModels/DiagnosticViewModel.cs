using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using DreamAlchemist.ViewModels.Base;
using System.Text;
using System.Diagnostics;

namespace DreamAlchemist.ViewModels;

public partial class DiagnosticViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IMarketService _marketService;
    private readonly IGameStateService _gameStateService;
    private readonly ITravelService _travelService;
    private readonly ICraftingService _craftingService;
    private readonly IInventoryService _inventoryService;
    private readonly IEventService _eventService;

    [ObservableProperty]
    private string diagnosticOutput = "Click 'Run Diagnostics' to start comprehensive system checks...";

    [ObservableProperty]
    private string overallStatus = "";

    [ObservableProperty]
    private int totalTests = 0;

    [ObservableProperty]
    private int passedTests = 0;

    [ObservableProperty]
    private int failedTests = 0;

    [ObservableProperty]
    private int warningTests = 0;

    public DiagnosticViewModel(
        INavigationService navigationService,
        IDatabaseService databaseService,
        IMarketService marketService,
        IGameStateService gameStateService,
        ITravelService travelService,
        ICraftingService craftingService,
        IInventoryService inventoryService,
        IEventService eventService)
        : base(navigationService)
    {
        Title = "System Diagnostics";
        _databaseService = databaseService;
        _marketService = marketService;
        _gameStateService = gameStateService;
        _travelService = travelService;
        _craftingService = craftingService;
        _inventoryService = inventoryService;
        _eventService = eventService;
    }

    [RelayCommand]
    private async Task RunDiagnosticsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var output = new StringBuilder();
            TotalTests = 0;
            PassedTests = 0;
            FailedTests = 0;
            WarningTests = 0;

            output.AppendLine("═══════════════════════════════════════════════");
            output.AppendLine("     DREAM ALCHEMIST SYSTEM DIAGNOSTICS");
            output.AppendLine("═══════════════════════════════════════════════");
            output.AppendLine($"Run at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            output.AppendLine($"Platform: {DeviceInfo.Platform}");
            output.AppendLine($"Version: {DeviceInfo.VersionString}");
            output.AppendLine();

            // Database Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│           DATABASE TESTS                    │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunDatabaseTests(output);
            output.AppendLine();

            // Service Initialization Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│      SERVICE INITIALIZATION TESTS           │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunServiceTests(output);
            output.AppendLine();

            // Game State Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│          GAME STATE TESTS                   │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunGameStateTests(output);
            output.AppendLine();

            // Market System Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│         MARKET SYSTEM TESTS                 │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunMarketTests(output);
            output.AppendLine();

            // Crafting System Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│        CRAFTING SYSTEM TESTS                │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunCraftingTests(output);
            output.AppendLine();

            // Travel System Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│         TRAVEL SYSTEM TESTS                 │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunTravelTests(output);
            output.AppendLine();

            // Event System Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│         EVENT SYSTEM TESTS                  │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunEventTests(output);
            output.AppendLine();

            // Data Integrity Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│       DATA INTEGRITY TESTS                  │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunDataIntegrityTests(output);
            output.AppendLine();

            // Performance Tests
            output.AppendLine("┌─────────────────────────────────────────────┐");
            output.AppendLine("│         PERFORMANCE TESTS                   │");
            output.AppendLine("└─────────────────────────────────────────────┘");
            await RunPerformanceTests(output);
            output.AppendLine();

            // Summary
            output.AppendLine("═══════════════════════════════════════════════");
            output.AppendLine("                  SUMMARY");
            output.AppendLine("═══════════════════════════════════════════════");
            output.AppendLine($"Total Tests:    {TotalTests}");
            output.AppendLine($"✅ Passed:      {PassedTests}");
            output.AppendLine($"❌ Failed:      {FailedTests}");
            output.AppendLine($"⚠️  Warnings:    {WarningTests}");
            output.AppendLine();

            if (FailedTests == 0 && WarningTests == 0)
            {
                output.AppendLine("Overall Status: ✅ ALL SYSTEMS OPERATIONAL");
                OverallStatus = "✅ ALL SYSTEMS OPERATIONAL";
            }
            else if (FailedTests > 0)
            {
                output.AppendLine("Overall Status: ❌ CRITICAL ISSUES DETECTED");
                OverallStatus = "❌ CRITICAL ISSUES DETECTED";
            }
            else
            {
                output.AppendLine("Overall Status: ⚠️ WARNINGS PRESENT");
                OverallStatus = "⚠️ WARNINGS PRESENT";
            }
            output.AppendLine("═══════════════════════════════════════════════");

            DiagnosticOutput = output.ToString();
        });
    }

    private async Task RunDatabaseTests(StringBuilder output)
    {
        // Test 1: Database Connection
        await TestAsync(output, "Database Connection", async () =>
        {
            var ingredients = await _databaseService.GetIngredientsAsync();
            return (ingredients != null, "", false);
        });

        // Test 2: Ingredients Loading
        await TestAsync(output, "Ingredients Loading", async () =>
        {
            var ingredients = await _databaseService.GetIngredientsAsync();
            var count = ingredients?.Count ?? 0;
            if (count < 10) return (false, $"Only {count} ingredients (expected 15+)", false);
            if (count > 20) return (false, $"Too many ingredients: {count} (expected ~15)", false);
            return (true, $"{count} ingredients loaded", false);
        });

        // Test 3: Recipes Loading
        await TestAsync(output, "Recipes Loading", async () =>
        {
            var recipes = await _databaseService.GetRecipesAsync();
            var count = recipes?.Count ?? 0;
            if (count == 0) return (false, "No recipes found", false);
            return (true, $"{count} recipes loaded", false);
        });

        // Test 4: Cities Loading
        await TestAsync(output, "Cities Loading", async () =>
        {
            var cities = await _databaseService.GetCitiesAsync();
            var count = cities?.Count ?? 0;
            if (count != 5) return (false, $"Expected 5 cities, found {count}", false);
            return (true, $"{count} cities loaded", false);
        });

        // Test 5: Events Loading
        await TestAsync(output, "Events Loading", async () =>
        {
            var events = await _databaseService.GetEventsAsync();
            var count = events?.Count ?? 0;
            if (count < 5) return (false, $"Only {count} events (expected 10+)", false);
            return (true, $"{count} events loaded", false);
        });

        // Test 6: Player State Exists
        await TestAsync(output, "Player State Persistence", async () =>
        {
            var state = await _databaseService.GetPlayerStateAsync();
            return (state != null, "", false);
        });
    }

    private async Task RunServiceTests(StringBuilder output)
    {
        // Test 1: Game State Service
        await TestAsync(output, "GameStateService Initialization", async () =>
        {
            return (await Task.FromResult(_gameStateService != null), "", false);
        });

        // Test 2: Market Service
        await TestAsync(output, "MarketService Initialization", async () =>
        {
            return (await Task.FromResult(_marketService != null), "", false);
        });

        // Test 3: Crafting Service
        await TestAsync(output, "CraftingService Initialization", async () =>
        {
            return (await Task.FromResult(_craftingService != null), "", false);
        });

        // Test 4: Inventory Service
        await TestAsync(output, "InventoryService Initialization", async () =>
        {
            return (await Task.FromResult(_inventoryService != null), "", false);
        });

        // Test 5: Travel Service
        await TestAsync(output, "TravelService Initialization", async () =>
        {
            return (await Task.FromResult(_travelService != null), "", false);
        });

        // Test 6: Event Service
        await TestAsync(output, "EventService Initialization", async () =>
        {
            return (await Task.FromResult(_eventService != null), "", false);
        });
    }

    private async Task RunGameStateTests(StringBuilder output)
    {
        // Test 1: Player State Loaded
        await TestAsync(output, "Player State Loaded", async () =>
        {
            var state = _gameStateService.PlayerState;
            return (await Task.FromResult(state != null), "", false);
        });

        // Test 2: Current City Set
        await TestAsync(output, "Current City Valid", async () =>
        {
            var city = _gameStateService.CurrentCity;
            if (city == null) return (false, "No current city set", false);
            return (true, $"Current city: {city.Name}", false);
        });

        // Test 3: Player Coins Valid
        await TestAsync(output, "Player Coins Valid", async () =>
        {
            var coins = _gameStateService.PlayerState?.Coins ?? 0;
            if (coins < 0) return (false, $"Negative coins: {coins}", false);
            return (true, $"Coins: {coins:N0}", false);
        });

        // Test 4: Reputation Values Valid
        await TestAsync(output, "Reputation Values Valid", async () =>
        {
            var state = _gameStateService.PlayerState;
            if (state == null) return (false, "", false);
            
            var trust = state.TrustReputation;
            var infamy = state.InfamyReputation;
            var lucidity = state.LucidityReputation;
            
            if (trust < 0 || trust > 100) return (false, $"Trust out of range: {trust}", false);
            if (infamy < 0 || infamy > 100) return (false, $"Infamy out of range: {infamy}", false);
            if (lucidity < 0 || lucidity > 100) return (false, $"Lucidity out of range: {lucidity}", false);
            
            return (true, $"T:{trust} I:{infamy} L:{lucidity}", false);
        });

        // Test 5: Inventory Accessible
        await TestAsync(output, "Inventory Accessible", async () =>
        {
            var inventory = _gameStateService.PlayerState?.Inventory;
            return (await Task.FromResult(inventory != null), "", false);
        });

        // Test 6: Day Counter Valid
        await TestAsync(output, "Day Counter Valid", async () =>
        {
            var day = _gameStateService.PlayerState?.CurrentDay ?? 0;
            if (day < 1) return (false, $"Invalid day: {day}", false);
            return (true, $"Day {day}", false);
        });
    }

    private async Task RunMarketTests(StringBuilder output)
    {
        // Test 1: Get Current Prices
        await TestAsync(output, "Get Market Prices", async () =>
        {
            if (_gameStateService.CurrentCity == null) return (false, "No current city", false);
            var prices = await _marketService.GetCurrentPricesAsync(_gameStateService.CurrentCity.Id);
            if (prices == null || prices.Count == 0) return (false, "No prices returned", false);
            return (true, $"{prices.Count} items priced", false);
        });

        // Test 2: Price Calculation Range
        await TestAsync(output, "Price Calculation Valid", async () =>
        {
            if (_gameStateService.CurrentCity == null) return (false, "No current city", false);
            var prices = await _marketService.GetCurrentPricesAsync(_gameStateService.CurrentCity.Id);
            if (prices == null || prices.Count == 0) return (false, "", false);
            
            var invalidPrices = prices.Where(p => p.CurrentPrice <= 0).ToList();
            if (invalidPrices.Any())
                return (false, $"{invalidPrices.Count} items with invalid prices", false);
            
            var avgPrice = prices.Average(p => p.CurrentPrice);
            return (true, $"Avg price: {avgPrice:N2}", false);
        });

        // Test 3: Buy Operation
        await TestAsync(output, "Buy Operation Logic", async () =>
        {
            if (_gameStateService.CurrentCity == null) return (false, "No current city", false);
            var prices = await _marketService.GetCurrentPricesAsync(_gameStateService.CurrentCity.Id);
            if (prices == null || prices.Count == 0) return (false, "", false);
            
            var cheapest = prices.OrderBy(p => p.CurrentPrice).First();
            var canAfford = _gameStateService.PlayerState?.Coins >= cheapest.CurrentPrice;
            
            return (true, canAfford ? "Can buy cheapest item" : "Cannot afford cheapest item (expected early game)", false);
        });

        // Test 4: Price History
        await TestAsync(output, "Price History Tracking", async () =>
        {
            if (_gameStateService.CurrentCity == null) return (false, "No current city", false);
            var prices = await _marketService.GetCurrentPricesAsync(_gameStateService.CurrentCity.Id);
            if (prices == null || prices.Count == 0) return (false, "", false);
            
            // Price change tracking is available via PriceChangePercent
            var withChange = prices.Count(p => p.PriceChangePercent != 0);
            return (true, $"{withChange} items with price changes", withChange == 0);
        });
    }

    private async Task RunCraftingTests(StringBuilder output)
    {
        // Test 1: Known Recipes Available
        await TestAsync(output, "Known Recipes Available", async () =>
        {
            var recipes = await _databaseService.GetRecipesAsync();
            var count = recipes?.Count ?? 0;
            if (count == 0) return (false, "No recipes in database", false);
            return (true, $"{count} recipes available", false);
        });

        // Test 2: Recipe Ingredient Validation
        await TestAsync(output, "Recipe Ingredient IDs Valid", async () =>
        {
            var recipes = await _databaseService.GetRecipesAsync();
            var ingredients = await _databaseService.GetIngredientsAsync();
            
            if (recipes == null || ingredients == null) return (false, "", false);
            
            var ingredientIds = ingredients.Select(i => i.Id).ToHashSet();
            var invalidRecipes = new List<string>();
            
            foreach (var recipe in recipes)
            {
                if (recipe.RequiredIngredients != null)
                {
                    foreach (var reqId in recipe.RequiredIngredients)
                    {
                        if (!ingredientIds.Contains(reqId))
                        {
                            invalidRecipes.Add(recipe.Name);
                            break;
                        }
                    }
                }
            }
            
            if (invalidRecipes.Any())
                return (false, $"{invalidRecipes.Count} recipes with invalid ingredients", false);
            
            return (true, "All recipe ingredients valid", false);
        });

        // Test 3: Crafting Service Can Match Recipes
        await TestAsync(output, "Recipe Matching Logic", async () =>
        {
            // This is a logic test, actual crafting would require ingredients
            return (true, "Service ready", true);
        });
    }

    private async Task RunTravelTests(StringBuilder output)
    {
        // Test 1: Get All Cities
        await TestAsync(output, "Get All Cities", async () =>
        {
            var cities = await _travelService.GetAllCitiesAsync();
            if (cities == null || cities.Count != 5)
                return (false, $"Expected 5 cities, got {cities?.Count ?? 0}", false);
            return (true, $"{cities.Count} cities available", false);
        });

        // Test 2: Unlocked Cities Logic
        await TestAsync(output, "Unlocked Cities Logic", async () =>
        {
            var unlocked = await _travelService.GetUnlockedCitiesAsync();
            if (unlocked == null || unlocked.Count == 0)
                return (false, "No cities unlocked", false);
            
            var hasSomnia = unlocked.Any(c => c.Id == "somnia_terminal");
            if (!hasSomnia)
                return (false, "Starting city (Somnia Terminal) not unlocked", false);
            
            return (true, $"{unlocked.Count} cities unlocked", false);
        });

        // Test 3: City Unlock Check
        await TestAsync(output, "City Unlock Check (Somnia)", async () =>
        {
            var isUnlocked = _travelService.IsCityUnlocked("somnia_terminal");
            if (!isUnlocked)
                return (false, "Somnia Terminal should always be unlocked", false);
            return (true, "Starting city accessible", false);
        });

        // Test 4: Can Travel Logic
        await TestAsync(output, "Can Travel To Logic", async () =>
        {
            var canTravel = await _travelService.CanTravelToAsync("somnia_terminal");
            if (!canTravel)
                return (false, "Cannot travel to unlocked city", false);
            return (true, "Travel logic valid", false);
        });

        // Test 5: City Data Integrity
        await TestAsync(output, "City Tag Modifiers Valid", async () =>
        {
            var cities = await _travelService.GetAllCitiesAsync();
            if (cities == null) return (false, "", false);
            
            foreach (var city in cities)
            {
                if (city.TagModifiers == null || city.TagModifiers.Count == 0)
                    return (false, $"{city.Name} has no tag modifiers", false);
            }
            
            return (true, "All cities have tag modifiers", false);
        });
    }

    private async Task RunEventTests(StringBuilder output)
    {
        // Test 1: Events Exist
        await TestAsync(output, "Events Database", async () =>
        {
            var events = await _databaseService.GetEventsAsync();
            var count = events?.Count ?? 0;
            if (count == 0) return (false, "No events in database", false);
            return (true, $"{count} events available", false);
        });

        // Test 2: Event Generation
        await TestAsync(output, "Event Generation Logic", async () =>
        {
            // Event generation is random, just verify service works
            return (true, "Service ready", true);
        });

        // Test 3: Active Events
        await TestAsync(output, "Active Events Tracking", async () =>
        {
            var activeEvents = _gameStateService.PlayerState?.ActiveEvents;
            if (activeEvents == null) return (false, "Cannot access active events", false);
            return (true, $"{activeEvents.Count} active events", false);
        });
    }

    private async Task RunDataIntegrityTests(StringBuilder output)
    {
        // Test 1: No Orphaned Recipe Ingredients
        await TestAsync(output, "Recipe-Ingredient Integrity", async () =>
        {
            var recipes = await _databaseService.GetRecipesAsync();
            var ingredients = await _databaseService.GetIngredientsAsync();
            
            if (recipes == null || ingredients == null) return (false, "", false);
            
            var ingredientIds = ingredients.Select(i => i.Id).ToHashSet();
            var orphanedCount = 0;
            
            foreach (var recipe in recipes)
            {
                if (recipe.RequiredIngredients != null)
                {
                    orphanedCount += recipe.RequiredIngredients.Count(id => !ingredientIds.Contains(id));
                }
            }
            
            if (orphanedCount > 0)
                return (false, $"{orphanedCount} orphaned ingredient references", false);
            
            return (true, "No orphaned references", false);
        });

        // Test 2: Ingredient Tags Valid
        await TestAsync(output, "Ingredient Tags Format", async () =>
        {
            var ingredients = await _databaseService.GetIngredientsAsync();
            if (ingredients == null) return (false, "", false);
            
            var noTags = ingredients.Where(i => i.Tags == null || i.Tags.Count == 0).ToList();
            if (noTags.Any())
                return (false, $"{noTags.Count} ingredients without tags", true);
            
            return (true, "All ingredients have tags", false);
        });

        // Test 3: City Tag Modifiers Format
        await TestAsync(output, "City Tag Modifiers Format", async () =>
        {
            var cities = await _databaseService.GetCitiesAsync();
            if (cities == null) return (false, "", false);
            
            foreach (var city in cities)
            {
                if (city.TagModifiers == null || city.TagModifiers.Count == 0)
                    return (false, $"{city.Name} missing tag modifiers", false);
            }
            
            return (true, "All cities have modifiers", false);
        });
    }

    private async Task RunPerformanceTests(StringBuilder output)
    {
        // Test 1: Database Query Performance
        await TestAsync(output, "Database Query Speed", async () =>
        {
            var sw = Stopwatch.StartNew();
            await _databaseService.GetIngredientsAsync();
            sw.Stop();
            
            var ms = sw.ElapsedMilliseconds;
            if (ms > 1000) return (false, $"Too slow: {ms}ms", true);
            if (ms > 500) return (true, $"{ms}ms (acceptable)", true);
            return (true, $"{ms}ms (fast)", false);
        });

        // Test 2: Market Price Calculation Speed
        await TestAsync(output, "Market Calculation Speed", async () =>
        {
            if (_gameStateService.CurrentCity == null) return (false, "No current city", false);
            
            var sw = Stopwatch.StartNew();
            await _marketService.GetCurrentPricesAsync(_gameStateService.CurrentCity.Id);
            sw.Stop();
            
            var ms = sw.ElapsedMilliseconds;
            if (ms > 2000) return (false, $"Too slow: {ms}ms", true);
            if (ms > 1000) return (true, $"{ms}ms (acceptable)", true);
            return (true, $"{ms}ms (fast)", false);
        });

        // Test 3: Memory Usage Check
        await TestAsync(output, "Memory Usage", async () =>
        {
            var usedMB = GC.GetTotalMemory(false) / 1024.0 / 1024.0;
            if (usedMB > 500) return (false, $"High memory: {usedMB:F1}MB", true);
            if (usedMB > 200) return (true, $"{usedMB:F1}MB (normal)", true);
            return (true, $"{usedMB:F1}MB (low)", false);
        });
    }

    private async Task TestAsync(StringBuilder output, string testName, Func<Task<(bool success, string message, bool isWarning)>> test)
    {
        TotalTests++;
        
        try
        {
            var (success, message, isWarning) = await test();
            
            if (success && !isWarning)
            {
                PassedTests++;
                output.AppendLine($"✅ PASS: {testName}");
                if (!string.IsNullOrEmpty(message))
                    output.AppendLine($"         {message}");
            }
            else if (success && isWarning)
            {
                WarningTests++;
                output.AppendLine($"⚠️  WARN: {testName}");
                if (!string.IsNullOrEmpty(message))
                    output.AppendLine($"         {message}");
            }
            else
            {
                FailedTests++;
                output.AppendLine($"❌ FAIL: {testName}");
                if (!string.IsNullOrEmpty(message))
                    output.AppendLine($"         {message}");
            }
        }
        catch (Exception ex)
        {
            FailedTests++;
            output.AppendLine($"❌ FAIL: {testName}");
            output.AppendLine($"         Exception: {ex.Message}");
        }
    }
}
