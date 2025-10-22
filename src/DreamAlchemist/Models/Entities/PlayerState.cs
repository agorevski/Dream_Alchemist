using SQLite;
using Newtonsoft.Json;
using DreamAlchemist.Models.Supporting;

namespace DreamAlchemist.Models.Entities;

[Table("PlayerState")]
public class PlayerState
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string PlayerName { get; set; } = "Novice Peddler";

    public int Coins { get; set; } = 5000;

    public int CurrentDay { get; set; } = 1;

    public string CurrentCityId { get; set; } = "somnia_terminal";

    /// <summary>
    /// Player progression tier (1-5)
    /// </summary>
    public int Tier { get; set; } = 1;

    /// <summary>
    /// Reputation scores
    /// </summary>
    public int TrustReputation { get; set; }
    public int InfamyReputation { get; set; }
    public int LucidityReputation { get; set; }

    /// <summary>
    /// Total inventory weight
    /// </summary>
    public int CurrentWeight { get; set; }

    /// <summary>
    /// Maximum inventory capacity
    /// </summary>
    public int MaxWeight { get; set; } = 100;

    /// <summary>
    /// Inventory items: Key = IngredientId, Value = Quantity
    /// </summary>
    [Ignore]
    public Dictionary<string, int> Inventory { get; set; } = new();

    [Column("InventoryJson")]
    public string InventoryJson
    {
        get => JsonConvert.SerializeObject(Inventory);
        set => Inventory = JsonConvert.DeserializeObject<Dictionary<string, int>>(value) ?? new();
    }

    /// <summary>
    /// Crafted dreams in storage
    /// </summary>
    [Ignore]
    public List<CraftedDream> CraftedDreams { get; set; } = new();

    [Column("CraftedDreamsJson")]
    public string CraftedDreamsJson
    {
        get => JsonConvert.SerializeObject(CraftedDreams);
        set => CraftedDreams = JsonConvert.DeserializeObject<List<CraftedDream>>(value) ?? new();
    }

    /// <summary>
    /// Discovered recipe IDs
    /// </summary>
    [Ignore]
    public List<string> DiscoveredRecipes { get; set; } = new();

    [Column("DiscoveredRecipesJson")]
    public string DiscoveredRecipesJson
    {
        get => JsonConvert.SerializeObject(DiscoveredRecipes);
        set => DiscoveredRecipes = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Unlocked cities
    /// </summary>
    [Ignore]
    public List<string> UnlockedCities { get; set; } = new() { "somnia_terminal" };

    [Column("UnlockedCitiesJson")]
    public string UnlockedCitiesJson
    {
        get => JsonConvert.SerializeObject(UnlockedCities);
        set => UnlockedCities = JsonConvert.DeserializeObject<List<string>>(value) ?? new();
    }

    /// <summary>
    /// Active event instances
    /// </summary>
    [Ignore]
    public List<ActiveEvent> ActiveEvents { get; set; } = new();

    [Column("ActiveEventsJson")]
    public string ActiveEventsJson
    {
        get => JsonConvert.SerializeObject(ActiveEvents);
        set => ActiveEvents = JsonConvert.DeserializeObject<List<ActiveEvent>>(value) ?? new();
    }

    public DateTime LastSaved { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Helper methods
    public bool CanAddToInventory(Ingredient ingredient, int quantity)
    {
        var totalWeight = CurrentWeight + (ingredient.Weight * quantity);
        return totalWeight <= MaxWeight;
    }

    public void AddToInventory(Ingredient ingredient, int quantity)
    {
        if (Inventory.ContainsKey(ingredient.Id))
        {
            Inventory[ingredient.Id] += quantity;
        }
        else
        {
            Inventory[ingredient.Id] = quantity;
        }
        CurrentWeight += ingredient.Weight * quantity;
    }

    public bool RemoveFromInventory(string ingredientId, int quantity)
    {
        if (!Inventory.ContainsKey(ingredientId) || Inventory[ingredientId] < quantity)
            return false;

        Inventory[ingredientId] -= quantity;
        if (Inventory[ingredientId] == 0)
        {
            Inventory.Remove(ingredientId);
        }
        
        return true;
    }
}
