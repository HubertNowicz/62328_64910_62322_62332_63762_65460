using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeIngredientViewModel
    {
        [Required]
        public required  string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public double Amount { get; set; }

        public int UnitId { get; set; }

        [ValidateNever]
        public required MeasurementUnitViewModel Unit { get; set; }
    }
}
