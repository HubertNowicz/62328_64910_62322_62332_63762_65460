using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.DAL.Interfaces
{
    public interface IFavoriteRecipeRepository
    {
        Task<FavoriteRecipe?> GetByUserAndRecipeAsync(int userId, int recipeId);
        Task AddAsync(FavoriteRecipe favoriteRecipe);
        void Remove(FavoriteRecipe favoriteRecipe);
        Task<List<int>> GetRecipeIdsByUserAsync(int userId);
        Task<List<Recipe>> GetFavoriteRecipesByUserAsync(int userId);
        Task SaveChangesAsync();
    }
}
