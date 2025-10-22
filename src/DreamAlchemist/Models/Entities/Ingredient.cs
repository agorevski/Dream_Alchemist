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
        set => Tags = JsonConvert.DeserializeObject<List<DreamTag>>(value) ?? new();
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
