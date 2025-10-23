using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.ViewModels.Base;

namespace DreamAlchemist.ViewModels;

public partial class WelcomeViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;

    [ObservableProperty]
    private bool hasExistingSave;

    [ObservableProperty]
    private string saveStatusMessage = string.Empty;

    public WelcomeViewModel(
        INavigationService navigationService,
        IDatabaseService databaseService,
        IGameStateService gameStateService)
        : base(navigationService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
        Title = "Dream Alchemist";
    }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await CheckForExistingSaveAsync();
    }

    private async Task CheckForExistingSaveAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Check if database has a player state
            var playerState = await _databaseService.GetPlayerStateAsync();
            HasExistingSave = playerState != null;

            if (HasExistingSave)
            {
                SaveStatusMessage = $"Last played: {playerState!.LastSaved:MMM dd, yyyy}";
            }
            else
            {
                SaveStatusMessage = "No saved game found. Start a new adventure!";
            }
        });
    }

    [RelayCommand]
    private async Task StartNewGameAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Clear the database and reinitialize
            await _databaseService.ClearDatabaseAsync();
            
            // Initialize game state (this will create a new player)
            await _gameStateService.InitializeAsync(forceNewGame: true);
            
            // Navigate to main game page
            await NavigationService.NavigateToAsync("//MainPage");
        }, "Starting new game...");
    }

    [RelayCommand]
    private async Task ContinueGameAsync()
    {
        if (!HasExistingSave)
            return;

        await ExecuteAsync(async () =>
        {
            // Initialize game state (this will load existing save)
            await _gameStateService.InitializeAsync(forceNewGame: false);
            
            // Navigate to main game page
            await NavigationService.NavigateToAsync("//MainPage");
        }, "Loading game...");
    }
}
