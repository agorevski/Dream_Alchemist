namespace DreamAlchemist.Models.Supporting;

public class EventChoice
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int CoinsCost { get; set; }
    public int ReputationEffect { get; set; }
    public string? ResultText { get; set; }
    public Dictionary<string, int>? ItemRewards { get; set; }
}
