using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DreamAlchemist.ViewModels.Base;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Game;
using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.ViewModels;

public partial class TravelViewModel : BaseViewModel
{
    private readonly ITravelService _travelService;
    private readonly IGameStateService _gameStateService;

    [ObservableProperty]
    private ObservableCollection<CityViewModel> cities = new();

    [ObservableProperty]
    private CityViewModel? selectedCity;

    [ObservableProperty]
    private string currentCityName = string.Empty;

    [ObservableProperty]
    private int playerCoins;

    [ObservableProperty]
    private int trustReputation;

    [ObservableProperty]
    private int infamyReputation;

    [ObservableProperty]
    private int lucidityReputation;

    [ObservableProperty]
    private int totalReputation;

    [ObservableProperty]
    private string playerTierName = string.Empty;

    [ObservableProperty]
    private bool showTravelConfirmation;

    [ObservableProperty]
    private string travelConfirmationMessage = string.Empty;

    public TravelViewModel(
        INavigationService navigationService,
        ITravelService travelService,
        IGameStateService gameStateService)
        : base(navigationService)
    {
        _travelService = travelService;
        _gameStateService = gameStateService;
        
        Title = "Travel";
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadCitiesAsync();
    }

    [RelayCommand]
    private async Task LoadCitiesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var currentCity = _gameStateService.CurrentCity;
            var playerState = _gameStateService.PlayerState;
            
            CurrentCityName = currentCity.Name;
            PlayerCoins = playerState.Coins;
            PlayerTierName = Helpers.GameConstants.TierNames[playerState.Tier];
            TrustReputation = playerState.TrustReputation;
            InfamyReputation = playerState.InfamyReputation;
            LucidityReputation = playerState.LucidityReputation;
            TotalReputation = playerState.TrustReputation + playerState.InfamyReputation + playerState.LucidityReputation;

            var allCities = await _travelService.GetAllCitiesAsync();
            
            Cities.Clear();
            foreach (var city in allCities)
            {
                var isUnlocked = _travelService.IsCityUnlocked(city.Id);
                var isCurrent = city.Id == currentCity.Id;
                var canTravel = await _travelService.CanTravelToAsync(city.Id);

                Cities.Add(new CityViewModel
                {
                    City = city,
                    IsUnlocked = isUnlocked,
                    IsCurrentCity = isCurrent,
                    CanTravel = canTravel && !isCurrent,
                    TravelCost = city.TravelCost,
                    TravelDays = city.TravelDays,
                    RequiredReputation = city.RequiredReputation
                });
            }
        });
    }

    [RelayCommand]
    private async Task SelectCityAsync(CityViewModel cityViewModel)
    {
        SelectedCity = cityViewModel;
        
        if (cityViewModel.IsCurrentCity)
        {
            TravelConfirmationMessage = "You are already in this city.";
            ShowTravelConfirmation = false;
        }
        else if (!cityViewModel.CanTravel)
        {
            var playerState = _gameStateService.PlayerState;
            var totalReputation = playerState.TrustReputation + 
                                playerState.InfamyReputation + 
                                playerState.LucidityReputation;
            
            // Check what's blocking travel
            if (totalReputation < cityViewModel.RequiredReputation)
            {
                var needed = cityViewModel.RequiredReputation - totalReputation;
                TravelConfirmationMessage = $"🔒 Locked: Requires {cityViewModel.RequiredReputation} total reputation.\n" +
                    $"Current: {totalReputation} (need {needed} more)";
            }
            else if (playerState.Coins < cityViewModel.TravelCost)
            {
                var needed = cityViewModel.TravelCost - playerState.Coins;
                TravelConfirmationMessage = $"💰 Insufficient funds: Need {cityViewModel.TravelCost} coins.\n" +
                    $"Current: {playerState.Coins} (need {needed} more)";
            }
            else
            {
                TravelConfirmationMessage = "Cannot travel to this city.";
            }
            ShowTravelConfirmation = false;
        }
        else
        {
            var unlockMessage = !cityViewModel.IsUnlocked ? "\n✨ This will unlock a new city!" : "";
            TravelConfirmationMessage = $"Travel to {cityViewModel.City.Name}?{unlockMessage}\n" +
                $"💰 Cost: {cityViewModel.TravelCost} coins\n" +
                $"⏰ Time: {cityViewModel.TravelDays} days";
            ShowTravelConfirmation = true;
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ConfirmTravelAsync()
    {
        if (SelectedCity == null) return;

        await ExecuteAsync(async () =>
        {
            var result = await _travelService.TravelToAsync(SelectedCity.City.Id);
            
            ShowTravelConfirmation = false;
            
            if (result.Success)
            {
                await LoadCitiesAsync();
                
                // Show travel result
                var message = result.Message;
                if (result.EventsTriggered.Count > 0)
                {
                    message += $"\n\nEvents during travel:\n" + 
                              string.Join("\n", result.EventsTriggered);
                }
                
                ErrorMessage = string.Empty;
                TravelConfirmationMessage = message;
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }, "Failed to travel");
    }

    [RelayCommand]
    private void CancelTravel()
    {
        ShowTravelConfirmation = false;
        SelectedCity = null;
    }
}

public partial class CityViewModel : ObservableObject
{
    [ObservableProperty]
    private City city = null!;

    [ObservableProperty]
    private bool isUnlocked;

    [ObservableProperty]
    private bool isCurrentCity;

    [ObservableProperty]
    private bool canTravel;

    [ObservableProperty]
    private int travelCost;

    [ObservableProperty]
    private int travelDays;

    [ObservableProperty]
    private int requiredReputation;
}
