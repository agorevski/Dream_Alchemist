using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Models.Supporting;

public class CraftedDream
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RecipeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public Rarity Rarity { get; set; }
    public string NarrativeText { get; set; } = string.Empty;
    public DateTime CraftedAt { get; set; } = DateTime.UtcNow;
}
