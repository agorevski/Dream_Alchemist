using DreamAlchemist.Models.Entities;

namespace DreamAlchemist.Services.Game;

public interface IEventService
{
    /// <summary>
    /// Get all active events for the current player
    /// </summary>
    Task<List<GameEvent>> GetActiveEventsAsync();
    
    /// <summary>
    /// Trigger a random event
    /// </summary>
    Task<GameEvent?> TriggerRandomEventAsync();
    
    /// <summary>
    /// Trigger a specific event
    /// </summary>
    Task<bool> TriggerEventAsync(string eventId);
    
    /// <summary>
    /// Process an event choice made by the player
    /// </summary>
    Task<string> ProcessEventChoiceAsync(string eventId, string choiceId);
    
    /// <summary>
    /// Update active events (called on time progression)
    /// </summary>
    Task UpdateActiveEventsAsync(int daysPassed);
    
    /// <summary>
    /// Get event multiplier for an ingredient in a city
    /// </summary>
    decimal GetEventMultiplier(string ingredientId, string cityId);
    
    /// <summary>
    /// Get tag-based event multiplier
    /// </summary>
    decimal GetTagMultiplier(string cityId, Models.Enums.DreamTag tag);
}
