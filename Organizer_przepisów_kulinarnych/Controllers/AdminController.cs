using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.Models;
using Organizer_przepisów_kulinarnych.Models.Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{

    private readonly IRecipeService _recipeService;
    private readonly IAdminService _adminService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AdminController(IRecipeService recipeService, IAdminService adminService, IMapper mapper, IUserService userService)
    {
        _recipeService = recipeService;
        _adminService = adminService;
        _mapper = mapper;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ManageIngredients()
    {
        var ingredientDtos = await _recipeService.GetAllIngredientsAsync();
        var ingredientViewModels = _mapper.Map<List<IngredientViewModel>>(ingredientDtos);

        var pendingIngredientDtos = await _adminService.GetAllPendingIngredientsAsync();
        var pendingIngredientViewModels = _mapper.Map<List<PendingIngredientViewModel>>(pendingIngredientDtos);

        var measurementUnitDtos = await _recipeService.GetAllUnitsAsync();
        var measurementUnitsViewModels = _mapper.Map<List<MeasurementUnitViewModel>>(measurementUnitDtos);

        var model = new AdminIngredientsViewModel
        {
            AllUnits = measurementUnitsViewModels,
            Ingredients = ingredientViewModels,
            PendingIngredients = pendingIngredientViewModels
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveSuggestion(int id)
    {
        await _adminService.ApprovePendingIngredientAsync(id);

        return Ok(new { message = "Approved successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> RejectSuggestion(int id)
    {
        await _adminService.RejectPendingIngredientAsync(id);

        return Ok(new { message = "Ingredient rejected." });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var result = await _adminService.DeleteIngredientAsync(id);

        if (result.Success)
        {
            return Ok(new { message = result.Message });
        }
        return BadRequest(new { message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> AddIngredient(string ingredientName, List<int> unitIds)
    {
        var (success, errorMessage) = await _adminService.AddIngredientAsync(ingredientName, unitIds);

        if (success)
        {
            return Ok(new { message = "Ingredient processed successfully." });
        }

        return BadRequest(new { message = errorMessage ?? "Failed to add ingredient." });
    }

    [HttpGet]
    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users);
        var model = _mapper.Map<List<UserViewModel>>(userDtos);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> UserRecipes(int id)
    {
        var recipesDto = await _recipeService.GetUserRecipesAsync(id);
        var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipesDto);

        var model = new RecipeListViewModel
        {
            Recipes = recipeViewModels,
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var userDto = await _userService.GetUserByIdAsync(id);
        if (userDto == null)
            return NotFound();

        var model = _mapper.Map<UserViewModel>(userDto);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(UserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userDto = _mapper.Map<UserDto>(model);
        var result = await _userService.UpdateUserAsync(userDto);

        if (!result)
        {
            return NotFound();
        }

        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Nie znaleziono użytkownika.";
            return RedirectToAction("ManageUsers");
        }

        var success = await _userService.DeleteUserAsync(id);
        
        if (!success)
        {
            TempData["Error"] = "Nie udało się usunąć użytkownika.";
        }
        else
        {
            TempData["Info"] = "Użytkownik został usunięty.";
        }

        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRecipe(int id, int userId)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);
     
        if (recipe == null || recipe.UserId != userId)
        {
            TempData["Error"] = "Nie znaleziono przepisu lub nie należy do użytkownika.";
            return RedirectToAction("UserRecipes", new { id = userId });
        }

        await _recipeService.DeleteRecipeAsync(id);

        TempData["Info"] = "Przepis został usunięty.";
        return RedirectToAction("UserRecipes", new { id = userId });
    }

}

