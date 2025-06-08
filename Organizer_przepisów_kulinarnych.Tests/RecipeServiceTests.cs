using AutoMapper;
using Moq;
using Xunit;
using Organizer_przepisów_kulinarnych.BLL.Services;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Organizer_przepisów_kulinarnych.DAL.Entities;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _recipeRepoMock = new();
    private readonly Mock<IIngredientRepository> _ingredientRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly Mock<IMeasurementUnitRepository> _unitRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly RecipeService _recipeService;

    public RecipeServiceTests()
    {
        _recipeService = new RecipeService(
            _recipeRepoMock.Object,
            _ingredientRepoMock.Object,
            _categoryRepoMock.Object,
            _unitRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllRecipesAsync_ReturnsOkResultWithMappedRecipes()
    {
        // Arrange
        var recipes = new List<Recipe> { new Recipe { Id = 1, RecipeName = "Test" } };
        var mappedRecipes = new List<RecipeDto> { new RecipeDto { Id = 1, RecipeName = "Test" } };

        _recipeRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(recipes);
        _mapperMock.Setup(m => m.Map<List<RecipeDto>>(recipes)).Returns(mappedRecipes);

        // Act
        var result = await _recipeService.GetAllRecipesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(mappedRecipes, result.Data);
    }

    [Fact]
    public async Task GetAllRecipesAsync_ReturnsFailResultWhenExceptionThrown()
    {
        // Arrange
        _recipeRepoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new System.Exception("DB error"));

        // Act
        var result = await _recipeService.GetAllRecipesAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error fetching recipes", result.Error);
    }

    [Fact]
    public async Task GetRecipeByIdAsync_ReturnsOkResultWhenRecipeFound()
    {
        var recipe = new Recipe { Id = 1, RecipeName = "Recipe1" };
        var dto = new RecipeDto { Id = 1, RecipeName = "Recipe1" };

        _recipeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);
        _mapperMock.Setup(m => m.Map<RecipeDto>(recipe)).Returns(dto);

        var result = await _recipeService.GetRecipeByIdAsync(1);

        Assert.True(result.Success);
        Assert.Equal(dto, result.Data);
    }

    [Fact]
    public async Task GetRecipeByIdAsync_ReturnsFailResultWhenRecipeNotFound()
    {
        _recipeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Recipe)null);

        var result = await _recipeService.GetRecipeByIdAsync(1);

        Assert.False(result.Success);
        Assert.Equal("Recipe not found.", result.Error);
    }

    [Fact]
    public async Task DeleteRecipeAsync_ReturnsOkResultWhenRecipeDeleted()
    {
        var recipe = new Recipe { Id = 1 };

        _recipeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);
        _recipeRepoMock.Setup(r => r.DeleteAsync(recipe)).Returns(Task.CompletedTask);
        _recipeRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _recipeService.DeleteRecipeAsync(1);

        Assert.True(result.Success);
        Assert.Equal("Recipe deleted successfully.", result.Message);
    }

    [Fact]
    public async Task DeleteRecipeAsync_ReturnsFailWhenRecipeNotFound()
    {
        _recipeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Recipe)null);

        var result = await _recipeService.DeleteRecipeAsync(1);

        Assert.False(result.Success);
        Assert.Equal("Recipe not found.", result.Error);
    }

    [Fact]
    public async Task GetFilteredRecipes_ReturnsFilteredUnder30()
    {
        // Arrange
        var recipes = new List<RecipeDto>
    {
        new() { RecipeName = "Quick Meal", Preptime = 20 },
        new() { RecipeName = "Long Meal", Preptime = 60 }
    };

        var filter = new RecipeFilter
        {
            FilterUnder30 = true
        };

        // Act
        var result = await _recipeService.GetFilteredRecipes(recipes, filter);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data);
        Assert.Equal("Quick Meal", result.Data.First().RecipeName);
    }

    [Fact]
    public async Task GetFilteredRecipes_ReturnsSortedByPopularity()
    {
        // Arrange
        var recipes = new List<RecipeDto>
    {
        new() { RecipeName = "A", FavoriteCount = 10 },
        new() { RecipeName = "B", FavoriteCount = 100 }
    };

        var filter = new RecipeFilter
        {
            SortOption = Organizer_przepisów_kulinarnych.DAL.Entities.Enums.RecipeSortOption.Popularity
        };

        // Act
        var result = await _recipeService.GetFilteredRecipes(recipes, filter);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("B", result.Data.First().RecipeName);
    }

    [Fact]
    public async Task CreateRecipeAsync_SuccessfullyCreatesRecipe()
    {
        // Arrange
        var dto = new RecipeCreateDto
        {
            RecipeName = "Pizza",
            UserId = 1,
            RecipeIngredients = new List<RecipeIngredientDto>
        {
            new() { Name = "Cheese", UnitId = 1, Amount = 100 }
        }
        };

        var mappedRecipe = new Recipe
        {
            RecipeName = "Pizza",
            RecipeIngredients = new List<RecipeIngredient>()
        {
            new RecipeIngredient { Name = "Cheese", UnitId = 1, Amount = 100 }
        }
        };

        _mapperMock.Setup(m => m.Map<Recipe>(dto)).Returns(mappedRecipe);
        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ingredient>());
        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(new List<PendingIngredient>());
        _ingredientRepoMock.Setup(r => r.AddPendingAsync(It.IsAny<PendingIngredient>())).Returns(Task.CompletedTask);
        _recipeRepoMock.Setup(r => r.AddAsync(It.IsAny<Recipe>())).Returns(Task.CompletedTask);
        _recipeRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _recipeService.CreateRecipeAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Recipe created successfully.", result.Message);
    }

    [Fact]
    public async Task CreateRecipeAsync_ThrowsExceptionOnNullDto()
    {
        // Act
        var result = await _recipeService.CreateRecipeAsync(null);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error creating recipe", result.Error);
    }

    [Fact]
    public async Task UpdateRecipeAsync_ReturnsFailWhenRecipeNotFound()
    {
        _recipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Recipe)null);

        var result = await _recipeService.UpdateRecipeAsync(1, new RecipeCreateDto(), 1);

        Assert.False(result.Success);
        Assert.Equal("Recipe not found.", result.Error);
    }

    [Fact]
    public async Task UpdateRecipeAsync_SuccessWithNewIngredientSuggestion()
    {
        var existingRecipe = new Recipe
        {
            Id = 1,
            RecipeIngredients = new List<RecipeIngredient>(),
            InstructionSteps = new List<RecipeInstructionStep>()
        };

        var dto = new RecipeCreateDto
        {
            RecipeName = "Updated",
            Description = "Updated Desc",
            Preptime = 45,
            CategoryId = 2,
            RecipeIngredients = new List<RecipeIngredientDto>
        {
            new() { Name = "Avocado", Amount = 1, UnitId = 1 }
        },
            InstructionSteps = new List<RecipeInstructionStepDto>
        {
            new() { StepNumber = 1, Description = "Step 1" }
        }
        };

        _recipeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRecipe);
        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ingredient>());
        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(new List<PendingIngredient>());
        _ingredientRepoMock.Setup(r => r.AddPendingAsync(It.IsAny<PendingIngredient>())).Returns(Task.CompletedTask);
        _recipeRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _recipeService.UpdateRecipeAsync(1, dto, 5);

        Assert.True(result.Success);
        Assert.Equal("Recipe updated successfully.", result.Message);
    }
    
    [Fact]
    public async Task DeleteRecipeAsync_RecipeNotFound_ReturnsFail()
    {
        // Arrange
        _recipeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Recipe)null);

        // Act
        var result = await _recipeService.DeleteRecipeAsync(123);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Recipe not found.", result.Error);
    }

    [Fact]
    public async Task DeleteRecipeAsync_ValidRecipe_DeletesAndReturnsSuccess()
    {
        // Arrange
        var recipe = new Recipe { Id = 123 };
        _recipeRepoMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync(recipe);
        _recipeRepoMock.Setup(r => r.DeleteAsync(recipe)).Returns(Task.CompletedTask);
        _recipeRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _recipeService.DeleteRecipeAsync(123);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Recipe deleted successfully.", result.Message);
    }

    [Fact]
    public async Task MatchingIngredients_ReturnsMatchingNames()
    {
        // Arrange
        var ingredients = new List<IngredientDto>
    {
        new() { Name = "Tomato" },
        new() { Name = "Potato" },
        new() { Name = "Avocado" }
    };

        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ingredients.Select(i => new Ingredient { Name = i.Name }).ToList());
        _mapperMock.Setup(m => m.Map<List<IngredientDto>>(It.IsAny<IEnumerable<Ingredient>>())).Returns(ingredients);

        // Act
        var result = await _recipeService.MatchingIngredients("to");

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Tomato", result.Data);
        Assert.Contains("Potato", result.Data);
    }

    [Fact]
    public async Task MatchingIngredients_IngredientFetchFails_ReturnsFail()
    {
        // Arrange
        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _recipeService.MatchingIngredients("any");

        // Assert
        Assert.False(result.Success);
        Assert.StartsWith("Error fetching ingredients", result.Error);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsMappedCategoryDtos()
    {
        // Arrange
        var categories = new List<Category> { new() { Name = "Dessert" } };
        var categoryDtos = new List<CategoryDto> { new() { Name = "Dessert" } };

        _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mapperMock.Setup(m => m.Map<List<CategoryDto>>(categories)).Returns(categoryDtos);

        // Act
        var result = await _recipeService.GetAllCategoriesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data);
        Assert.Equal("Dessert", result.Data.First().Name);
    }

    [Fact]
    public async Task GetAllUnitsAsync_ReturnsMappedUnits()
    {
        // Arrange
        var units = new List<MeasurementUnit> { new() { Name = "Gram" } };
        var unitDtos = new List<MeasurementUnitDto> { new() { Name = "Gram" } };

        _unitRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(units);
        _mapperMock.Setup(m => m.Map<IEnumerable<MeasurementUnitDto>>(units)).Returns(unitDtos);

        // Act
        var result = await _recipeService.GetAllUnitsAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Gram", result.Data.First().Name);
    }

    [Fact]
    public async Task GetUnitsForIngredientAsync_ReturnsUnits()
    {
        // Arrange
        var units = new List<MeasurementUnit> { new() { Name = "ml" } };
        _ingredientRepoMock.Setup(r => r.GetUnitsByIngredientNameAsync("Milk")).ReturnsAsync(units);

        // Act
        var result = await _recipeService.GetUnitsForIngredientAsync("Milk");

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data);
        Assert.Equal("ml", result.Data.First().Name);
    }





}
