using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.Models;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    [Authorize]
    public class FavoriteRecipeController : Controller
    {
        private readonly IFavortieRecipeService _favoriteRecipeService;
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;

        public FavoriteRecipeController(ApplicationDbContext context, IFavortieRecipeService favortieRecipe, IUserService userService, IRecipeService recipeService)
        {
            _favoriteRecipeService = favortieRecipe;
            _userService = userService;
            _recipeService = recipeService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int recipeId)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            await _favoriteRecipeService.ToggleFavoriteAsync(userId, recipeId);
            return RedirectToAction("Index", "FavoriteRecipe");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var userFavoriteRecipes = await _favoriteRecipeService.GetFavoriteRecipesForUserAsync(userId);

            var users = await _userService.GetAllUsersAsync();
            var categories = await _recipeService.GetAllCategoriesAsync();

            var viewModel = userFavoriteRecipes.Select(r =>
            {
                var user = users.FirstOrDefault(u => u.Id == r.UserId);
                var category = categories.FirstOrDefault(c => c.Id == r.CategoryId);

                return new FavouriteRecipeViewModel
                {
                    RecipeId = r.Id,
                    RecipeName = r.RecipeName,
                    Description = r.Description,
                    Ingredients = r.RecipeIngredients.Select(ri => new IngredientViewModel
                    {
                        Name = ri.Name,
                        Amount = ri.Amount,
                        Unit = ri.Unit
                    }).ToList(),
                    Instructions = r.Instructions,
                    Preptime = r.Preptime,
                    CreatedAt = r.CreatedAt,
                    Name = category?.Name ?? "Brak",
                    FirstName = user?.FirstName ?? "Brak",
                    Surname = user?.Surname ?? "Brak",
                };
            }).ToList();

            return View(viewModel);
        }
    }

}
