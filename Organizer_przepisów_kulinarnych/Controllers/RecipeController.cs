using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.Models;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;
        private readonly IFavortieRecipeService _favoriteRecipeService;
        private readonly IMapper _mapper;
        public RecipeController(IUserService userService, IRecipeService recipeService, IFavortieRecipeService favortieRecipeService, IMapper mapper)
        {
            _userService = userService;
            _recipeService = recipeService;
            _favoriteRecipeService = favortieRecipeService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] RecipeFilter filter)
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            var filteredRecipes = await _recipeService.GetFilteredRecipes(recipes, filter);

            var categories = await _recipeService.GetAllCategoriesAsync();

            var favoriteRecipesIds = new List<int>();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                favoriteRecipesIds = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId);
            }

            var recipeDtos = _mapper.Map<List<RecipeDto>>(filteredRecipes);
            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipeDtos);

            foreach (var vm in recipeViewModels)
            {
                vm.IsFavorite = favoriteRecipesIds.Contains(vm.Id);
            }

            return View(new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                Filters = filter,
                Categories = categories.Select(c => c.Name).ToList(),
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Category(string name, [FromQuery] RecipeFilter filter)
        {
            var recipes = await _recipeService.GetRecipesByCategoryAsync(name);
            var filteredRecipes = await _recipeService.GetFilteredRecipes(recipes, filter);

            var favoriteRecipeIds = new List<int>();
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                favoriteRecipeIds = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId);
            }

            var recipeDtos = _mapper.Map<List<RecipeDto>>(filteredRecipes);
            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipeDtos);
            foreach (var viewModel in recipeViewModels)
            {
                viewModel.IsFavorite = favoriteRecipeIds.Contains(viewModel.Id);
            }

            return View("Category", new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                Filters = filter,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }
        [HttpGet]
        public async Task<IActionResult> MyRecipes([FromQuery] RecipeFilter filter)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var recipes = await _recipeService.GetUserRecipesAsync(userId);
            var filteredRecipes = await _recipeService.GetFilteredRecipes(recipes, filter);
            var favoriteRecipeIds = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId);

            var recipeDtos = _mapper.Map<List<RecipeDto>>(filteredRecipes);
            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipeDtos);
            foreach (var vm in recipeViewModels)
            {
                vm.IsFavorite = favoriteRecipeIds.Contains(vm.Id);
                vm.IsUserRecipe = true;
            }

            return View(new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                Filters = filter,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            var recipeViewModel = _mapper.Map<RecipeViewModel>(recipe);

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                var favoriteRecipesIds = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId);

                recipeViewModel.IsFavorite = favoriteRecipesIds.Contains(recipe.Id);
                recipeViewModel.IsUserRecipe = recipe.User.Id == userId;
            }

            return View(recipeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _recipeService.GetAllCategoriesAsync();
            var units = await _recipeService.GetAllUnitsAsync();

            var viewModel = new RecipeCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }),
                Units = units.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Abbreviation})"
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _recipeService.GetAllCategoriesAsync();
                var units = await _recipeService.GetAllUnitsAsync();

                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

                model.Units = units.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Abbreviation})"
                });

                return View(model);
            }

            var userId = await _userService.GetCurrentUserIdAsync(User);
            var recipeCreateDto = _mapper.Map<RecipeCreateDto>(model);
            recipeCreateDto.UserId = userId;

            await _recipeService.CreateRecipeAsync(recipeCreateDto);

            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var recipeDto = await _recipeService.GetRecipeByIdAsync(id);
            if (recipeDto == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<RecipeCreateViewModel>(recipeDto);

            var categories = await _recipeService.GetAllCategoriesAsync();
            var units = await _recipeService.GetAllUnitsAsync();

            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            model.Units = units.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Name} ({u.Abbreviation})"
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _recipeService.GetAllCategoriesAsync();
                var units = await _recipeService.GetAllUnitsAsync();

                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

                model.Units = units.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Abbreviation})"
                });

                return View(model);
            }

            var userId = await _userService.GetCurrentUserIdAsync(User);

            var recipeUpdateDto = _mapper.Map<RecipeCreateDto>(model);
            recipeUpdateDto.UserId = userId; 

            await _recipeService.UpdateRecipeAsync(id, recipeUpdateDto, userId);

            return RedirectToAction(nameof(Details), new { id = id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int recipeId, string returnUrl)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            await _favoriteRecipeService.ToggleFavoriteAsync(userId, recipeId);

            return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _recipeService.DeleteRecipeAsync(id);

            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpGet]
        public async Task<IActionResult> SearchIngredients(string term)
        {
            var matchingIngredients = await _recipeService.MatchingIngredients(term);
            return Json(new
            {
                ingredientExists = matchingIngredients.Any(),
                suggestions = matchingIngredients
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnitsForIngredient(string name)
        {
            var units = await _recipeService.GetUnitsForIngredientAsync(name);
            return Json(units.Select(u => new { u.Id, u.Name, u.Abbreviation }));
        }
    }
}
