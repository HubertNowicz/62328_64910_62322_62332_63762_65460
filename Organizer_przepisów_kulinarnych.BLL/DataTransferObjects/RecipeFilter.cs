using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeFilter
    {
        public RecipeSortOption SortOption { get; set; } = RecipeSortOption.None;
        public bool FilterUnder30 { get; set; }
        public bool FilterBetween30And60 { get; set; }
        public bool FilterOver60 { get; set; }
        public string SelectedCategory { get; set; } = string.Empty;
    }
}

