namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class PendingIngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime SuggestedAt { get; set; }

        public UserDto SuggestedByUser { get; set; }
        public MeasurementUnitDto MeasurementUnit { get; set; }
    }
}
