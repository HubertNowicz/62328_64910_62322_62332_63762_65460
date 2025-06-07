namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<IngredientUnit> IngredientUnits { get; set; } = new List<IngredientUnit>();
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    }
}
