using DreamAlchemist.Models.Entities;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Models.Supporting;
using DreamAlchemist.Helpers;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;

namespace DreamAlchemist.Services.Game;

public class EventService : IEventService
{
    private readonly IDatabaseService _databaseService;
    private readonly IGameStateService _gameStateService;
    private readonly Random _random = new();

    public EventService(
        IDatabaseService databaseService,
        IGameStateService gameStateService)
    {
        _databaseService = databaseService;
        _gameStateService = gameStateService;
        
        // Subscribe to day progression
        _gameStateService.DayProgressed += OnDayProgressed;
    }

    private async void OnDayProgressed(object? sender, int daysPassed)
    {
        await UpdateActiveEventsAsync(daysPassed);
    }

    public async Task<List<GameEvent>> GetActiveEventsAsync()
    {
        var playerState = _gameStateService.PlayerState;
        var activeEventIds = playerState.ActiveEvents.Select(ae => ae.EventId).ToList();
        
        var allEvents = await _databaseService.GetEventsAsync();
        return allEvents.Where(e => activeEventIds.Contains(e.Id)).ToList();
    }

    public async Task<GameEvent?> TriggerRandomEventAsync()
    {
        var playerState = _gameStateService.PlayerState;
        var currentCity = _gameStateService.CurrentCity;
        
        // Check if we can add more events
        if (playerState.ActiveEvents.Count >= GameConstants.MAX_SIMULTANEOUS_EVENTS)
            return null;

        // Get events available for current city
        var availableEvents = await _databaseService.GetEventsAsync();
        var cityEvents = availableEvents
            .Where(e => currentCity.EventPool.Contains(e.Id))
            .Where(e => !playerState.ActiveEvents.Any(ae => ae.EventId == e.Id))
            .ToList();

        if (cityEvents.Count == 0)
            return null;

        // Roll for event based on probability
        foreach (var evt in cityEvents)
        {
            if (_random.NextDouble() < evt.Probability)
            {
                await TriggerEventAsync(evt.Id);
                return evt;
            }
        }

        return null;
    }

    public async Task<bool> TriggerEventAsync(string eventId)
    {
        var evt = await _databaseService.GetEventAsync(eventId);
        if (evt == null)
            return false;

        var playerState = _gameStateService.PlayerState;
        var currentCity = _gameStateService.CurrentCity;

        // Check if already active
        if (playerState.ActiveEvents.Any(ae => ae.EventId == eventId))
            return false;

        // Add to active events
        var activeEvent = new ActiveEvent
        {
            EventId = eventId,
            CityId = currentCity.Id,
            DaysRemaining = evt.Duration,
            StartedAt = DateTime.UtcNow
        };

        playerState.ActiveEvents.Add(activeEvent);

        // Apply reputation effect if any
        if (evt.ReputationEffect != 0)
        {
            await _gameStateService.UpdateReputationAsync(evt.ReputationEffect, 0, 0);
        }

        await _gameStateService.SaveGameAsync();
        
        System.Diagnostics.Debug.WriteLine($"Event triggered: {evt.Name}");
        
        return true;
    }

    public async Task<string> ProcessEventChoiceAsync(string eventId, string choiceId)
    {
        var evt = await _databaseService.GetEventAsync(eventId);
        if (evt == null || evt.Choices == null)
            return "Event not found";

        var choice = evt.Choices.FirstOrDefault(c => c.Id == choiceId);
        if (choice == null)
            return "Choice not found";

        var playerState = _gameStateService.PlayerState;

        // Apply costs
        if (choice.CoinsCost > 0)
        {
            if (playerState.Coins < choice.CoinsCost)
                return "Not enough coins";
            
            playerState.Coins -= choice.CoinsCost;
        }

        // Apply reputation effects
        if (choice.ReputationEffect != 0)
        {
            await _gameStateService.UpdateReputationAsync(choice.ReputationEffect, 0, 0);
        }

        // Give item rewards
        if (choice.ItemRewards != null)
        {
            foreach (var reward in choice.ItemRewards)
            {
                var ingredient = await _databaseService.GetIngredientAsync(reward.Key);
                if (ingredient != null)
                {
                    playerState.AddToInventory(ingredient, reward.Value);
                }
            }
        }

        // Remove event from active events
        var activeEvent = playerState.ActiveEvents.FirstOrDefault(ae => ae.EventId == eventId);
        if (activeEvent != null)
        {
            playerState.ActiveEvents.Remove(activeEvent);
        }

        await _gameStateService.SaveGameAsync();

        return choice.ResultText ?? "Choice processed";
    }

    public async Task UpdateActiveEventsAsync(int daysPassed)
    {
        var playerState = _gameStateService.PlayerState;
        var expiredEvents = new List<ActiveEvent>();

        foreach (var activeEvent in playerState.ActiveEvents)
        {
            activeEvent.DaysRemaining -= daysPassed;
            
            if (activeEvent.DaysRemaining <= 0)
            {
                expiredEvents.Add(activeEvent);
            }
        }

        // Remove expired events
        foreach (var expiredEvent in expiredEvents)
        {
            playerState.ActiveEvents.Remove(expiredEvent);
            System.Diagnostics.Debug.WriteLine($"Event expired: {expiredEvent.EventId}");
        }

        if (expiredEvents.Count > 0)
        {
            await _gameStateService.SaveGameAsync();
        }

        // Chance to trigger a new random event
        if (_random.NextDouble() < GameConstants.BASE_EVENT_PROBABILITY)
        {
            await TriggerRandomEventAsync();
        }
    }

    public decimal GetEventMultiplier(string ingredientId, string cityId)
    {
        var playerState = _gameStateService.PlayerState;
        decimal multiplier = 1.0m;

        foreach (var activeEvent in playerState.ActiveEvents)
        {
            if (activeEvent.CityId != cityId || activeEvent.DaysRemaining <= 0)
                continue;

            // Get the event details
            var evt = _databaseService.GetEventAsync(activeEvent.EventId).Result;
            if (evt == null)
                continue;

            // Check for ingredient-specific modifiers
            if (evt.PriceModifiers.TryGetValue(ingredientId, out var modifier))
            {
                multiplier *= modifier;
            }
        }

        return multiplier;
    }

    public decimal GetTagMultiplier(string cityId, DreamTag tag)
    {
        var playerState = _gameStateService.PlayerState;
        decimal multiplier = 1.0m;

        foreach (var activeEvent in playerState.ActiveEvents)
        {
            if (activeEvent.CityId != cityId || activeEvent.DaysRemaining <= 0)
                continue;

            // Get the event details
            var evt = _databaseService.GetEventAsync(activeEvent.EventId).Result;
            if (evt == null)
                continue;

            // Check for tag-specific modifiers
            if (evt.TagModifiers.TryGetValue(tag, out var modifier))
            {
                multiplier *= modifier;
            }
        }

        return multiplier;
    }
}
