namespace DreamAlchemist.Helpers;

public static class GameConstants
{
    // Starting values
    public const int STARTING_COINS = 5000;
    public const int STARTING_WEIGHT_CAPACITY = 100;
    public const string STARTING_CITY = "somnia_terminal";

    // Tier thresholds
    public static readonly Dictionary<int, string> TierNames = new()
    {
        { 1, "Novice Peddler" },
        { 2, "Dream Artisan" },
        { 3, "Dream Broker" },
        { 4, "Dream Cartel Leader" },
        { 5, "Lucid Architect" }
    };

    // Reputation levels
    public const int REPUTATION_MAX = 100;
    public const int REPUTATION_MIN = -100;

    // Economy
    public const decimal MIN_PRICE_MULTIPLIER = 0.5m;
    public const decimal MAX_PRICE_MULTIPLIER = 3.0m;

    // Crafting
    public const int MIN_INGREDIENTS_PER_RECIPE = 2;
    public const int MAX_INGREDIENTS_PER_RECIPE = 3;

    // Events
    public const double BASE_EVENT_PROBABILITY = 0.15;
    public const int MAX_SIMULTANEOUS_EVENTS = 3;
}
