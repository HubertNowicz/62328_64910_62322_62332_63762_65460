namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class IngredientUnit
    {
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        public int UnitId { get; set; }
        public MeasurementUnit Unit { get; set; }
    }
}
