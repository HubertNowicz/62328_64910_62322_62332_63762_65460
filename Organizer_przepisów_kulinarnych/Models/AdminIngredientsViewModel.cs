using Organizer_przepisów_kulinarnych.Models.Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class AdminIngredientsViewModel
    {
        public required List<IngredientViewModel> Ingredients { get; set; }
        public required List<MeasurementUnitViewModel> AllUnits { get; set; }
        public required List<PendingIngredientViewModel> PendingIngredients { get; set; }
    }
}
