using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;

namespace Organizer_przepisów_kulinarnych.ViewComponents
{
    public class CategoryDropdownViewComponent : ViewComponent
    {
        private readonly IRecipeService _recipeService;

        public CategoryDropdownViewComponent(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _recipeService.GetAllCategoriesAsync();
            return View(categories.Data);
        }
    }
}
