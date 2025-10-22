using DreamAlchemist.Models.DTOs;
using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.Services.Game;

public interface ITravelService
{
    /// <summary>
    /// Get all available cities
    /// </summary>
    Task<List<City>> GetAllCitiesAsync();
    
    /// <summary>
    /// Get unlocked cities for the player
    /// </summary>
    Task<List<City>> GetUnlockedCitiesAsync();
    
    /// <summary>
    /// Check if player can travel to a specific city
    /// </summary>
    Task<bool> CanTravelToAsync(string cityId);
    
    /// <summary>
    /// Travel to a specific city
    /// </summary>
    Task<TravelResultDto> TravelToAsync(string cityId);
    
    /// <summary>
    /// Get travel cost for a specific city
    /// </summary>
    Task<(int coins, int days)> GetTravelCostAsync(string cityId);
    
    /// <summary>
    /// Check if city is unlocked
    /// </summary>
    bool IsCityUnlocked(string cityId);
}
