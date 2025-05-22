
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.DAL.Repository
{
    public class FavoriteRecipeRepository : IFavoriteRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteRecipe?> GetByUserAndRecipeAsync(int userId, int recipeId)
        {
            return await _context.FavoriteRecipes
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        }

        public async Task AddAsync(FavoriteRecipe favoriteRecipe)
        {
            await _context.FavoriteRecipes.AddAsync(favoriteRecipe);
        }

        public void Remove(FavoriteRecipe favoriteRecipe)
        {
            _context.FavoriteRecipes.Remove(favoriteRecipe);
        }
        public async Task<List<int>> GetRecipeIdsByUserAsync(int userId)
        {
            return await _context.FavoriteRecipes
                .Where(fr => fr.UserId == userId)
                .Select(fr => fr.RecipeId)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetFavoriteRecipesByUserAsync(int userId)
        {
            return await _context.Recipes
                .Where(r => r.FavoriteRecipes.Any(fr => fr.UserId == userId))
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
