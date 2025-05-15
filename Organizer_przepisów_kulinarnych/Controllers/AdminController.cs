using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
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

    public AdminController(IRecipeService recipeService, IAdminService adminService)
    {
        _recipeService = recipeService;
        _adminService = adminService;
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

}

