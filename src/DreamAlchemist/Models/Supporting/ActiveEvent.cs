namespace DreamAlchemist.Models.Supporting;

public class ActiveEvent
{
    public string EventId { get; set; } = string.Empty;
    public string CityId { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
}
