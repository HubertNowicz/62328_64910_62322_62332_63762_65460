namespace Organizer_przepisów_kulinarnych.Models
{
    public class FavouriteRecipeViewModel
    {
        public int RecipeId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public List<IngredientViewModel> Ingredients { get; set; } = [];
        public int Preptime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
    }

       
        
    
}
