using DreamAlchemist.Services.Game;
using DreamAlchemist.Services.Core;
using DreamAlchemist.Services.Data;
using DreamAlchemist.Models.Enums;
using DreamAlchemist.Tests.TestHelpers;

namespace DreamAlchemist.Tests.Unit.Services;

/// <summary>
/// Unit tests for CraftingService
/// Tests recipe matching, dream creation, and crafting mechanics
/// </summary>
public class CraftingServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IGameStateService> _mockGameStateService;
    private readonly CraftingService _sut;

    public CraftingServiceTests()
    {
        _mockDatabaseService = MockFactory.CreateMockDatabaseService();
        _mockGameStateService = MockFactory.CreateMockGameStateService();
        
        _sut = new CraftingService(
            _mockDatabaseService.Object,
            _mockGameStateService.Object);
    }

    #region CraftDreamAsync Tests

    [Fact]
    public async Task CraftDreamAsync_ExactRecipeMatch_Success()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" });
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CraftedDream.Should().NotBeNull();
        result.CraftedDream!.Name.Should().Be("Test Dream");
    }

    [Fact]
    public async Task CraftDreamAsync_InsufficientIngredients_Fails()
    {
        // Arrange
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 0; // Not enough

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.Success.Should().BeFalse();
        result.CraftedDream.Should().BeNull();
    }

    [Fact]
    public async Task CraftDreamAsync_EmptyIngredientList_Fails()
    {
        // Act
        var result = await _sut.CraftDreamAsync(new List<string>());

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("ingredients");
    }

    [Fact]
    public async Task CraftDreamAsync_TooManyIngredients_Fails()
    {
        // Arrange
        var ingredients = Enumerable.Range(1, 10).Select(i => $"ing{i}").ToList();

        // Act
        var result = await _sut.CraftDreamAsync(ingredients);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("maximum");
    }

    [Fact]
    public async Task CraftDreamAsync_RemovesIngredientsFromInventory()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" });
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");

        // Act
        await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        playerState.Inventory["ing1"].Should().Be(4);
        playerState.Inventory["ing2"].Should().Be(4);
    }

    [Fact]
    public async Task CraftDreamAsync_AddsCraftedDreamToInventory()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" });
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");
        var initialDreamsCount = playerState.CraftedDreams.Count;

        // Act
        await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        playerState.CraftedDreams.Should().HaveCount(initialDreamsCount + 1);
    }

    [Fact]
    public async Task CraftDreamAsync_SavesGameState()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" });
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");

        // Act
        await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        _mockGameStateService.Verify(x => x.SaveGameAsync(), Times.Once);
    }

    [Fact]
    public async Task CraftDreamAsync_NewRecipeDiscovery_SetsWasNewRecipeTrue()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" });
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        // NOT in discovered recipes

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.WasNewRecipe.Should().BeTrue();
        playerState.DiscoveredRecipes.Should().Contain("recipe1");
    }

    [Fact]
    public async Task CraftDreamAsync_KnownRecipe_SetsWasNewRecipeFalse()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" });
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.WasNewRecipe.Should().BeFalse();
    }

    [Theory]
    [InlineData(Rarity.Common, Rarity.Common)]
    [InlineData(Rarity.Uncommon, Rarity.Uncommon)]
    [InlineData(Rarity.Rare, Rarity.Rare)]
    public async Task CraftDreamAsync_UsesRecipeRarity_WhenMatched(Rarity recipeRarity, Rarity expectedRarity)
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test Dream", new[] { "ing1", "ing2" }, recipeRarity);
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe> { recipe });
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.CraftedDream!.Rarity.Should().Be(expectedRarity);
    }

    #endregion

    #region Experimental Crafting Tests

    [Fact]
    public async Task CraftDreamAsync_NoMatchingRecipe_CreatesExperimentalDream()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe>()); // No recipes
        
        var ing1 = TestDataBuilder.CreateTestIngredient("ing1", "Test1", Rarity.Common);
        var ing2 = TestDataBuilder.CreateTestIngredient("ing2", "Test2", Rarity.Uncommon);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ing1);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ing2);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.Success.Should().BeTrue();
        result.CraftedDream.Should().NotBeNull();
        result.WasNewRecipe.Should().BeFalse(); // No recipe discovered
    }

    [Fact]
    public async Task CraftDreamAsync_ExperimentalCrafting_CalculatesRarityFromIngredients()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe>());
        
        var ing1 = TestDataBuilder.CreateTestIngredient("ing1", "Test1", Rarity.Rare);
        var ing2 = TestDataBuilder.CreateTestIngredient("ing2", "Test2", Rarity.Epic);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ing1);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ing2);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.CraftedDream!.Rarity.Should().BeOneOf(Rarity.Rare, Rarity.Epic);
    }

    [Fact]
    public async Task CraftDreamAsync_ExperimentalCrafting_CalculatesValueFromIngredients()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(new List<Recipe>());
        
        var ing1 = TestDataBuilder.CreateTestIngredient("ing1", "Test1", Rarity.Common, 10m);
        var ing2 = TestDataBuilder.CreateTestIngredient("ing2", "Test2", Rarity.Common, 20m);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ing1);
        MockFactory.SetupIngredientRetrieval(_mockDatabaseService, ing2);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;

        // Act
        var result = await _sut.CraftDreamAsync(new List<string> { "ing1", "ing2" });

        // Assert
        result.CraftedDream!.Value.Should().BeGreaterThan(30m); // Combined value with some multiplier
    }

    #endregion

    #region GetKnownRecipesAsync Tests

    [Fact]
    public async Task GetKnownRecipesAsync_ReturnsOnlyDiscoveredRecipes()
    {
        // Arrange
        var recipes = new List<Recipe>
        {
            TestDataBuilder.CreateTestRecipe("recipe1", "Known Recipe", new[] { "ing1", "ing2" }),
            TestDataBuilder.CreateTestRecipe("recipe2", "Unknown Recipe", new[] { "ing3", "ing4" })
        };
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(recipes);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.DiscoveredRecipes.Add("recipe1");

        // Act
        var result = await _sut.GetKnownRecipesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be("recipe1");
    }

    [Fact]
    public async Task GetKnownRecipesAsync_NoDiscoveredRecipes_ReturnsEmpty()
    {
        // Arrange
        var recipes = TestDataBuilder.CreateTestRecipes();
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(recipes);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.DiscoveredRecipes.Clear();

        // Act
        var result = await _sut.GetKnownRecipesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region CanCraftRecipe Tests

    [Fact]
    public async Task CanCraftRecipe_HasAllIngredients_ReturnsTrue()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test", new[] { "ing1", "ing2" });
        MockFactory.SetupRecipeRetrieval(_mockDatabaseService, recipe);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 3;
        playerState.Inventory["ing2"] = 3;

        // Act
        var result = await _sut.CanCraftRecipeAsync("recipe1");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCraftRecipe_MissingIngredient_ReturnsFalse()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test", new[] { "ing1", "ing2" });
        MockFactory.SetupRecipeRetrieval(_mockDatabaseService, recipe);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 3;
        // ing2 not in inventory

        // Act
        var result = await _sut.CanCraftRecipeAsync("recipe1");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCraftRecipe_InsufficientQuantity_ReturnsFalse()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateTestRecipe("recipe1", "Test", new[] { "ing1", "ing2" });
        MockFactory.SetupRecipeRetrieval(_mockDatabaseService, recipe);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 0; // Has key but quantity is 0
        playerState.Inventory["ing2"] = 3;

        // Act
        var result = await _sut.CanCraftRecipeAsync("recipe1");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCraftRecipe_InvalidRecipeId_ReturnsFalse()
    {
        // Arrange
        _mockDatabaseService.Setup(x => x.GetRecipeAsync("invalid"))
            .ReturnsAsync((Recipe?)null);

        // Act
        var result = await _sut.CanCraftRecipeAsync("invalid");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetCraftableRecipesAsync Tests

    [Fact]
    public async Task GetCraftableRecipesAsync_ReturnsOnlyRecipesPlayerCanCraft()
    {
        // Arrange
        var recipes = new List<Recipe>
        {
            TestDataBuilder.CreateTestRecipe("recipe1", "Craftable", new[] { "ing1", "ing2" }),
            TestDataBuilder.CreateTestRecipe("recipe2", "Not Craftable", new[] { "ing3", "ing4" })
        };
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(recipes);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory["ing1"] = 5;
        playerState.Inventory["ing2"] = 5;
        playerState.DiscoveredRecipes.Add("recipe1");
        playerState.DiscoveredRecipes.Add("recipe2");

        // Act
        var result = await _sut.GetCraftableRecipesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be("recipe1");
    }

    [Fact]
    public async Task GetCraftableRecipesAsync_NoIngredients_ReturnsEmpty()
    {
        // Arrange
        var recipes = TestDataBuilder.CreateTestRecipes();
        _mockDatabaseService.Setup(x => x.GetRecipesAsync())
            .ReturnsAsync(recipes);
        
        var playerState = _mockGameStateService.Object.PlayerState;
        playerState.Inventory.Clear();
        playerState.DiscoveredRecipes.AddRange(recipes.Select(r => r.Id));

        // Act
        var result = await _sut.GetCraftableRecipesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
