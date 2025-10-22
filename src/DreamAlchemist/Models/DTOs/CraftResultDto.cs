using DreamAlchemist.Models.Supporting;

namespace DreamAlchemist.Models.DTOs;

public class CraftResultDto
{
    public bool Success { get; set; }
    public string? RecipeId { get; set; }
    public string? RecipeName { get; set; }
    public bool NewDiscovery { get; set; }
    public CraftedDream? CraftedDream { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? NarrativeText { get; set; }
}
