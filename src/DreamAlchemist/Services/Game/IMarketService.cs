using DreamAlchemist.Models.DTOs;

namespace DreamAlchemist.Services.Game;

public interface IMarketService
{
    /// <summary>
    /// Get current market prices for a city
    /// </summary>
    Task<List<MarketPriceDto>> GetCurrentPricesAsync(string cityId);
    
    /// <summary>
    /// Get price for a specific ingredient in a city
    /// </summary>
    Task<MarketPriceDto?> GetIngredientPriceAsync(string cityId, string ingredientId);
    
    /// <summary>
    /// Purchase an ingredient
    /// </summary>
    Task<bool> BuyIngredientAsync(string ingredientId, int quantity);
    
    /// <summary>
    /// Sell an ingredient
    /// </summary>
    Task<bool> SellIngredientAsync(string ingredientId, int quantity);
    
    /// <summary>
    /// Sell a crafted dream
    /// </summary>
    Task<bool> SellCraftedDreamAsync(string dreamId);
    
    /// <summary>
    /// Update all market prices (called on time progression)
    /// </summary>
    Task UpdateMarketPricesAsync();
    
    /// <summary>
    /// Get price history for an ingredient
    /// </summary>
    Task<Dictionary<string, decimal>> GetPriceHistoryAsync(string ingredientId, int days);
    
    /// <summary>
    /// Get trending items in a city
    /// </summary>
    Task<List<MarketTrendDto>> GetMarketTrendsAsync(string cityId);
}

public class MarketTrendDto
{
    public string IngredientId { get; set; } = string.Empty;
    public string IngredientName { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public decimal ChangePercent { get; set; }
}
