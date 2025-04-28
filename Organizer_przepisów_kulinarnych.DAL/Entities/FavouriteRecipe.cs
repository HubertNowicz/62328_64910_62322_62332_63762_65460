namespace Organizer_przepisów_kulinarnych.BLL.Entities
{
    public class FavoriteRecipe
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }

        public User User { get; set; }
        public Recipe Recipe { get; set; }
    }
}
