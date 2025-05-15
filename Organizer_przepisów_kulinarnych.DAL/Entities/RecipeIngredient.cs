namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int UnitId { get; set; }
        public MeasurementUnit Unit { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int? IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }
    }

}
