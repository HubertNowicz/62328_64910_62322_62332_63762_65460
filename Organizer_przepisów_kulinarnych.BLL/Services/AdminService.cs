using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.Helpers;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<PendingIngredient>> GetAllPendingIngredientsAsync()
        {
            var suggestions = await _context.PendingIngredients
                             .Include(s => s.SuggestedByUser)
                             .ToListAsync();

            return suggestions;
        }
        public async Task ApprovePendingIngredientAsync(int suggestedIngredientId)
        {
            var allIngredients = await _context.Ingredients.ToListAsync();
            var allPendingIngredients = await _context.PendingIngredients.ToListAsync();

            var suggestion = allPendingIngredients
                .First(s => s.Id == suggestedIngredientId);

            int ingredientId = 0;
            var exists = allIngredients
                .Any(i => StringHelper.FuzzyMatch(i.Name, suggestion.Name));
            if (!exists)
            {
                var capitalizedName = StringHelper.CapitalizeFirstLetter(suggestion.Name);
                var newIngredient = new Ingredient
                {
                    Name = capitalizedName
                };
                await _context.Ingredients.AddAsync(newIngredient);
                await _context.SaveChangesAsync();
                ingredientId = newIngredient.Id;
            }

            var matchingSuggestions = allPendingIngredients
                .Where(s => StringHelper.FuzzyMatch(s.Name, suggestion.Name));

            _context.PendingIngredients.RemoveRange(matchingSuggestions);

            var recipesUsingPendingIngredient = await _context.RecipeIngredients.ToListAsync();

            var matchingRecipesIngredients = recipesUsingPendingIngredient
                .Where(ri => StringHelper
                .FuzzyMatch(ri.Name, suggestion.Name))
                .ToList();

            foreach (var recipeIngredient in matchingRecipesIngredients)
            {
                recipeIngredient.IngredientId = ingredientId;
            }
            await _context.SaveChangesAsync();
        }

        public async Task RejectPendingIngredientAsync(int suggestedIngredientId)
        {
            var allIngredients = await _context.Ingredients.ToListAsync();
            var allPendingIngredients = await _context.PendingIngredients.ToListAsync();

            var suggestion = allPendingIngredients
                .FirstOrDefault(s => s.Id == suggestedIngredientId);

            if (suggestion != null)
            {

                var matchingSuggestions = allPendingIngredients
                    .Where(s => StringHelper.FuzzyMatch(s.Name, suggestion.Name));

                _context.PendingIngredients.RemoveRange(matchingSuggestions);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(bool Success, string ErrorMessage)> AddIngredientAsync(string ingredientName)
        {
            var allIngredients = await _context.Ingredients.ToListAsync();
            var allPendingIngredients = await _context.PendingIngredients.ToListAsync();

            var exists = allIngredients
                .Any(i => StringHelper.FuzzyMatch(i.Name, ingredientName));

            if (exists)
            {
                return (false, "Składnik już istnieje w bazie.");
            }

            var capitalizedName = StringHelper.CapitalizeFirstLetter(ingredientName);
            var ingredient = new Ingredient
            {
                Name = capitalizedName
            };

            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string Message)> DeleteIngredientAsync(int id)
        {
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
            {
                return (false, "Ingredient not found.");
            }

            var isInUseInRecipes = await _context.RecipeIngredients
                .AnyAsync(ri => ri.IngredientId == id);

            if (isInUseInRecipes)
            {
                var recipeNames = await _context.RecipeIngredients
                    .Where(ri => ri.IngredientId == id)
                    .Select(ri => ri.Recipe.RecipeName)
                    .ToListAsync();

                string recipeList = string.Join(", ", recipeNames);
                return (false, $"Cannot delete this ingredient as it is used in the following recipes: {recipeList}.");
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return (true, "Ingredient deleted successfully.");
        }
    }
}
