using AutoMapper;
using Moq;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Services;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

public class FavoriteRecipeServiceTests
{
    private readonly Mock<IFavoriteRecipeRepository> _favRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly FavoriteRecipeService _service;

    public FavoriteRecipeServiceTests()
    {
        _favRepoMock = new Mock<IFavoriteRecipeRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new FavoriteRecipeService(_favRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_InvalidIds_ReturnsFail()
    {
        var result = await _service.ToggleFavoriteAsync(0, -1);
        Assert.False(result.Success);
        Assert.Equal("Invalid user or recipe ID.", result.Error);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_ExistingFavorite_RemovesFavorite()
    {
        var existing = new FavoriteRecipe { UserId = 1, RecipeId = 2 };
        _favRepoMock.Setup(r => r.GetByUserAndRecipeAsync(1, 2)).ReturnsAsync(existing);

        var result = await _service.ToggleFavoriteAsync(1, 2);

        _favRepoMock.Verify(r => r.Remove(existing), Times.Once);
        _favRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_NotExisting_AddsFavorite()
    {
        _favRepoMock.Setup(r => r.GetByUserAndRecipeAsync(1, 2)).ReturnsAsync((FavoriteRecipe)null);

        var result = await _service.ToggleFavoriteAsync(1, 2);

        _favRepoMock.Verify(r => r.AddAsync(It.Is<FavoriteRecipe>(f => f.UserId == 1 && f.RecipeId == 2)), Times.Once);
        _favRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetFavoriteRecipesIdsForUserAsync_ReturnsIds()
    {
        var ids = new List<int> { 10, 20 };
        _favRepoMock.Setup(r => r.GetRecipeIdsByUserAsync(1)).ReturnsAsync(ids);

        var result = await _service.GetFavoriteRecipesIdsForUserAsync(1);

        Assert.True(result.Success);
        Assert.Equal(ids, result.Data);
    }

    [Fact]
    public async Task GetFavoriteRecipesIdsForUserAsync_Throws_ReturnsFail()
    {
        _favRepoMock.Setup(r => r.GetRecipeIdsByUserAsync(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetFavoriteRecipesIdsForUserAsync(1);

        Assert.False(result.Success);
        Assert.Contains("Failed to retrieve favorite recipe IDs", result.Error);
    }

    [Fact]
    public async Task GetFavoriteRecipesForUserAsync_InvalidId_ReturnsFail()
    {
        var result = await _service.GetFavoriteRecipesForUserAsync(-10);
        Assert.False(result.Success);
        Assert.Equal("Invalid user ID.", result.Error);
    }

    [Fact]
    public async Task GetFavoriteRecipesForUserAsync_ValidUser_ReturnsFavoriteDtos()
    {
        var recipes = new List<Recipe> { new Recipe { Id = 1 }, new Recipe { Id = 2 } };
        var dtos = new List<RecipeDto>
    {
        new() { Id = 1, IsFavorite = false },
        new() { Id = 2, IsFavorite = false }
    };

        _favRepoMock.Setup(r => r.GetFavoriteRecipesByUserAsync(1)).ReturnsAsync(recipes);
        _mapperMock.Setup(m => m.Map<List<RecipeDto>>(recipes)).Returns(dtos);

        var result = await _service.GetFavoriteRecipesForUserAsync(1);

        Assert.True(result.Success);
        Assert.All(result.Data, dto => Assert.True(dto.IsFavorite));
    }

    [Fact]
    public async Task GetFavoriteRecipesForUserAsync_Throws_ReturnsFail()
    {
        _favRepoMock.Setup(r => r.GetFavoriteRecipesByUserAsync(1)).ThrowsAsync(new Exception());

        var result = await _service.GetFavoriteRecipesForUserAsync(1);

        Assert.False(result.Success);
        Assert.Equal("Failed to retrieve favorite recipes.", result.Error);
    }






}
