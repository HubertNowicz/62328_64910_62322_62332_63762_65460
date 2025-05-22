namespace Organizer_przepisów_kulinarnych.Models
{
    public class PendingIngredientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime SuggestedAt { get; set; }

        public UserViewModel SuggestedByUser { get; set; }
        public MeasurementUnitViewModel MeasurementUnit { get; set; }
    }
}