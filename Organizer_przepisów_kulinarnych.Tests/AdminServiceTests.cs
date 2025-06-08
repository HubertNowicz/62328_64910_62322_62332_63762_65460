using AutoMapper;
using Moq;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Services;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

public class AdminServiceTests
{
    private readonly Mock<IIngredientRepository> _ingredientRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
        _ingredientRepoMock = new Mock<IIngredientRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new AdminService(_ingredientRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllPendingIngredientsAsync_ReturnsMappedDtos()
    {
        var pendingList = new List<PendingIngredient> { new() { Id = 1 } };
        var dtoList = new List<PendingIngredientDto> { new() { Id = 1 } };

        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(pendingList);
        _mapperMock.Setup(m => m.Map<List<PendingIngredientDto>>(pendingList)).Returns(dtoList);

        var result = await _service.GetAllPendingIngredientsAsync();

        Assert.True(result.Success);
        Assert.Equal(dtoList, result.Data);
    }

    [Fact]
    public async Task ApprovePendingIngredientAsync_NotFound_ReturnsFail()
    {
        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(new List<PendingIngredient>());

        var result = await _service.ApprovePendingIngredientAsync(99);

        Assert.False(result.Success);
        Assert.Equal("Suggested ingredient not found.", result.Error);
    }

    [Fact]
    public async Task ApprovePendingIngredientAsync_NewIngredient_AddsIngredientAndUnit()
    {
        var suggestion = new PendingIngredient { Id = 1, Name = "paprika", MeasurementUnitId = 5 };
        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(new List<PendingIngredient> { suggestion });
        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ingredient>());
        _ingredientRepoMock.Setup(r => r.GetAllRecipeIngredientsAsync()).ReturnsAsync(new List<RecipeIngredient>());

        _ingredientRepoMock.Setup(r => r.AddIngredientAsync(It.IsAny<Ingredient>()))
            .Callback<Ingredient>(i => i.Id = 42)
            .Returns(Task.CompletedTask);

        var result = await _service.ApprovePendingIngredientAsync(1);

        _ingredientRepoMock.Verify(r => r.AddIngredientAsync(It.IsAny<Ingredient>()), Times.Once);
        _ingredientRepoMock.Verify(r => r.AddIngredientUnitAsync(It.Is<IngredientUnit>(iu => iu.IngredientId == 42 && iu.UnitId == 5)), Times.Once);
        _ingredientRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RejectPendingIngredientAsync_NotFound_ReturnsFail()
    {
        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(new List<PendingIngredient>());

        var result = await _service.RejectPendingIngredientAsync(5);

        Assert.False(result.Success);
        Assert.Equal("Suggestion not found.", result.Error);
    }

    [Fact]
    public async Task RejectPendingIngredientAsync_RemovesSuggestions()
    {
        var suggestion = new PendingIngredient { Id = 1, Name = "oregano" };
        var suggestions = new List<PendingIngredient> { suggestion };
        _ingredientRepoMock.Setup(r => r.GetAllPendingAsync()).ReturnsAsync(suggestions);

        var result = await _service.RejectPendingIngredientAsync(1);

        _ingredientRepoMock.Verify(r => r.RemovePendingRangeAsync(It.IsAny<IEnumerable<PendingIngredient>>()), Times.Once);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task AddIngredientAsync_IngredientExists_AddsNewUnits()
    {
        var ingredient = new Ingredient
        {
            Id = 1,
            Name = "Salt",
            IngredientUnits = new List<IngredientUnit>()
        };

        var unit = new MeasurementUnit { Id = 10 };

        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ingredient> { ingredient });
        _ingredientRepoMock.Setup(r => r.GetUnitsByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<MeasurementUnit> { unit });

        var result = await _service.AddIngredientAsync("Salt", new List<int> { 10 });

        _ingredientRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task AddIngredientAsync_NewIngredient_AddsIngredientAndUnits()
    {
        _ingredientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ingredient>());
        _ingredientRepoMock.Setup(r => r.GetUnitsByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<MeasurementUnit> { new() { Id = 1 } });

        var result = await _service.AddIngredientAsync("Garlic", new List<int> { 1 });

        _ingredientRepoMock.Verify(r => r.AddIngredientAsync(It.Is<Ingredient>(i => i.Name == "Garlic")), Times.Once);
        _ingredientRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task DeleteIngredientAsync_NotFound_ReturnsFail()
    {
        _ingredientRepoMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((Ingredient)null);

        var result = await _service.DeleteIngredientAsync(100);

        Assert.False(result.Success);
        Assert.Equal("Ingredient not found.", result.Error);
    }

    [Fact]
    public async Task DeleteIngredientAsync_UsedInRecipes_ReturnsFail()
    {
        _ingredientRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Ingredient());
        _ingredientRepoMock.Setup(r => r.IsIngredientUsedInRecipesAsync(1)).ReturnsAsync(true);
        _ingredientRepoMock.Setup(r => r.GetRecipeNamesUsingIngredientAsync(1))
            .ReturnsAsync(new List<string> { "Pizza", "Pasta" });

        var result = await _service.DeleteIngredientAsync(1);

        Assert.False(result.Success);
        Assert.Contains("Pizza", result.Error);
    }

    [Fact]
    public async Task DeleteIngredientAsync_Unused_DeletesSuccessfully()
    {
        var ingredient = new Ingredient();
        _ingredientRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ingredient);
        _ingredientRepoMock.Setup(r => r.IsIngredientUsedInRecipesAsync(1)).ReturnsAsync(false);

        var result = await _service.DeleteIngredientAsync(1);

        _ingredientRepoMock.Verify(r => r.RemoveIngredientAsync(ingredient), Times.Once);
        _ingredientRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result.Success);
    }
}
