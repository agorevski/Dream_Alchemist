using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Models.Entities;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;

namespace DreamAlchemist.Services.Game;

public class TravelService : ITravelService
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;
    private readonly IEventService _eventService;

    public TravelService(
        IDatabaseService databaseService,
        IGameStateService gameStateService,
        IEventService eventService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
        _eventService = eventService;
    }

    public async Task<List<City>> GetAllCitiesAsync()
    {
        return await _databaseService.GetCitiesAsync();
    }

    public async Task<List<City>> GetUnlockedCitiesAsync()
    {
        var allCities = await _databaseService.GetCitiesAsync();
        var playerState = _gameStateService.PlayerState;
        
        return allCities
            .Where(c => playerState.UnlockedCities.Contains(c.Id))
            .ToList();
    }

    public async Task<bool> CanTravelToAsync(string cityId)
    {
        var city = await _databaseService.GetCityAsync(cityId);
        if (city == null)
            return false;

        var playerState = _gameStateService.PlayerState;
        
        // Check if already in this city
        if (playerState.CurrentCityId == cityId)
            return false;

        // Check reputation requirement (for both locked and unlocked cities)
        var totalReputation = playerState.TrustReputation + 
                            playerState.InfamyReputation + 
                            playerState.LucidityReputation;
        
        if (totalReputation < city.RequiredReputation)
            return false;

        // Check if player can afford travel
        if (playerState.Coins < city.TravelCost)
            return false;

        return true;
    }

    public async Task<TravelResultDto> TravelToAsync(string cityId)
    {
        var city = await _databaseService.GetCityAsync(cityId);
        if (city == null)
        {
            return new TravelResultDto
            {
                Success = false,
                Message = "City not found"
            };
        }

        if (!await CanTravelToAsync(cityId))
        {
            return new TravelResultDto
            {
                Success = false,
                Message = "Cannot travel to this city"
            };
        }

        var playerState = _gameStateService.PlayerState;
        var wasLocked = !playerState.UnlockedCities.Contains(cityId);

        // Deduct travel cost
        playerState.Coins -= city.TravelCost;

        // Progress time
        await _gameStateService.ProgressTimeAsync(city.TravelDays);

        // Change city
        await _gameStateService.TravelToCityAsync(cityId);

        // Unlock city if needed
        if (wasLocked)
        {
            playerState.UnlockedCities.Add(cityId);
            await _gameStateService.SaveGameAsync();
        }

        // Check for events that may have triggered during travel
        var eventsTriggered = new List<string>();
        var activeEvents = await _eventService.GetActiveEventsAsync();
        
        foreach (var evt in activeEvents)
        {
            eventsTriggered.Add(evt.Name);
        }

        return new TravelResultDto
        {
            Success = true,
            DestinationCityId = cityId,
            DestinationCityName = city.Name,
            DaysPassed = city.TravelDays,
            CoinsCost = city.TravelCost,
            EventsTriggered = eventsTriggered,
            Message = wasLocked ? 
                $"Discovered and traveled to {city.Name}!" : 
                $"Traveled to {city.Name}"
        };
    }

    public async Task<(int coins, int days)> GetTravelCostAsync(string cityId)
    {
        var city = await _databaseService.GetCityAsync(cityId);
        if (city == null)
            return (0, 0);

        return (city.TravelCost, city.TravelDays);
    }

    public bool IsCityUnlocked(string cityId)
    {
        var playerState = _gameStateService.PlayerState;
        return playerState.UnlockedCities.Contains(cityId);
    }
}
