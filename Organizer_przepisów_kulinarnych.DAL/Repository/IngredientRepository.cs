using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.DAL.Repository
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ingredient>> GetAllAsync()
        {
            return await _context.Ingredients
                .Include(i => i.IngredientUnits)
                    .ThenInclude(iu => iu.Unit)
                .ToListAsync();
        }
        public async Task<List<RecipeIngredient>> GetAllRecipeIngredientsAsync()
        {
            return await _context.RecipeIngredients.ToListAsync();
        }

        public async Task<Ingredient?> GetByNameAsync(string name)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }

        public async Task<List<MeasurementUnit>> GetUnitsByIngredientNameAsync(string name)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.IngredientUnits)
                    .ThenInclude(iu => iu.Unit)
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());

            return ingredient?.IngredientUnits.Select(iu => iu.Unit).ToList()
                   ?? await _context.MeasurementUnits.ToListAsync();
        }

        public async Task<List<PendingIngredient>> GetAllPendingAsync()
        {
            return await _context.PendingIngredients
                .Include(p => p.MeasurementUnit)
                .Include(p => p.SuggestedByUser)
                .ToListAsync();
        }

        public async Task AddPendingAsync(PendingIngredient pending)
        {
            await _context.PendingIngredients.AddAsync(pending);
            await _context.SaveChangesAsync();
        }

        // --- New Methods ---

        public async Task<Ingredient?> GetByIdAsync(int id)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddIngredientAsync(Ingredient ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task AddIngredientUnitAsync(IngredientUnit unit)
        {
            await _context.IngredientUnits.AddAsync(unit);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MeasurementUnit>> GetUnitsByIdsAsync(List<int> unitIds)
        {
            return await _context.MeasurementUnits
                .Where(u => unitIds.Contains(u.Id))
                .ToListAsync();
        }
        public Task RemovePendingRangeAsync(IEnumerable<PendingIngredient> pendingList)
        {
            _context.PendingIngredients.RemoveRange(pendingList);
            return _context.SaveChangesAsync();
        }

        public async Task<bool> IsIngredientUsedInRecipesAsync(int ingredientId)
        {
            return await _context.RecipeIngredients.AnyAsync(ri => ri.IngredientId == ingredientId);
        }

        public async Task<List<string>> GetRecipeNamesUsingIngredientAsync(int ingredientId)
        {
            return await _context.RecipeIngredients
                .Where(ri => ri.IngredientId == ingredientId)
                .Select(ri => ri.Recipe.RecipeName)
                .ToListAsync();
        }

        public Task RemoveIngredientAsync(Ingredient ingredient)
        {
            _context.Ingredients.Remove(ingredient);
            return _context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Ingredient ingredient)
        {
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }
    }
}
