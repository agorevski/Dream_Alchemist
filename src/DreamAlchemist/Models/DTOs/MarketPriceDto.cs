using DreamAlchemist.Models.Enums;

namespace DreamAlchemist.Models.DTOs;

public class MarketPriceDto
{
    public string IngredientId { get; set; } = string.Empty;
    public string IngredientName { get; set; } = string.Empty;
    public Rarity Rarity { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal BasePrice { get; set; }
    public decimal PriceChangePercent { get; set; }
    public bool IsTrending { get; set; }
    public int AvailableQuantity { get; set; }
    public List<DreamTag> Tags { get; set; } = new();
    public string Color { get; set; } = string.Empty;
    public string IconId { get; set; } = string.Empty;
}
