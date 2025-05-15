
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeListViewModel
    {
        public List<RecipeViewModel> Recipes { get; set; } = [];
        public List<string> Categories { get; set; } = new List<string>();
        public string SelectedCategory { get; set; } = string.Empty;
        public bool FilterUnder30 { get; set; }
        public bool FilterBetween30And60 { get; set; }
        public bool FilterOver60 { get; set; }
        public SortOption SortOption { get; set; } = SortOption.None;
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

    }

}
