namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class PendingIngredientDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime SuggestedAt { get; set; }

        public required UserDto SuggestedByUser { get; set; }
        public required MeasurementUnitDto MeasurementUnit { get; set; }
    }
}
