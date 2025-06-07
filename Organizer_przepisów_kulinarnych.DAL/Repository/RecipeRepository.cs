using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.DAL.Repository
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetAllAsync()
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.FavoriteRecipes)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Unit)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.FavoriteRecipes)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Unit)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Recipe>> GetByUserIdAsync(int userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.Category)
                .Include(r => r.User)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Unit)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetByCategoryAsync(string categoryName)
        {
            return await _context.Recipes
                .Where(r => r.Category.Name == categoryName)
                .Include(r => r.Category)
                .Include(r => r.User)
                .Include(r => r.FavoriteRecipes)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Unit)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
        }
        public async Task DeleteAsync(Recipe recipe)
        {
            var relatedFavorites = _context.FavoriteRecipes.Where(fr => fr.RecipeId == recipe.Id);
            _context.FavoriteRecipes.RemoveRange(relatedFavorites);
            _context.Recipes.Remove(recipe);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }

}
