using SQLite;
using Newtonsoft.Json;
using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Models.Entities;

[Table("Ingredients")]
public class Ingredient
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Rarity Rarity { get; set; }

    public decimal BaseValue { get; set; }

    /// <summary>
    /// Weight for inventory capacity calculation
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Whether this ingredient can trigger volatility events
    /// </summary>
    public bool Volatile { get; set; }

    /// <summary>
    /// Tags for market modifiers and recipe matching
    /// Stored as JSON string in SQLite
    /// </summary>
    [Ignore]
    public List<DreamTag> Tags { get; set; } = new();

    [Column("TagsJson")]
    public string TagsJson
    {
        get => JsonConvert.SerializeObject(Tags);
        set
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    Tags = new();
                    return;
                }

                // Try to deserialize as integer array first (most common case)
                var intArray = JsonConvert.DeserializeObject<List<int>>(value);
                if (intArray != null)
                {
                    Tags = intArray.Select(i => (DreamTag)i).ToList();
                    return;
                }
            }
            catch
            {
                // If that fails, try deserializing directly as enum array
                try
                {
                    Tags = JsonConvert.DeserializeObject<List<DreamTag>>(value) ?? new();
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize TagsJson: {value}");
                    Tags = new();
                }
            }
        }
    }

    /// <summary>
    /// Visual representation color (hex)
    /// </summary>
    public string Color { get; set; } = "#8B5CF6";

    /// <summary>
    /// Icon identifier for UI
    /// </summary>
    public string IconId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
