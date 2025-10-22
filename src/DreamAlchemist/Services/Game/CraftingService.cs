using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Models.Entities;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Models.Supporting;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;

namespace DreamAlchemist.Services.Game;

public class CraftingService : ICraftingService
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;

    public CraftingService(
        IDatabaseService databaseService,
        IGameStateService gameStateService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
    }

    public async Task<CraftResultDto> CraftDreamAsync(List<string> ingredientIds)
    {
        if (ingredientIds == null || ingredientIds.Count < 2 || ingredientIds.Count > 3)
        {
            return new CraftResultDto
            {
                Success = false,
                Message = "Must use 2-3 ingredients to craft a dream"
            };
        }

        var playerState = _gameStateService.PlayerState;
        
        // Verify player has all ingredients
        foreach (var ingredientId in ingredientIds)
        {
            if (!playerState.Inventory.ContainsKey(ingredientId) || 
                playerState.Inventory[ingredientId] < 1)
            {
                return new CraftResultDto
                {
                    Success = false,
                    Message = "Missing required ingredients"
                };
            }
        }

        // Try to find matching recipe
        var recipes = await _databaseService.GetRecipesAsync();
        var matchingRecipe = FindMatchingRecipe(recipes, ingredientIds);

        if (matchingRecipe != null)
        {
            // Known recipe found
            return await CraftKnownRecipe(matchingRecipe, ingredientIds);
        }
        else
        {
            // Experimental crafting - random chance of discovery
            return await CraftExperimentalDream(ingredientIds);
        }
    }

    public async Task<List<Recipe>> GetDiscoveredRecipesAsync()
    {
        return await _databaseService.GetDiscoveredRecipesAsync();
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        return await _databaseService.GetRecipesAsync();
    }

    public async Task<bool> CanCraftRecipeAsync(string recipeId)
    {
        var recipe = await _databaseService.GetRecipeAsync(recipeId);
        if (recipe == null)
            return false;

        var playerState = _gameStateService.PlayerState;
        
        foreach (var ingredientId in recipe.RequiredIngredients)
        {
            if (!playerState.Inventory.ContainsKey(ingredientId) || 
                playerState.Inventory[ingredientId] < 1)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<CraftResultDto> CraftRecipeAsync(string recipeId)
    {
        var recipe = await _databaseService.GetRecipeAsync(recipeId);
        if (recipe == null)
        {
            return new CraftResultDto
            {
                Success = false,
                Message = "Recipe not found"
            };
        }

        if (!await CanCraftRecipeAsync(recipeId))
        {
            return new CraftResultDto
            {
                Success = false,
                Message = "Missing required ingredients"
            };
        }

        return await CraftKnownRecipe(recipe, recipe.RequiredIngredients);
    }

    public async Task<List<Recipe>> GetCraftingSuggestionsAsync()
    {
        var playerState = _gameStateService.PlayerState;
        var discoveredRecipes = await GetDiscoveredRecipesAsync();
        
        var suggestions = new List<Recipe>();
        
        foreach (var recipe in discoveredRecipes)
        {
            // Check if player has at least one of the required ingredients
            var hasIngredient = recipe.RequiredIngredients
                .Any(id => playerState.Inventory.ContainsKey(id) && 
                          playerState.Inventory[id] > 0);
            
            if (hasIngredient)
            {
                suggestions.Add(recipe);
            }
        }

        return suggestions;
    }

    // Private helper methods

    private Recipe? FindMatchingRecipe(List<Recipe> recipes, List<string> ingredientIds)
    {
        var sortedInput = ingredientIds.OrderBy(x => x).ToList();
        
        foreach (var recipe in recipes)
        {
            var sortedRecipe = recipe.RequiredIngredients.OrderBy(x => x).ToList();
            
            if (sortedInput.Count == sortedRecipe.Count && 
                sortedInput.SequenceEqual(sortedRecipe))
            {
                return recipe;
            }
        }

        return null;
    }

    private async Task<CraftResultDto> CraftKnownRecipe(Recipe recipe, List<string> ingredientIds)
    {
        var playerState = _gameStateService.PlayerState;
        
        // Remove ingredients from inventory
        foreach (var ingredientId in ingredientIds)
        {
            playerState.RemoveFromInventory(ingredientId, 1);
        }

        // Get ingredient details for value calculation
        var ingredients = new List<Ingredient>();
        foreach (var id in ingredientIds)
        {
            var ingredient = await _databaseService.GetIngredientAsync(id);
            if (ingredient != null)
            {
                ingredients.Add(ingredient);
                playerState.CurrentWeight -= ingredient.Weight;
            }
        }

        // Calculate dream value
        var baseValue = ingredients.Sum(i => i.BaseValue);
        var dreamValue = baseValue * recipe.ValueMultiplier;

        // Create crafted dream
        var craftedDream = new CraftedDream
        {
            Id = Guid.NewGuid().ToString(),
            RecipeId = recipe.Id,
            Name = recipe.Name,
            Value = dreamValue,
            Rarity = recipe.Rarity,
            NarrativeText = recipe.NarrativeText ?? GenerateNarrativeText(recipe, ingredients),
            CraftedAt = DateTime.UtcNow
        };

        playerState.CraftedDreams.Add(craftedDream);

        // Check if this is a new discovery
        var newDiscovery = !recipe.Discovered;
        if (newDiscovery)
        {
            recipe.Discovered = true;
            playerState.DiscoveredRecipes.Add(recipe.Id);
            await _databaseService.SaveRecipeAsync(recipe);
            
            // Reputation bonus for discovery
            await _gameStateService.UpdateReputationAsync(0, 0, 5);
        }

        await _gameStateService.SaveGameAsync();

        return new CraftResultDto
        {
            Success = true,
            RecipeId = recipe.Id,
            RecipeName = recipe.Name,
            NewDiscovery = newDiscovery,
            CraftedDream = craftedDream,
            Message = newDiscovery ? 
                $"Discovery! You've created: {recipe.Name}" : 
                $"Successfully crafted: {recipe.Name}",
            NarrativeText = craftedDream.NarrativeText
        };
    }

    private async Task<CraftResultDto> CraftExperimentalDream(List<string> ingredientIds)
    {
        var playerState = _gameStateService.PlayerState;
        var random = new Random();
        
        // 20% chance of successful experimental craft
        var successChance = 0.2;
        
        // Increase chance based on player's Lucidity reputation
        successChance += playerState.LucidityReputation / 1000.0;
        
        if (random.NextDouble() > successChance)
        {
            // Failed experiment - still consume ingredients
            foreach (var ingredientId in ingredientIds)
            {
                playerState.RemoveFromInventory(ingredientId, 1);
                var ingredient = await _databaseService.GetIngredientAsync(ingredientId);
                if (ingredient != null)
                {
                    playerState.CurrentWeight -= ingredient.Weight;
                }
            }
            
            await _gameStateService.SaveGameAsync();
            
            return new CraftResultDto
            {
                Success = false,
                Message = "The ingredients dissolved into meaningless fragments. Experiment failed.",
                NarrativeText = "The dream ingredients refused to coalesce, their essences scattering like morning mist."
            };
        }

        // Success - create a basic dream
        var ingredients = new List<Ingredient>();
        foreach (var id in ingredientIds)
        {
            var ingredient = await _databaseService.GetIngredientAsync(id);
            if (ingredient != null)
            {
                ingredients.Add(ingredient);
            }
        }

        // Remove ingredients
        foreach (var ingredientId in ingredientIds)
        {
            playerState.RemoveFromInventory(ingredientId, 1);
        }

        // Calculate value (lower than recipe crafting)
        var baseValue = ingredients.Sum(i => i.BaseValue);
        var dreamValue = baseValue * 1.2m; // Lower multiplier for experimental

        // Determine rarity (average of ingredients)
        var avgRarity = (int)Math.Round(ingredients.Average(i => (int)i.Rarity));
        var dreamRarity = (Rarity)Math.Clamp(avgRarity, 1, 5);

        var craftedDream = new CraftedDream
        {
            Id = Guid.NewGuid().ToString(),
            RecipeId = string.Empty,
            Name = $"Experimental {string.Join("-", ingredients.Take(2).Select(i => i.Name.Split(' ').First()))} Dream",
            Value = dreamValue,
            Rarity = dreamRarity,
            NarrativeText = GenerateExperimentalNarrativeText(ingredients),
            CraftedAt = DateTime.UtcNow
        };

        playerState.CraftedDreams.Add(craftedDream);
        
        // Small lucidity reputation gain
        await _gameStateService.UpdateReputationAsync(0, 0, 2);
        await _gameStateService.SaveGameAsync();

        return new CraftResultDto
        {
            Success = true,
            RecipeName = craftedDream.Name,
            NewDiscovery = false,
            CraftedDream = craftedDream,
            Message = $"Experimental success! Created: {craftedDream.Name}",
            NarrativeText = craftedDream.NarrativeText
        };
    }

    private string GenerateNarrativeText(Recipe recipe, List<Ingredient> ingredients)
    {
        // Simple narrative generation (can be enhanced with AI later)
        var ingredientNames = string.Join(", ", ingredients.Select(i => i.Name));
        return $"By combining {ingredientNames}, you've woven {recipe.Name}. " +
               $"This {recipe.Alignment} dream carries the essence of {recipe.Tags.FirstOrDefault().ToString().ToLower()}.";
    }

    private string GenerateExperimentalNarrativeText(List<Ingredient> ingredients)
    {
        var primaryIngredient = ingredients.First();
        return $"An experimental fusion of dream fragments. The {primaryIngredient.Name} dominates the composition, " +
               $"creating an unpredictable yet stable dream construct.";
    }
}
