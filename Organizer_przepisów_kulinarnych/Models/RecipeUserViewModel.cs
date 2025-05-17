namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeUserViewModel
    {
        public int Id { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Preptime { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public int FavoriteCount { get; set; }
    }
}
