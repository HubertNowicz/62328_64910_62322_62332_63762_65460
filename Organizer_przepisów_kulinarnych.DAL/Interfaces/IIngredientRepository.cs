using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.DAL.Interfaces
{
    public interface IIngredientRepository
    {

        Task<List<Ingredient>> GetAllAsync();
        Task<List<RecipeIngredient>> GetAllRecipeIngredientsAsync();
        Task<Ingredient?> GetByNameAsync(string name);
        Task<List<MeasurementUnit>> GetUnitsByIngredientNameAsync(string name);
        Task<List<PendingIngredient>> GetAllPendingAsync();
        Task AddPendingAsync(PendingIngredient pending);
        Task<Ingredient?> GetByIdAsync(int id);
        Task AddIngredientAsync(Ingredient ingredient);
        Task DeleteAsync(Ingredient ingredient);
        Task AddIngredientUnitAsync(IngredientUnit unit);
        Task<List<MeasurementUnit>> GetUnitsByIdsAsync(List<int> unitIds);
        Task RemovePendingRangeAsync(IEnumerable<PendingIngredient> pendingList);
        Task<bool> IsIngredientUsedInRecipesAsync(int ingredientId);
        Task<List<string>> GetRecipeNamesUsingIngredientAsync(int ingredientId);
        Task RemoveIngredientAsync(Ingredient ingredient);
        Task SaveChangesAsync();

    }

}
