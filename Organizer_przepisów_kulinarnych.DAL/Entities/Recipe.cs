namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public int Preptime { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
