namespace DreamAlchemist.Models.DTOs;

public class TravelResultDto
{
    public bool Success { get; set; }
    public string DestinationCityId { get; set; } = string.Empty;
    public string DestinationCityName { get; set; } = string.Empty;
    public int DaysPassed { get; set; }
    public int CoinsCost { get; set; }
    public List<string> EventsTriggered { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
