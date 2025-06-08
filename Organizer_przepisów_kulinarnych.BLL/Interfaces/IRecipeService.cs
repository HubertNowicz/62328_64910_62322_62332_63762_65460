using Organizer_przepisów_kulinarnych.BLL.Common;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;


namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IRecipeService
    {
        Task<Result<List<RecipeDto>>> GetAllRecipesAsync();
        Task<Result<List<RecipeDto>>> GetFilteredRecipes(List<RecipeDto> recipes, RecipeFilter filter);
        Task<Result<List<RecipeDto>>> GetUserRecipesAsync(int userId);
        Task<Result<RecipeDto>> GetRecipeByIdAsync(int id);
        Task<Result<List<RecipeDto>>> GetRecipesByCategoryAsync(string categoryName);
        Task<Result<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<Result<List<IngredientDto>>> GetAllIngredientsAsync();
        Task<Result<IEnumerable<MeasurementUnitDto>>> GetAllUnitsAsync();
        Task<Result> CreateRecipeAsync(RecipeCreateDto recipeDto);
        Task<Result> UpdateRecipeAsync(int recipeId, RecipeCreateDto recipeDto, int userId);
        Task<Result> DeleteRecipeAsync(int recipeId);
        Task<Result<List<string>>> MatchingIngredients(string term);
        Task<Result<List<MeasurementUnit>>> GetUnitsForIngredientAsync(string ingredientName);
    }
}
