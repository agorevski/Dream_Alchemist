using DreamAlchemist.Models.Entities;
using DreamAlchemist.Helpers;
using DreamAlchemist.Services.Data;

namespace DreamAlchemist.Services.Core;

public class GameStateService : IGameStateService
{
    private readonly IDatabaseService _databaseService;
    private PlayerState _playerState = null!;
    private City _currentCity = null!;

    public PlayerState PlayerState => _playerState;
    public City CurrentCity => _currentCity;
    public int CurrentDay => _playerState?.CurrentDay ?? 1;
    public bool IsInitialized { get; private set; }

    public event EventHandler<PlayerState>? PlayerStateChanged;
    public event EventHandler<City>? CityChanged;
    public event EventHandler<int>? DayProgressed;

    public GameStateService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        await _databaseService.InitializeDatabaseAsync();
        await LoadGameAsync();
        
        IsInitialized = true;
    }

    public async Task LoadGameAsync()
    {
        var playerState = await _databaseService.GetPlayerStateAsync();
        
        if (playerState == null)
        {
            // Create new game
            playerState = new PlayerState
            {
                PlayerName = "Novice Peddler",
                Coins = GameConstants.STARTING_COINS,
                CurrentDay = 1,
                CurrentCityId = GameConstants.STARTING_CITY,
                Tier = 1,
                MaxWeight = GameConstants.STARTING_WEIGHT_CAPACITY
            };
            await _databaseService.SavePlayerStateAsync(playerState);
        }

        _playerState = playerState;
        
        var city = await _databaseService.GetCityAsync(playerState.CurrentCityId);
        _currentCity = city ?? throw new InvalidOperationException("Current city not found");

        PlayerStateChanged?.Invoke(this, _playerState);
        CityChanged?.Invoke(this, _currentCity);
    }

    public async Task SaveGameAsync()
    {
        await _databaseService.SavePlayerStateAsync(_playerState);
    }

    public async Task ProgressTimeAsync(int days)
    {
        _playerState.CurrentDay += days;
        
        DayProgressed?.Invoke(this, days);
        PlayerStateChanged?.Invoke(this, _playerState);
        
        await SaveGameAsync();
    }

    public async Task TravelToCityAsync(string cityId)
    {
        var city = await _databaseService.GetCityAsync(cityId);
        if (city == null)
            throw new ArgumentException("City not found", nameof(cityId));

        if (!_playerState.UnlockedCities.Contains(cityId))
            throw new InvalidOperationException("City not unlocked");

        _playerState.CurrentCityId = cityId;
        _currentCity = city;

        CityChanged?.Invoke(this, _currentCity);
        PlayerStateChanged?.Invoke(this, _playerState);
        
        await SaveGameAsync();
    }

    public async Task UpdateReputationAsync(int trust, int infamy, int lucidity)
    {
        _playerState.TrustReputation = Math.Clamp(
            _playerState.TrustReputation + trust,
            GameConstants.REPUTATION_MIN,
            GameConstants.REPUTATION_MAX);

        _playerState.InfamyReputation = Math.Clamp(
            _playerState.InfamyReputation + infamy,
            GameConstants.REPUTATION_MIN,
            GameConstants.REPUTATION_MAX);

        _playerState.LucidityReputation = Math.Clamp(
            _playerState.LucidityReputation + lucidity,
            GameConstants.REPUTATION_MIN,
            GameConstants.REPUTATION_MAX);

        PlayerStateChanged?.Invoke(this, _playerState);
        
        await CheckAndProgressTierAsync();
        await SaveGameAsync();
    }

    public async Task CheckAndProgressTierAsync()
    {
        // Calculate tier based on total reputation
        var totalReputation = _playerState.TrustReputation + 
                            _playerState.InfamyReputation + 
                            _playerState.LucidityReputation;

        var newTier = totalReputation switch
        {
            < 50 => 1,
            < 120 => 2,
            < 200 => 3,
            < 300 => 4,
            _ => 5
        };

        if (newTier > _playerState.Tier)
        {
            _playerState.Tier = newTier;
            _playerState.PlayerName = GameConstants.TierNames[newTier];
            
            // Increase capacity on tier up
            _playerState.MaxWeight += 50;
            
            PlayerStateChanged?.Invoke(this, _playerState);
            await SaveGameAsync();
        }
    }
}
