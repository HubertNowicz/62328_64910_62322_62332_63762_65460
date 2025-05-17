using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    private readonly IRecipeService _recipeService;
    private readonly IAdminService _adminService;
    private readonly IAdminUserService _adminUserService;
    

    public AdminController(IRecipeService recipeService, IAdminService adminService, IAdminUserService adminUserService)
    {
        _recipeService = recipeService;
        _adminService = adminService;
        _adminUserService = adminUserService;
       
    }

    [HttpGet]
    public async Task<IActionResult> ManageIngredients()
    {
        var ingredients = await _recipeService.GetAllIngredientsAsync();
        var pendingIngredients = await _adminService.GetAllPendingIngredientsAsync();
        var units = await _recipeService.GetAllUnitsAsync();
        var model = new AdminIngredientsViewModel
        {
            AllUnits = units.ToList(),
            Ingredients = ingredients.ToList(),
            PendingIngredients = pendingIngredients.Select(x => new PendingIngredientViewModel
            {
                Id = x.Id,
                Name = x.Name,
                SuggestedAt = x.SuggestedAt,
                SuggestedBy = $"{x.SuggestedByUser.FirstName} {x.SuggestedByUser.Surname}"
            }).ToList()
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

    public async Task<IActionResult> UsersList()
    {
        var users = await _adminUserService.GetAllUsersAsync();

        var viewModel = users.Select(u => new UserListView
        {
            Id = u.Id,
            Email = u.Email,
            Username = u.Username,
            FirstName = u.FirstName,
            Surname = u.Surname
        }).ToList();

        return View(viewModel);
    }

    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _adminUserService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var model = new UserEditViewModel
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            Surname = user.Surname,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(UserEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            Id = model.Id,
            Username = model.Username,
            FirstName = model.FirstName,
            Surname = model.Surname,
            Email = model.Email
        };

        await _adminUserService.UpdateUserAsync(user);
        return RedirectToAction("Userslist");
    }
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _adminUserService.GetUserByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Nie znaleziono użytkownika.";
            return RedirectToAction("UsersList");
        }

        
        if (user.UserRole == 0)
        {
            TempData["Error"] = "Nie możesz usunąć konta administratora.";
            return RedirectToAction("UsersList");
        }
        var success = await _adminUserService.DeleteUserAsync(id);
        
      
        if (!success)
        {
            TempData["Error"] = "Nie udało się usunąć użytkownika.";
        }
        else
        {
            TempData["Info"] = "Użytkownik został usunięty.";
        }

        return RedirectToAction("UsersList");
    }

    public async Task<IActionResult> UserRecipes(int id)
    {
        var user = await _adminUserService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        var recipes = await _recipeService.GetUserRecipesAsync(id); // <- pobieranie danych

        var viewModels = recipes.Select(r => new RecipeUserViewModel
        {
            Id = r.Id,
            RecipeName = r.RecipeName,
            Description = r.Description,
            Preptime = r.Preptime,
            CreatedAt = r.CreatedAt,
            CategoryName = r.Category?.Name ?? "Brak",
            FavoriteCount = r.FavoriteRecipes?.Count ?? 0
        }).ToList();
        ViewBag.UserId = user.Id;
        ViewBag.UserFullName = $"{user.FirstName} {user.Surname}";

        return View(viewModels); 
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

        await _recipeService.DeleteRecipeAsync(recipe);

        TempData["Info"] = "Przepis został usunięty.";
        return RedirectToAction("UserRecipes", new { id = userId });
    }

}

