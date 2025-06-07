namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class IngredientUnit
    {
        public int IngredientId { get; set; }
        public required Ingredient Ingredient { get; set; }

        public int UnitId { get; set; }
        public required MeasurementUnit Unit { get; set; }
    }
}
