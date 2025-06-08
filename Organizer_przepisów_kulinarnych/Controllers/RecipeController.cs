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
        private readonly IFavoriteRecipeService _favoriteRecipeService;
        private readonly IMapper _mapper;

        public RecipeController(
            IUserService userService,
            IRecipeService recipeService,
            IFavoriteRecipeService favoriteRecipeService,
            IMapper mapper)
        {
            _userService = userService;
            _recipeService = recipeService;
            _favoriteRecipeService = favoriteRecipeService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] RecipeFilter filter)
        {
            var allRecipesResult = await _recipeService.GetAllRecipesAsync();
            var categoriesResult = await _recipeService.GetAllCategoriesAsync();

            if (!allRecipesResult.Success || !categoriesResult.Success)
                return View("Error");

            var filteredRecipesResult = await _recipeService.GetFilteredRecipes(allRecipesResult.Data, filter);
            if (!filteredRecipesResult.Success)
                return View("Error");

            var favoriteRecipesIds = new List<int>();
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                var favoriteResult = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId.Data);
                if (favoriteResult.Success)
                    favoriteRecipesIds = favoriteResult.Data;
            }

            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(filteredRecipesResult.Data);
            foreach (var vm in recipeViewModels)
                vm.IsFavorite = favoriteRecipesIds.Contains(vm.Id);

            return View(new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                Filters = filter,
                Categories = categoriesResult.Data.Select(c => c.Name).ToList(),
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Category(string name, [FromQuery] RecipeFilter filter)
        {
            var recipesResult = await _recipeService.GetRecipesByCategoryAsync(name);
            var categoriesResult = await _recipeService.GetAllCategoriesAsync();
            if (!recipesResult.Success || !categoriesResult.Success)
                return View("Error");

            var filteredResult = await _recipeService.GetFilteredRecipes(recipesResult.Data, filter);
            if (!filteredResult.Success)
                return View("Error");

            var favoriteRecipeIds = new List<int>();
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                var favoriteResult = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId.Data);
                if (favoriteResult.Success)
                    favoriteRecipeIds = favoriteResult.Data;
            }

            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(filteredResult.Data);
            foreach (var vm in recipeViewModels)
                vm.IsFavorite = favoriteRecipeIds.Contains(vm.Id);

            return View("Category", new RecipeListViewModel
            {
                Recipes = recipeViewModels,
                Filters = filter,
                Categories = categoriesResult.Data.Select(c => c.Name).ToList(),
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName
            });
        }

        [HttpGet]
        public async Task<IActionResult> MyRecipes([FromQuery] RecipeFilter filter)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            var recipesResult = await _recipeService.GetUserRecipesAsync(userId.Data);
            if (!recipesResult.Success)
                return View("Error");

            var filteredResult = await _recipeService.GetFilteredRecipes(recipesResult.Data, filter);
            if (!filteredResult.Success)
                return View("Error");

            var favoriteResult = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId.Data);
            var favoriteIds = favoriteResult.Success ? favoriteResult.Data : new List<int>();

            var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(filteredResult.Data);
            foreach (var vm in recipeViewModels)
            {
                vm.IsFavorite = favoriteIds.Contains(vm.Id);
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
            var result = await _recipeService.GetRecipeByIdAsync(id);
            if (!result.Success)
                return NotFound();

            var recipeViewModel = _mapper.Map<RecipeViewModel>(result.Data);

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = await _userService.GetCurrentUserIdAsync(User);
                var favoriteResult = await _favoriteRecipeService.GetFavoriteRecipesIdsForUserAsync(userId.Data);
                if (favoriteResult.Success)
                    recipeViewModel.IsFavorite = favoriteResult.Data.Contains(result.Data.Id);

                recipeViewModel.IsUserRecipe = result.Data.User.Id == userId.Data;
            }

            return View(recipeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categoriesResult = await _recipeService.GetAllCategoriesAsync();
            var unitsResult = await _recipeService.GetAllUnitsAsync();

            if (!categoriesResult.Success || !unitsResult.Success)
                return View("Error");

            var viewModel = new RecipeCreateViewModel
            {
                Categories = categoriesResult.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }),
                Units = unitsResult.Data.Select(u => new SelectListItem
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

                model.Categories = categories.Data?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }) ?? Enumerable.Empty<SelectListItem>();

                model.Units = units.Data?.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Abbreviation})"
                }) ?? Enumerable.Empty<SelectListItem>();

                return View(model);
            }

            var userId = await _userService.GetCurrentUserIdAsync(User);
            var recipeCreateDto = _mapper.Map<RecipeCreateDto>(model);
            recipeCreateDto.UserId = userId.Data;

            var result = await _recipeService.CreateRecipeAsync(recipeCreateDto);
            if (!result.Success)
                return View("Error");

            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _recipeService.GetRecipeByIdAsync(id);
            if (!result.Success)
                return NotFound();

            var model = _mapper.Map<RecipeCreateViewModel>(result.Data);

            var categoriesResult = await _recipeService.GetAllCategoriesAsync();
            var unitsResult = await _recipeService.GetAllUnitsAsync();
            if (!categoriesResult.Success || !unitsResult.Success)
                return View("Error");

            model.Categories = categoriesResult.Data.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            model.Units = unitsResult.Data.Select(u => new SelectListItem
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

                model.Categories = categories.Data?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }) ?? Enumerable.Empty<SelectListItem>();

                model.Units = units.Data?.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Abbreviation})"
                }) ?? Enumerable.Empty<SelectListItem>();

                return View(model);
            }

            var userId = await _userService.GetCurrentUserIdAsync(User);

            var recipeUpdateDto = _mapper.Map<RecipeCreateDto>(model);
            recipeUpdateDto.UserId = userId.Data;

            var result = await _recipeService.UpdateRecipeAsync(id, recipeUpdateDto, userId.Data);
            if (!result.Success)
                return View("Error");

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int recipeId, string returnUrl)
        {
            var userId = await _userService.GetCurrentUserIdAsync(User);
            await _favoriteRecipeService.ToggleFavoriteAsync(userId.Data, recipeId);
            return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipeService.DeleteRecipeAsync(id);
            if (!result.Success)
                return View("Error");

            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpGet]
        public async Task<IActionResult> SearchIngredients(string term)
        {
            var result = await _recipeService.MatchingIngredients(term);
            return Json(new
            {
                ingredientExists = result.Success && result.Data.Any(),
                suggestions = result.Success ? result.Data : new List<string>()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnitsForIngredient(string name)
        {
            var result = await _recipeService.GetUnitsForIngredientAsync(name);
            if (!result.Success)
                return Json(new List<object>());

            return Json(result.Data.Select(u => new { u.Id, u.Name, u.Abbreviation }));
        }
    }

}
