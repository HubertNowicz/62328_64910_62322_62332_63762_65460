using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.Models;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    [Authorize]
    public class FavoriteRecipeController : Controller
    {
        private readonly IFavoriteRecipeService _favoriteRecipeService;
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;

        public FavoriteRecipeController(IFavoriteRecipeService favoriteRecipe, IUserService userService, IRecipeService recipeService, IMapper mapper)
        {
            _favoriteRecipeService = favoriteRecipe;
            _userService = userService;
            _recipeService = recipeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] RecipeFilter filter)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var userFavoriteRecipes = await _favoriteRecipeService.GetFavoriteRecipesForUserAsync(userId.Data);
            var filteredRecipes = await _recipeService.GetFilteredRecipes(userFavoriteRecipes.Data, filter);

            var recipeDtos = _mapper.Map<List<RecipeDto>>(filteredRecipes.Data);
            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipeDtos);

            foreach (var vm in recipeViewModels)
            {
                vm.IsFavorite = true;
            }

            return View(new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                Filters = filter,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int recipeId, string returnUrl)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var result = await _favoriteRecipeService.ToggleFavoriteAsync(userId.Data, recipeId);
            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction("Index", "Error");
            }

            return Redirect(returnUrl);
        }
    }
}
