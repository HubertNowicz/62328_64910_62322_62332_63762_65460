using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllRecipesAsync();
        List<Recipe> GetFilteredRecipes(
            List<Recipe> recipes,
            bool filterUnder30,
            bool filterBetween30And60,
            bool filterOver60,
            SortOption sortOption);
        Task<List<Recipe>> GetUserRecipesAsync(int userId);
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<List<Recipe>> GetRecipesByCategoryAsync(string categoryName);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Ingredient>> GetAllIngredientsAsync();
        Task<IEnumerable<MeasurementUnit>> GetAllUnitsAsync();
        Task CreateRecipeAsync(Recipe recipe, int userid);
        Task DeleteRecipeAsync(Recipe recipe);
        Task<List<string>> MatchingIngredients(string term);
        Task<List<MeasurementUnit>> GetUnitsForIngredientAsync(string ingredientName);
    }
}
