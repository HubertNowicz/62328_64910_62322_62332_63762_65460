namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeIngredientDto
    {
        public int RecipeIngredientId { get; set; }
        public string IngredientName { get; set; }
        public double Amount { get; set; }
        public int UnitId { get; set; }
        public MeasurementUnitDto Unit { get; set; }
    }
}
