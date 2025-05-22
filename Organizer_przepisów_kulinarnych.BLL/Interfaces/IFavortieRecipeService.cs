using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface  IFavortieRecipeService
    {
        Task ToggleFavoriteAsync(int userId, int recipeId);
        Task<List<RecipeDto>> GetFavoriteRecipesForUserAsync(int userId);
        Task<List<int>> GetFavoriteRecipesIdsForUserAsync(int userId);
    }
}
