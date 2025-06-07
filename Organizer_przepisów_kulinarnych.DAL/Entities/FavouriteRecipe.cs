namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class FavoriteRecipe
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }

        public required User User { get; set; }
        public required Recipe Recipe { get; set; }
    }
}
