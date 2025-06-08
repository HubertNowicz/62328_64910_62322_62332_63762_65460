using Organizer_przepisów_kulinarnych.BLL.Common;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface  IFavoriteRecipeService
    {
        Task<Result> ToggleFavoriteAsync(int userId, int recipeId);
        Task<Result<List<RecipeDto>>> GetFavoriteRecipesForUserAsync(int userId);
        Task<Result<List<int>>> GetFavoriteRecipesIdsForUserAsync(int userId);
    }
}
