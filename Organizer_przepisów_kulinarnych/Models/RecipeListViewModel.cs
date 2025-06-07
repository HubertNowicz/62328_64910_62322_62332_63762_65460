
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeListViewModel
    {
        public required string ControllerName { get; set; }
        public required string ActionName { get; set; }
        public List<RecipeViewModel> Recipes { get; set; } = [];
        public List<string> Categories { get; set; } = new List<string>();
        public RecipeFilter Filters { get; set; } = new();
    }

}
