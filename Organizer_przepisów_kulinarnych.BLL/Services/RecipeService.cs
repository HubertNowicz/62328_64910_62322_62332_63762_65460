using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.Helpers;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class RecipeService : IRecipeService
    {
        private ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var recipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.FavoriteRecipes)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            return recipes;
        }

        public List<Recipe> GetFilteredRecipes(
            List<Recipe> recipes,
            bool filterUnder30,
            bool filterBetween30And60,
            bool filterOver60,
            SortOption sortOption)
        {
            if (filterUnder30 || filterBetween30And60 || filterOver60)
            {
                recipes = recipes.Where(r =>
                    (filterUnder30 && r.Preptime <= 30) ||
                    (filterBetween30And60 && r.Preptime >= 30 && r.Preptime <= 60) ||
                    (filterOver60 && r.Preptime > 60)
                ).ToList();
            }

            recipes = sortOption switch
            {
                SortOption.Newest => recipes.OrderByDescending(r => r.CreatedAt).ToList(),
                SortOption.Popularity => recipes.OrderByDescending(r => r.FavoriteRecipes.Count).ToList(),
                _ => recipes
            };

            return recipes;
        }


        public async Task<List<Recipe>> GetUserRecipesAsync(int userId)
        {
            var recipes = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.User)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .Where(r => r.UserId == userId)
                .ToListAsync();
            return recipes;
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.FavoriteRecipes)
                .Include(r => r.InstructionSteps)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .FirstAsync(r => r.Id == id);
        }

        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string categoryName)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.InstructionSteps)
                .Include(r => r.FavoriteRecipes)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Unit)
                .Where(r => r.Category.Name == categoryName)
                .ToListAsync();
        }

        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients
                .Include(i => i.IngredientUnits)
                    .ThenInclude(iu => iu.Unit)
                .ToListAsync();
        }
        public async Task<IEnumerable<MeasurementUnit>> GetAllUnitsAsync()
        {
            return await _context.MeasurementUnits.ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task CreateRecipeAsync(Recipe recipe, int userId)
        {
            var allIngredients = await _context.Ingredients.ToListAsync();
            var allPendingIngredients = await _context.PendingIngredients.ToListAsync();
            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var existingIngredient = allIngredients
                    .FirstOrDefault(i => StringHelper.FuzzyMatch(i.Name, recipeIngredient.Name));

                if (existingIngredient != null)
                {
                    recipeIngredient.IngredientId = existingIngredient.Id;
                    recipeIngredient.Name = existingIngredient.Name;
                }
                else
                {
                    recipeIngredient.IngredientId = null;
                    recipeIngredient.Ingredient = null;
                    var capitalizedName = StringHelper.CapitalizeFirstLetter(recipeIngredient.Name);

                    var alreadySuggested = allPendingIngredients
                        .Any(s => StringHelper.FuzzyMatch(s.Name, capitalizedName));

                    if (!alreadySuggested)
                    {
                        _context.PendingIngredients.Add(new PendingIngredient
                        {
                            Name = capitalizedName,
                            SuggestedByUserId = userId,
                            MeasurementUnitId = recipeIngredient.UnitId
                        });
                    }
                }
            }
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(Recipe recipe)
        {
            var relatedFavorites = _context.FavoriteRecipes.Where(fr => fr.RecipeId == recipe.Id);
            _context.FavoriteRecipes.RemoveRange(relatedFavorites);

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> MatchingIngredients(string term)
        {
            var ingredients = await GetAllIngredientsAsync();

            var matches = ingredients
                .Where(i => i.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.Name)
                .Distinct()
                .Take(10)
                .ToList();
            return matches;
        }
        public async Task<List<MeasurementUnit>> GetUnitsForIngredientAsync(string ingredientName)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.IngredientUnits)
                .ThenInclude(iu => iu.Unit)
                .FirstOrDefaultAsync(i => i.Name.ToLower() == ingredientName.ToLower());

            if (ingredient == null)
            {
                return await _context.MeasurementUnits.ToListAsync();
            } 

            return ingredient.IngredientUnits.Select(iu => iu.Unit).ToList();
        }

    }
}
