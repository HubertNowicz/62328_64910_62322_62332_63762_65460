using Organizer_przepisów_kulinarnych.Models.Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class AdminIngredientsViewModel
    {
        public List<IngredientViewModel> Ingredients { get; set; }
        public List<MeasurementUnitViewModel> AllUnits { get; set; }
        public List<PendingIngredientViewModel> PendingIngredients { get; set; }
    }
}
