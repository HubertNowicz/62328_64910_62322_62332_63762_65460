using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetAllRecipesAsync();
        Task<List<RecipeDto>> GetFilteredRecipes(List<RecipeDto> recipes, RecipeFilter filter);
        Task<List<RecipeDto>> GetUserRecipesAsync(int userId);
        Task<RecipeDto> GetRecipeByIdAsync(int id);
        Task<List<RecipeDto>> GetRecipesByCategoryAsync(string categoryName);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<List<IngredientDto>> GetAllIngredientsAsync();
        Task<IEnumerable<MeasurementUnitDto>> GetAllUnitsAsync();
        Task CreateRecipeAsync(RecipeCreateDto recipeDto);
        Task UpdateRecipeAsync(int recipeId, RecipeCreateDto recipeDto, int userId);
        Task DeleteRecipeAsync(int recipeId);
        Task<List<string>> MatchingIngredients(string term);
        Task<List<MeasurementUnit>> GetUnitsForIngredientAsync(string ingredientName);
    }
}
