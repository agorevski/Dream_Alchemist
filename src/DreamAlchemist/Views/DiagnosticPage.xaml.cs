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

    public DiagnosticPage(
        IDatabaseService databaseService,
        IMarketService marketService,
        IGameStateService gameStateService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _marketService = marketService;
        _gameStateService = gameStateService;
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

        }
        catch (Exception ex)
        {
            output.AppendLine($"\nERROR: {ex.Message}");
            output.AppendLine($"Stack: {ex.StackTrace}");
        }

        DiagnosticOutput.Text = output.ToString();
    }
}
