namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Preptime { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public required User User { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }

        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }  = new List<RecipeIngredient>();
        public ICollection<RecipeInstructionStep> InstructionSteps { get; set; } = [];
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = [];
 

    }
}
