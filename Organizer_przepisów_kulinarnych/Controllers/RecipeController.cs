using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.Models;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;
        private readonly IFavortieRecipeService _favortieRecipeService;

        public RecipeController(ApplicationDbContext context, IUserService userService, IRecipeService recipeService, IFavortieRecipeService favortieRecipeService)
        {
            _userService = userService;
            _recipeService = recipeService;
            _favortieRecipeService = favortieRecipeService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();

            List<int> favoriteRecipeIds = [];

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                favoriteRecipeIds = await _favortieRecipeService.GetFavoriteRecipesIdsForUserAsync(userId);
            }

            var recipeViewModels = recipes.Select(r => new RecipeViewModel
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
                FirstName = r.User.FirstName,
                Surname = r.User.Surname,
                CreatedAt = r.CreatedAt,
                CategoryName = r.Category.Name,
                IsFavorite = favoriteRecipeIds.Contains(r.Id)
            }).ToList();

            return View(recipeViewModels);
        }

        public async Task<IActionResult> MyRecipes()
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var recipes = await _recipeService.GetUserRecipesAsync(userId);

            var recipeViewModels = recipes.Select(r => new RecipeViewModel
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
                FirstName = r.User.FirstName,
                Surname = r.User.Surname,
                CreatedAt = r.CreatedAt,
                CategoryName = r.Category.Name
            }).ToList();

            return View(recipeViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _recipeService.GetAllCategoriesAsync();
            var viewModel = new RecipeCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeCreateViewModel model)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);

            var recipe = new Recipe
            {
                RecipeName = model.RecipeName,
                Description = model.Description,
                Instructions = model.Instructions,
                Preptime = model.Preptime,
                CategoryId = model.CategoryId,
                UserId = userId,
                RecipeIngredients = model.Ingredients.Select(i => new RecipeIngredient
                {
                    Amount = i.Amount,
                    Unit = i.Unit,
                    Name = i.Name,
                }).ToList()
            };
            await _recipeService.CreateRecipeAsync(recipe, userId);
            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int recipeId)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            await _favortieRecipeService.ToggleFavoriteAsync(userId, recipeId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            await _recipeService.DeleteRecipeAsync(recipe);
            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpGet]
        public async Task<IActionResult> SearchIngredients(string term)
        {
            var matches = await _recipeService.MatchingIngredients(term);

            return Json(matches);
        }

    }
}
