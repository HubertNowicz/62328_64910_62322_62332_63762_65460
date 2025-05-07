using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class IngredientViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public double Amount { get; set; }

        [Required]
        public string Unit { get; set; }
    }


}
