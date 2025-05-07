using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface  IFavortieRecipeService
    {
        Task ToggleFavoriteAsync(int userId, int recipeId);
        Task<List<Recipe>> GetFavoriteRecipesForUserAsync(int userId);
        Task<List<int>> GetFavoriteRecipesIdsForUserAsync(int userId);
    }
}
