namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<MeasurementUnitDto> Units { get; set; } = new();
    }
}
