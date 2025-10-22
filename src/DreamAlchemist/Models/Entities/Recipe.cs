using SQLite;
using Newtonsoft.Json;
using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Models.Entities;

[Table("Recipes")]
public class Recipe
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Rarity Rarity { get; set; }

    /// <summary>
    /// Required ingredient IDs (2-3 ingredients)
    /// Stored as JSON array
    /// </summary>
    [Ignore]
    public List<string> RequiredIngredients { get; set; } = new();

    [Column("RequiredIngredientsJson")]
    public string RequiredIngredientsJson
    {
        get => JsonConvert.SerializeObject(RequiredIngredients);
        set => RequiredIngredients = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Dream alignment (e.g., melancholic, joyful, fearful)
    /// </summary>
    public string Alignment { get; set; } = string.Empty;

    /// <summary>
    /// Multiplier applied to base ingredient values
    /// </summary>
    public decimal ValueMultiplier { get; set; }

    /// <summary>
    /// Whether player has discovered this recipe
    /// </summary>
    public bool Discovered { get; set; }

    /// <summary>
    /// Tags inherited from ingredients plus unique tags
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
    /// Special effect when consumed (optional)
    /// </summary>
    public string? EffectId { get; set; }

    /// <summary>
    /// AI-generated narrative description (optional)
    /// </summary>
    public string? NarrativeText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
