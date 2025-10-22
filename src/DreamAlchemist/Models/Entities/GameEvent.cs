using SQLite;
using Newtonsoft.Json;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Models.Supporting;

namespace DreamAlchemist.Models.Entities;

[Table("GameEvents")]
public class GameEvent
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public EventType Type { get; set; }

    /// <summary>
    /// Probability of this event occurring (0.0 to 1.0)
    /// </summary>
    public double Probability { get; set; }

    /// <summary>
    /// Duration in days
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Price modifiers by ingredient ID
    /// Key: IngredientId, Value: Multiplier
    /// </summary>
    [Ignore]
    public Dictionary<string, decimal> PriceModifiers { get; set; } = new();

    [Column("PriceModifiersJson")]
    public string PriceModifiersJson
    {
        get => JsonConvert.SerializeObject(PriceModifiers);
        set => PriceModifiers = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(value) ?? new();
    }

    /// <summary>
    /// Tag-based modifiers (affects all ingredients with tag)
    /// </summary>
    [Ignore]
    public Dictionary<DreamTag, decimal> TagModifiers { get; set; } = new();

    [Column("TagModifiersJson")]
    public string TagModifiersJson
    {
        get => JsonConvert.SerializeObject(TagModifiers);
        set
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    TagModifiers = new();
                    return;
                }

                // Try to deserialize as Dictionary<string, decimal> first (JSON uses string keys)
                var stringDict = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(value);
                if (stringDict != null)
                {
                    TagModifiers = new Dictionary<DreamTag, decimal>();
                    foreach (var kvp in stringDict)
                    {
                        if (int.TryParse(kvp.Key, out int tagValue))
                        {
                            TagModifiers[(DreamTag)tagValue] = kvp.Value;
                        }
                    }
                    return;
                }
            }
            catch
            {
                // If that fails, try deserializing directly as enum dictionary
                try
                {
                    TagModifiers = JsonConvert.DeserializeObject<Dictionary<DreamTag, decimal>>(value) ?? new();
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize TagModifiersJson: {value}");
                    TagModifiers = new();
                }
            }
        }
    }

    /// <summary>
    /// Reputation change when event triggers
    /// </summary>
    public int ReputationEffect { get; set; }

    /// <summary>
    /// Narrative text for event popup
    /// </summary>
    public string NarrativeText { get; set; } = string.Empty;

    /// <summary>
    /// Optional choices for player
    /// </summary>
    [Ignore]
    public List<EventChoice>? Choices { get; set; }

    [Column("ChoicesJson")]
    public string? ChoicesJson
    {
        get => Choices != null ? JsonConvert.SerializeObject(Choices) : null;
        set => Choices = !string.IsNullOrEmpty(value) 
            ? JsonConvert.DeserializeObject<List<EventChoice>>(value) 
            : null;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
