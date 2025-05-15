namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class PendingIngredientDto
    {
        public int PendingIngredientId { get; set; }
        public string PendingIngredientName { get; set; }
        public DateTime SuggestedAt { get; set; }
        public string SuggestedByUserFirstName { get; set; }
        public string SuggestedByUserSurname { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public string MeasurementUnitAbbreviation { get; set; }
    }
}
