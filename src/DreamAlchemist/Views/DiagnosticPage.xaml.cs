using DreamAlchemist.Services.Data;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using System.Text;

namespace DreamAlchemist.Views;

public partial class DiagnosticPage : ContentPage
{
    private readonly IDatabaseService _databaseService;
    private readonly IMarketService _marketService;
    private readonly IGameStateService _gameStateService;
    private readonly ITravelService _travelService;

    public DiagnosticPage(
        IDatabaseService databaseService,
        IMarketService marketService,
        IGameStateService gameStateService,
        ITravelService travelService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _marketService = marketService;
        _gameStateService = gameStateService;
        _travelService = travelService;
    }

    private async void OnRunDiagnostics(object sender, EventArgs e)
    {
        var output = new StringBuilder();
        output.AppendLine("=== MARKET DIAGNOSTICS ===\n");

        try
        {
            // Check ingredients in database
            output.AppendLine("1. Checking ingredients in database...");
            var ingredients = await _databaseService.GetIngredientsAsync();
            output.AppendLine($"   Found {ingredients?.Count ?? 0} ingredients");
            
            if (ingredients != null && ingredients.Count > 0)
            {
                var first = ingredients[0];
                output.AppendLine($"   First: {first.Name}");
                output.AppendLine($"   Tags: {first.Tags?.Count ?? 0} tags");
                if (first.Tags != null && first.Tags.Count > 0)
                {
                    output.AppendLine($"   First tag: {first.Tags[0]}");
                }
            }
            output.AppendLine();

            // Check cities in database
            output.AppendLine("2. Checking cities in database...");
            var cities = await _databaseService.GetCitiesAsync();
            output.AppendLine($"   Found {cities?.Count ?? 0} cities");
            
            if (cities != null && cities.Count > 0)
            {
                var somnia = cities.FirstOrDefault(c => c.Id == "somnia_terminal");
                if (somnia != null)
                {
                    output.AppendLine($"   Somnia Terminal found: {somnia.Name}");
                    output.AppendLine($"   Tag modifiers: {somnia.TagModifiers?.Count ?? 0}");
                }
                else
                {
                    output.AppendLine("   ERROR: Somnia Terminal not found!");
                }
            }
            output.AppendLine();

            // Check game state
            output.AppendLine("3. Checking game state...");
            output.AppendLine($"   Current city: {_gameStateService.CurrentCity?.Name ?? "NULL"}");
            output.AppendLine($"   Current city ID: {_gameStateService.CurrentCity?.Id ?? "NULL"}");
            output.AppendLine($"   Player coins: {_gameStateService.PlayerState?.Coins ?? 0}");
            output.AppendLine();

            // Check market service
            output.AppendLine("4. Checking market service...");
            if (_gameStateService.CurrentCity != null)
            {
                var prices = await _marketService.GetCurrentPricesAsync(_gameStateService.CurrentCity.Id);
                output.AppendLine($"   Market returned {prices?.Count ?? 0} items");
                
                if (prices != null && prices.Count > 0)
                {
                    output.AppendLine($"   First item: {prices[0].IngredientName}");
                    output.AppendLine($"   Price: {prices[0].CurrentPrice} coins");
                }
                else
                {
                    output.AppendLine("   ERROR: No market items returned!");
                }
            }
            else
            {
                output.AppendLine("   ERROR: No current city!");
            }
            output.AppendLine();

            // Check travel service - SOMNIA TERMINAL DIAGNOSTICS
            output.AppendLine("5. SOMNIA TERMINAL UNLOCK DIAGNOSTICS...");
            var playerState = _gameStateService.PlayerState;
            output.AppendLine($"   Player UnlockedCities list: [{string.Join(", ", playerState.UnlockedCities)}]");
            output.AppendLine($"   Contains 'somnia_terminal': {playerState.UnlockedCities.Contains("somnia_terminal")}");
            output.AppendLine();
            
            output.AppendLine("   Testing IsCityUnlocked('somnia_terminal'):");
            var isSomniaUnlocked = _travelService.IsCityUnlocked("somnia_terminal");
            output.AppendLine($"   Result: {isSomniaUnlocked}");
            output.AppendLine();
            
            output.AppendLine("   Testing CanTravelToAsync('somnia_terminal'):");
            var canTravelToSomnia = await _travelService.CanTravelToAsync("somnia_terminal");
            output.AppendLine($"   Result: {canTravelToSomnia}");
            output.AppendLine();
            
            output.AppendLine("   Testing GetUnlockedCitiesAsync():");
            var unlockedCities = await _travelService.GetUnlockedCitiesAsync();
            output.AppendLine($"   Total unlocked: {unlockedCities.Count}");
            var somniaInList = unlockedCities.FirstOrDefault(c => c.Id == "somnia_terminal");
            output.AppendLine($"   Somnia Terminal in list: {somniaInList != null}");
            if (somniaInList != null)
            {
                output.AppendLine($"   Somnia name: {somniaInList.Name}");
            }
            output.AppendLine();
            
            output.AppendLine("   All cities from GetAllCitiesAsync():");
            var allCities = await _travelService.GetAllCitiesAsync();
            foreach (var city in allCities)
            {
                var unlocked = _travelService.IsCityUnlocked(city.Id);
                var canTravel = await _travelService.CanTravelToAsync(city.Id);
                output.AppendLine($"   - {city.Name} ({city.Id})");
                output.AppendLine($"     Unlocked: {unlocked}, CanTravel: {canTravel}");
            }

        }
        catch (Exception ex)
        {
            output.AppendLine($"\nERROR: {ex.Message}");
            output.AppendLine($"Stack: {ex.StackTrace}");
        }

        DiagnosticOutput.Text = output.ToString();
    }
}
