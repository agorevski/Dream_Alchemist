using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DreamAlchemist.ViewModels.Base;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;

namespace DreamAlchemist.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IGameStateService _gameStateService;
    private readonly IMarketService _marketService;
    private readonly IEventService _eventService;

    [ObservableProperty]
    private string playerName = string.Empty;

    [ObservableProperty]
    private int playerCoins;

    [ObservableProperty]
    private int currentDay;

    [ObservableProperty]
    private string currentCityName = string.Empty;

    [ObservableProperty]
    private int playerTier;

    [ObservableProperty]
    private string playerTierName = string.Empty;

    [ObservableProperty]
    private int trustReputation;

    [ObservableProperty]
    private int infamyReputation;

    [ObservableProperty]
    private int lucidityReputation;

    [ObservableProperty]
    private int inventoryCount;

    [ObservableProperty]
    private int craftedDreamsCount;

    [ObservableProperty]
    private int activeEventsCount;

    [ObservableProperty]
    private string quickStatus = string.Empty;

    public MainViewModel(
        INavigationService navigationService,
        IGameStateService gameStateService,
        IMarketService marketService,
        IEventService eventService)
        : base(navigationService)
    {
        _gameStateService = gameStateService;
        _marketService = marketService;
        _eventService = eventService;
        
        Title = "Dream Alchemist";
        
        // Subscribe to game state changes
        _gameStateService.PlayerStateChanged += OnPlayerStateChanged;
        _gameStateService.CityChanged += OnCityChanged;
        _gameStateService.DayProgressed += OnDayProgressed;
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadDashboardDataAsync();
    }

    [RelayCommand]
    private async Task LoadDashboardDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var playerState = _gameStateService.PlayerState;
            var currentCity = _gameStateService.CurrentCity;

            PlayerName = playerState.PlayerName;
            PlayerCoins = playerState.Coins;
            CurrentDay = playerState.CurrentDay;
            CurrentCityName = currentCity.Name;
            PlayerTier = playerState.Tier;
            PlayerTierName = Helpers.GameConstants.TierNames[playerState.Tier];
            
            TrustReputation = playerState.TrustReputation;
            InfamyReputation = playerState.InfamyReputation;
            LucidityReputation = playerState.LucidityReputation;
            
            InventoryCount = playerState.Inventory.Values.Sum();
            CraftedDreamsCount = playerState.CraftedDreams.Count;
            
            var activeEvents = await _eventService.GetActiveEventsAsync();
            ActiveEventsCount = activeEvents.Count;
            
            UpdateQuickStatus();
        });
    }

    [RelayCommand]
    private async Task NavigateToMarketAsync()
    {
        await NavigationService.NavigateToAsync("MarketPage");
    }

    [RelayCommand]
    private async Task NavigateToLabAsync()
    {
        await NavigationService.NavigateToAsync("LabPage");
    }

    [RelayCommand]
    private async Task NavigateToInventoryAsync()
    {
        await NavigationService.NavigateToAsync("InventoryPage");
    }

    [RelayCommand]
    private async Task NavigateToTravelAsync()
    {
        await NavigationService.NavigateToAsync("TravelPage");
    }

    [RelayCommand]
    private async Task ProgressDayAsync()
    {
        await ExecuteAsync(async () =>
        {
            await _gameStateService.ProgressTimeAsync(1);
            await LoadDashboardDataAsync();
        }, "Failed to progress day");
    }

    private void UpdateQuickStatus()
    {
        if (ActiveEventsCount > 0)
        {
            QuickStatus = $"{ActiveEventsCount} active event(s)";
        }
        else if (CraftedDreamsCount > 5)
        {
            QuickStatus = $"{CraftedDreamsCount} dreams ready to sell";
        }
        else if (InventoryCount > 20)
        {
            QuickStatus = $"{InventoryCount} ingredients in stock";
        }
        else
        {
            QuickStatus = $"Day {CurrentDay} • {CurrentCityName}";
        }
    }

    private void OnPlayerStateChanged(object? sender, Models.Entities.PlayerState e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await LoadDashboardDataAsync();
        });
    }

    private void OnCityChanged(object? sender, Models.Entities.City e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CurrentCityName = e.Name;
        });
    }

    private void OnDayProgressed(object? sender, int days)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CurrentDay += days;
        });
    }
}
