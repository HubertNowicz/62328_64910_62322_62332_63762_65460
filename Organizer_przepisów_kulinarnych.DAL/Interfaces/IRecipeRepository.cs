using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.DAL.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetAllAsync();
        Task<Recipe?> GetByIdAsync(int id);
        Task<List<Recipe>> GetByUserIdAsync(int userId);
        Task<List<Recipe>> GetByCategoryAsync(string categoryName);
        Task AddAsync(Recipe recipe);
        Task DeleteAsync(Recipe recipe);
        Task SaveChangesAsync();
    }

}
