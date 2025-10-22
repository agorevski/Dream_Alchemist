namespace DreamAlchemist.Models.DTOs;

public class ReputationDto
{
    public int Trust { get; set; }
    public int Infamy { get; set; }
    public int Lucidity { get; set; }
    public string TrustLevel { get; set; } = string.Empty;
    public string InfamyLevel { get; set; } = string.Empty;
    public string LucidityLevel { get; set; } = string.Empty;
    public List<string> UnlockedFeatures { get; set; } = new();
}
