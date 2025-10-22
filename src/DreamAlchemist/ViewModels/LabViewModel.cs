using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DreamAlchemist.ViewModels.Base;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Models.Entities;
using DreamAlchemist.Models.Supporting;

namespace DreamAlchemist.ViewModels;

public partial class LabViewModel : BaseViewModel
{
    private readonly ICraftingService _craftingService;
    private readonly IInventoryService _inventoryService;
    private readonly IGameStateService _gameStateService;

    [ObservableProperty]
    private ObservableCollection<Ingredient> availableIngredients = new();

    [ObservableProperty]
    private ObservableCollection<Ingredient> selectedIngredients = new();

    [ObservableProperty]
    private ObservableCollection<Recipe> discoveredRecipes = new();

    [ObservableProperty]
    private ObservableCollection<CraftedDream> craftedDreams = new();

    [ObservableProperty]
    private Recipe? selectedRecipe;

    [ObservableProperty]
    private CraftedDream? selectedDream;

    [ObservableProperty]
    private bool canCraft;

    [ObservableProperty]
    private string craftResultMessage = string.Empty;

    [ObservableProperty]
    private bool showCraftResult;

    [ObservableProperty]
    private int maxIngredients = 3;

    public LabViewModel(
        INavigationService navigationService,
        ICraftingService craftingService,
        IInventoryService inventoryService,
        IGameStateService gameStateService)
        : base(navigationService)
    {
        _craftingService = craftingService;
        _inventoryService = inventoryService;
        _gameStateService = gameStateService;
        
        Title = "Synthesis Lab";
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadLabDataAsync();
    }

    [RelayCommand]
    private async Task LoadLabDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Load available ingredients from inventory
            var inventory = await _inventoryService.GetInventoryItemsAsync();
            AvailableIngredients.Clear();
            foreach (var kvp in inventory)
            {
                if (kvp.Value > 0)
                {
                    AvailableIngredients.Add(kvp.Key);
                }
            }

            // Load discovered recipes
            var recipes = await _craftingService.GetDiscoveredRecipesAsync();
            DiscoveredRecipes.Clear();
            foreach (var recipe in recipes)
            {
                DiscoveredRecipes.Add(recipe);
            }

            // Load crafted dreams
            var playerState = _gameStateService.PlayerState;
            CraftedDreams.Clear();
            foreach (var dream in playerState.CraftedDreams)
            {
                CraftedDreams.Add(dream);
            }

            UpdateCanCraft();
        });
    }

    [RelayCommand]
    private void AddIngredient(Ingredient ingredient)
    {
        if (SelectedIngredients.Count < MaxIngredients && 
            !SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Add(ingredient);
            UpdateCanCraft();
        }
    }

    [RelayCommand]
    private void RemoveIngredient(Ingredient ingredient)
    {
        SelectedIngredients.Remove(ingredient);
        UpdateCanCraft();
    }

    [RelayCommand]
    private void ClearIngredients()
    {
        SelectedIngredients.Clear();
        UpdateCanCraft();
    }

    [RelayCommand]
    private async Task CraftDreamAsync()
    {
        if (!CanCraft) return;

        await ExecuteAsync(async () =>
        {
            var ingredientIds = SelectedIngredients.Select(i => i.Id).ToList();
            var result = await _craftingService.CraftDreamAsync(ingredientIds);

            CraftResultMessage = result.Message;
            ShowCraftResult = true;

            if (result.Success)
            {
                ClearIngredients();
                await LoadLabDataAsync();
            }
        }, "Failed to craft dream");
    }

    [RelayCommand]
    private async Task CraftRecipeAsync(Recipe? recipe = null)
    {
        var recipeToCraft = recipe ?? SelectedRecipe;
        if (recipeToCraft == null) return;

        await ExecuteAsync(async () =>
        {
            var canCraft = await _craftingService.CanCraftRecipeAsync(recipeToCraft.Id);
            if (!canCraft)
            {
                ErrorMessage = "Missing required ingredients";
                return;
            }

            var result = await _craftingService.CraftRecipeAsync(recipeToCraft.Id);

            CraftResultMessage = result.Message;
            ShowCraftResult = true;

            if (result.Success)
            {
                await LoadLabDataAsync();
            }
        }, "Failed to craft recipe");
    }

    [RelayCommand]
    private void DismissCraftResult()
    {
        ShowCraftResult = false;
        CraftResultMessage = string.Empty;
    }

    [RelayCommand]
    private async Task LoadRecipeIngredientsAsync(Recipe recipe)
    {
        SelectedIngredients.Clear();
        
        foreach (var ingredientId in recipe.RequiredIngredients)
        {
            var ingredient = AvailableIngredients.FirstOrDefault(i => i.Id == ingredientId);
            if (ingredient != null)
            {
                SelectedIngredients.Add(ingredient);
            }
        }

        UpdateCanCraft();
        await Task.CompletedTask;
    }

    private void UpdateCanCraft()
    {
        CanCraft = SelectedIngredients.Count >= 2 && SelectedIngredients.Count <= MaxIngredients;
    }

    partial void OnSelectedIngredientsChanged(ObservableCollection<Ingredient> value)
    {
        UpdateCanCraft();
    }
}
