namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class PendingIngredient
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime SuggestedAt { get; set; } = DateTime.UtcNow;
        public int SuggestedByUserId { get; set; }
        public required User SuggestedByUser { get; set; }
        public int MeasurementUnitId { get; set; }
        public required MeasurementUnit MeasurementUnit { get; set; }
    }
}
