using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Organizer_przepisów_kulinarnych.Models;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities; // dopasuj do lokalizacji FavouriteRecipe

namespace Organizer_przepisów_kulinarnych.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int recipeId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized();

            var existing = await _context.FavoriteRecipes
                .FirstOrDefaultAsync(f => f.RecipeId == recipeId && f.UserId == userId);

            if (existing != null)
            {
                // odznaczenie = usuń
                _context.FavoriteRecipes.Remove(existing);
            }
            else
            {
                // zaznaczenie = dodaj
                var fav = new FavoriteRecipe
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    
                };
                _context.FavoriteRecipes.Add(fav);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Recipe");
        }
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
                return Unauthorized();

            var favoriteRecipeIds = await _context.FavoriteRecipes
                .Where(f => f.UserId == userId)
                .Select(f => f.RecipeId)
                .ToListAsync();

            var recipes = await _context.Recipes
                .Where(r => favoriteRecipeIds.Contains(r.Id))
                .ToListAsync();

            var users = await _context.Users.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var viewModel = recipes.Select(r =>
            {
                var user = users.FirstOrDefault(u => u.Id == r.UserId);
                var category = categories.FirstOrDefault(c => c.Id == r.CategoryId);

                return new FavouriteRecipe
                {
                    RecipeId = r.Id,
                    RecipeName = r.RecipeName,
                    Description = r.Description,
                    Ingredients = r.Ingredients,
                    Instructions = r.Instructions,
                    Preptime = r.Preptime,
                    CreatedAt = r.CreatedAt,
                    CategoryId = r.CategoryId,
                    Name = category?.Name ?? "Brak",
                    FirstName = user?.FirstName ?? "Brak",
                    SurName = user?.Surname ?? "Brak",
                    FavoriteRecipe = true
                };
            }).ToList();

            return View(viewModel);
        }
    }
    
}
