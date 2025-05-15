using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class FavoriteRecipeService : IFavortieRecipeService
    {
        private readonly ApplicationDbContext _context;
        public FavoriteRecipeService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task ToggleFavoriteAsync(int userId, int recipeId)
        {
            var existing = await _context.FavoriteRecipes
                .FirstOrDefaultAsync(f => f.RecipeId == recipeId && f.UserId == userId);

            if (existing != null)
            {
                _context.FavoriteRecipes.Remove(existing);
            }
            else
            {
                _context.FavoriteRecipes.Add(new FavoriteRecipe
                {
                    UserId = userId,
                    RecipeId = recipeId
                });
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<int>> GetFavoriteRecipesIdsForUserAsync(int userId)
        {
            return await _context.FavoriteRecipes
                .Where(fr => fr.UserId == userId)
                .Select(fr => fr.RecipeId)
                .ToListAsync();
        }
        public async Task<List<Recipe>> GetFavoriteRecipesForUserAsync(int userId)
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

    }
}
