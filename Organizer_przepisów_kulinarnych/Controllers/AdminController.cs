using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var model = new AdminIngredientsViewModel
        {
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveSuggestion(int id)
    {
        await _adminService.ApprovePendingIngredientAsync(id);
        return RedirectToAction(nameof(ManageIngredients));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectSuggestion(int id)
    {
        await _adminService.RejectPendingIngredientAsync(id);
        return RedirectToAction(nameof(ManageIngredients));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var result = await _adminService.DeleteIngredientAsync(id);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(ManageIngredients));
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(ManageIngredients));
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddIngredient(string ingredientName)
    {
        var (success, errorMessage) = await _adminService.AddIngredientAsync(ingredientName);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, errorMessage);
            return View();
        }

        TempData["SuccessMessage"] = "Składnik został dodany pomyślnie.";
        return RedirectToAction(nameof(ManageIngredients));
    }
}

