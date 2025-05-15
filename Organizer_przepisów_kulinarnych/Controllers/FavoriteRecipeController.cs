using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.Models;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    [Authorize]
    public class FavoriteRecipeController : Controller
    {
        private readonly IFavortieRecipeService _favoriteRecipeService;
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;

        public FavoriteRecipeController(ApplicationDbContext context, IFavortieRecipeService favortieRecipe, IUserService userService, IRecipeService recipeService, IMapper mapper)
        {
            _favoriteRecipeService = favortieRecipe;
            _userService = userService;
            _recipeService = recipeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            bool filterUnder30,
            bool filterBetween30And60,
            bool filterOver60,
            SortOption sortOption)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var userFavoriteRecipes = await _favoriteRecipeService.GetFavoriteRecipesForUserAsync(userId);

            var filteredRecipes = _recipeService.GetFilteredRecipes(
                userFavoriteRecipes, filterUnder30, filterBetween30And60, filterOver60, sortOption);

            var recipeDtos = _mapper.Map<List<RecipeDto>>(filteredRecipes);
            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipeDtos);

            foreach (var vm in recipeViewModels)
            {
                vm.IsFavorite = true;
            }

            return View(new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                FilterUnder30 = filterUnder30,
                FilterBetween30And60 = filterBetween30And60,
                FilterOver60 = filterOver60,
                SortOption = sortOption,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int recipeId, string returnUrl)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            await _favoriteRecipeService.ToggleFavoriteAsync(userId, recipeId);

            return Redirect(returnUrl);
        }
    }

}
