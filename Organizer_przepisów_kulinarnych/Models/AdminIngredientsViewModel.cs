using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class AdminIngredientsViewModel
    {
        public List<Ingredient> Ingredients { get; set; } = [];
        public List<PendingIngredientViewModel> PendingIngredients { get; set; } = [];
    }
}
