using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllRecipesAsync();
        Task<List<Recipe>> GetUserRecipesAsync(int userId);
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Ingredient>> GetAllIngredientsAsync();
        Task CreateRecipeAsync(Recipe recipe, int userid);
        Task DeleteRecipeAsync(Recipe recipe);
        Task<List<string>> MatchingIngredients(string term);
    }
}
