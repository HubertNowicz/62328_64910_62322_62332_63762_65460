namespace Organizer_przepisów_kulinarnych.Models
{
    namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
    {
        public class IngredientViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public List<MeasurementUnitViewModel> Units { get; set; }
        }
    }

}
