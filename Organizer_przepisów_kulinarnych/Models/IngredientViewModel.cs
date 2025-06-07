namespace Organizer_przepisów_kulinarnych.Models
{
    namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
    {
        public class IngredientViewModel
        {
            public int Id { get; set; }
            public required string Name { get; set; }

            public required List<MeasurementUnitViewModel> Units { get; set; }
        }
    }

}
