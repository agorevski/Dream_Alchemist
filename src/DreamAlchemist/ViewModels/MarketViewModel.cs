using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DreamAlchemist.ViewModels.Base;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Models.DTOs;

namespace DreamAlchemist.ViewModels;

public partial class MarketViewModel : BaseViewModel
{
    private readonly IMarketService _marketService;
    private readonly IGameStateService _gameStateService;

    [ObservableProperty]
    private ObservableCollection<MarketPriceDto> marketItems = new();

    [ObservableProperty]
    private MarketPriceDto? selectedItem;

    [ObservableProperty]
    private int playerCoins;

    [ObservableProperty]
    private string currentCityName = string.Empty;

    [ObservableProperty]
    private int buyQuantity = 1;

    [ObservableProperty]
    private int sellQuantity = 1;

    [ObservableProperty]
    private string filterText = string.Empty;

    [ObservableProperty]
    private bool showTrendingOnly;

    public MarketViewModel(
        INavigationService navigationService,
        IMarketService marketService,
        IGameStateService gameStateService)
        : base(navigationService)
    {
        _marketService = marketService;
        _gameStateService = gameStateService;
        
        Title = "Dream Market";
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadMarketDataAsync();
    }

    [RelayCommand]
    private async Task LoadMarketDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var currentCity = _gameStateService.CurrentCity;
            CurrentCityName = currentCity.Name;
            PlayerCoins = _gameStateService.PlayerState.Coins;
            
            System.Diagnostics.Debug.WriteLine($"Loading market data for city: {currentCity.Name} (ID: {currentCity.Id})");
            
            var prices = await _marketService.GetCurrentPricesAsync(currentCity.Id);
            
            System.Diagnostics.Debug.WriteLine($"Got {prices?.Count ?? 0} prices from market service");
            
            // Apply filters
            if (ShowTrendingOnly)
            {
                prices = prices.Where(p => p.IsTrending).ToList();
                System.Diagnostics.Debug.WriteLine($"After trending filter: {prices.Count} items");
            }
            
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                prices = prices.Where(p => 
                    p.IngredientName.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                System.Diagnostics.Debug.WriteLine($"After text filter: {prices.Count} items");
            }
            
            MarketItems.Clear();
            foreach (var price in prices)
            {
                MarketItems.Add(price);
                System.Diagnostics.Debug.WriteLine($"Added market item: {price.IngredientName} - {price.CurrentPrice} coins");
            }
            
            System.Diagnostics.Debug.WriteLine($"Final MarketItems count: {MarketItems.Count}");
        });
    }

    [RelayCommand]
    private async Task BuyItemAsync(MarketPriceDto? item = null)
    {
        var itemToBuy = item ?? SelectedItem;
        if (itemToBuy == null) return;

        await ExecuteAsync(async () =>
        {
            var success = await _marketService.BuyIngredientAsync(
                itemToBuy.IngredientId,
                BuyQuantity);
            
            if (success)
            {
                await LoadMarketDataAsync();
                BuyQuantity = 1;
            }
            else
            {
                ErrorMessage = "Purchase failed. Check coins and inventory space.";
            }
        }, "Failed to purchase item");
    }

    [RelayCommand]
    private async Task SellItemAsync(MarketPriceDto? item = null)
    {
        var itemToSell = item ?? SelectedItem;
        if (itemToSell == null) return;

        await ExecuteAsync(async () =>
        {
            var success = await _marketService.SellIngredientAsync(
                itemToSell.IngredientId,
                SellQuantity);
            
            if (success)
            {
                await LoadMarketDataAsync();
                SellQuantity = 1;
            }
            else
            {
                ErrorMessage = "Sale failed. Check inventory.";
            }
        }, "Failed to sell item");
    }

    [RelayCommand]
    private async Task RefreshMarketAsync()
    {
        await LoadMarketDataAsync();
    }

    [RelayCommand]
    private async Task ToggleTrendingFilterAsync()
    {
        ShowTrendingOnly = !ShowTrendingOnly;
        await LoadMarketDataAsync();
    }

    [RelayCommand]
    private async Task ApplyFilterAsync()
    {
        await LoadMarketDataAsync();
    }

    [RelayCommand]
    private void IncreaseBuyQuantity()
    {
        BuyQuantity++;
    }

    [RelayCommand]
    private void DecreaseBuyQuantity()
    {
        if (BuyQuantity > 1)
            BuyQuantity--;
    }

    [RelayCommand]
    private void IncreaseSellQuantity()
    {
        SellQuantity++;
    }

    [RelayCommand]
    private void DecreaseSellQuantity()
    {
        if (SellQuantity > 1)
            SellQuantity--;
    }

    partial void OnSelectedItemChanged(MarketPriceDto? value)
    {
        BuyQuantity = 1;
        SellQuantity = 1;
    }
}
