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
        var ingredientViewModels = _mapper.Map<List<IngredientViewModel>>(ingredientDtos.Data);

        var pendingIngredientDtos = await _adminService.GetAllPendingIngredientsAsync();
        var pendingIngredientViewModels = _mapper.Map<List<PendingIngredientViewModel>>(pendingIngredientDtos.Data);

        var measurementUnitDtos = await _recipeService.GetAllUnitsAsync();
        var measurementUnitsViewModels = _mapper.Map<List<MeasurementUnitViewModel>>(measurementUnitDtos.Data);

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
        var result = await _adminService.ApprovePendingIngredientAsync(id);
        if (result.Success)
            return Ok(new { message = "Approved successfully." });
        else
            return BadRequest(new { message = result.Error });
    }


    [HttpPost]
    public async Task<IActionResult> RejectSuggestion(int id)
    {
        var result = await _adminService.RejectPendingIngredientAsync(id);
        if (result.Success)
            return Ok(new { message = "Ingredient rejected." });
        else
            return BadRequest(new { message = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> AddIngredient(string ingredientName, List<int> unitIds)
    {
        var result = await _adminService.AddIngredientAsync(ingredientName, unitIds);

        if (result.Success)
        {
            return Ok(new { message = result.Message ?? "Ingredient processed successfully." });
        }

        return BadRequest(new { message = result.Error ?? "Failed to add ingredient." });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var result = await _adminService.DeleteIngredientAsync(id);

        if (result.Success)
        {
            return Ok(new { message = result.Message });
        }

        return BadRequest(new { message = result.Error });
    }



    [HttpGet]
    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users.Data);
        var model = _mapper.Map<List<UserViewModel>>(userDtos);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> UserRecipes(int id)
    {
        var recipesDto = await _recipeService.GetUserRecipesAsync(id);
        var recipeViewModels = _mapper.Map<List<RecipeViewModel>>(recipesDto.Data);

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

        var model = _mapper.Map<UserViewModel>(userDto.Data);
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

        if (!result.Success)
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

        var result = await _userService.DeleteUserAsync(id);
        
        if (!result.Success)
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
        var recipeResult = await _recipeService.GetRecipeByIdAsync(id);
        if (!recipeResult.Success || recipeResult.Data.UserId != userId)
        {
            TempData["Error"] = "Nie znaleziono przepisu lub nie należy do użytkownika.";
            return RedirectToAction("UserRecipes", new { id = userId });
        }

        var deleteResult = await _recipeService.DeleteRecipeAsync(id);
        if (!deleteResult.Success)
        {
            TempData["Error"] = deleteResult.Error ?? "Wystąpił błąd podczas usuwania przepisu.";
            return RedirectToAction("UserRecipes", new { id = userId });
        }

        TempData["Info"] = "Przepis został usunięty.";
        return RedirectToAction("UserRecipes", new { id = userId });
    }
}

