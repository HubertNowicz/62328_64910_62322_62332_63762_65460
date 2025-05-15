namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class FavoriteRecipeDto
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
    }
}
