namespace Organizer_przepisów_kulinarnych.Models
{
    public class PendingIngredientViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime SuggestedAt { get; set; }

        public required UserViewModel SuggestedByUser { get; set; }
        public required MeasurementUnitViewModel MeasurementUnit { get; set; }
    }
}