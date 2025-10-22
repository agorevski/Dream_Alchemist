using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DreamAlchemist.ViewModels.Base;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.ViewModels;

public partial class InventoryViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly IGameStateService _gameStateService;

    [ObservableProperty]
    private ObservableCollection<InventoryItemViewModel> inventoryItems = new();

    [ObservableProperty]
    private InventoryItemViewModel? selectedItem;

    [ObservableProperty]
    private int currentWeight;

    [ObservableProperty]
    private int maxWeight;

    [ObservableProperty]
    private double capacityPercentage;

    [ObservableProperty]
    private string capacityText = string.Empty;

    [ObservableProperty]
    private InventorySortMode currentSortMode = InventorySortMode.Name;

    public InventoryViewModel(
        INavigationService navigationService,
        IInventoryService inventoryService,
        IGameStateService gameStateService)
        : base(navigationService)
    {
        _inventoryService = inventoryService;
        _gameStateService = gameStateService;
        
        Title = "Inventory";
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadInventoryAsync();
    }

    [RelayCommand]
    private async Task LoadInventoryAsync()
    {
        await ExecuteAsync(async () =>
        {
            CurrentWeight = _inventoryService.GetCurrentWeight();
            MaxWeight = _inventoryService.GetMaxCapacity();
            CapacityPercentage = MaxWeight > 0 ? (double)CurrentWeight / MaxWeight * 100 : 0;
            CapacityText = $"{CurrentWeight} / {MaxWeight}";

            var inventory = await _inventoryService.GetSortedInventoryAsync(CurrentSortMode);
            
            InventoryItems.Clear();
            foreach (var kvp in inventory)
            {
                InventoryItems.Add(new InventoryItemViewModel
                {
                    Ingredient = kvp.Key,
                    Quantity = kvp.Value,
                    TotalWeight = kvp.Key.Weight * kvp.Value,
                    TotalValue = kvp.Key.BaseValue * kvp.Value
                });
            }
        });
    }

    [RelayCommand]
    private async Task SortByAsync(string sortMode)
    {
        if (Enum.TryParse<InventorySortMode>(sortMode, out var mode))
        {
            CurrentSortMode = mode;
            await LoadInventoryAsync();
        }
    }

    [RelayCommand]
    private async Task SortByNameAsync()
    {
        CurrentSortMode = InventorySortMode.Name;
        await LoadInventoryAsync();
    }

    [RelayCommand]
    private async Task SortByRarityAsync()
    {
        CurrentSortMode = InventorySortMode.Rarity;
        await LoadInventoryAsync();
    }

    [RelayCommand]
    private async Task SortByQuantityAsync()
    {
        CurrentSortMode = InventorySortMode.Quantity;
        await LoadInventoryAsync();
    }

    [RelayCommand]
    private async Task SortByValueAsync()
    {
        CurrentSortMode = InventorySortMode.Value;
        await LoadInventoryAsync();
    }
}

public partial class InventoryItemViewModel : ObservableObject
{
    [ObservableProperty]
    private Ingredient ingredient = null!;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private int totalWeight;

    [ObservableProperty]
    private decimal totalValue;
}
