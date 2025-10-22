using SQLite;
using Newtonsoft.Json;
using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Models.Entities;

[Table("Cities")]
public class City
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Visual theme color
    /// </summary>
    public string ThemeColor { get; set; } = "#8B5CF6";

    /// <summary>
    /// Ambient music track identifier
    /// </summary>
    public string MusicTrackId { get; set; } = string.Empty;

    /// <summary>
    /// Market modifiers for dream tags
    /// Key: DreamTag, Value: Multiplier
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
    /// Available event IDs for this city
    /// </summary>
    [Ignore]
    public List<string> EventPool { get; set; } = new();

    [Column("EventPoolJson")]
    public string EventPoolJson
    {
        get => JsonConvert.SerializeObject(EventPool);
        set => EventPool = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Travel cost from starting city
    /// </summary>
    public int TravelCost { get; set; }

    /// <summary>
    /// Days required to travel here
    /// </summary>
    public int TravelDays { get; set; }

    /// <summary>
    /// Minimum reputation required to access
    /// </summary>
    public int RequiredReputation { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
