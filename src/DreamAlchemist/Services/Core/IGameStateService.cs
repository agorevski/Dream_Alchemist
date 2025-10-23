using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.Services.Core;

public interface IGameStateService
{
    PlayerState PlayerState { get; }
    City CurrentCity { get; }
    int CurrentDay { get; }
    bool IsInitialized { get; }
    
    event EventHandler<PlayerState>? PlayerStateChanged;
    event EventHandler<City>? CityChanged;
    event EventHandler<int>? DayProgressed;
    
    Task InitializeAsync(bool forceNewGame = false);
    Task LoadGameAsync();
    Task SaveGameAsync();
    Task ProgressTimeAsync(int days);
    Task TravelToCityAsync(string cityId);
    Task UpdateReputationAsync(int trust, int infamy, int lucidity);
    Task CheckAndProgressTierAsync();
}
