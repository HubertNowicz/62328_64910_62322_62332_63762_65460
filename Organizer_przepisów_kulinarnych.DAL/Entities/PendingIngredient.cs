namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class PendingIngredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime SuggestedAt { get; set; } = DateTime.UtcNow;
        public int SuggestedByUserId { get; set; }
        public User SuggestedByUser { get; set; }
    }
}
