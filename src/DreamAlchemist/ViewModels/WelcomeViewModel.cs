using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.ViewModels.Base;
using System.Collections.ObjectModel;

namespace DreamAlchemist.ViewModels;

public partial class WelcomeViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private bool hasExistingSave;

    [ObservableProperty]
    private string saveStatusMessage = string.Empty;

#if DEBUG
    [ObservableProperty]
    private bool isDebugMode = true;

    [ObservableProperty]
    private string selectedTheme = "Vibrant Energy";

    [ObservableProperty]
    private string selectedFont = "Classic Mystical";

    [ObservableProperty]
    private string currentCombination = "Vibrant Energy + Classic Mystical";

    public ObservableCollection<string> ThemeOptions { get; } = new()
    {
        "Vibrant Energy",
        "Ethereal Dream",
        "Mystical Jewel"
    };

    public ObservableCollection<string> FontOptions { get; } = new()
    {
        "Classic Mystical",
        "Ethereal Light",
        "Futuristic Dream"
    };
#else
    [ObservableProperty]
    private bool isDebugMode = false;
#endif

    public WelcomeViewModel(
        INavigationService navigationService,
        IDatabaseService databaseService,
        IGameStateService gameStateService,
        IThemeService themeService)
        : base(navigationService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
        _themeService = themeService;
        Title = "Dream Alchemist";
    }

    partial void OnSelectedThemeChanged(string value)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"🎨 OnSelectedThemeChanged called with value: {value}");
        ApplyThemeSelection();
#endif
    }

    partial void OnSelectedFontChanged(string value)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"✏️ OnSelectedFontChanged called with value: {value}");
        ApplyFontSelection();
#endif
    }

#if DEBUG
    private void ApplyThemeSelection()
    {
        System.Diagnostics.Debug.WriteLine($"🎨 ApplyThemeSelection: SelectedTheme = '{SelectedTheme}'");
        
        var theme = SelectedTheme switch
        {
            "Vibrant Energy" => ThemeVariant.VibrantEnergy,
            "Ethereal Dream" => ThemeVariant.EtherealDream,
            "Mystical Jewel" => ThemeVariant.MysticalJewel,
            _ => ThemeVariant.VibrantEnergy
        };

        System.Diagnostics.Debug.WriteLine($"🎨 Mapped to theme enum: {theme}");
        _themeService.SetTheme(theme);
        UpdateCurrentCombination();
        System.Diagnostics.Debug.WriteLine($"🎨 Theme application complete: {theme}");
    }

    private void ApplyFontSelection()
    {
        System.Diagnostics.Debug.WriteLine($"✏️ ApplyFontSelection: SelectedFont = '{SelectedFont}'");
        
        var fontScheme = SelectedFont switch
        {
            "Classic Mystical" => FontScheme.ClassicMystical,
            "Ethereal Light" => FontScheme.EtherealLight,
            "Futuristic Dream" => FontScheme.FuturisticDream,
            _ => FontScheme.ClassicMystical
        };

        System.Diagnostics.Debug.WriteLine($"✏️ Mapped to font enum: {fontScheme}");
        _themeService.SetFontScheme(fontScheme);
        UpdateCurrentCombination();
        System.Diagnostics.Debug.WriteLine($"✏️ Font scheme application complete: {fontScheme}");
    }

    private void UpdateCurrentCombination()
    {
        CurrentCombination = $"{SelectedTheme} + {SelectedFont}";
        System.Diagnostics.Debug.WriteLine($"📝 Current Combination updated: {CurrentCombination}");
    }
#endif

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
